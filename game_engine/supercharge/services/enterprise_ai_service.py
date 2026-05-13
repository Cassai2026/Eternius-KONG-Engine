"""Google-backed enterprise AI service with local fallback behavior."""

from __future__ import annotations

import json
import os
from typing import Any
from urllib import error, request


class EnterpriseAIService:
    def __init__(self, timeout_seconds: float = 10.0) -> None:
        self.timeout_seconds = timeout_seconds
        self.online = False

    def start(self) -> None:
        self.online = True

    def stop(self) -> None:
        self.online = False

    def provider_status(self) -> dict[str, Any]:
        provider, model = self._resolve_provider()
        return {
            "online": self.online,
            "provider": provider,
            "model": model,
            "enterprise_ready": provider != "local_fallback",
            "google_cloud_project": os.getenv("GOOGLE_CLOUD_PROJECT", ""),
            "google_cloud_location": os.getenv("GOOGLE_CLOUD_LOCATION", "global"),
        }

    def generate_enterprise_briefing(
        self,
        *,
        objective: str,
        mission_context: str,
        telemetry: dict[str, Any],
        constraints: list[str] | None = None,
    ) -> dict[str, Any]:
        active_constraints = [constraint.strip() for constraint in constraints or [] if constraint.strip()]
        risk_level = self._compute_risk_level(telemetry)
        recommended_actions = self._build_actions(
            objective=objective,
            mission_context=mission_context,
            telemetry=telemetry,
            risk_level=risk_level,
        )
        provider, model = self._resolve_provider()
        prompt = self._build_prompt(
            objective=objective,
            mission_context=mission_context,
            telemetry=telemetry,
            constraints=active_constraints,
            risk_level=risk_level,
            recommended_actions=recommended_actions,
        )
        summary = self._request_google_summary(prompt)
        if not summary:
            summary = self._build_local_summary(
                objective=objective,
                mission_context=mission_context,
                risk_level=risk_level,
                recommended_actions=recommended_actions,
            )
            provider = "local_fallback"
            model = "deterministic-edge-briefing"

        return {
            "provider": provider,
            "model": model,
            "risk_level": risk_level,
            "summary": summary,
            "recommended_actions": recommended_actions,
            "constraints": active_constraints,
            "telemetry_snapshot": telemetry,
        }

    def _resolve_provider(self) -> tuple[str, str]:
        if os.getenv("GOOGLE_CLOUD_PROJECT") and os.getenv("GOOGLE_CLOUD_ACCESS_TOKEN"):
            return (
                "google_vertex_ai",
                os.getenv("GOOGLE_CLOUD_MODEL", "gemini-2.0-flash-001"),
            )
        if os.getenv("GEMINI_API_KEY"):
            return ("google_gemini_api", os.getenv("GEMINI_MODEL", "gemini-2.0-flash"))
        return ("local_fallback", "deterministic-edge-briefing")

    def _build_prompt(
        self,
        *,
        objective: str,
        mission_context: str,
        telemetry: dict[str, Any],
        constraints: list[str],
        risk_level: str,
        recommended_actions: list[str],
    ) -> str:
        payload = {
            "objective": objective,
            "mission_context": mission_context,
            "risk_level": risk_level,
            "telemetry": telemetry,
            "constraints": constraints,
            "recommended_actions": recommended_actions,
        }
        return (
            "You are an enterprise operations copilot for the Eternius KONG Engine. "
            "Write a concise executive briefing in 2-4 sentences. Focus on field safety, "
            "edge resilience, and next actions. Avoid markdown.\n"
            f"Payload: {json.dumps(payload, sort_keys=True)}"
        )

    def _request_google_summary(self, prompt: str) -> str | None:
        vertex_summary = self._request_vertex_summary(prompt)
        if vertex_summary:
            return vertex_summary
        return self._request_gemini_summary(prompt)

    def _request_vertex_summary(self, prompt: str) -> str | None:
        project = os.getenv("GOOGLE_CLOUD_PROJECT")
        access_token = os.getenv("GOOGLE_CLOUD_ACCESS_TOKEN")
        if not project or not access_token:
            return None

        location = os.getenv("GOOGLE_CLOUD_LOCATION", "global")
        model = os.getenv("GOOGLE_CLOUD_MODEL", "gemini-2.0-flash-001")
        endpoint = (
            "https://aiplatform.googleapis.com/v1/projects/"
            f"{project}/locations/{location}/publishers/google/models/{model}:generateContent"
        )
        headers = {
            "Authorization": f"Bearer {access_token}",
            "Content-Type": "application/json",
        }
        payload = {
            "contents": [{"role": "user", "parts": [{"text": prompt}]}],
            "generationConfig": {"temperature": 0.2, "maxOutputTokens": 256},
        }
        return self._post_for_summary(endpoint=endpoint, headers=headers, payload=payload)

    def _request_gemini_summary(self, prompt: str) -> str | None:
        api_key = os.getenv("GEMINI_API_KEY")
        if not api_key:
            return None

        model = os.getenv("GEMINI_MODEL", "gemini-2.0-flash")
        endpoint = (
            "https://generativelanguage.googleapis.com/v1beta/models/"
            f"{model}:generateContent?key={api_key}"
        )
        headers = {"Content-Type": "application/json"}
        payload = {
            "contents": [{"role": "user", "parts": [{"text": prompt}]}],
            "generationConfig": {"temperature": 0.2, "maxOutputTokens": 256},
        }
        return self._post_for_summary(endpoint=endpoint, headers=headers, payload=payload)

    def _post_for_summary(
        self,
        *,
        endpoint: str,
        headers: dict[str, str],
        payload: dict[str, Any],
    ) -> str | None:
        encoded_payload = json.dumps(payload).encode("utf-8")
        http_request = request.Request(
            endpoint,
            data=encoded_payload,
            headers=headers,
            method="POST",
        )
        try:
            with request.urlopen(http_request, timeout=self.timeout_seconds) as response:
                response_payload = json.loads(response.read().decode("utf-8"))
        except (error.HTTPError, error.URLError, TimeoutError, json.JSONDecodeError):
            return None

        return self._extract_summary_text(response_payload)

    def _extract_summary_text(self, response_payload: dict[str, Any]) -> str | None:
        candidates = response_payload.get("candidates")
        if not isinstance(candidates, list) or not candidates:
            return None

        for candidate in candidates:
            content = candidate.get("content", {})
            parts = content.get("parts", [])
            if not isinstance(parts, list):
                continue
            for part in parts:
                text = part.get("text")
                if isinstance(text, str) and text.strip():
                    return text.strip()
        return None

    def _compute_risk_level(self, telemetry: dict[str, Any]) -> str:
        heart_rate = float(telemetry.get("heart_rate", 60))
        hazards = float(telemetry.get("hazards_in_view", 0))
        cognitive_load = float(telemetry.get("cognitive_load", 0.0))
        risk_score = hazards * 25 + max(heart_rate - 85, 0) + max(cognitive_load - 70, 0)
        if risk_score >= 60:
            return "high"
        if risk_score >= 20:
            return "medium"
        return "low"

    def _build_actions(
        self,
        *,
        objective: str,
        mission_context: str,
        telemetry: dict[str, Any],
        risk_level: str,
    ) -> list[str]:
        actions = [
            f"Advance objective: {objective.strip() or 'stabilize field operations'}",
            f"Keep mission context aligned with {mission_context.strip() or 'local edge execution'}",
        ]

        if risk_level == "high":
            actions.append("Reduce hazard exposure before expanding scope")
        elif risk_level == "medium":
            actions.append("Tighten operator focus and validate the next workload burst")
        else:
            actions.append("Maintain flow-state momentum and continue staged delivery")

        if float(telemetry.get("cognitive_load", 0.0)) >= 70:
            actions.append("Lower cognitive load with a narrower task bundle")
        if float(telemetry.get("hazards_in_view", 0)) > 0:
            actions.append("Clear live hazards from the operator view before the next sync")
        return actions

    def _build_local_summary(
        self,
        *,
        objective: str,
        mission_context: str,
        risk_level: str,
        recommended_actions: list[str],
    ) -> str:
        return (
            f"Local enterprise mode is active for '{objective}'. "
            f"The current mission context is '{mission_context}' with a {risk_level} risk profile. "
            f"Prioritize {recommended_actions[0].lower()} and {recommended_actions[-1].lower()}."
        )

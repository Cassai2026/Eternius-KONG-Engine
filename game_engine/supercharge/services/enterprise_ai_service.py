"""Google-backed enterprise AI service with local fallback behavior."""

from __future__ import annotations

import json
import logging
import os
import ssl
from hashlib import sha256
from typing import Any
from urllib import error, request

LOGGER = logging.getLogger(__name__)
DEFAULT_VERTEX_MODEL = "gemini-2.0-flash-001"
DEFAULT_GEMINI_MODEL = "gemini-2.0-flash"
DEFAULT_LOCAL_MODEL = "deterministic-edge-briefing"
DEFAULT_VERTEX_LOCATION = "global"
DEFAULT_TEMPERATURE = 0.2
DEFAULT_MAX_OUTPUT_TOKENS = 256
HEART_RATE_RISK_FLOOR = 85.0
COGNITIVE_LOAD_RISK_FLOOR = 70.0
HAZARD_RISK_WEIGHT = 25.0
MEDIUM_RISK_THRESHOLD = 20.0
HIGH_RISK_THRESHOLD = 60.0


class EnterpriseAIService:
    def __init__(self, timeout_seconds: float = 10.0) -> None:
        self.timeout_seconds = timeout_seconds
        self.ssl_context = ssl.create_default_context()
        self.online = False

    def start(self) -> None:
        self.online = True

    def stop(self) -> None:
        self.online = False

    def provider_status(self) -> dict[str, Any]:
        google_provider, google_model = self._resolve_google_provider()
        return {
            "online": self.online,
            "provider": "local_fallback",
            "model": DEFAULT_LOCAL_MODEL,
            "enterprise_ready": False,
            "google_available": google_provider is not None,
            "google_provider": google_provider,
            "google_model": google_model,
            "google_cloud_project": os.getenv("GOOGLE_CLOUD_PROJECT", ""),
            "google_cloud_location": os.getenv("GOOGLE_CLOUD_LOCATION", DEFAULT_VERTEX_LOCATION),
        }

    def generate_enterprise_briefing(
        self,
        *,
        objective: str,
        mission_context: str,
        telemetry: dict[str, Any],
        constraints: list[str] | None = None,
        use_google_enterprise: bool = False,
    ) -> dict[str, Any]:
        active_constraints = [constraint.strip() for constraint in constraints or [] if constraint.strip()]
        risk_level = self._compute_risk_level(telemetry)
        recommended_actions = self._build_actions(
            objective=objective,
            mission_context=mission_context,
            telemetry=telemetry,
            risk_level=risk_level,
        )
        summary = self._build_local_summary(
            objective=objective,
            mission_context=mission_context,
            risk_level=risk_level,
            recommended_actions=recommended_actions,
        )
        provider = "local_fallback"
        model = DEFAULT_LOCAL_MODEL

        if use_google_enterprise:
            google_provider, google_model = self._resolve_google_provider()
            if google_provider and google_model:
                prompt = self._build_prompt(
                    objective=objective,
                    mission_context=mission_context,
                    telemetry=telemetry,
                    constraints=active_constraints,
                    risk_level=risk_level,
                    recommended_actions=recommended_actions,
                    local_summary=summary,
                )
                google_summary = self._request_google_summary(prompt)
                if google_summary:
                    summary = google_summary
                    provider = google_provider
                    model = google_model

        return {
            "provider": provider,
            "model": model,
            "risk_level": risk_level,
            "summary": summary,
            "recommended_actions": recommended_actions,
            "constraints": active_constraints,
            "telemetry_snapshot": telemetry,
        }

    def _resolve_google_provider(self) -> tuple[str | None, str | None]:
        if os.getenv("GOOGLE_CLOUD_PROJECT") and os.getenv("GOOGLE_CLOUD_ACCESS_TOKEN"):
            return (
                "google_vertex_ai",
                os.getenv("GOOGLE_CLOUD_MODEL", DEFAULT_VERTEX_MODEL),
            )
        if os.getenv("GEMINI_API_KEY"):
            return ("google_gemini_api", os.getenv("GEMINI_MODEL", DEFAULT_GEMINI_MODEL))
        return (None, None)

    def _build_prompt(
        self,
        *,
        objective: str,
        mission_context: str,
        telemetry: dict[str, Any],
        constraints: list[str],
        risk_level: str,
        recommended_actions: list[str],
        local_summary: str,
    ) -> str:
        telemetry_profile = self._build_telemetry_profile(telemetry)
        payload = {
            "objective": objective,
            "mission_context": mission_context,
            "risk_level": risk_level,
            "local_summary": local_summary,
            "telemetry_profile": telemetry_profile,
            "constraints": constraints,
            "recommended_actions": recommended_actions,
        }
        return (
            "You are an enterprise operations copilot for the Eternius KONG Engine. "
            "Write a concise executive briefing in 2-4 sentences. Focus on field safety, "
            "edge resilience, and next actions. Avoid markdown. Never ask for raw telemetry.\n"
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

        location = os.getenv("GOOGLE_CLOUD_LOCATION", DEFAULT_VERTEX_LOCATION)
        model = os.getenv("GOOGLE_CLOUD_MODEL", DEFAULT_VERTEX_MODEL)
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
            "generationConfig": {
                "temperature": DEFAULT_TEMPERATURE,
                "maxOutputTokens": DEFAULT_MAX_OUTPUT_TOKENS,
            },
        }
        return self._post_for_summary(
            endpoint=endpoint,
            headers=headers,
            payload=payload,
            provider_name="google_vertex_ai",
        )

    def _request_gemini_summary(self, prompt: str) -> str | None:
        api_key = os.getenv("GEMINI_API_KEY")
        if not api_key:
            return None

        model = os.getenv("GEMINI_MODEL", DEFAULT_GEMINI_MODEL)
        endpoint = (
            "https://generativelanguage.googleapis.com/v1beta/models/"
            f"{model}:generateContent"
        )
        headers = {
            "Content-Type": "application/json",
            "x-goog-api-key": api_key,
        }
        payload = {
            "contents": [{"role": "user", "parts": [{"text": prompt}]}],
            "generationConfig": {
                "temperature": DEFAULT_TEMPERATURE,
                "maxOutputTokens": DEFAULT_MAX_OUTPUT_TOKENS,
            },
        }
        return self._post_for_summary(
            endpoint=endpoint,
            headers=headers,
            payload=payload,
            provider_name="google_gemini_api",
        )

    def _post_for_summary(
        self,
        *,
        endpoint: str,
        headers: dict[str, str],
        payload: dict[str, Any],
        provider_name: str,
    ) -> str | None:
        encoded_payload = json.dumps(payload).encode("utf-8")
        http_request = request.Request(
            endpoint,
            data=encoded_payload,
            headers=headers,
            method="POST",
        )
        try:
            with request.urlopen(
                http_request,
                timeout=self.timeout_seconds,
                context=self.ssl_context,
            ) as response:
                response_payload = json.loads(response.read().decode("utf-8"))
        except error.HTTPError as exc:
            LOGGER.warning("%s request failed with HTTP %s", provider_name, exc.code)
            return None
        except error.URLError as exc:
            LOGGER.warning("%s request failed with network error: %s", provider_name, exc.reason)
            return None
        except TimeoutError:
            LOGGER.warning("%s request timed out", provider_name)
            return None
        except json.JSONDecodeError:
            LOGGER.warning("%s returned malformed JSON", provider_name)
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

    def _build_telemetry_profile(self, telemetry: dict[str, Any]) -> dict[str, str]:
        payload = json.dumps(telemetry, sort_keys=True).encode("utf-8")
        heart_rate = float(telemetry.get("heart_rate", 60))
        hazards = float(telemetry.get("hazards_in_view", 0))
        cognitive_load = float(telemetry.get("cognitive_load", 0.0))
        return {
            "heart_rate_band": self._band_heart_rate(heart_rate),
            "hazard_band": self._band_hazards(hazards),
            "cognitive_load_band": self._band_cognitive_load(cognitive_load),
            "telemetry_fingerprint": sha256(payload).hexdigest()[:16],
        }

    def _band_heart_rate(self, heart_rate: float) -> str:
        if heart_rate >= 110:
            return "critical"
        if heart_rate >= HEART_RATE_RISK_FLOOR:
            return "elevated"
        return "stable"

    def _band_hazards(self, hazards: float) -> str:
        if hazards >= 3:
            return "dense"
        if hazards > 0:
            return "present"
        return "clear"

    def _band_cognitive_load(self, cognitive_load: float) -> str:
        if cognitive_load >= 85:
            return "overloaded"
        if cognitive_load >= COGNITIVE_LOAD_RISK_FLOOR:
            return "elevated"
        return "stable"

    def _compute_risk_level(self, telemetry: dict[str, Any]) -> str:
        heart_rate = float(telemetry.get("heart_rate", 60))
        hazards = float(telemetry.get("hazards_in_view", 0))
        cognitive_load = float(telemetry.get("cognitive_load", 0.0))
        risk_score = (
            hazards * HAZARD_RISK_WEIGHT
            + max(heart_rate - HEART_RATE_RISK_FLOOR, 0)
            + max(cognitive_load - COGNITIVE_LOAD_RISK_FLOOR, 0)
        )
        if risk_score >= HIGH_RISK_THRESHOLD:
            return "high"
        if risk_score >= MEDIUM_RISK_THRESHOLD:
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

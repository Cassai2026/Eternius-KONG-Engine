from typing import Any

from fastapi import FastAPI, WebSocket
from pydantic import BaseModel, Field

from game_engine.supercharge.services.enterprise_ai_service import EnterpriseAIService

app = FastAPI(title="Enki-AI Sovereign Vanguard Bridge", version="10.48")
enterprise_ai = EnterpriseAIService()


class EnterpriseBriefingRequest(BaseModel):
    objective: str = Field(
        default="Stabilize sovereign field operations",
        min_length=3,
        max_length=240,
    )
    mission_context: str = Field(
        default="Local encrypted edge coordination",
        min_length=3,
        max_length=240,
    )
    heart_rate: int = Field(default=60, ge=0, le=240)
    hazards_in_view: int = Field(default=0, ge=0, le=100)
    cognitive_load: float = Field(default=0.0, ge=0.0, le=100.0)
    constraints: list[str] = Field(default_factory=list, max_items=8)


@app.on_event("startup")
async def startup_event() -> None:
    enterprise_ai.start()


@app.on_event("shutdown")
async def shutdown_event() -> None:
    enterprise_ai.stop()


@app.get("/enterprise/status")
async def enterprise_status() -> dict[str, Any]:
    return enterprise_ai.provider_status()


@app.post("/enterprise/briefing")
async def enterprise_briefing(payload: EnterpriseBriefingRequest) -> dict[str, Any]:
    telemetry = {
        "heart_rate": payload.heart_rate,
        "hazards_in_view": payload.hazards_in_view,
        "cognitive_load": payload.cognitive_load,
    }
    return enterprise_ai.generate_enterprise_briefing(
        objective=payload.objective,
        mission_context=payload.mission_context,
        telemetry=telemetry,
        constraints=payload.constraints,
    )


@app.websocket("/vanguard/sync")
async def sync(ws: WebSocket):
    await ws.accept()
    while True:
        data = await ws.receive_json()
        # Enki AI Module 37: Cortisol-to-Capital
        await ws.send_json({"biological_roi": 100, "oakley_hud_command": "FLOW_STATE_LUMEN"})

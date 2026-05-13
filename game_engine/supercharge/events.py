"""Event primitives for engine messaging."""

from dataclasses import dataclass


@dataclass(frozen=True)
class EngineEvent:
    name: str
    payload: dict

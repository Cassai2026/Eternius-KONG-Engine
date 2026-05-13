"""Event primitives for engine messaging."""

from dataclasses import dataclass
from typing import Any


@dataclass(frozen=True)
class EngineEvent:
    name: str
    payload: dict[str, Any]

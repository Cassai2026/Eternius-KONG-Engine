"""Runtime configuration models."""

from dataclasses import dataclass

from .constants import DEFAULT_TICK_RATE


@dataclass(frozen=True)
class RuntimeConfig:
    tick_rate: int = DEFAULT_TICK_RATE
    environment: str = "development"

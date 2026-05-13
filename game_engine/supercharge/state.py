"""Mutable engine runtime state."""

from dataclasses import dataclass, field


@dataclass
class RuntimeState:
    status: str = "idle"
    flags: dict[str, str] = field(default_factory=dict)

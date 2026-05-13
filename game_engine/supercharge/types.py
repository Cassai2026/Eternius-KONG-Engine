"""Shared typing contracts for systems and services."""

from typing import Protocol


class Lifecycle(Protocol):
    def start(self) -> None: ...
    def stop(self) -> None: ...

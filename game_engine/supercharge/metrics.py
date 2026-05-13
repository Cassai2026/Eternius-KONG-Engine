"""Minimal runtime metrics collector."""

from collections import Counter


class Metrics:
    def __init__(self) -> None:
        self._counter = Counter()

    def inc(self, key: str, amount: int = 1) -> None:
        self._counter[key] += amount

    def snapshot(self) -> dict[str, int]:
        return dict(self._counter)

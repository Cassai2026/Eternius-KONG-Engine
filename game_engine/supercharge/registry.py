"""Simple registry for runtime components."""

from dataclasses import dataclass, field

from .types import Lifecycle


@dataclass
class ComponentRegistry:
    components: dict[str, Lifecycle] = field(default_factory=dict)

    def register(self, name: str, component: Lifecycle) -> None:
        self.components[name] = component

    def start_all(self) -> None:
        for component in self.components.values():
            component.start()

    def stop_all(self) -> None:
        for component in self.components.values():
            component.stop()

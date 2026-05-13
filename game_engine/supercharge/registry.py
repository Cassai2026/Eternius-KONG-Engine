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
        errors: list[tuple[str, Exception]] = []
        for component_name, component in self.components.items():
            try:
                component.stop()
            except Exception as exc:
                errors.append((component_name, exc))

        if errors:
            error_details = ", ".join(
                f"{name}: {error}" for name, error in errors
            )
            raise RuntimeError(f"One or more components failed to stop: {error_details}")

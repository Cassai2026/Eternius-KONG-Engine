"""Simple registry for runtime components."""

from dataclasses import dataclass, field

from .types import Lifecycle


@dataclass
class ComponentRegistry:
    components: dict[str, Lifecycle] = field(default_factory=dict)

    def register(self, name: str, component: Lifecycle) -> None:
        self.components[name] = component

    def start_all(self) -> None:
        started_components: list[tuple[str, Lifecycle]] = []
        for component_name, component in self.components.items():
            try:
                component.start()
                started_components.append((component_name, component))
            except Exception as exc:
                self._stop_started_components(started_components)
                raise RuntimeError(
                    f"Failed to start component '{component_name}': {exc}"
                ) from exc

    def stop_all(self) -> None:
        errors: list[tuple[str, Exception]] = []
        for component_name, component in reversed(tuple(self.components.items())):
            try:
                component.stop()
            except Exception as exc:
                errors.append((component_name, exc))

        if errors:
            error_details = ", ".join(
                f"{name}: {error}" for name, error in errors
            )
            raise RuntimeError(
                f"{len(errors)} of {len(self.components)} components failed to stop: {error_details}"
            )

    def health_snapshot(self) -> dict[str, bool | None]:
        return {
            component_name: self._resolve_component_online_state(component)
            for component_name, component in self.components.items()
        }

    def _stop_started_components(
        self, started_components: list[tuple[str, Lifecycle]]
    ) -> None:
        for _, component in reversed(started_components):
            try:
                component.stop()
            except Exception:
                continue

    def _resolve_component_online_state(self, component: Lifecycle) -> bool | None:
        online_state = getattr(component, "online", None)
        if isinstance(online_state, bool):
            return online_state
        return None

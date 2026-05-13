"""Default runtime assembly."""

from .registry import ComponentRegistry
from .services.db_service import DatabaseService
from .systems.achievement_system import AchievementSystem
from .systems.build_system import BuildSystem
from .systems.learning_system import LearningSystem
from .systems.network_system import NetworkSystem
from .systems.resource_system import ResourceSystem
from .systems.safety_system import SafetySystem


def build_default_runtime(db_path: str = "enki_knowledge.db") -> ComponentRegistry:
    registry = ComponentRegistry()
    registry.register("database", DatabaseService(db_path))
    registry.register("resources", ResourceSystem())
    registry.register("build", BuildSystem())
    registry.register("network", NetworkSystem())
    registry.register("learning", LearningSystem())
    registry.register("safety", SafetySystem())
    registry.register("achievements", AchievementSystem())
    return registry

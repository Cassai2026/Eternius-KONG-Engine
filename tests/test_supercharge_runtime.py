import unittest

from game_engine.supercharge.bootstrap import build_default_runtime
from game_engine.supercharge.registry import ComponentRegistry
from game_engine.supercharge.services.db_service import DatabaseService


class _StubComponent:
    def __init__(self, *, online: bool = False, fail_start: bool = False) -> None:
        self.online = online
        self.fail_start = fail_start
        self.started = False
        self.stopped = False

    def start(self) -> None:
        if self.fail_start:
            raise RuntimeError("boom")
        self.online = True
        self.started = True

    def stop(self) -> None:
        self.online = False
        self.stopped = True


class SuperchargeRuntimeTests(unittest.TestCase):
    def test_bootstrap_includes_mesh_component(self) -> None:
        runtime = build_default_runtime()
        self.assertIn("mesh", runtime.components)

    def test_start_all_rolls_back_started_components_on_failure(self) -> None:
        runtime = ComponentRegistry()
        first = _StubComponent()
        failing = _StubComponent(fail_start=True)
        runtime.register("first", first)
        runtime.register("failing", failing)

        with self.assertRaises(RuntimeError):
            runtime.start_all()

        self.assertTrue(first.started)
        self.assertTrue(first.stopped)
        self.assertFalse(first.online)

    def test_health_snapshot_tracks_online_flags(self) -> None:
        runtime = ComponentRegistry()
        runtime.register("active", _StubComponent(online=True))
        runtime.register("inactive", _StubComponent(online=False))
        runtime.register("database", DatabaseService(":memory:", enable_wal=False))

        snapshot = runtime.health_snapshot()

        self.assertEqual(snapshot["active"], True)
        self.assertEqual(snapshot["inactive"], False)
        self.assertIsNone(snapshot["database"])


if __name__ == "__main__":
    unittest.main()

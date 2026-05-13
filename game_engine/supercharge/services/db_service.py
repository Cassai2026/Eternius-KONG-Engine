"""Database service wrapper."""

import sqlite3

from ..world_schema import ensure_world_schema


class DatabaseService:
    def __init__(
        self,
        db_path: str,
        enable_concurrent_access: bool = False,
        busy_timeout_ms: int = 5000,
        enable_wal: bool = True,
    ) -> None:
        parsed_timeout_ms = int(busy_timeout_ms)
        if parsed_timeout_ms < 0:
            raise ValueError("busy_timeout_ms must be >= 0")

        self.db_path = db_path
        self.enable_concurrent_access = enable_concurrent_access
        self.busy_timeout_ms = parsed_timeout_ms
        self.enable_wal = enable_wal
        self.connection: sqlite3.Connection | None = None

    def start(self) -> None:
        self.connection = sqlite3.connect(
            self.db_path,
            check_same_thread=not self.enable_concurrent_access,
        )
        self.connection.execute("PRAGMA foreign_keys = ON")
        self.connection.execute(f"PRAGMA busy_timeout = {self.busy_timeout_ms}")
        self.connection.execute("PRAGMA synchronous = NORMAL")
        if self.enable_wal:
            self.connection.execute("PRAGMA journal_mode = WAL")
        ensure_world_schema(self.connection)
        self.connection.commit()

    def stop(self) -> None:
        if self.connection is not None:
            try:
                self.connection.close()
            except sqlite3.Error as exc:
                raise RuntimeError(
                    f"Failed to close SQLite connection to {self.db_path}: {exc}"
                ) from exc
            finally:
                self.connection = None

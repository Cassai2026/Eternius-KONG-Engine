"""Database service wrapper."""

import sqlite3

from ..world_schema import ensure_world_schema


class DatabaseService:
    def __init__(self, db_path: str, enable_concurrent_access: bool = False) -> None:
        self.db_path = db_path
        self.enable_concurrent_access = enable_concurrent_access
        self.connection: sqlite3.Connection | None = None

    def start(self) -> None:
        self.connection = sqlite3.connect(
            self.db_path,
            check_same_thread=not self.enable_concurrent_access,
        )
        ensure_world_schema(self.connection)

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

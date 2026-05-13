"""Database service wrapper."""

import sqlite3


class DatabaseService:
    def __init__(self, db_path: str, allow_multithread: bool = False) -> None:
        self.db_path = db_path
        self.allow_multithread = allow_multithread
        self.connection: sqlite3.Connection | None = None

    def start(self) -> None:
        self.connection = sqlite3.connect(
            self.db_path,
            check_same_thread=not self.allow_multithread,
        )

    def stop(self) -> None:
        if self.connection is not None:
            try:
                self.connection.close()
            except sqlite3.Error:
                pass
            finally:
                self.connection = None

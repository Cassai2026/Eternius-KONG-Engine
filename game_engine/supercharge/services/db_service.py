"""Database service wrapper."""

import sqlite3


class DatabaseService:
    def __init__(self, db_path: str) -> None:
        self.db_path = db_path
        self.connection: sqlite3.Connection | None = None

    def start(self) -> None:
        self.connection = sqlite3.connect(self.db_path)

    def stop(self) -> None:
        if self.connection is not None:
            self.connection.close()
            self.connection = None

"""SQLite helpers for bootstrapping shared world data."""

from __future__ import annotations

import sqlite3
from typing import Final

DEFAULT_BUILD_MENU_ENTRY: Final[dict[str, str]] = {
    "item_name": "WebRTC Mesh Node",
    "engineering_logic": "Spider-Web Handshake v1.0",
    "base_material": "Enterprise STUN/TURN Logic",
    "purpose": "Decentralized P2P Data Exchange",
}


def ensure_world_schema(connection: sqlite3.Connection) -> None:
    connection.execute(
        """
        CREATE TABLE IF NOT EXISTS build_menu (
            id INTEGER PRIMARY KEY,
            item_name TEXT NOT NULL UNIQUE,
            engineering_logic TEXT NOT NULL,
            base_material TEXT NOT NULL,
            purpose TEXT NOT NULL
        )
        """
    )


def upsert_build_menu_item(
    connection: sqlite3.Connection,
    *,
    item_name: str,
    engineering_logic: str,
    base_material: str,
    purpose: str,
) -> bool:
    cursor = connection.execute(
        """
        INSERT OR IGNORE INTO build_menu (
            item_name,
            engineering_logic,
            base_material,
            purpose
        ) VALUES (?, ?, ?, ?)
        """,
        (item_name, engineering_logic, base_material, purpose),
    )
    return cursor.rowcount > 0


def ensure_default_build_menu_entry(connection: sqlite3.Connection) -> bool:
    ensure_world_schema(connection)
    return upsert_build_menu_item(connection, **DEFAULT_BUILD_MENU_ENTRY)


def fetch_first_build_menu_item(connection: sqlite3.Connection) -> str | None:
    ensure_world_schema(connection)
    row = connection.execute(
        "SELECT item_name FROM build_menu ORDER BY id ASC LIMIT 1"
    ).fetchone()
    if row is None:
        return None
    return str(row[0])

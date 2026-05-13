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
    """Create the shared world tables required by mesh and HUD scripts."""

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


def insert_build_menu_item_if_missing(
    connection: sqlite3.Connection,
    *,
    item_name: str,
    engineering_logic: str,
    base_material: str,
    purpose: str,
) -> bool:
    """Insert a build menu item once and return True only when a row was added."""

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
    """Ensure the default WebRTC mesh build item exists and report if it was inserted."""

    ensure_world_schema(connection)
    return insert_build_menu_item_if_missing(connection, **DEFAULT_BUILD_MENU_ENTRY)


def fetch_first_build_menu_item(connection: sqlite3.Connection) -> str | None:
    """Return the oldest build menu item name, or None when the menu is empty."""

    ensure_world_schema(connection)
    row = connection.execute(
        "SELECT item_name FROM build_menu ORDER BY id ASC LIMIT 1"
    ).fetchone()
    if row is None:
        return None
    return str(row[0])

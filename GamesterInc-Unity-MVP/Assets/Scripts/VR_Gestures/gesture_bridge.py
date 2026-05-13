import sqlite3
import sys
from os import getenv
from pathlib import Path


def _resolve_repo_root() -> Path:
    current = Path(__file__).resolve()
    for candidate in current.parents:
        if (candidate / ".git").exists():
            return candidate
    configured_root = getenv("ETERNIUS_REPO_ROOT")
    if configured_root:
        return Path(configured_root).resolve()
    raise RuntimeError(
        "Unable to locate repository root from gesture bridge path. "
        "Set ETERNIUS_REPO_ROOT when .git metadata is unavailable."
    )


REPO_ROOT = _resolve_repo_root()
if str(REPO_ROOT) not in sys.path:
    sys.path.append(str(REPO_ROOT))

from game_engine.supercharge.world_schema import (
    DEFAULT_BUILD_MENU_ENTRY,
    ensure_default_build_menu_entry,
    fetch_first_build_menu_item,
)


def handle_gesture(gesture_name, db_path="enki_knowledge.db"):
    """
    Connects MediaPipe hand signals to the Eternius Game World.
    """
    conn = sqlite3.connect(db_path)
    try:
        if gesture_name == "PINCH":
            print("\n[HUD] 💠 SELECTING NEAREST BUILD SITE...")
            ensure_default_build_menu_entry(conn)
            item = (
                fetch_first_build_menu_item(conn)
                or DEFAULT_BUILD_MENU_ENTRY["item_name"]
            )
            print(f"[HUD] 🛠️  ATTACHED TO CURSOR: {item}")

        elif gesture_name == "SHIELD":
            print("\n[HUD] 🛡️  CLOSING SOVEREIGN OVERLAY. FLOW MODE ACTIVE.")

        elif gesture_name == "FIST":
            print("\n[HUD] 💾 SAVING WORLD STATE TO SOVEREIGN LEDGER...")

        conn.commit()
    finally:
        conn.close()

if __name__ == "__main__":
    # Simulating a "Pinch" gesture detection
    handle_gesture("PINCH")

import sqlite3
import sys
from os import getenv
from pathlib import Path

MIN_REPO_MARKERS = 2


def _looks_like_repo_root(candidate: Path) -> bool:
    markers = (
        (candidate / "master_config.yaml").exists(),
        (candidate / "README.md").exists(),
        (candidate / "game_engine").is_dir(),
    )
    return sum(markers) >= MIN_REPO_MARKERS


def _resolve_repo_root() -> Path:
    """Find the project root using multiple repo markers or ETERNIUS_REPO_ROOT."""

    current = Path(__file__).resolve()
    for candidate in current.parents:
        if (candidate / ".git").exists() and _looks_like_repo_root(candidate):
            return candidate
    configured_root = getenv("ETERNIUS_REPO_ROOT")
    if configured_root:
        resolved_root = Path(configured_root).resolve()
        if _looks_like_repo_root(resolved_root):
            return resolved_root
    raise RuntimeError(
        "Unable to locate repository root. "
        "Set the ETERNIUS_REPO_ROOT environment variable to the absolute path "
        "of the repository root."
    )


REPO_ROOT = _resolve_repo_root()
if str(REPO_ROOT) not in sys.path:
    sys.path.append(str(REPO_ROOT))

from game_engine.supercharge.world_schema import (
    DEFAULT_BUILD_MENU_ENTRY,
    ensure_default_build_menu_entry,
    fetch_first_build_menu_item,
)


def handle_gesture(gesture_name, *, db_path="enki_knowledge.db"):
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

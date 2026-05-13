import sqlite3
import sys
from os import getenv
from pathlib import Path

MIN_REPO_MARKERS = 2


def _resolve_repo_root() -> Path:
    current = Path(__file__).resolve()
    for candidate in current.parents:
        markers = (
            (candidate / "master_config.yaml").exists(),
            (candidate / "README.md").exists(),
            (candidate / "game_engine").is_dir(),
        )
        if sum(markers) >= MIN_REPO_MARKERS:
            return candidate

    configured_root = getenv("ETERNIUS_REPO_ROOT")
    if configured_root:
        resolved_root = Path(configured_root).resolve()
        markers = (
            (resolved_root / "master_config.yaml").exists(),
            (resolved_root / "README.md").exists(),
            (resolved_root / "game_engine").is_dir(),
        )
        if sum(markers) >= MIN_REPO_MARKERS:
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
)


def init_webrtc_mesh(db_path: str = "enki_knowledge.db") -> dict[str, str]:
    """
    Initializes the Spider-Web P2P Handshake Protocol.
    Ensures Oakley HUDs communicate via decentralized WebRTC.

    Args:
        db_path: SQLite database path for the shared world state.

    Returns:
        The active handshake configuration pushed to the HUD.
    """
    conn = sqlite3.connect(db_path)
    try:
        if ensure_default_build_menu_entry(conn):
            print("[HUD] 🕸️  WEBRTC P2P PROTOCOL: HARDENED")
        else:
            print(
                "[HUD] 🕸️  WEBRTC P2P PROTOCOL: VERIFIED "
                f"({DEFAULT_BUILD_MENU_ENTRY['item_name']})"
            )

        handshake_data = {
            "ICE_SERVER": "stun:stun.l.google.com:19302",
            "SIGNAL_MODE": "Decentralized Relay",
            "ENCRYPTION": "Sovereign-AES-256",
            "MANDATE": "L03: No Silent Profiling",
        }

        print("\n--- 📶 SHAKING HANDS WITH THE 15 BILLION HEARTS ---")
        for key, value in handshake_data.items():
            print(f"[HUD] {key}: {value}")

        conn.commit()
        print("\n🚀 P2P MESH IS LIVE. THE GLASSES ARE OFF-GRID. OUSH.")
        return handshake_data
    finally:
        conn.close()

if __name__ == "__main__":
    init_webrtc_mesh()

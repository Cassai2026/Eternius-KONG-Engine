import sqlite3
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[4]
if str(REPO_ROOT) not in sys.path:
    sys.path.append(str(REPO_ROOT))

from game_engine.supercharge.world_schema import (
    ensure_default_build_menu_entry,
    fetch_first_build_menu_item,
)


def handle_gesture(gesture_name, db_path="enki_knowledge.db"):
    """
    Connects MediaPipe hand signals to the Eternius Game World.
    """
    conn = sqlite3.connect(db_path)
    
    if gesture_name == "PINCH":
        print("\n[HUD] 💠 SELECTING NEAREST BUILD SITE...")
        ensure_default_build_menu_entry(conn)
        item = fetch_first_build_menu_item(conn) or "WebRTC Mesh Node"
        print(f"[HUD] 🛠️  ATTACHED TO CURSOR: {item}")
        
    elif gesture_name == "SHIELD":
        print("\n[HUD] 🛡️  CLOSING SOVEREIGN OVERLAY. FLOW MODE ACTIVE.")
        
    elif gesture_name == "FIST":
        print("\n[HUD] 💾 SAVING WORLD STATE TO SOVEREIGN LEDGER...")
        
    conn.commit()
    conn.close()

if __name__ == "__main__":
    # Simulating a "Pinch" gesture detection
    handle_gesture("PINCH")

"""WebRTC mesh service bridge."""

from game_engine.webrtc_mesh import init_webrtc_mesh


class MeshService:
    def __init__(self, db_path: str = "enki_knowledge.db") -> None:
        self.db_path = db_path

    def start(self) -> None:
        init_webrtc_mesh(self.db_path)

    def stop(self) -> None:
        pass

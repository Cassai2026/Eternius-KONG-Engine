"""WebRTC mesh service bridge."""

from game_engine.webrtc_mesh import init_webrtc_mesh


class MeshService:
    def start(self) -> None:
        init_webrtc_mesh()

    def stop(self) -> None:
        pass

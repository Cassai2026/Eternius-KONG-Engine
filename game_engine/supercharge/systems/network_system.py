"""Networking and P2P gameplay connectivity."""


class NetworkSystem:
    def __init__(self) -> None:
        self.online = False

    def start(self) -> None:
        self.online = True

    def stop(self) -> None:
        self.online = False

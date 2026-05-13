"""Achievement and progression rules."""


class AchievementSystem:
    def __init__(self) -> None:
        self.online = False

    def start(self) -> None:
        self.online = True

    def stop(self) -> None:
        self.online = False

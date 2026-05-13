"""CLI entry helpers for local runtime boot."""

from .bootstrap import build_default_runtime


def main() -> None:
    runtime = build_default_runtime()
    runtime.start_all()
    runtime.stop_all()


if __name__ == "__main__":
    main()

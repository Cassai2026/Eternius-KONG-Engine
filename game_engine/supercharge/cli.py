"""CLI entry helpers for local runtime boot."""

import time

from .bootstrap import build_default_runtime


def main() -> None:
    runtime = build_default_runtime()
    runtime.start_all()
    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        try:
            runtime.stop_all()
        except RuntimeError as exc:
            print(f"Shutdown completed with errors: {exc}")


if __name__ == "__main__":
    main()

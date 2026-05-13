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
        runtime.stop_all()


if __name__ == "__main__":
    main()

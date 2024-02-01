from __future__ import annotations
from pmakefile import *  # type: ignore
import urllib.request

def download(url: str, path: Path, *, print: bool = True):
    if print:
        log(f"retriving {url} to {path}")
    path.parent.mkdir(exist_ok=True, parents=True, mode=0o755)

    urllib.request.urlretrieve(url, path)

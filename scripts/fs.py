from __future__ import annotations
import os
import shutil
import stat
import os
from pmakefile import *  # type: ignore

def on_rm_error(func, path, exc_info):
    # from: https://stackoverflow.com/questions/4829043/how-to-remove-read-only-attrib-directory-with-python-in-windows
    path = str(path)
    path = os.path.abspath(path)
    os.chmod(path, stat.S_IWRITE)
    os.unlink(path)

def rm(path: Path):
    if path.is_file():
        path.unlink(missing_ok=True)
        return
    try:
        shutil.rmtree(path.absolute(), onerror=on_rm_error)
    except IOError as e:
        log(str(e), "warn")

def copyfs(src: Path, dest: Path):
    if src.is_file():
        shutil.copy2(src, dest)
        try:
            shutil.copy2(src, dest)
        except IOError:
            pass
    else:
        if dest.is_dir():
            shutil.rmtree(dest)
            Path(dest).unlink(missing_ok=True)
            shutil.copytree(src, dest, copy_function=shutil.copy2)
        elif not dest.exists():
            shutil.copytree(src, dest, copy_function=shutil.copy2)
        else:
            raise IOError("Cannot copy directory to file")

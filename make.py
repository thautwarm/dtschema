from pmakefile import * # type: ignore
from scripts import utils
import subprocess
import config
import shlex

try:
    shlex.split
except AttributeError:
    log('shlex.split is not available, update your python version to 3.8+')
    exit(1)

phony([
    './build/deps/tbnf',
    'pgen'
])

@recipe(name='./build/deps/tbnf')
def download_tbnf():

    if get_os() == 'windows':
        url = f'https://github.com/thautwarm/Typed-BNF/releases/download/v{config.tbnf_version}/tbnf-win.exe'
    elif get_os() == 'linux':
        url = f'https://github.com/thautwarm/Typed-BNF/releases/download/v{config.tbnf_version}/tbnf-linux'
    elif get_os() == 'macos':
        url = f'https://github.com/thautwarm/Typed-BNF/releases/download/v{config.tbnf_version}/tbnf-macos'
    else:
        raise NotImplementedError(f'unsupported os: {get_os()}')

    utils.web.download(url, Path('./build/deps/tbnf'), print=True)

@recipe(name='pgen', rebuild='always')
def codegen_parser():
    exe = Path('./build/deps/tbnf').absolute()
    cmds = shlex.split('DTSchema.tbnf -be csharp-antlr -conf tbnf.config.js -o ./src/Parser/Generated -lang DTSchema')

    if code := subprocess.call([exe, *cmds], executable=exe):
        exit(code)

    cmds = shlex.split('antlr4 src/Parser/Generated/DTSchema.g4 -package DTSchema.Parser -o src/Parser/Generated')
    if code := subprocess.call(cmds):
        exit(code)

import * as NM from "nomake";
// import Zit from "nomake-toolchain/zit";

const BuildConfig = {
  TARGET_OS: "windows" as "windows" | "linux" | "macos",
  TBNF_VERSION: "0.3",
};

NM.option("TARGET_OS", ({ value }) => {
  switch (value.toLowerCase()) {
    case "windows":
      BuildConfig.TARGET_OS = "windows";
      break;
    case "linux":
      BuildConfig.TARGET_OS = "linux";
      break;
    case "macos":
      BuildConfig.TARGET_OS = "macos";
      break;
    default:
      throw new Error(`Unknown TARGET_OS: ${value}`);
  }
});

NM.parseOptions();

const TypedBNFBinaries = {
  windows: NM.webArchive(
    `https://github.com/thautwarm/Typed-BNF/releases/download/v${BuildConfig.TBNF_VERSION}/tbnf-win.exe`,
    { suffixRespectUrl: true },
  ),
  linux: NM.webArchive(
    `https://github.com/thautwarm/Typed-BNF/releases/download/v${BuildConfig.TBNF_VERSION}/tbnf-linux`,
  ),
  macos: NM.webArchive(
    `https://github.com/thautwarm/Typed-BNF/releases/download/v${BuildConfig.TBNF_VERSION}/tbnf-macos`,
  ),
};

const TypedBNFNativeDownload = TypedBNFBinaries[BuildConfig.TARGET_OS];

const TypedBNFNative = NM.target(
  {
    name: "tmp/bin/tbnf.exe",
    deps: { file: TypedBNFNativeDownload },
    async build({ target, deps }) {
      await new NM.Path(target).parent.mkdir({
        parents: true,
        onError: "ignore",
      });
      await new NM.Path(deps.file).copyTo(target);
      if (BuildConfig.TARGET_OS != "windows") {
        await new NM.Path(target).chmod(0o755);
      }
      NM.Log.ok(`Built ${target}`);
    },
  },
);

const generated = NM.target({
  name: "src/Parser/Generated/DTSchemaParser.cs",
  deps: {
    tbnfGrammar: "DTSchema.tbnf",
    tbnfExe: TypedBNFNative,
    config: "tbnf.config.js",
  },
  async build({ target, deps }) {
    // run 'tbnf'
    // workaround: tbnf now works under C drive
    const cacheDir = NM.Path.home().join(".dtschema-build-cache");
    await cacheDir.mkdir({ parents: true, onError: "ignore" });
    await new NM.Path(deps.tbnfGrammar).copyTo(cacheDir.join("DTSchema.tbnf"));
    await new NM.Path(deps.config).copyTo(cacheDir.join("tbnf.config.js"));
    const targetTBNFExe = cacheDir.join("tbnf.exe");
    await new NM.Path(deps.tbnfExe).copyTo(targetTBNFExe);
    await new NM.Path("src/Parser/Generated").mkdir({
      parents: true,
      onError: "ignore",
    });
    await cacheDir.join("Generated").mkdir({
      parents: true,
      onError: "ignore",
    });

    await NM.Shell.runChecked(
      [
        targetTBNFExe.asOsPath(),
        ...NM.Shell.split(
          `DTSchema.tbnf -be csharp-antlr -conf tbnf.config.js -o Generated -lang DTSchema`,
        ),
      ],
      {
        cwd: cacheDir.asOsPath(),
        printCmd: true,
        logError: true,
      },
    );
    await cacheDir.join("Generated", "DTSchema.g4").copyTo(
      new NM.Path(target).parent.join("DTSchema.g4"),
    );

    // run 'antlr4'
    await NM.Shell.runChecked(
      NM.Shell.split(
        `antlr4 src/Parser/Generated/DTSchema.g4 -package DTSchema.Parser -o src/Parser/Generated`,
      ),
      {
        printCmd: true,
        logError: true,
      },
    );
    NM.Log.ok(`Built ${target}`);
  },
});

const out = NM.target({
  name: `dist/${BuildConfig.TARGET_OS}/dts.exe`,
  deps: {
    generated: generated,
    files: NM.Path.glob("src/**/*.cs"),
  },
  async build({ deps, target }) {
    await new NM.Path(target).parent.mkdir({
      parents: true,
      onError: "ignore",
    });
    const build = new NM.Bflat.Build();
    build.mode = "exe";
    build.separateSymbols = true;
    build.aotTrimming = { noGlobalization: true };
    build.sourceFiles = Array.from(
      new Set([...deps.files, deps.generated]),
    ).sort();
    build.referencedILAssemblies = ["3rd-party/Antlr4.Runtime.Standard.dll"];
    if (BuildConfig.TARGET_OS == "macos") {
      NM.fail("macOS is not supported yet");
    }
    build.os = BuildConfig.TARGET_OS;
    await build.run(target, {
      buildDir: "scripts",
    });
  },
});

NM.target(
  {
    name: "build",
    virtual: true,
    deps: [out],
    build() {
      NM.Log.ok("Build finished");
    },
  },
);

NM.target(
  {
    name: "install",
    virtual: true,
    rebuild: "always",
    deps: ["build"],
    async build() {
      const targetInstallDir = NM.Path.home().join(".local", "bin");
      await targetInstallDir.mkdir({ parents: true, onError: "ignore" });
      const binFile = BuildConfig.TARGET_OS === "windows" ? "dts.exe" : "dts";
      await new NM.Path(`dist/${BuildConfig.TARGET_OS}/dts.exe`).copyTo(
        targetInstallDir.join(binFile),
      );
    },
  },
);

await NM.makefile();

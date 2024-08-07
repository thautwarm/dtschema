# DTSchema

A framework for serializing and deserializing data structures generically for multiple programming languages. Variants are natively supported to ease framework/system development.

Serializing/deserializing functions are supported for specific application architectures, e.g., server-driven UI (SDUI).

## Usage

```shell
dtschema-windows-x64.exe [InputFile (.dts)] [OutputFile] --backend [dart|typescript|ts] --require file_that_provides_basic_ops

# e.g., the following command generates a .ts file that imports & exports 'ops.ts'
#    dtschema-linux-arm64 example.dts example.ts --backend ts --require ops.ts
```

## Motivation

This project was created to rule out the huge workloads of serialization/deserialization support in developing SDUI based on Flutter, QT or any other UI framework.

<p align="center">
<img width="600px" src="https://raw.githubusercontent.com/thautwarm/dtschema/master/static/sdui.png"/>
</p>


This is a simplified example of that SDUI schema:

```ocaml
type UI =
    | Touch(child: UI, onTap: fn [])
    | Text(value: str, style?: TextStyle)
    | VStack(children: [UI])
    | HStack(children: [UI])
    | FittedBox(child: UI, fit?: enum Fit)
    | ...

enum Fit =
    | fill
    | contain
    | ...

type Padding(left?: float, right?: float, top?: float, bottom?: float)
```

With the schema defined, primitives, enums, classes/records, variants and even functions can be serialized/deserialized from "X" to Flutter and vice versa. "X" here is designed to be Julia but prototyped in TypeScript.


# Hack VM Translator

This is part of the project assignments in the [Nand2Tetris](https://www.nand2tetris.org/) course.

## Requirements
.NET 5

## Usage

Build the project 

```bash
dotnet build --output build
```
cd into the build dir and execute
```bash
./VMTranslator Foo.vm
```
Running the above command creates a file in the same directory as Foo.vm named Foo.asm.
## What else?

The Makefile is supplied as a requirement to the autograder. If you choose to run it, you'll have to clean up after it. Or push a PR with a clean target. Pull requests are welcome :)

## License
[WTFPL](http://www.wtfpl.net/)
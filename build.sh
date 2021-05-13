#!/bin/bash
cd csharp_rust
cargo build
cd ../
cp csharp_rust/target/debug/libcsharp_rust.so bin/Debug/net5.0
dotnet clean
dotnet build



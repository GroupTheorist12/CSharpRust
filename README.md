# Guide to Calling Rust Functions from C# for .NET Core

#### 1. Create CSharpRust .NET Core Console App using CLI.

Run the following commands from a terminal window.

```bash
$ dotnet new console -o CSharpRust
The template "Console Application" was created successfully.

Processing post-creation actions...
Running 'dotnet restore' on CSharpRust/CSharpRust.csproj...
  Determining projects to restore...
  Restored /home/brigg/CSharpRust/CSharpRust.csproj (in 73 ms).
Restore succeeded.
$ cd CSharpRust

```

#### 2. Create Test Harness

In the CSharpRust directory using your favorite editor to cut and paste the following into a file called *MethodRunner.cs*   and save it.

```c#
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace CSharpRust
{
    public class MethodRunner
    {
        private static Hashtable htTestFuncs = new Hashtable();

        public static int RunIt(string hashEntry)
        {
            try
            {
                MethodInfo mi = (MethodInfo)htTestFuncs[hashEntry];
                return (int)mi.Invoke(null, null);

            }
            catch
            {
                return -1;
            }
        }

        static MethodRunner()
        {

            // get all public static methods of MethodRunner type
            MethodInfo[] methodInfos = typeof(MethodRunner).GetMethods(BindingFlags.Public |
                                                                BindingFlags.Static);
            // sort methods by name
            Array.Sort(methodInfos,
                    delegate (MethodInfo methodInfo1, MethodInfo methodInfo2)
                    { return methodInfo1.Name.CompareTo(methodInfo2.Name); });

            // write method names to hash
            foreach (MethodInfo methodInfo in methodInfos)
            {
                if (methodInfo.Name.IndexOf("Test_") == -1)
                {
                    continue;
                }

                string miKey = methodInfo.Name.Replace("Test_", "");
                //Console.WriteLine(miKey);

                htTestFuncs[miKey] = methodInfo;
            }


        }

        public static int Test_InitTest()
        {
            Console.WriteLine("This is the initial test of the test harness");
            return 0;
        }

    }
}
```

#### 3. Modify Program.cs

Using your handy text editor, modify *Program.cs* to look like below:

```c#
using System;

namespace CSharpRust
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 1)
            {
                int ret = MethodRunner.RunIt(args[0]);
                if(ret != 0) //name doesn't match.
                {
                    Console.WriteLine("Test harness name not found {0}", args[0]);
                }
            }
            else
            {
                Console.WriteLine("Usage is: CSharpRust TestHarnessName");
            }
        }
    }
}

```

#### 4. Run App using Test Harness Name

Type the following in a terminal window:

```bash
$ dotnet run InitTest
```

And you should see the following output:

```bash
This is the initial test of the test harness
```

#### 5. Create csharp_rust Crate

Enter the following in a terminal window from the *CSharpRust* directory.

```bash
cargo new csharp_rust --lib
```

#### 6. Modify Cargo.toml

Change directories to *csharp_rust* the edit the *Cargo.toml* to look as below. You can keep the author the same.

```rust
[package]
name = "csharp_rust"
version = "0.1.0"
authors = ["brad rigg"]
edition = "2018"

# See more keys and their definitions at https://doc.rust-lang.org/cargo/reference/manifest.html

[lib]
name="csharp_rust"
crate-type = ["dylib"]

[dependencies]
byte-tools = "0.3.1"
libc = "0.2.94"

```

#### 7. Build the crate

Build the crate by running the following in the *csharp_rust* directory.

```bash
$ cargo build
Updating crates.io index
Downloaded byte-tools v0.3.1
Downloaded libc v0.2.94
Downloaded 2 crates (517.2 KB) in 4.81s
Compiling libc v0.2.94
Compiling byte-tools v0.3.1
Compiling csharp_rust v0.1.0 (/home/brigg/CSharpRust/csharp_rust)
Finished dev [unoptimized + debuginfo] target(s) in 49.26s
```

#### 8. Create Shell Script to Build and Copy Rust Shared Library to C# Executable Directory

We will create a shell script that will build the Rust shared library then copy it to the debug directory so that the C# App can find it without giving it the full path to the library. Open your favorite editor and cut and paste the code below and save it as *build.sh* in the *CSharpRust* directory. 

```bash
#!/bin/bash
cd csharp_rust
cargo build
cd ../
cp csharp_rust/target/debug/libcsharp_rust.so bin/Debug/net5.0
dotnet clean
dotnet build

```

Then from a terminal window run the following:

```bash
$ chmod 777 build.sh  
```

#### 9. Modify lib.rs 

In the *csharp_rust/src* directory fire up your trusty editor and make the *lib.rs* file look as below:

```rust
use libc::c_char;
use std::ffi::CStr;
use std::ffi::CString;
use std::slice;

#[no_mangle]
pub extern "C" fn add_numbers(number1: i32, number2: i32) -> i32 {
    number1 + number2
}

```

Run the *build.sh* shell script from the *CSharpRust* directory.

```bash
$ ./build.sh
```

#### 10. Modify Method.cs

Edit the *Method.cs* file and place the following in the end of the file.

```c#
        [DllImport("libcsharp_rust.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 add_numbers(Int32 number1, Int32 number2);
        public static int Test_AddNumbers()
        {
            var addedNumbers = add_numbers(10, 5);

            Console.WriteLine($"Rust returned from add_numbers: {addedNumbers}");

            return 0;
        }

```

#### 11. Call the add_numbers Rust Function

Run the following in the *CSharpRust* directory:

```bash
$ ./build.sh
$ dotnet run AddNumbers
```

Should produce the following output:

```bash
$ dotnet run AddNumbers
Rust returned from add_numbers: 15
```


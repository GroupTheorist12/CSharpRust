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


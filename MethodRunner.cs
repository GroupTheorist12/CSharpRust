using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Runtime.InteropServices;

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

        [DllImport("libcsharp_rust.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 add_numbers(Int32 number1, Int32 number2);
        public static int Test_AddNumbers()
        {
            var addedNumbers = add_numbers(10, 5);

            Console.WriteLine($"Rust returned from add_numbers: {addedNumbers}");

            return 0;
        }

        [DllImport("libcsharp_rust.so", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void string_from_rust2(IntPtr ret);
        private static extern void string_from_rust(StringBuilder ret);

        public static int Test_StringFromRust()
        {
            StringBuilder charPtr = new StringBuilder();
            string_from_rust(charPtr);
            Console.WriteLine($"Rust returned from string_from_rust: {charPtr.ToString()}");
            return 0;
        }

        [DllImport("libcsharp_rust.so", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void string_from_rust2(IntPtr ret);
        private static extern void string_from_rust_intptr(IntPtr ret);
        public static int Test_StringFromRustIntPtr()
        {
            IntPtr ptr = Marshal.AllocHGlobal(100);

            string_from_rust_intptr(ptr);
            string msg = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            Console.WriteLine($"Rust returned from string_from_rust: {msg}");
            return 0;
        }

        [DllImport("libcsharp_rust.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern void string_to_rust(string message);

        public static int Test_StringToRust()
        {
            string message = "C#";
            string_to_rust(message);

            return 0;
        }

    }
}
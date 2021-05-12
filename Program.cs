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

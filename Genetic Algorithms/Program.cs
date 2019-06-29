using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic_Algorithms
{
    class Program
    {
        static void Main(string[] args)
        {
            RunGAHelloWorld(true);
            Console.ReadKey();
        }

        static void RunGAHelloWorld(bool print)
        {
            const int ITERATION_COUNT = 100;

            var module = new GAHelloWorld();
            uint result = 0;
            for (int i = 0; i < ITERATION_COUNT; ++i)
            {
                result += module.Run(print);

                if (print == false && i % 10 == 0) Console.Write(".");
            }
            if (print == false) Console.WriteLine();

            result /= ITERATION_COUNT;
            Console.WriteLine("Average solution iterations for {0} tries: {1}", ITERATION_COUNT, result);
        }
    } // END OF CLASS Program
} // END OF NAMESPACE

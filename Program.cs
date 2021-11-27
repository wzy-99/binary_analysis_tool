using CommandDotNet;
using System;

namespace binary_analysis_tool
{
    class Program
    {
        static void Main(string[] args) // -i demo1.bin -o output.bin -L 100 -c 0 -c 1 -c 2 -c 3 -c 4 -c 5 -c 6 -c 7 -c 8 -l 128
        {
            new AppRunner<BinaryAnalysis>().Run(args);
        }
    }
}

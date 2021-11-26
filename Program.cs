using System;

namespace binary_analysis_tool
{
    class Program
    {
        static void Main(string[] args)
        {
            BinaryAnalysis ba = new BinaryAnalysis();
            byte[] head = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8};
            ba.FramingWriting("demo1.bin", "output.bin", head, 100);
        }
    }
}

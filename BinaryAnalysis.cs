using System;
using System.Threading.Tasks.Dataflow;

namespace binary_analysis_tool
{
    public class BinaryAnalysis
    {
        public void FramingWriting(string inputFile, string outputFile, byte[] candidate, int frameLength, int length=1024, int cycle=0)
        {
            IReadStrategy readStrategy = new FileReadStrategy(inputFile, length, cycle);
            ReadBlock readBlock = new ReadBlock(readStrategy);

            FramingStrategy framingStrategy = new FramingStrategy(candidate, frameLength);
            FramingBlock framingBlock = new FramingBlock(framingStrategy);

            IWriteStrategy writeStrategy = new FileWriteStrategy(outputFile);
            WriteBlock writeBlock = new WriteBlock(writeStrategy);

            framingBlock.SetAction((input) =>
            {
                writeBlock.Post(input);
            });

            readBlock.SetAction((input) =>
            {
                framingBlock.Post(input);
            });

            readBlock.Start();

            Console.ReadLine();
        }
    }
}

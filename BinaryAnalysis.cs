using System;
using System.Threading.Tasks.Dataflow;
using CommandDotNet;
using Serilog;
using Serilog.Events;

namespace binary_analysis_tool
{
    [Command(Description = "frame and write binary file")]
    public class BinaryAnalysis
    {
        [DefaultCommand,
        Command(Name = "fbl",
        Usage = "fbl <string> <string> <int> <byte[]> [int] [int]",
        Description = "framing by length and write binary file",
        ExtendedHelpText = "more details and examples")]
        public void FramingByLength(
            [Option(LongName = "inputFile", ShortName = "i",
                    Description = "the path of input file")]
            string inputFile,
            [Option(LongName = "outputFile", ShortName = "o",
                    Description = "the path of output file")]
            string outputFile,
            [Option(LongName = "frameLength", ShortName = "L",
                    Description = "the length of frame")]
            int frameLength,
            [Option(LongName = "candidate", ShortName = "c",
                    Description = "the candidate bytes")]
            byte[] candidate,
            [Option(LongName = "length", ShortName = "l",
                    Description = "the length of read block, default is 1024")]
            int length=1024,
            [Option(LongName = "cycle", ShortName = "t",
                    Description = "the times of reading， 0 for reading all the file")]
            int cycle=0)
        {
            InitLog();

            IReadStrategy readStrategy = new FileReadStrategy(inputFile, length, cycle);
            ReadBlock readBlock = new ReadBlock(readStrategy);

            IFramingStrategy framingStrategy = new FramingLengthStrategy(candidate, frameLength);
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

            Log.Information("Finish");
            Console.ReadLine();
        }
        [Command(Name = "fbh",
        Usage = "fbh <string> <string> <byte[]> [int] [int]",
        Description = "framing by head and write binary file",
        ExtendedHelpText = "more details and examples")]
        public void FramingByHead(
            [Option(LongName = "inputFile", ShortName = "i",
                    Description = "the path of input file")]
            string inputFile,
            [Option(LongName = "outputFile", ShortName = "o",
                    Description = "the path of output file")]
            string outputFile,
            [Option(LongName = "candidate", ShortName = "c",
                    Description = "the candidate bytes")]
            byte[] candidate,
            [Option(LongName = "length", ShortName = "l",
                    Description = "the length of read block, default is 1024")]
            int length=1024,
            [Option(LongName = "cycle", ShortName = "t",
                    Description = "the times of reading， 0 for reading all the file")]
            int cycle=0)
        {
            InitLog();

            IReadStrategy readStrategy = new FileReadStrategy(inputFile, length, cycle);
            ReadBlock readBlock = new ReadBlock(readStrategy);

            IFramingStrategy framingStrategy = new FramingHeadStrategy(candidate);
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

            Log.Information("Finish");
            Console.ReadLine();
        }
        private void InitLog()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File("log/log.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();
        }
    }
}

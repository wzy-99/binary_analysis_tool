using System;
using System.Threading.Tasks.Dataflow;

namespace binary_analysis_tool
{
    public class WriteBlock
    {
        private IWriteStrategy writeStrategy;
        private ActionBlock<Tuple<byte[], bool>> _block;
        public WriteBlock(IWriteStrategy writeStrategy)
        {
            this.writeStrategy = writeStrategy;
            _block = new ActionBlock<Tuple<byte[], bool>>((input) => Write(input.Item1, input.Item2));
        }
        public void Write(byte[] input, bool complete)
        {
            writeStrategy.Write(input, complete);
        }
        public void Post(Tuple<byte[], bool> input)
        {
            _block.Post(input);
        }
    }
}
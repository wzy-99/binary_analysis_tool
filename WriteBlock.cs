using System;
using System.Threading.Tasks.Dataflow;

namespace binary_analysis_tool
{
    public class WriteBlock
    {
        private IWriteStrategy writeStrategy;
        private ActionBlock<byte[]> _block;
        public WriteBlock(IWriteStrategy writeStrategy)
        {
            this.writeStrategy = writeStrategy;
            _block = new ActionBlock<byte[]>((input) => Write(input));
        }
        public void Write(byte[] input)
        {
            writeStrategy.Write(input);
        }
        public void Post(byte[] input)
        {
            _block.Post(input);
        }
    }
}
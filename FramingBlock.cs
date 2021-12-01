using System;
using System.Threading.Tasks.Dataflow;

namespace binary_analysis_tool
{
    public class FramingBlock
    {
        private IFramingStrategy framingStrategy;
        private ActionBlock<byte[]> _block;
        public FramingBlock(IFramingStrategy framingStrategy)
        {
            this.framingStrategy = framingStrategy;
            _block = new ActionBlock<byte[]>((input) => Framing(input));
        }
        public void Framing(Byte[] input)
        {
            framingStrategy.Framing(input);
        }
        public void SetAction(Action<Tuple<byte[], bool>> action)
        {
            framingStrategy.SetAction(action);
        }
        public void Post(byte[] input)
        {
            _block.Post(input);
        }
    }
}

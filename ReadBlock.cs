using System;

namespace binary_analysis_tool
{
    class ReadBlock
    {
        private IReadStrategy readStrategy;
        public ReadBlock(IReadStrategy readStrategy)
        {
            this.readStrategy = readStrategy;
        }
        public void Start()
        {
            readStrategy.Start();
        }
        public void SetAction(Action<Byte[]> action)
        {
            readStrategy.SetAction(action);
        }
    }
}

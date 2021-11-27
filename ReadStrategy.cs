using System;
using System.IO;
using System.Threading.Tasks.Dataflow;
using Serilog;

namespace binary_analysis_tool
{
    public interface IReadStrategy
    {
        public void Start();
        public void SetAction(Action<Byte[]> action);
    }
    class FileReadStrategy : IReadStrategy
    {
        private string _path;
        private int _length;
        private int _cycle;
        private int _times;
        private BinaryReader _br;
        private Action<Byte[]> dataArrived;
        public FileReadStrategy(string path, int length=1024, int cycle=0)
        {
            _path = path;
            _length = length;
            _cycle = cycle;
        }
        public void Start()
        {
            try
            {
                _br = new BinaryReader(new FileStream(_path, FileMode.Open));
                Log.Information("Read start");
            }
            catch (IOException e)
            {
                Log.Error(e.Message + "\n Cannot open file.");
            }
            try
            {
                byte[] bytes;
                int times = 0;
                // 读文件，每次只读_length字节，只读_cycle次循环
                while (times < _cycle || _cycle == 0)
                {
                    bytes = _br.ReadBytes(_length);
                    if (bytes.Length != 0)
                    {
                        Log.Verbose("Success read {length} bytes", bytes.Length);
                        times += 1;
                        dataArrived?.Invoke(bytes);
                    }
                    else
                    {
                        Log.Information("Read fininshed");
                        break;
                    }
                }
            }
            catch (IOException e)
            {
                Log.Error(e.Message + "\n Cannot read from file.");
            }
        }
        public void SetAction(Action<Byte[]> action)
        {
            dataArrived = action;
        }
    }
}

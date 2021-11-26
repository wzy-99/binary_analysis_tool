using System;
using System.IO;
using System.Threading.Tasks.Dataflow;
using Serilog;

namespace binary_analysis_tool
{
    public interface IWriteStrategy
    {
        public void Write(byte[] bytes);
    }
    public class FileWriteStrategy : IWriteStrategy
    {
        private string _path;
        private BinaryWriter _bw;
        public FileWriteStrategy(string path)
        {
            _path = path;
        }
        public void Write(byte[] bytes)
        {
            try
            {
                _bw = new BinaryWriter(new FileStream(_path, FileMode.Append));
            }
            catch (IOException e)
            {
                Log.Error(e.Message + "\n Cannot create file.");
            }
            try
            {
                _bw.Write(bytes);
            }
            catch (IOException e)
            {
                Log.Error(e.Message + "\n Cannot write to file.");
                return;
            }
            _bw.Close();
        }
    }
}

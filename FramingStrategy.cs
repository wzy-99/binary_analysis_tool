using Serilog;
using System;
using System.Linq;

namespace binary_analysis_tool
{
    public interface IFramingStrategy
    {
        public void Framing(Byte[] bytes);
        public void SetAction(Action<Byte[]> action);

    }

    public class FramingStrategy : IFramingStrategy
    {
        private byte[] _buffer;
        private byte[] _candidate;
        private int _length;
        private Action<Byte[]> dataArrived;
        public FramingStrategy(byte[] candidate, int length)
        {
            _candidate = candidate;
            _length = length;
        }
        public void Framing(Byte[] bytes)
        {
            if (_buffer == null)
            {
                _buffer = bytes;
            }
            else
            {
                _buffer = _buffer.Concat(bytes).ToArray();
            }

            if (_buffer.Length >= _length)
            {
                int i = 0;
                for (; i <= _buffer.Length - _length; i++)
                {
                    if (!IsMatch(bytes, i))
                    {
                        continue;
                    }
                    else
                    {
                        byte[] ret = new byte[_length];
                        Array.Copy(_buffer, i, ret, 0, _length);
                        Log.Verbose("Success frame {Bytes}", ret);
                        dataArrived?.Invoke(ret);
                    }
                }
                byte[] new_buffer = new byte[_buffer.Length - i];
                Array.Copy(_buffer, i, new_buffer, 0, _buffer.Length - i);
                _buffer = new_buffer;
            }
        }
        private bool IsMatch(byte[] array, int position)
        {
            for (int i = 0; i < _candidate.Length; i++)
                if (array[position + i] != _candidate[i])
                    return false;

            return true;
        }
        public void SetAction(Action<Byte[]> action)
        {
            dataArrived = action;
        }

    }
}

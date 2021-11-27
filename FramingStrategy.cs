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
            // 将到来的bytes储放至_buffer
            if (_buffer == null)
            {
                _buffer = bytes;
            }
            else
            {
                _buffer = _buffer.Concat(bytes).ToArray();
            }

            // 如果_buffer中的长度大于最小数据帧长度
            if (_buffer.Length >= _length)
            {
                int i = 0;
                for (; i <= _buffer.Length - _length; i++)
                {
                    // 判断是否匹配head
                    if (!IsMatch(bytes, i))
                    {
                        continue;
                    }
                    else
                    {
                        // 取定长bytes作为一帧返回
                        byte[] ret = new byte[_length];
                        Array.Copy(_buffer, i, ret, 0, _length);
                        Log.Verbose("Success frame {Bytes}", ret);
                        dataArrived?.Invoke(ret);
                    }
                }
                // 删除_buffer内已经被处理的字节
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

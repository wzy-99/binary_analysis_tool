using Serilog;
using System;
using System.Linq;

namespace binary_analysis_tool
{
    public interface IFramingStrategy
    {
        public void Framing(Byte[] bytes);
        public void SetAction(Action<Tuple<byte[], bool>> action);

    }

    public class FramingLengthStrategy : IFramingStrategy
    {
        private byte[] _buffer;
        private byte[] _candidate;
        private int _length;
        private Action<Tuple<byte[], bool>> dataArrived;
        public FramingLengthStrategy(byte[] candidate, int length)
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
                    if (!IsMatch(_buffer, i))
                    {
                        continue;
                    }
                    else
                    {
                        // 取定长bytes作为一帧返回
                        byte[] ret = new byte[_length];
                        Array.Copy(_buffer, i, ret, 0, _length);
                        Log.Verbose("Success frame {Bytes}", ret);
                        dataArrived?.Invoke(new Tuple<byte[], bool>(ret, true));
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
        public void SetAction(Action<Tuple<byte[], bool>> action)
        {
            dataArrived = action;
        }

    }

    public class FramingHeadStrategy : IFramingStrategy
    {
        private byte[] _buffer;
        private byte[] _candidate;
        private int _length;
        private int _second_index;
        private int _first_index;
        private bool _find_flag;
        private Action<Tuple<byte[], bool>> dataArrived;
        public FramingHeadStrategy(byte[] candidate)
        {
            _candidate = candidate;
            _length = candidate.Length;
            _second_index = 0;
            _first_index = 0;
            _find_flag = false;
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

            // 如果_buffer中的长度大于帧头长度
            if (_buffer.Length >= _length)
            {
                int i = 0;

                for (; i <= _buffer.Length - _length; i++)
                {
                    if (!IsMatch(_buffer, i))
                    {
                        continue;
                    }
                    else
                    {
                        // 如果之前已经找到第一个帧头，则发送一个完整的一帧
                        if (_find_flag)
                        {
                            _second_index = i;
                            Send(_first_index, _second_index - _first_index);
                            _first_index = _second_index;
                        }
                        // 如果之前尚未找到第一个帧头，则发送一个部分的一帧
                        else // 搜索第一个head
                        {
                            _first_index = i;
                            _find_flag = true;
                            Send(0, _first_index - 0);
                        }
                    }
                }
               
                // 如果没找到第二个帧头，也把缓冲中的内容发送出去
                int remain_length = _length - 1;
                byte[] new_buffer = new byte[remain_length];
                Send(_first_index, _buffer.Length - _first_index - remain_length, false);
                Array.Copy(_buffer, _buffer.Length - remain_length, new_buffer, 0, remain_length);
                _buffer = new_buffer;
                _find_flag = false;
                _first_index = 0;
            }
        }
        private byte[] Send(int from, int length, bool complete=true)
        {
            byte[] ret = new byte[length];
            Array.Copy(_buffer, from, ret, 0, length);
            dataArrived?.Invoke(new Tuple<byte[], bool>(ret, complete));
            Log.Verbose("Success frame {Bytes}", ret);
            return ret;
        }
        private bool IsMatch(byte[] array, int position)
        {
            for (int i = 0; i < _candidate.Length; i++)
                if (array[position + i] != _candidate[i])
                    return false;

            return true;
        }
        public void SetAction(Action<Tuple<byte[], bool>> action)
        {
            dataArrived = action;
        }

    }
}

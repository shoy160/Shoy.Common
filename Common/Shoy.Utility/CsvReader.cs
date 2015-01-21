using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shoy.Utility
{
    /// <summary>
    /// Csv阅读辅助
    /// </summary>
    public class CsvReader
    {
        private readonly string _content = string.Empty;

        private int _index;

        private int _state;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="file"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public CsvReader(string file)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException("文件未找到", file);
            }
            var sr = new StreamReader(file, Encoding.Default);

            _content = sr.ReadToEnd();

            sr.Close();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CsvReader(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var sr = new StreamReader(stream, Encoding.Default);

            _content = sr.ReadToEnd();

            sr.Close();
        }

        /// <summary>
        /// 获取一行的数据
        /// </summary>
        /// <returns></returns>
        public string[] ReadLine()
        {
            if (_index >= _content.Length)
                return null;

            var list = new List<string>();

            var cell = new StringBuilder();

            bool finish = false;

            for (; _index < _content.Length; _index++)
            {
                char character = _content[_index];

                switch (character)
                {
                    case ',':
                        if (_state == 0)
                        {
                            list.Add(cell.ToString());
                            cell.Remove(0, cell.Length);
                        }
                        else if (_state == 1)
                        {
                            cell.Append(character);
                        }
                        break;

                    case '"':
                        if (_state == 0)
                        {
                            _state = 1;
                            break;
                        }
                        if (_state == 1)
                        {
                            if (_index + 1 < _content.Length && _content[_index + 1] == '"')
                            {
                                _index = _index + 1;
                                cell.Append(character);
                                break;
                            }
                            _state = 0;
                        }
                        break;

                    default:
                        cell.Append(character);
                        break;

                    case '\r':
                        if (_state == 0 && (_index + 1) < _content.Length && _content[_index + 1] == '\n')
                        {
                            list.Add(cell.ToString());
                            _index = _index + 2;
                            finish = true;
                            break;
                        }

                        if (_state == 1 && (_index + 1) < _content.Length && _content[_index + 1] == '\n')
                        {
                            _index = _index + 1;
                        }
                        //cell.Append(character);
                        break;
                }

                if (finish)
                {
                    break;
                }
            }

            if (_index >= _content.Length && cell.Length > 0)
                list.Add(cell.ToString());

            return list.ToArray();
        }
    }
}

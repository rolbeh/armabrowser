/*
 * https://developer.valvesoftware.com/wiki/Steam_Web_API
 * 
 * This is a quick and dirty implementation
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Data.DefaultImpl
{
    class SteamConfigReader : IDisposable
    {
        readonly System.IO.Stream _file;
        bool _disposed;

        public SteamConfigReader(string filepath)
        {
            _file = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
        }

        ~SteamConfigReader()
        {
            Dispose();
        }

        #region IDisposable

        public void Dispose()
        {
            if (_disposed)
            {
                _disposed = true;
                _file.Dispose();
            }
        }

        #endregion IDisposable

        public string GetValueOf(string firstPart)
        {
            if (firstPart == null) throw new ArgumentNullException("path");

            _file.Position = 0;

            using (var reader = new System.IO.StreamReader(_file, Encoding.ASCII, false, 1024 * 1024, true))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (line != null && line.StartsWith(firstPart))
                    {
                        line = line.TrimStart();
                        return line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("\"", string.Empty).Replace(@"\\", @"\");
                    }
                }
                return string.Empty;
            }
        }


        //public Element GetElement(string path)
        //{
        //    if (path == null) throw new ArgumentNullException("path");

        //    if (path.StartsWith("/"))
        //    {
        //        _file.Position = 0;
        //    }
        //    var pathParts = path.Split(new []{'/'}, StringSplitOptions.RemoveEmptyEntries);
        //    for (int i = 0; i < pathParts.Length; i++)
        //    {
        //        var loopItem = ReadLine();
        //        while (true)
        //        {
        //            if (loopItem.Name == pathParts[i] && loopItem.level == i)
        //            {
        //                ReadLine();
        //                break;
        //            }
        //            loopItem = ReadLine();
        //        }
        //    }

        //    return _currentElement;
        //}

        //Element ReadLine()
        //{
        //    var pos = _file.Position;
        //    using (var reader = new System.IO.StreamReader(_file, Encoding.ASCII, false, 1024*1024, true))
        //    {
        //        var line = reader.ReadLine();
        //        var a = line.Split(new[] { '"' }, StringSplitOptions.RemoveEmptyEntries);
        //        var nextChar = reader.Peek();
        //        var result = new Element(a[0])
        //        {
        //            StartPos = pos,
        //            EndPos = _file.Position,
        //            Value = null
        //        };
        //        switch (nextChar)
        //        {
        //            case '{':
        //                result.ElementType = ElementType.Array;
        //                break;
        //            default:
        //                break;
        //        }
        //        return result;
        //    }
        //}


        public enum ElementType
        {
            Array,
            Attribute
        }

        public struct Element
        {
            public long StartPos;
            public long EndPos;
            public string Name;
            public ElementType ElementType;
            public string Value;
            public int level;

            //public Element()
            //{
            //    StartPos = 0;
            //    EndPos = 0;
            //    Name = "";
            //    ElementType = SteamConfigReader.ElementType.Array;
            //    Value = null;
            //    level = 0;
            //}

            public Element(string name)
            {
                StartPos = 0;
                EndPos = 0;
                Value = null;
                level = 0;
                ElementType = SteamConfigReader.ElementType.Array;
                Name = name.TrimStart();
                for (int i = 0; i < name.Length; i++)
                {
                    if (name[i] != '\t')
                    {
                        level = i;
                        break;
                    }
                }
            }

            public override string ToString()
            {
                return Name;
            }
        }

    }


}

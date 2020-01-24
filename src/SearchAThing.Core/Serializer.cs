#region SearchAThing.Core, Copyright(C) 2015-2016 Lorenzo Delana, License under MIT
/*
* The MIT License(MIT)
* Copyright(c) 2016 Lorenzo Delana, https://searchathing.com
*
* Permission is hereby granted, free of charge, to any person obtaining a
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
* DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace SearchAThing
{

    public static partial class Extensions
    {

        /// <summary>
        /// serialize to string
        /// </summary>        
        public static string Serialize<T>(this T obj, bool binary = true, IEnumerable<Type> knownTypes = null)
        {
            var res = "";

            using (var ms = new MemoryStream())
            {
                obj.Serialize(ms, binary, knownTypes);

                ms.Seek(0, SeekOrigin.Begin);

                using (var sr = new StreamReader(ms))
                {
                    res = sr.ReadToEnd();
                }
            }

            return res;
        }

        /// <summary>
        /// serialize to file
        /// </summary>        
        public static void Serialize<T>(this T obj, string dstPathfilename, bool binary = true, IEnumerable<Type> knownTypes = null)
        {
            if (File.Exists(dstPathfilename)) File.Delete(dstPathfilename);

            using (var fs = new FileStream(dstPathfilename, FileMode.CreateNew))
            {
                obj.Serialize(fs, binary, knownTypes);
            }
        }

        /// <summary>
        /// serialize to stream
        /// </summary>        
        public static void Serialize<T>(this T obj, Stream stream, bool binary = true, IEnumerable<Type> knownTypes = null)
        {
            var settings = new DataContractSerializerSettings()
            {
                PreserveObjectReferences = true
            };

            if (knownTypes != null) settings.KnownTypes = knownTypes;

            var serializer = new DataContractSerializer(typeof(T), settings);

            if (binary)
            {
                using (var bw = XmlDictionaryWriter.CreateBinaryWriter(stream))
                {
                    serializer.WriteObject(bw, obj);
                }
            }
            else
            {
                var wrSettings = new XmlWriterSettings() { Indent = true };
                using (var tw = XmlWriter.Create(stream, wrSettings))
                {
                    serializer.WriteObject(tw, obj);
                }
            }
        }

        //-------------------------------------------------------------------


        /// <summary>
        /// Deserialize from string
        /// </summary>        
        public static T DeserializeString<T>(this string str, bool binary = true, IEnumerable<Type> knownTypes = null)
        {
            T res;

            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    sw.Write(str);
                    sw.Flush();

                    ms.Seek(0, SeekOrigin.Begin);
                    
                    res = ms.Deserialize<T>(binary, knownTypes);
                }
            }

            return res;
        }


        /// <summary>
        /// Deserialize from file
        /// </summary>        
        public static T DeserializeFile<T>(this string srcPathfilename, bool binary = true, IEnumerable<Type> knownTypes = null)
        {
            T res;

            using (var fs = new FileStream(srcPathfilename, FileMode.Open))
            {
                res = fs.Deserialize<T>(binary, knownTypes);
            }

            return res;
        }

        /// <summary>
        /// Deserialize from stream
        /// </summary>        
        public static T Deserialize<T>(this Stream stream, bool binary = true, IEnumerable<Type> knownTypes = null)
        {
            var settings = new DataContractSerializerSettings()
            {
                PreserveObjectReferences = true
            };

            if (knownTypes != null) settings.KnownTypes = knownTypes;

            var serializer = new DataContractSerializer(typeof(T), settings);

            T res;

            if (binary)
            {
                using (var br = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
                {
                    res = (T)serializer.ReadObject(br);
                }
            }
            else
            {
                using (var tr = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max))
                {
                    res = (T)serializer.ReadObject(tr);
                }
            }

            return res;
        }

    }

}

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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;
using Thirdy;

namespace SearchAThing
{

    public static partial class Dynamic
    {

        public static bool ContainsField(dynamic value, string field)
        {
            if (value is JObject)
            {
                var jo = (JObject)value;

                var q = jo.Children().Cast<JProperty>().ToList();

                return q.Any(w => w != null && w.Name == field);
            }

            return ((IDictionary<string, object>)value).ContainsKey(field);
        }

        /// <summary>
        /// safe retrieve dynamic string type
        /// </summary>        
        public static string GetString(dynamic value, string valueIfNull = "")
        {
            if (value == null || value is DBNull) return valueIfNull;

            return (string)value;
        }

        /// <summary>
        /// safe retrieve dynamic bool type
        /// </summary>        
        public static bool GetBool(dynamic value, bool valueIfNull = false)
        {
            if (value == null || value is DBNull) return valueIfNull;

            return (bool)value;
        }

        /// <summary>
        /// safe retrieve dynamic double type
        /// </summary>        
        public static double GetDouble(dynamic value, double valueIfNull = 0)
        {
            if (value == null || value is DBNull) return valueIfNull;

            return (double)value;
        }

        /// <summary>
        /// safe retrieve dynamic double type
        /// </summary>        
        public static double? GetNullableDouble(dynamic value)
        {
            if (value == null || value is DBNull) return new double?();

            return (double)value;
        }

        /// <summary>
        /// safe retrieve dynamic long type
        /// </summary>        
        public static long GetLong(dynamic value, long valueIfNull = 0L)
        {
            if (value == null || value is DBNull) return valueIfNull;

            return (long)value;
        }

        /// <summary>
        /// safe retrieve dynamic int type
        /// </summary>        
        public static int GetInt(dynamic value, int valueIfNull = 0)
        {
            if (value == null || value is DBNull) return valueIfNull;

            return (int)value;
        }

        static Type JValueType = typeof(Newtonsoft.Json.Linq.JValue);

        /// <summary>
        /// sweep dynamic array enumerating it
        /// </summary>        
        public static IEnumerable<T> Enum<T>(dynamic arr)
        {
            foreach (var x in arr)
            {
                if (x.GetType() == JValueType)
                    yield return x.Value;
                else
                    yield return x;
            }
        }

        /// <summary>
        /// sweep dynamic array enumerating it
        /// </summary>        
        public static List<T> ToList<T>(dynamic arr)
        {
            return ((IEnumerable<T>)Enum<T>(arr)).ToList();
        }

        /// <summary>
        /// sweep dynamic array
        /// </summary>        
        public static T[] ToArray<T>(dynamic arr)
        {
            return ((IEnumerable<T>)Enum<T>(arr)).ToArray();
        }

        /// <summary>
        /// sweep dynamic array enumerating it and retrieving an hashset
        /// </summary>        
        public static HashSet<T> ToHashSet<T>(dynamic arg)
        {
            return new HashSet<T>(ToList<T>(arg));
        }

        /// <summary>
        /// retrieve an expando object, ready for members add
        /// </summary>        
        public static IDictionary<string, object> NewExpandoObject()
        {
            return new ExpandoObject();
        }

        /// <summary>
        /// retrieve a new empty list of ExpandoObject type
        /// </summary>        
        public static List<IDictionary<string, object>> NewExpandoObjectList()
        {
            return new List<IDictionary<string, object>>();
        }

        /// <summary>
        /// convert given object to an ExpandoObject
        /// </summary>        
        public static ExpandoObject ToExpando(this object obj)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            var type = obj.GetType();

            foreach (var property in type.GetProperties()) expando.Add(property.Name, property.GetValue(obj));

            return expando as ExpandoObject;
        }

        /// <summary>
        /// convert given expando object to a JObject
        /// </summary>        
        public static JObject AsJObject(dynamic expando)
        {
            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(expando));
        }

        /// <summary>
        /// creates a dynamic from an anonymous lambda
        /// </summary>        
        public static dynamic Eval<T>(this Func<T> fn)
        {
            return ToExpando(fn());
        }

        public static string SerializeToJson<T>(this Func<T> fn)
        {
            return JsonConvert.SerializeObject(Eval(fn));
        }

        /// <summary>
        /// create a dynamic from given xml element
        /// this uses code from http://www.codeproject.com/Articles/461677/Creating-a-dynamic-object-from-XML-using-ExpandoOb
        /// </summary>        
        public static dynamic ParseXML(this XElement xml)
        {
            var root = new ExpandoObject();
            ExpandoObjectHelper.Parse(root, xml);
            return root;
        }

    }

}

namespace Thirdy
{

    #region http://www.codeproject.com/Articles/461677/Creating-a-dynamic-object-from-XML-using-ExpandoOb

    public static class ExpandoObjectHelper
    {
        private static List<string> KnownLists;
        public static void Parse(dynamic parent, XElement node, List<string> knownLists = null)
        {
            if (knownLists != null)
            {
                KnownLists = knownLists;
            }
            IEnumerable<XElement> sorted = from XElement elt in node.Elements()
                                           orderby node.Elements(elt.Name.LocalName).Count() descending
                                           select elt;

            if (node.HasElements)
            {
                int nodeCount = node.Elements(sorted.First().Name.LocalName).Count();
                bool foundNode = false;
                if (KnownLists != null && KnownLists.Count > 0)
                {
                    foundNode = (from XElement el in node.Elements()
                                 where KnownLists.Contains(el.Name.LocalName)
                                 select el).Count() > 0;
                }

                if (nodeCount > 1 || foundNode == true)
                {
                    // At least one of the child elements is a list
                    var item = new ExpandoObject();
                    List<dynamic> list = null;
                    string elementName = string.Empty;
                    foreach (var element in sorted)
                    {
                        if (element.Name.LocalName != elementName)
                        {
                            list = new List<dynamic>();
                            elementName = element.Name.LocalName;
                        }
                        if (element.HasElements ||
                            (KnownLists != null && KnownLists.Contains(element.Name.LocalName)))
                        {
                            Parse(list, element);
                            AddProperty(item, element.Name.LocalName, list);
                        }
                        else
                        {
                            Parse(item, element);
                        }
                    }

                    foreach (var attribute in node.Attributes())
                    {
                        AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                    }

                    AddProperty(parent, node.Name.ToString(), item);
                }
                else
                {
                    var item = new ExpandoObject();

                    foreach (var attribute in node.Attributes())
                    {
                        AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                    }

                    //element
                    foreach (var element in sorted)
                    {
                        Parse(item, element);
                    }
                    AddProperty(parent, node.Name.ToString(), item);
                }
            }
            else
            {
                AddProperty(parent, node.Name.ToString(), node.Value.Trim());
            }
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            if (parent is List<dynamic>)
            {
                (parent as List<dynamic>).Add(value);
            }
            else
            {
                (parent as IDictionary<string, object>)[name] = value;
            }
        }
    }
    #endregion

}

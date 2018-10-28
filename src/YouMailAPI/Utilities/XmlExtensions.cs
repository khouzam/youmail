/*************************************************************************************************
 * Copyright (c) 2018 Gilles Khouzam
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software withou
 * restriction, including without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
/*************************************************************************************************/

namespace MagikInfo.XmlSerializerExtensions
{
    using System.Diagnostics;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Xml.Serialization;

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }
    }

    public static class XmlSerializerExtensions
    {
        /// <summary>
        /// Use an XMLSerializer to convert from an object to its XML representation
        /// </summary>
        /// <typeparam name="T">The type of the object that we are serializing</typeparam>
        /// <returns>A string in UTF-8</returns>
        public static string ToXml<T>(this T type)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new Utf8StringWriter())
            {
                serializer.Serialize(writer, type);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Use an XMLSerializer to convert from an object to its XML representation
        /// </summary>
        /// <typeparam name="T">The type of the object that we are serializing</typeparam>
        /// <param name="stream">The stream to write to</param>
        public static void ToXml<T>(this T type, Stream stream)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                serializer.Serialize(writer, type);
            }
        }

        /// <summary>
        /// Serialize a type into an HttpContent type
        /// </summary>
        /// <typeparam name="T">The type to serialize</typeparam>
        /// <returns>An HttpContent object that represents the XML object</returns>
        public static HttpContent ToXmlHttpContent<T>(this T type)
        {
            return new StringContent(type.ToXml(), Encoding.UTF8, "text/xml");
        }

        /// <summary>
        /// Use an XMLSerializer to convert from an XML fragment to an object
        /// </summary>
        /// <typeparam name="T">The type of the object that we are serializing</typeparam>
        /// <returns>The newly created object</returns>
        public static T FromXml<T>(this string input) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            var reader = new StringReader(input);
            return serializer.Deserialize(reader) as T;
        }

        /// <summary>
        /// Use an XMLSerializer to convert from an XML fragment to an object
        /// </summary>
        /// <typeparam name="T">The type of the object that we are serializing</typeparam>
        /// <returns>The newly created object</returns>
        public static T FromXml<T>(this Stream stream) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            return serializer.Deserialize(stream) as T;
        }

        /// <summary>
        ///  Uses an XMLSerializer to convert from an XML fragment to an object
        ///  and outputs the XML content to the Debug output.
        /// </summary>
        /// <typeparam name="T">The type of the object that we are serializing</typeparam>
        /// <returns>The newly created object</returns>
        public static T FromXmlDebug<T>(this Stream stream) where T : class
        {
            var reader = new StreamReader(stream);
            var xmlString = reader.ReadToEnd();
            Debug.WriteLine(xmlString);
            return xmlString.FromXml<T>();
        }
    }
}

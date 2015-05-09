/**
/// HashTag.Core Library
/// Copyright © 2010-2014
///
/// This module is Copyright © 2010-2014 Steve Powell
/// All rights reserved.
///
/// This library is free software; you can redistribute it and/or
/// modify it under the terms of the Microsoft Public License (Ms-PL)
/// 
/// This library is distributed in the hope that it will be
/// useful, but WITHOUT ANY WARRANTY; without even the implied
/// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
/// PURPOSE.  See theMicrosoft Public License (Ms-PL) License for more
/// details.
///
/// You should have received a copy of the Microsoft Public License (Ms-PL)
/// License along with this library; if not you may 
/// find it here: http://www.opensource.org/licenses/ms-pl.html
///
/// Steve Powell, hashtagdonet@gmail.com
**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization.Formatters.Binary;
using HashTag.IO;
using System.Xml;
using System.IO.Compression;
using HashTag.Text;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json;
using System.Dynamic;
using Newtonsoft.Json.Converters;

namespace HashTag
{
    public static partial class Serialize
    {
        /// <summary>
        /// Rehydrate objects from string representation
        /// </summary>
        public static class From
        {
            /// <summary>
            /// Hydrateds a .Net object from a BSON serialized object
            /// </summary>
            /// <typeparam name="T">Type of object that is being created</typeparam>
            /// <param name="bsonSerializedObject">Bytes that were serialized</param>
            /// <returns>Hydrated object</returns>
            public static T BSON<T>(byte[] bsonSerializedObject)
            {
                using (var inputStream = new MemoryStream(bsonSerializedObject))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    BsonReader reader = new BsonReader(inputStream);
                    return serializer.Deserialize<T>(reader);
                }
            }

            /// <summary>
            /// Hydrateds a .Net object from a BSON serialized object
            /// </summary>
            /// <typeparam name="T">Type of object that is being created</typeparam>
            /// <param name="base64BSONString">String representation of object</param>
            /// <returns>Hydrated object</returns>
            public static T BSONBase64<T>(string base64BSONString)
            {
                return BSON<T>(Convert.FromBase64String(base64BSONString));
            }


            /// <summary>
            /// Create a new object based on input JSON string
            /// </summary>
            /// <typeparam name="T">Type to convert string into</typeparam>
            /// <param name="json">Hydrated object in JSON format</param>
            /// <returns>newly created object</returns>
            public static T Json<T>(string json) 
            {
                return JsonConvert.DeserializeObject<T>(json);
            }

            /// <summary>
            /// Deserialize into a dynamic object from Json
            /// </summary>
            /// <param name="json"></param>
            /// <returns></returns>
            public static dynamic Json(string json)
            {
                var converter = new ExpandoObjectConverter();
                return JsonConvert.DeserializeObject<ExpandoObject>(json, converter);
            }

            /// <summary>
            /// Decompress an array of bytes
            /// </summary>
            /// <param name="obj">Bytes that were gzip compressd</param>
            /// <returns>Array of compressed bytes</returns>
            public static byte[] Compressed(byte[] compressedBytes)
            {
                using (MemoryStream outputStream = new MemoryStream()) //see http://blogs.msdn.com/b/bclteam/archive/2006/05/10/592551.aspx
                {
                    using (GZipStream stream = new GZipStream(new MemoryStream(compressedBytes), CompressionMode.Decompress))
                    {
                        const int size = 4096;
                        byte[] buffer = new byte[size];

                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                outputStream.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        stream.Flush();
                        stream.Close();
                        
                    }
                    outputStream.Flush();
                    return outputStream.ToArray();
                }
            }

            /// <summary>
            /// Decompress an array of bytes
            /// </summary>
            /// <param name="obj">Bytes that were gzip compressd</param>
            /// <returns>Array of compressed bytes</returns>
            public static T Compressed<T>(byte[] compressedBytes)
            {
                return Binary<T>(Compressed(compressedBytes));
            }

            /// <summary>
            /// Compress an object into zip format
            /// </summary>
            /// <param name="compressedBase64">String representation of compressed bytes</param>
            /// <returns>String of Base64 characters of compressed object</returns>
            public static T CompressedBase64<T>(string compressedBase64)
            {
                var uncompressedBytes = Compressed(Convert.FromBase64String(compressedBase64));
                return Binary<T>(uncompressedBytes);
            }

            /// <summary>
            /// Create a new object based on input byte array
            /// </summary>
            /// <typeparam name="T">Type to convert byte array to</typeparam>
            /// <param name="binaryObject">Hydrated object in binary format</param>
            /// <returns>Newly created object</returns>
            public static T Binary<T>(byte[] binaryObject)
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    BinaryFormatter binForm = new BinaryFormatter();
                    memStream.Write(binaryObject, 0, binaryObject.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    return (T)binForm.Deserialize(memStream);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="serializedObject"></param>
            /// <returns></returns>
            public static T Xml<T>(string serializedObject)
            {
                return Xml<T>(Transform.StringToUTF8ByteArray(serializedObject));
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="serializedObject"></param>
            /// <returns></returns>
            public static T Xml<T>(byte[] serializedObject)
            {
                return (T)Xml(serializedObject, typeof(T));
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="serializedObject"></param>
            /// <param name="targetObjectType"></param>
            /// <returns></returns>
            public static object Xml(byte[] serializedObject, Type targetObjectType)
            {
                XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
                xmlnsEmpty.Add("", "");
                XmlSerializer xs = new XmlSerializer(targetObjectType);

                using (MemoryStream memoryStream = new MemoryStream(serializedObject))
                {
                    return xs.Deserialize(memoryStream);
                }
            }
            /// <summary>
            /// Reconstruct object from XML (SOAP format)
            /// </summary>
            /// <param name="soapXml">source string</param>
            /// <returns>Constructed object or null if not found</returns>
            public static T Soap<T>(string soapXml) where T : class
            {
                object obj = null;
                using (MemoryStream ms = new MemoryStream((new System.Text.ASCIIEncoding()).GetBytes(soapXml)))
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    SoapFormatter sf = new SoapFormatter(null, new StreamingContext(StreamingContextStates.Persistence));
                    obj = sf.Deserialize(ms);
                }
                return (T)obj;
            }

            /// <summary>
            /// Convert a serialized string back into hydrated object
            /// </summary>
            /// <typeparam name="T">Type to convert string into</typeparam>
            /// <param name="dcObject">Previously serialized object using DataContractSerializer</param>
            /// <returns>Serialized object or null if <paramref name="dcObject"/> is null</returns>            
            [Citation("http://msmvps.com/Blogs/PeterRitchie/", Author = "Peter Ritchie", CitationType = CitationType.AllSource, SourceDate = "2009-04-04 21:14")]
            public static T DataContract<T>(string dcObject) where T : class
            {
                MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(dcObject));
                using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(memoryStream, Encoding.Unicode, new XmlDictionaryReaderQuotas(), null))
                {
                    DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T));
                    return dataContractSerializer.ReadObject(reader) as T;
                }
            }
        }
    }
}
/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Newtonsoft.Json;

    public static class Serialization
    {
        public static T CloneBinary<T>(T objectToClone)
            where T : class
        {
            if (objectToClone == null) return null;

            using (var objectStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, objectToClone);
                objectStream.Seek(0, SeekOrigin.Begin);

                return (T)formatter.Deserialize(objectStream);
            }
        }

        public static T CloneJson<T>(T objectToClone)
            where T : class
        {
            var serialized = ToJson(objectToClone);

            var objectCloned = FromJson(serialized, objectToClone.GetType());

            return objectCloned as T;
        }

        public static String ToJson(Object objectToSerialize, bool indented = false)
        {
            var jsonSerializer
                = JsonSerializer.Create(
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = indented ? Formatting.Indented : Formatting.None
                });

            using (var textWriter = new StringWriter())
            {
                jsonSerializer.Serialize(textWriter, objectToSerialize);

                var json = textWriter.ToString();

                return json;
            }
        }

        public static T FromJson<T>(String json)
        {
            return (T)FromJson(json, typeof(T));
        }


        public static Object FromJson(String json, Type type, bool createDefaultOnError = true)
        {
            try
            {
                var jsonSerializer
                    = JsonSerializer.Create(
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                using (var textReader = new StringReader(json))
                using (var jsonReader = new JsonTextReader(textReader))
                {

                    var objectDeserialzed
                        = jsonSerializer.Deserialize(jsonReader, type);

                    return objectDeserialzed;
                }
            }
            catch
            {
                if (createDefaultOnError)
                {
                    return CreateDefault(type);
                }

                return null;
            }
        }

        public static Object CreateDefault(Type type)
        {
            if (!type.IsValueType)
            {
                return null;

            }

            return Activator.CreateInstance(type);
        }

        public static MemoryStream ToMemoryStream(Object objectToSerialize)
        {
            if (objectToSerialize == null) return null;

            using (var objectStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, objectToSerialize);

                return objectStream;
            }
        }

        public static byte[] ToBinary(Object objectToSerialize)
        {
            if (objectToSerialize == null) return null;

            using (var memoryStream = ToMemoryStream(objectToSerialize))
            {
                return memoryStream.ToArray();
            }
        }

        public static byte[] ToBinary(Stream stream)
        {
            if (stream == null) return null;

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static T FromBinary<T>(Byte[] objectToDeserialize)
            where T : class
        {

            if (objectToDeserialize == null) return null;

            try
            {
                using (var objectStream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();

                    objectStream.Write(objectToDeserialize, 0, objectToDeserialize.Length);

                    objectStream.Seek(0, SeekOrigin.Begin);

                    return formatter.Deserialize(objectStream) as T;
                }
            }
            catch
            {
                return null;
            }
        }

        public static bool EqualsJson(Object item1, Object item2)
        {
            var item1Json = ToJson(item1);
            var item2Json = ToJson(item2);

            return item1Json.Equals(item2Json);
        }

    }
}

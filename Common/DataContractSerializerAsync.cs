/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common
{
    using System;
    using System.IO;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Serialization;

    public static class DataContractSerializerAsync
    {
        public static String XmlSerialize(Object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                var dataContractSerializer = new DataContractSerializer(obj.GetType());
                dataContractSerializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        public static T XmlDeserialize<T>(String xml)
        {
            using (var memoryStream = new MemoryStream())
            {
                var data = System.Text.Encoding.UTF8.GetBytes(xml);
                memoryStream.Write(data, 0, data.Length);
                memoryStream.Position = 0;
                var dataContractSerializer = new DataContractSerializer(typeof(T));
                return (T)dataContractSerializer.ReadObject(memoryStream);
            }
        }

        public static IAsyncResult BeginXmlSerialize(
            Object serializationTarget,
            AsyncCallback asyncCallback,
            Object state)
        {
            Func<Object, String> func = XmlSerialize;

            var iAsynResult = func.BeginInvoke(serializationTarget, asyncCallback, state);

            return iAsynResult;
        }

        public static String EndXmlSerialize(IAsyncResult iAsyncResult)
        {
            var asyncResult = (AsyncResult)iAsyncResult;

            var asyncDelegate = (Func<Object, String>)asyncResult.AsyncDelegate;

            return asyncDelegate.EndInvoke(iAsyncResult);
        }

        public static IAsyncResult BeginXmlDeserialize<T>(
            String serializedTarget,
            AsyncCallback asyncCallback,
            Object state)
        {
            Func<String, T> func = XmlDeserialize<T>;

            var iAsynResult = func.BeginInvoke(serializedTarget, asyncCallback, state);

            return iAsynResult;
        }

        public static T EndXmlDeserialize<T>(IAsyncResult iAsyncResult)
        {
            var asyncResult = (AsyncResult)iAsyncResult;

            var asyncDelegate = (Func<String, T>)asyncResult.AsyncDelegate;

            var result = asyncDelegate.EndInvoke(iAsyncResult);

            return result;
        }
    }
}

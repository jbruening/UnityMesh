using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityModel
{
    public interface IObjectSerializer
    {
        void Serialize(Object obj, ref BinaryWriter writer, SerializerFactory serializerFactory);

        /// <summary>
        /// Deserialize from the stream
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="parent">The "container" for this object. Gameobject for components, parent for transforms</param>
        /// <param name="serializerFactory"></param>
        /// <returns></returns>
        Object Deserialize(ref BinaryReader reader, Object parent, SerializerFactory serializerFactory);
        /// <summary>
        /// unique identifier for T
        /// </summary>
        int TypeID { get; }

        Type Type { get; }
    }

    /// <summary>
    /// a bit more rigidity to classes that would like to implement the interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AObjectSerializer<T> : IObjectSerializer where T : Object
    {
        void IObjectSerializer.Serialize(Object obj, ref BinaryWriter writer, SerializerFactory serializerFactory)
        {
            Serialize(obj as T, ref writer, serializerFactory);
        }

        Object IObjectSerializer.Deserialize(ref BinaryReader reader, Object parent, SerializerFactory serializerFactory)
        {
            return Deserialize(ref reader, parent, serializerFactory);
        }

        protected abstract T Deserialize(ref BinaryReader reader, Object parent, SerializerFactory serializer);
        protected abstract void Serialize(T obj, ref BinaryWriter writer, SerializerFactory serializer);

        public abstract int TypeID { get; }
        public Type Type { get { return typeof (T); } }
    }
}
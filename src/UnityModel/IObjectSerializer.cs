using System;
using System.IO;
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
}
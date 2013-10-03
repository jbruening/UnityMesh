using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityModel
{
    public class GameObjectSerializer : IObjectSerializer
    {
        void IObjectSerializer.Serialize(Object obj, ref BinaryWriter writer, SerializerFactory serializerFactory)
        {
            var gobj = obj as GameObject;
            writer.Write(gobj.name);

            //let transform deal with heirarchy and components attached
            serializerFactory.InternalSerialize(gobj.transform, ref writer);
        }

        Object IObjectSerializer.Deserialize(ref BinaryReader reader, Object parent, SerializerFactory serializerFactory)
        {
            var gobj = new GameObject();
            gobj.name = reader.ReadString();

            //go right on to our transform
            serializerFactory.InternalDeserialize(ref reader, gobj);
            return gobj;
        }

        int IObjectSerializer.TypeID { get { return 0; } }
        Type IObjectSerializer.Type { get { return typeof (GameObject); } }
    }
}
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityModel
{
    public class GameObject : IObjectSerializer<UnityEngine.GameObject>
    {

        public void Serialize(Object obj, ref BinaryWriter writer, SerializerFactory serializerFactory)
        {
            var gobj = obj as UnityEngine.GameObject;
            writer.Write(gobj.name);

            //let transform deal with heirarchy and components attached
            serializerFactory.InternalSerialize(gobj.transform, ref writer);
        }

        UnityEngine.GameObject IObjectSerializer<UnityEngine.GameObject>.Deserialize(ref BinaryReader reader, Object parent, SerializerFactory serializerFactory)
        {
            var gobj = new UnityEngine.GameObject();
            gobj.name = reader.ReadString();

            //go right on to our transform
            serializerFactory.InternalDeserialize(ref reader, gobj);
            return gobj;
        }

        public int TypeID { get { return 0; } }
    }
}
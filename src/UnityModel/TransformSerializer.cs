using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityModel
{
    public class TransformSerializer : IObjectSerializer
    {
        void IObjectSerializer.Serialize(Object obj, ref BinaryWriter writer, SerializerFactory serializerFactory)
        {
            var trans = obj as Transform;

            //my id
            writer.Write(serializerFactory.ReferenceMap[trans]);

            //parent id
            if (trans.parent == null)
                writer.Write(-1);
            else
                writer.Write(serializerFactory.ReferenceMap[trans.parent]);

            writer.Write(trans.localPosition);
            writer.Write(trans.localRotation);
            writer.Write(trans.localScale);

            var components = trans.GetComponents(typeof (Component));
            writer.Write(components.Length);

            foreach (var comp in components)
            {
                serializerFactory.InternalSerialize(comp, ref writer);
            }

            writer.Write(trans.childCount);

            foreach (Transform child in trans)
            {
                SerializerFactory.Instance.InternalSerialize(child.gameObject, ref writer);
            }
        }

        Object IObjectSerializer.Deserialize(ref BinaryReader reader, Object parent, SerializerFactory serializerFactory)
        {
            var par = parent as GameObject;
            var trans = par.transform;
            //my id
            reader.ReadInt32();

            var parId = reader.ReadInt32();
            if (parId != -1)
            {
                trans.parent = serializerFactory.ReferenceMap[parId] as Transform;
            }

            trans.localPosition = reader.ReadVector3();
            trans.localRotation = reader.ReadQuaternion();
            trans.localScale = reader.ReadVector3();

            var compCount = reader.ReadInt32();
            for (int i = 0; i < compCount; i++)
            {
                serializerFactory.InternalDeserialize(ref reader, par);
            }

            var childCount = reader.ReadInt32();
            for (int i = 0; i < childCount; i++)
            {
                serializerFactory.InternalDeserialize(ref reader, par);
            }

                return trans;
        }

        int IObjectSerializer.TypeID { get { return 1; } }
        Type IObjectSerializer.Type { get { return typeof (Transform); } }
    }
}

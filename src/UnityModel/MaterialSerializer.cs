using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityModel
{
    class MaterialSerializer : IObjectSerializer
    {
        void IObjectSerializer.Serialize(Object obj, ref BinaryWriter writer, SerializerFactory serializerFactory)
        {
            throw new NotImplementedException();
        }

        Object IObjectSerializer.Deserialize(ref BinaryReader reader, Object parent, SerializerFactory serializerFactory)
        {
            throw new NotImplementedException();
        }

        int IObjectSerializer.TypeID { get { return 2; } }
        Type IObjectSerializer.Type { get { return typeof (Material); } }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Object = UnityEngine.Object;

namespace UnityModel
{
    class Material : IObjectSerializer<UnityEngine.Material>
    {
        public void Serialize(Object obj, ref BinaryWriter writer, SerializerFactory serializerFactory)
        {
            
        }

        public UnityEngine.Material Deserialize(ref BinaryReader reader, Object parent, SerializerFactory serializerFactory)
        {
            throw new NotImplementedException();
        }

        public int TypeID { get; private set; }
    }
}

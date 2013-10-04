using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityModel
{
    class MaterialSerializer : AObjectSerializer<Material>
    {
        protected override void Serialize(Material obj, ref BinaryWriter writer, SerializerFactory serializerFactory)
        {
            //TODO
        }

        protected override Material Deserialize(ref BinaryReader reader, Object parent, SerializerFactory serializerFactory)
        {
            //TODO
            return null;
        }

        public override int TypeID { get { return 2; } }
    }
}

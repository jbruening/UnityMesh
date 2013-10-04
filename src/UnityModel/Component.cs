using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityModel
{
    public class ComponentSerializer : AObjectSerializer<Component>
    {
        protected override Component Deserialize(ref BinaryReader reader, Object parent, SerializerFactory serializer)
        {
            throw new NotImplementedException();
        }

        public override int TypeID
        {
            get { return 3; }
        }

        protected override void Serialize(Component obj, ref BinaryWriter writer, SerializerFactory serializer)
        {
            throw new NotImplementedException();
        }
    }
}

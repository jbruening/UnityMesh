using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityModel
{
    public class SkinnedMeshSerializer : AObjectSerializer<SkinnedMeshRenderer>
    {
        public bool StoreInFile = true;
        protected override SkinnedMeshRenderer Deserialize(ref BinaryReader reader, Object parent, SerializerFactory serializer)
        {
            throw new NotImplementedException();
        }

        public override int TypeID
        {
            get { return 4; }
        }

        protected override void Serialize(SkinnedMeshRenderer obj, ref BinaryWriter writer, SerializerFactory serializer)
        {
            //UnityMesh.UnityMeshFile.Write(writer, obj.sharedMesh);
        }
    }
}

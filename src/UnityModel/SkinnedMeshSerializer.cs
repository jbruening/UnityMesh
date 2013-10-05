using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityMesh;
using Object = UnityEngine.Object;

namespace UnityModel
{
    public class SkinnedMeshSerializer : AObjectSerializer<SkinnedMeshRenderer>
    {
        /// <summary>
        /// store the mesh inside the model file if true.
        /// Otherwise, store in a separate file - NOT IMPLEMENTED
        /// </summary>
        public bool StoreInline = true;
        private FileConfig meshConfig = FileConfig.DefaultConfig();

        protected override SkinnedMeshRenderer Deserialize(ref BinaryReader reader, Object parent, SerializerFactory serializer)
        {
            Stream stream = null;
            if (StoreInline)
            {
                stream = reader.BaseStream;
            }
            else
            {
                //TODO: write relative file path
                throw new NotImplementedException("Cannot read meshes from a separate file yet");
            }

            var skin = (parent as Transform).gameObject.AddComponent<SkinnedMeshRenderer>();

            var boneCount = reader.ReadInt32();
            var bones = new Transform[boneCount];
            for (int i = 0; i < bones.Length; i++)
            {
                var boneID = reader.ReadInt32();
                bones[i] = serializer.ReferenceMap[boneID] as Transform;
            }

            var materialCount = reader.ReadInt32();
            var materials = new Material[materialCount];
            //todo: serialize materials
            for (int i = 0; i < materialCount; i++)
            {
                materials[i] = serializer.InternalDeserialize(ref reader, skin) as Material;
            }

            var quality = (SkinQuality) reader.ReadInt32();
            var bounds = reader.ReadBounds();

            var mesh = UnityMeshFile.Read(stream, meshConfig);
            
            skin.sharedMesh = mesh;
            skin.bones = bones;
            skin.quality = quality;
            skin.localBounds = bounds;

            return skin;
        }

        public override int TypeID
        {
            get { return 4; }
        }

        protected override void Serialize(SkinnedMeshRenderer obj, ref BinaryWriter writer, SerializerFactory serializer)
        {
            Stream stream = null;
            if (StoreInline)
            {
                stream = writer.BaseStream;
            }
            else
            {
                //TODO: write relative file path
                throw new NotImplementedException("Cannot store meshes in a separate file yet");
            }

            //store bones
            writer.Write(obj.bones.Length);
            foreach (var bone in obj.bones)
            {
                writer.Write(serializer.ReferenceMap[bone]);
            }

            
            writer.Write(obj.sharedMaterials.Length);
            foreach (var material in obj.sharedMaterials)
            {
                serializer.InternalSerialize(material, ref writer);
            }

            writer.Write((int)obj.quality);
            writer.Write(obj.localBounds);
            
            //store mesh
            UnityMeshFile.Write(stream, obj.sharedMesh, meshConfig);
        }
    }
}

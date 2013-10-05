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
        /// <summary>
        /// the property names of colors to serialize
        /// </summary>
        public string[] Colors = new string[]{};
        /// <summary>
        /// the property names of textures to serialize
        /// </summary>
        public string[] Textures = new string[]{};
        /// <summary>
        /// the property names of floats to serialize
        /// </summary>
        public string[] Floats = new string[]{};
        /// <summary>
        /// the property names of matrices to serialize
        /// </summary>
        public string[] Matrices = new string[]{};
        protected override void Serialize(Material obj, ref BinaryWriter writer, SerializerFactory serializerFactory)
        {
            writer.Write(obj.shader.name);

            writer.Write(Colors.Length);
            writer.Write(Textures.Length);
            writer.Write(Floats.Length);
            writer.Write(Matrices.Length);

            foreach (var c in Colors)
            {
                //we write the property names so others can deserialize
                writer.Write(c);
                writer.Write(obj.GetColor(c));
            }
            
            foreach (var t in Textures)
            {
                writer.Write(t);
                var tex = obj.GetTexture(t);
                if (tex)
                    //todo: write a better texture path
                    writer.Write(tex.name);
                else
                    writer.Write("");
            }
            
            foreach (var f in Floats)
            {
                //todo: write float values
            }
            
            foreach (var m in Matrices)
            {
                //todo: write matrix values
            }
        }

        protected override Material Deserialize(ref BinaryReader reader, Object parent, SerializerFactory serializerFactory)
        {
            var shaderName = reader.ReadString();
            var shader = Shader.Find(shaderName);
            var mat = new Material(shader);

            var colorCount = reader.ReadInt32();
            var textureCount = reader.ReadInt32();
            var floatCount = reader.ReadInt32();
            var matrixCount = reader.ReadInt32();

            string prop;
            for (int i = 0; i < colorCount; i++)
            {
                prop = reader.ReadString();
                mat.SetColor(prop, reader.ReadColor());
            }
            for (int i = 0; i < textureCount; i++)
            {
                prop = reader.ReadString();
                var texPath = reader.ReadString();
                //TODO: load texture from path
            }
            for (int i = 0; i < floatCount; i++)
            {
                prop = reader.ReadString();
            }
            for (int i = 0; i < matrixCount; i++)
            {
                prop = reader.ReadString();
            }

            return mat;
        }

        public override int TypeID { get { return 2; } }
    }
}

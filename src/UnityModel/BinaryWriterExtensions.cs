using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityModel
{
    public static class BinaryWriterExtensions
    {
        public static void Write(this BinaryWriter writer, Vector3 vector3)
        {
            writer.Write(vector3.x);
            writer.Write(vector3.y);
            writer.Write(vector3.z);
        }
        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public static void Write(this BinaryWriter writer, Bounds bounds)
        {
            writer.Write(bounds.center);
            writer.Write(bounds.size);
        }
        public static Bounds ReadBounds(this BinaryReader reader)
        {
            return new Bounds(reader.ReadVector3(), reader.ReadVector3());
        }

        public static void Write(this BinaryWriter writer, Quaternion quaternion)
        {
            writer.Write(quaternion.x);
            writer.Write(quaternion.y);
            writer.Write(quaternion.z);
            writer.Write(quaternion.w);
        }
        public static Quaternion ReadQuaternion(this BinaryReader reader)
        {
            return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public static void Write(this BinaryWriter writer, Color color)
        {
            Color32 col = color;
            var bytes = new[] {col.r, col.g, col.b, col.a};
            writer.Write(bytes);
        }
        public static Color ReadColor(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            return new Color32(bytes[0], bytes[1], bytes[2], bytes[3]);
        }
    }
}

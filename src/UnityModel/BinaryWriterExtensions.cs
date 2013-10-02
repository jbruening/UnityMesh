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
    }
}

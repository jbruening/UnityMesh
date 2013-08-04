using System;
using UnityEngine;

namespace UnityMesh
{
    /// <summary>
    /// Chunk processor for mesh normals
    /// </summary>
    public class NormalsChunk : Chunk
    {
        const int VECTOR3_SIZE = sizeof(float) * 3;

        protected override void ReadChunk(byte[] chunkData, ref Mesh mesh)
        {
            var chunkLength = chunkData.Length;
            var normals = new Vector3[chunkLength / VECTOR3_SIZE];
            var floats = new float[chunkLength / sizeof(float)];
            Buffer.BlockCopy(chunkData, 0, floats, 0, chunkLength);
            for (int i = 0; i < floats.Length; i += 3)
            {
                normals[i / 3] = new Vector3(floats[i], floats[i + 1], floats[i + 2]);
            }

            mesh.normals = normals;
        }

        protected override void WriteChunk(out byte[] chunkData, Mesh mesh)
        {
            var normals = mesh.normals;
            var chunkLength = normals.Length * VECTOR3_SIZE;
            chunkData = new byte[chunkLength];
            var floats = new float[normals.Length * 3];

            for (int i = 0; i < floats.Length; i+=3)
            {
                var vert = normals[i / 3];
                floats[i] = vert.x;
                floats[i + 1] = vert.y;
                floats[i + 2] = vert.z;
            }
            Buffer.BlockCopy(floats, 0, chunkData, 0, chunkLength);
        }

        protected override ushort ChunkID
        {
            get { return 5; }
        }
    }
}
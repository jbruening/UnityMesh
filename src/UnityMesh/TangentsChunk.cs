using System;
using UnityEngine;

namespace UnityMesh
{
    public class TangentsChunk : Chunk
    {
        const int VECTOR4_SIZE = sizeof(float) * 3;

        protected override void ReadChunk(byte[] chunkData, ref Mesh mesh)
        {
            var chunkLength = chunkData.Length;
            var tangents = new Vector4[chunkLength / VECTOR4_SIZE];
            var floats = new float[chunkLength / sizeof(float)];
            Buffer.BlockCopy(chunkData, 0, floats, 0, chunkLength);
            for (int i = 0; i < floats.Length; i += 4)
            {
                tangents[i / 4] = new Vector4(floats[i], floats[i + 1], floats[i + 2], floats[i+3]);
            }

            mesh.tangents = tangents;
        }

        protected override void WriteChunk(out byte[] chunkData, Mesh mesh)
        {
            var tangents = mesh.tangents;
            var chunkLength = tangents.Length * VECTOR4_SIZE;
            chunkData = new byte[chunkLength];
            var floats = new float[tangents.Length * 4];

            for (int i = 0; i < floats.Length; i+=4)
            {
                var vert = tangents[i / 4];
                floats[i] = vert.x;
                floats[i + 1] = vert.y;
                floats[i + 2] = vert.z;
                floats[i + 3] = vert.w;
            }
            Buffer.BlockCopy(floats, 0, chunkData, 0, chunkLength);
        }

        protected override ushort ChunkID
        {
            get { return 6; }
        }
    }
}
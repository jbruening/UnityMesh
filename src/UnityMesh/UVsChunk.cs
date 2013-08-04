using System;
using UnityEngine;

namespace UnityMesh
{
    public class UVsChunk : Chunk
    {
        const int UV_SIZE = sizeof(float) * 2;
        private const int HEADER_SIZE = 8;

        protected override void ReadChunk(byte[] chunkData, ref Mesh mesh)
        {
            //number of bytes that are for uv1
            var uv1Count = BitConverter.ToInt32(chunkData, 0);
            //number of bytes that are for uv2
            var uv2Count = BitConverter.ToInt32(chunkData, sizeof(int));

            var uv1Floats = new float[uv1Count/sizeof (float)];
            var uv2Floats = new float[uv2Count/sizeof (float)];
            Buffer.BlockCopy(chunkData, HEADER_SIZE, uv1Floats, 0, uv1Count);
            Buffer.BlockCopy(chunkData, HEADER_SIZE + uv1Count, uv2Floats, 0, uv2Count);

            var uv1 = new Vector2[uv1Floats.Length/2];
            var uv2 = new Vector2[uv2Floats.Length/2];

            for (int i = 0; i < uv1Floats.Length; i += 2)
            {
                uv1[i / 2] = new Vector2(uv1Floats[i], uv1Floats[i + 1]);
            }
            for (int i = 0; i < uv2Floats.Length; i += 2)
            {
                uv2[i / 2] = new Vector2(uv2Floats[i], uv2Floats[i + 1]);
            }

            //mesh.uv1 and mesh.uv2 are the second uv set
            mesh.uv = uv1;
            mesh.uv1 = uv2;
        }

        protected override void WriteChunk(out byte[] chunkData, Mesh mesh)
        {
            //mesh.uv1 and mesh.uv2 are the second uv set
            var uv1 = mesh.uv;
            var uv2 = mesh.uv1;
            var chunkLength = uv1.Length*UV_SIZE + uv2.Length*UV_SIZE + HEADER_SIZE;
            chunkData = new byte[chunkLength];

            var uv1Floats = new float[uv1.Length*2];
            var uv2Floats = new float[uv2.Length*2];
            for (var i = 0; i < uv1Floats.Length; i += 2)
            {
                var vert = uv1[i / 2];
                uv1Floats[i] = vert.x;
                uv1Floats[i + 1] = vert.y;
            }
            for (var i = 0; i < uv2Floats.Length; i += 2)
            {
                var vert = uv2[i / 2];
                uv2Floats[i] = vert.x;
                uv2Floats[i + 1] = vert.y;
            }

            //do actual writing
            //byte counts
            var uv1Count = uv1Floats.Length*sizeof (float);
            var uv2Count = uv2Floats.Length*sizeof (float);
            //header
            CopyBytes(uv1Count, chunkData, 0);
            CopyBytes(uv2Count, chunkData, sizeof(int));
            //data
            Buffer.BlockCopy(uv1Floats, 0, chunkData, HEADER_SIZE, uv1Count);
            Buffer.BlockCopy(uv2Floats, 0, chunkData, HEADER_SIZE + uv1Count, uv2Count);
        }

        protected override ushort ChunkID
        {
            get { return 2; }
        }
    }
}
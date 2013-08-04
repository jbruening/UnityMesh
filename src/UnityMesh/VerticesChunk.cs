using System;
using UnityEngine;

namespace UnityMesh
{
    public class VerticesChunk : Chunk
    {
        const int VECTOR3_SIZE = sizeof(float) * 3;

        protected override void ReadChunk(byte[] chunkData, ref Mesh mesh)
        {
            var chunkLength = chunkData.Length;
            var vertices = new Vector3[chunkLength / VECTOR3_SIZE];
            var floats = new float[chunkLength / sizeof(float)];
            Buffer.BlockCopy(chunkData, 0, floats, 0, chunkLength);
            for (var i = 0; i < floats.Length; i += 3)
            {
                vertices[i / 3] = new Vector3(floats[i], floats[i + 1], floats[i + 2]);
            }

            mesh.vertices = vertices;
        }

        protected override void WriteChunk(out byte[] chunkData, Mesh mesh)
        {
            var chunkLength = mesh.vertexCount * VECTOR3_SIZE;
            chunkData = new byte[chunkLength];
            var floats = new float[mesh.vertexCount*3];

            var verts = mesh.vertices;
            for (var i = 0; i < floats.Length; i+=3)
            {
                var vert = verts[i/3];
                floats[i] = vert.x;
                floats[i + 1] = vert.y;
                floats[i + 2] = vert.z;
            }
            Buffer.BlockCopy(floats, 0, chunkData, 0, chunkLength);
        }

        protected override ushort ChunkID
        {
            get { return 1; }
        }
    }
}
using System;
using UnityEngine;

namespace UnityMesh
{
    public class TrianglesChunk : Chunk
    {
        const int TRI_SIZE = sizeof(int);

        protected override void ReadChunk(byte[] chunkData, ref Mesh mesh)
        {
            var chunkLength = chunkData.Length;
            var tris = new int[chunkLength/TRI_SIZE];
            Buffer.BlockCopy(chunkData, 0, tris, 0, chunkLength);
            mesh.triangles = tris;
        }

        protected override void WriteChunk(out byte[] chunkData, Mesh mesh)
        {
            var tris = mesh.triangles;
            var chunkLength = tris.Length*TRI_SIZE;
            chunkData = new byte[chunkLength];
            Buffer.BlockCopy(tris, 0, chunkData, 0, chunkLength);
        }

        protected override ushort ChunkID
        {
            get { return 3; }
        }
    }
}
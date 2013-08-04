using System;
using UnityEngine;

namespace UnityMesh
{
    /// <summary>
    /// Used if the mesh has more than 1 material
    /// </summary>
    public class SubMeshChunk : Chunk
    {
        const int INDEX_SIZE = sizeof(int);
        private const int FIXED_HEADER_SIZE = sizeof(int);

        protected override void ReadChunk(byte[] chunkData, ref Mesh mesh)
        {
            //header
            var subMeshCount = BitConverter.ToInt32(chunkData, 0);
            mesh.subMeshCount = subMeshCount;

            var headerSize = FIXED_HEADER_SIZE + subMeshCount*4;
            
            //read each submesh
            var chunkPosition = headerSize;
            for(var i = 0; i < subMeshCount; i++)
            {
                //read sizes out from header
                var subMesh = new int[BitConverter.ToInt32(chunkData, FIXED_HEADER_SIZE + (i*4))];
                //size of submesh in bytes
                var subMeshSize = subMesh.Length*INDEX_SIZE;
                //and then actually copy data
                Buffer.BlockCopy(chunkData, chunkPosition, subMesh, 0, subMeshSize);
                chunkPosition += subMeshSize;

                mesh.SetTriangles(subMesh, i);
            }
        }

        protected override void WriteChunk(out byte[] chunkData, Mesh mesh)
        {
            //get some data about what we have to write
            var subMeshes = new int[mesh.subMeshCount][];
            var headerSize = FIXED_HEADER_SIZE + subMeshes.Length * sizeof(int);
            int chunkLength = headerSize;
            for(var i = 0; i < subMeshes.Length; i++)
            {
                subMeshes[i] = mesh.GetTriangles(i);
                chunkLength += subMeshes[i].Length * INDEX_SIZE;
            }
            chunkData = new byte[chunkLength];

            //write first part of header
            Buffer.BlockCopy(BitConverter.GetBytes(subMeshes.Length), 0, chunkData, 0, 4);

            var headerPosition = FIXED_HEADER_SIZE;
            var chunkPosition = headerSize;
            for (var i = 0; i < subMeshes.Length; i++ )
            {
                var subMesh = subMeshes[i];
                //write size of submesh to the header
                CopyBytes(subMesh.Length, chunkData, headerPosition);
                headerPosition += sizeof (int);

                //write submesh
                var subMeshSize = subMesh.Length*INDEX_SIZE;
                Buffer.BlockCopy(subMesh, 0, chunkData, chunkPosition, subMeshSize);
                chunkPosition += subMeshSize;
            }
        }

        protected override ushort ChunkID
        {
            get { return 4; }
        }
    }
}
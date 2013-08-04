using System;
using UnityEngine;

namespace UnityMesh
{
    /// <summary>
    /// 
    /// </summary>
    public class BindPosesChunk : Chunk
    {
        const int BIND_SIZE = sizeof(float) * 16;

        protected override void ReadChunk(byte[] chunkData, ref Mesh mesh)
        {
            var chunkLength = chunkData.Length;
            var binds = new Matrix4x4[chunkLength/BIND_SIZE];
            for (int i = 0; i < binds.Length; i++ )
            {
                var nWeight = new Matrix4x4();
                for (int j = 0; j < 16; j++ )
                {
                    nWeight[j] = BitConverter.ToSingle(chunkData, i * BIND_SIZE + sizeof(float) * j);
                }
                binds[i] = nWeight;
            }
            mesh.bindposes = binds;
        }

        protected override void WriteChunk(out byte[] chunkData, Mesh mesh)
        {
            var binds = mesh.bindposes;
            var chunkLength = binds.Length * BIND_SIZE;
            chunkData = new byte[chunkLength];
            for(int i = 0; i < binds.Length;i++)
            {
                var weight = binds[i];
                for (int j = 0; j < 16; j++ )
                {
                    var ind = i*BIND_SIZE + sizeof (float)*j;
                    CopyBytes(weight[j], chunkData, ind);
                }
            }
        }

        protected override ushort ChunkID
        {
            get { return 9; }
        }
    }
}
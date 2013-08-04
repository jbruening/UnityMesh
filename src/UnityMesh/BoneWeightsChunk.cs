using System;
using UnityEngine;

namespace UnityMesh
{
    public class BoneWeightsChunk : Chunk
    {
        const int WEIGHT_SIZE = sizeof(int) * 4 + sizeof(float) * 4; //4 indices, 4 weights

        protected override void ReadChunk(byte[] chunkData, ref Mesh mesh)
        {
            var chunkLength = chunkData.Length;
            var weights = new BoneWeight[chunkLength/WEIGHT_SIZE];
            for (int i = 0; i < weights.Length; i++ )
            {
                var nWeight = new BoneWeight();
                nWeight.boneIndex0 = BitConverter.ToInt32(chunkData, i*WEIGHT_SIZE);
                nWeight.boneIndex1 = BitConverter.ToInt32(chunkData, i * WEIGHT_SIZE + sizeof(int));
                nWeight.boneIndex2 = BitConverter.ToInt32(chunkData, i * WEIGHT_SIZE + sizeof(int)*2);
                nWeight.boneIndex3 = BitConverter.ToInt32(chunkData, i * WEIGHT_SIZE + sizeof(int)*3);
                nWeight.weight0 = BitConverter.ToSingle(chunkData, i * WEIGHT_SIZE + sizeof(int)*4);
                nWeight.weight1 = BitConverter.ToSingle(chunkData, i * WEIGHT_SIZE + sizeof(int) * 4 + sizeof(float));
                nWeight.weight2 = BitConverter.ToSingle(chunkData, i * WEIGHT_SIZE + sizeof(int) * 4 + sizeof(float) * 2);
                nWeight.weight3 = BitConverter.ToSingle(chunkData, i * WEIGHT_SIZE + sizeof(int) * 4 + sizeof(float) * 3);

                weights[i] = nWeight;
            }
            mesh.boneWeights = weights;
        }

        protected override void WriteChunk(out byte[] chunkData, Mesh mesh)
        {
            var weights = mesh.boneWeights;
            var chunkLength = weights.Length * WEIGHT_SIZE;
            chunkData = new byte[chunkLength];
            for(int i = 0; i < weights.Length;i++)
            {
                var weight = weights[i];
                CopyBytes(weight.boneIndex0, chunkData, i*WEIGHT_SIZE);
                CopyBytes(weight.boneIndex1, chunkData, i * WEIGHT_SIZE + sizeof(int));
                CopyBytes(weight.boneIndex2, chunkData, i * WEIGHT_SIZE + sizeof(int)*2);
                CopyBytes(weight.boneIndex3, chunkData, i * WEIGHT_SIZE + sizeof(int)*3);
                CopyBytes(weight.weight0, chunkData, i*WEIGHT_SIZE + sizeof(int)*4);
                CopyBytes(weight.weight1, chunkData, i * WEIGHT_SIZE + sizeof(int) * 4 + sizeof(float));
                CopyBytes(weight.weight2, chunkData, i * WEIGHT_SIZE + sizeof(int) * 4 + sizeof(float)*2);
                CopyBytes(weight.weight3, chunkData, i * WEIGHT_SIZE + sizeof(int) * 4 + sizeof(float)*3);
            }
        }

        protected override ushort ChunkID
        {
            get { return 8; }
        }
    }
}
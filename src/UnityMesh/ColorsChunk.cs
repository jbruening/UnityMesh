using UnityEngine;

namespace UnityMesh
{
    /// <summary>
    /// Chunk for Mesh.colors32
    /// </summary>
    public class ColorsChunk : Chunk
    {
        const int COLOR_SIZE = sizeof(int);

        protected override void ReadChunk(byte[] chunkData, ref Mesh mesh)
        {
            var chunkLength = chunkData.Length;
            var colors = new Color32[chunkLength / COLOR_SIZE];
            for (var i = 0; i < colors.Length; i ++)
            {
                colors[i] = new Color32(chunkData[i * 4], chunkData[i * 4 + 1], chunkData[i * 4 + 2], chunkData[i * 4 + 3]);
            }
            mesh.colors32 = colors;
        }

        protected override void WriteChunk(out byte[] chunkData, Mesh mesh)
        {
            var colors = mesh.colors32;
            var chunkLength = colors.Length * COLOR_SIZE;
            chunkData = new byte[chunkLength];

            for (var i = 0; i < colors.Length; i++)
            {
                var color = colors[i];
                chunkData[i*4] = color.r;
                chunkData[i*4 + 1] = color.g;
                chunkData[i*4 + 2] = color.b;
                chunkData[i*4 + 3] = color.a;
            }
        }

        protected override ushort ChunkID
        {
            get { return 7; }
        }
    }
}
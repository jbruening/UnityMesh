using System;
using UnityEngine;

namespace UnityMesh
{
    /// <summary>
    /// Class used to read/write chunks in a unm stream
    /// </summary>
    public abstract class Chunk
    {
        internal void Read(byte[] chunkData, Mesh mesh)
        {
            ReadChunk(chunkData, ref mesh);
        }

        internal byte[] Write(Mesh mesh)
        {
            byte[] chunkData;
            WriteChunk(out chunkData, mesh);
            return chunkData;
        }
        protected abstract void ReadChunk(byte[] chunkData, ref Mesh mesh);
        protected abstract void WriteChunk(out byte[] chunkData, Mesh mesh);
        protected abstract ushort ChunkID { get; }
        internal ushort InternalChunkID { get { return ChunkID; } }

        internal static unsafe void CopyBytes(int value, byte[] buffer, int offset)
        {
            if (offset + sizeof(int) >= buffer.Length) throw new IndexOutOfRangeException();

            fixed (byte* numPtr = &buffer[offset])
                *(int*) numPtr = value;
        }
        internal static unsafe void CopyBytes(float value, byte[] buffer, int offset)
        {
            if (offset + sizeof(float) >= buffer.Length) throw new IndexOutOfRangeException();

            fixed (byte* numPtr = &buffer[offset])
                *(float*)numPtr = value;
        }
    }
}
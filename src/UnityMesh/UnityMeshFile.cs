using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityMesh
{
    /// <summary>
    /// Read/Write unitymesh (*.unm) files
    /// </summary>
    public static class UnityMeshFile
    {
        const string FILE_SIGNATURE = "UNM";
        private const byte VERSION = 2;
        private static readonly Encoding Iso8859 = Encoding.GetEncoding("ISO-8859-1");

        /// <summary>
        /// Write to the specified stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="mesh"></param>
        /// <param name="config"></param>
        public static void Write(Stream stream, Mesh mesh, FileConfig config = null)
        {
            if (mesh == null)
                throw new Exception("Cannot write a null Mesh");

            if (config == null)
                config = FileConfig.DefaultConfig();

            var writer = new BinaryWriter(stream);
            try
            {
                writer.Write(Iso8859.GetBytes(FILE_SIGNATURE));

                writer.Write(VERSION);

                writer.Write(config.Chunks.Count());

                foreach (var chunk in config.Chunks)
                {
                    byte[] chunkBytes = null;
                    try
                    {
                        chunkBytes = chunk.Write(mesh);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(
                            string.Format("[UnityMesh] Failed to write chunk {0} : {1}. Skipping to next chunk.",
                                          chunk.GetType().Name, e));
                    }

                    //skip writing any of the data if we failed to get the chunk data
                    if (chunkBytes == null) continue;

                    writer.Write(chunk.InternalChunkID);
                    writer.Write((uint) chunkBytes.Length);
                    stream.Write(chunkBytes, 0, chunkBytes.Length);
                }
            }
            finally
            {
                //do not dispose the writer, as it closes the stream. merely flush it.
                writer.Flush();
            }
        }

        public static Mesh Read(Stream stream, FileConfig config = null)
        {
            var mesh = new Mesh();
            ReadInto(mesh, stream, config);
            return mesh;
        }

        /// <summary>
        /// Read a mesh from the specified stream
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="stream"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static void ReadInto(Mesh mesh, Stream stream, FileConfig config = null)
        {
            if (config == null)
                config = FileConfig.DefaultConfig();

            //we're not wrapping the reading into a using because we don't want it to close the stream when done
            var reader = new BinaryReader(stream);
            
            var sigBytes = reader.ReadBytes(3);
            var sigString = Iso8859.GetString(sigBytes);

            if (sigString != FILE_SIGNATURE)
                throw new Exception("Stream did not have the UNM file signature");

            //TODO: care about the version
            var version = reader.ReadByte();
            if (version != VERSION)
                throw  new Exception(string.Format("Version {0} cannot read file version {1}", VERSION, version));

            var chunkCount = reader.ReadInt32();
            for(int i = 0; i < chunkCount; i++)
            {
                ushort chunkID;
                try
                {
                    chunkID = reader.ReadUInt16();
                }
                catch (EndOfStreamException)
                {
                    //no more chunks
                    break;
                }
                var chunkLength = (int) reader.ReadUInt32();
                var chunkBytes = new byte[chunkLength];
                var chunkReadBytes = stream.Read(chunkBytes, 0, chunkLength);
                if (chunkReadBytes < chunkLength)
                {
                    Debug.LogError(
                        string.Format(
                            "[UnityMesh] Stream ended unexpectedly. Expected {0} bytes, read only {1} bytes. Chunk ID {2}",
                            chunkLength, chunkReadBytes, chunkID));
                    break;
                }
                Chunk chunk;
                if (config.TryGetChunk(chunkID, out chunk))
                {
                    try
                    {
                        chunk.Read(chunkBytes, mesh);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(string.Format("[UnityMesh] Failed to read chunk {0} : {1}. Skipping.",
                                                        chunk.GetType().Name, e));
                    }
                }
                else
                {
                    Debug.LogWarning(string.Format("[UnityMesh] Unknown Chunk ID {0}, {1} bytes. Skipping.", chunkID,
                                                    chunkLength));
                    stream.Seek(chunkLength, SeekOrigin.Current);
                }
            }
        }
    }
}

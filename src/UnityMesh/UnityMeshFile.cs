using System;
using System.IO;
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

            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(Iso8859.GetBytes(FILE_SIGNATURE));

                writer.Write((byte)1);


                foreach (var chunk in config.Chunks)
                {
                    byte[] chunkBytes = null;
                    try
                    {
                        chunkBytes = chunk.Write(mesh);
                    }
                    catch(Exception e)
                    {
                        Debug.LogError(string.Format("[UnityMesh] Failed to write chunk {0} : {1}. Skipping to next chunk.", chunk.GetType().Name, e));
                    }
                    
                    //skip writing any of the data if we failed to get the chunk data
                    if (chunkBytes == null) continue;

                    writer.Write(chunk.InternalChunkID);
                    writer.Write((uint) chunkBytes.Length);
                    stream.Write(chunkBytes, 0, chunkBytes.Length);
                }
            }
        }

        /// <summary>
        /// Read a mesh from the specified stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Mesh Read(Stream stream, FileConfig config = null)
        {
            var mesh = new Mesh();

            if (config == null)
                config = FileConfig.DefaultConfig();

            using (var reader = new BinaryReader(stream))
            {
                var sigBytes = reader.ReadBytes(3);
                var sigString = Iso8859.GetString(sigBytes);

                if (sigString != FILE_SIGNATURE)
                    throw new Exception("Stream did not have the UNM file signature");

                //TODO: care about the version
                reader.ReadByte();

                while (true)
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
                    var chunkLength = (int)reader.ReadUInt32();
                    var chunkBytes = new byte[chunkLength];
                    var chunkReadBytes = stream.Read(chunkBytes, 0, chunkLength);
                    if (chunkReadBytes < chunkLength)
                    {
                        Debug.LogError(
                            string.Format("[UnityMesh] Stream ended unexpectedly. Expected {0} bytes, read only {1} bytes. Chunk ID {2}", chunkLength, chunkReadBytes, chunkID));
                        break;
                    }
                    Chunk chunk;
                    if (config.TryGetChunk(chunkID, out chunk))
                    {
                        try
                        {
                            chunk.Read(chunkBytes, mesh);
                        }
                        catch(Exception e)
                        {
                            Debug.LogError(string.Format("[UnityMesh] Failed to read chunk {0} : {1}. Skipping.", chunk.GetType().Name, e));
                        }
                    }
                    else
                    {
                        Debug.LogWarning(string.Format("[UnityMesh] Unknown Chunk ID {0}, {1} bytes. Skipping.", chunkID, chunkLength));
                        stream.Seek(chunkLength, SeekOrigin.Current);
                    }
                }
            }

            return mesh;
        }
    }
}

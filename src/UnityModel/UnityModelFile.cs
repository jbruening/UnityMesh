using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityModel
{
    public class UnityModelFile
    {
        private readonly SerializerFactory _serializer;
        private const string FileHeader = "UNMO";
        private const byte Version = 1;
        private static readonly Encoding Iso8859 = Encoding.GetEncoding("ISO-8859-1");

        /// <summary>
        /// set up with a DefaultFactory
        /// </summary>
        public UnityModelFile()
        {
            _serializer = SerializerFactory.DefaultFactory();
        }

        /// <summary>
        /// set up with a custom factory
        /// </summary>
        /// <param name="serializer"></param>
        public UnityModelFile(SerializerFactory serializer)
        {
            _serializer = serializer;
        }

        public void Serialize(Object obj, Stream stream)
        {
            if (!obj is GameObject)
                throw new NotImplementedException("Can't serialize anything other than GameObject heiarchies");

            using (var ms = new MemoryStream())
            {
                var writer = new BinaryWriter(ms);
                try
                {
                    writer.Write(Iso8859.GetBytes(FileHeader));
                    writer.Write(Version);

                    _serializer.Serialize(obj, writer);
                }
                finally
                {
                    //do not close the writer as it closes ms.
                    writer.Flush();
                }

                ms.Position = 0;
                //write memory to stream
                var buffer = new byte[32*1024];
                int bytesRead;
                while ((bytesRead = ms.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, bytesRead);
                }
            }
        }

        public Object Deserialize(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                //we're lazy and need byte counting. Load the whole thing into a memory stream.
                var buffer = new byte[32 * 1024];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                }
                ms.Position = 0;

                //not wrapped in using otherwise it closes the stream
                var reader = new BinaryReader(ms);

                var sig = reader.ReadBytes(FileHeader.Length);
                if (FileHeader != Iso8859.GetString(sig))
                    throw new Exception("File did not have UnityModelFile header");

                var version = reader.ReadByte();
                if (version != Version)
                    throw  new Exception("File is version " + version + ". This is version " + Version);

                return _serializer.Deserialize(reader);
            }
        }
    }
}
using System.Collections.Generic;

namespace UnityMesh
{
    /// <summary>
    /// Specifies how files are read/written
    /// </summary>
    public sealed class FileConfig
    {
        private readonly Dictionary<ushort, Chunk> _chunks = new Dictionary<ushort, Chunk>();

        /// <summary>
        /// Create a new FileConfig with the default chunk processors
        /// </summary>
        /// <returns></returns>
        public static FileConfig DefaultConfig()
        {
            var defConfig = new FileConfig();
            defConfig.RegisterChunk<VerticesChunk>();
            //FileConfig.RegisterChunk<TrianglesChunk>();
            defConfig.RegisterChunk<UVsChunk>();
            defConfig.RegisterChunk<SubMeshChunk>();
            defConfig.RegisterChunk<NormalsChunk>();
            defConfig.RegisterChunk<TangentsChunk>();
            defConfig.RegisterChunk<BoneWeightsChunk>();
            defConfig.RegisterChunk<BindPosesChunk>();
            return defConfig;
        }

        internal IEnumerable<Chunk> Chunks
        {
            get { return _chunks.Values; }
        }

        /// <summary>
        /// Register a chunk processor with this config
        /// </summary>
        /// <typeparam name="TChunk"></typeparam>
        /// <returns>the Chunk that will be doing the processing. Use this to change its behaviour if you have to</returns>
        public TChunk RegisterChunk<TChunk>() where TChunk : Chunk, new()
        {
            var nChunk = new TChunk();
            _chunks[nChunk.InternalChunkID] = nChunk;
            return nChunk;
        }

        /// <summary>
        /// If this config has the specified chunk type to process with
        /// </summary>
        /// <typeparam name="TChunk"></typeparam>
        /// <returns></returns>
        public bool HasChunk<TChunk>() where TChunk : Chunk, new()
        {
            var nChunk = new TChunk();
            Chunk chunk;
            return _chunks.TryGetValue(nChunk.InternalChunkID, out chunk);
        }

        /// <summary>
        /// Remove the specified chunk type from the processors
        /// </summary>
        /// <typeparam name="TChunk"></typeparam>
        public void RemoveChunk<TChunk>() where TChunk : Chunk, new()
        {
            var nChunk = new TChunk();
            _chunks.Remove(nChunk.InternalChunkID);
        }

        internal bool TryGetChunk(ushort chunkID, out Chunk chunk)
        {
            return _chunks.TryGetValue(chunkID, out chunk);
        }
    }
}
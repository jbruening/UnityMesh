using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Object = UnityEngine.Object;

namespace UnityModel
{
    public class SerializerFactory
    {
        private static SerializerFactory _instance;
        public static SerializerFactory Instance
        {
            get { return _instance ?? (_instance = new SerializerFactory()); }
        }

        readonly Dictionary<Type, IObjectSerializer> _serializers = new Dictionary<Type, IObjectSerializer>();
        readonly Dictionary<int, Type> _typeMap = new Dictionary<int, Type>();

        internal readonly BiDictionary<int, Object> ReferenceMap = new BiDictionary<int, Object>();
        private int _referenceKey;
        
        public event Action SerializeStart;
        public event Action DeserializeStart;


        public void RegisterSerializer(IObjectSerializer serializer)
        {
            _serializers[serializer.Type] = serializer;
            _typeMap[serializer.TypeID] = serializer.Type;
        }

        public void Serialize(Object obj, BinaryWriter writer)
        {
            if (!obj is GameObject)
                throw new NotImplementedException("Can't serialize anything other than GameObject heiarchies");

            if (SerializeStart != null)
                SerializeStart();

            ReferenceMap.Clear();
            _referenceKey = 0;

            InternalSerialize(obj, ref writer);
        }

        public Object Deserialize(BinaryReader reader)
        {
            if (DeserializeStart != null)
                DeserializeStart();

            ReferenceMap.Clear();
            _referenceKey = 0;

            return InternalDeserialize(ref reader, null);
        }

        internal void InternalSerialize(Object obj, ref BinaryWriter writer)
        {
            IObjectSerializer serializer;

            ReferenceMap[_referenceKey++] = obj;

            if (!_serializers.TryGetValue(obj.GetType(), out serializer)) return;

            writer.Write(serializer.TypeID);
            serializer.Serialize(obj, ref writer, this);
        }

        internal Object InternalDeserialize(ref BinaryReader reader, Object parent)
        {
            var typeID = reader.ReadInt32();
            Type type;

            Object ret = null;
            if (_typeMap.TryGetValue(typeID, out type))
            {
                ret = _serializers[type].Deserialize(ref reader, parent, this);
            }

            ReferenceMap[_referenceKey++] = ret;
            return ret;
        }
    }
}

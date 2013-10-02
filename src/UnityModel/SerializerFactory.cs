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

        readonly Dictionary<Type, object> _serializers = new Dictionary<Type, object>();
        readonly Dictionary<int, Type> _typeMap = new Dictionary<int, Type>();

        internal readonly BiDictionary<int, Object> ReferenceMap = new BiDictionary<int, Object>();
        private int _referenceKey;
        
        public event Action SerializeStart;
        public event Action DeserializeStart;


        public void RegisterSerializer<T>(IObjectSerializer<T> serializer) where T : Object
        {
            var type = typeof (T);
            _serializers[type] = serializer;
            _typeMap[serializer.TypeID] = type;
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
            object serializer;

            ReferenceMap[_referenceKey++] = obj;

            if (!_serializers.TryGetValue(obj.GetType(), out serializer)) return;

            var ser = serializer as IObjectSerializer<Object>;

            writer.Write(ser.TypeID);
            ser.Serialize(obj, ref writer, this);
        }

        internal Object InternalDeserialize(ref BinaryReader reader, Object parent)
        {
            var typeID = reader.ReadInt32();
            Type type;

            Object ret = null;
            if (_typeMap.TryGetValue(typeID, out type))
            {
                var ser = _serializers[type] as IObjectSerializer<Object>;
                ret = ser.Deserialize(ref reader, parent, this);
            }

            ReferenceMap[_referenceKey++] = ret;
            return ret;
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityModel
{
    /// <summary>
    /// Used for serializing/deserializng model files
    /// the class is thread safe, instances are not.
    /// </summary>
    public class SerializerFactory
    {
        public static SerializerFactory DefaultFactory()
        {
            var serializer = new SerializerFactory();
            serializer.RegisterSerializer(new MaterialSerializer());
            serializer.RegisterSerializer(new SkinnedMeshSerializer());
            return serializer;
        }

        public bool DebugEnabled = false;

        private const int HEIARCHY_ID = -2;
        private const int UNKNOWN_ID = -1;

        readonly Dictionary<Type, IObjectSerializer> _serializers = new Dictionary<Type, IObjectSerializer>();
        readonly Dictionary<int, Type> _typeMap = new Dictionary<int, Type>();

        internal readonly BiDictionary<int, Object> ReferenceMap = new BiDictionary<int, Object>();
        internal Dictionary<GameObject, Component[]> ComponentMap = new Dictionary<GameObject, Component[]>();
        private int _referenceKey;
        

        public event Action SerializeStart;
        public event Action DeserializeStart;


        public void RegisterSerializer(IObjectSerializer serializer)
        {
            if (serializer.TypeID < 0) throw new InvalidDataException("serializers cannot have TypeIDs less than 0");
            IObjectSerializer existingSer;
            if (_serializers.TryGetValue(serializer.Type, out existingSer))
            {
                Debug.LogWarning(string.Format("Overwriting existing serializer {0} with {1}", existingSer, serializer));
            }
            _serializers[serializer.Type] = serializer;

            Type existingType;
            if (_typeMap.TryGetValue(serializer.TypeID, out existingType))
            {
                Debug.LogWarning(string.Format("Overwriting existing serializer for type {0} with {1}", existingType.Name, serializer));
            }
            _typeMap[serializer.TypeID] = serializer.Type;
        }
        public bool TryGetSerializer(Type type, out IObjectSerializer serializer)
        {
            return _serializers.TryGetValue(type, out serializer);
        }

        internal void Serialize(Object obj, BinaryWriter writer)
        {
            if (!obj is GameObject)
                throw new NotImplementedException("Can't serialize anything other than GameObject heiarchies");

            var root = obj as GameObject;

            if (SerializeStart != null)
                SerializeStart();

            ReferenceMap.Clear();
            _referenceKey = 0;
            ComponentMap.Clear();

            //id of the heiarchy chunk
            writer.Write(HEIARCHY_ID);
            //size of the heiarchy chunk in bytes
            writer.Write(0L);

            //heiarchy dance
            var heiStartPoint = writer.BaseStream.Position;
            HeiSerialize(root.transform, ref writer);
            var heiEndPoint = writer.BaseStream.Position;
            var heiSize = heiEndPoint - heiStartPoint;

            //back up to the chunk size position
            writer.BaseStream.Position = heiStartPoint - sizeof(long);
            //and write out the size
            writer.Write(heiSize);
            writer.BaseStream.Position = heiEndPoint;

            //write component chunks
            foreach (var kvp in ComponentMap)
            {
                foreach (var comp in kvp.Value)
                {
                    if (comp is Transform) continue; //transforms are already written in heiarchy
                    InternalSerialize(obj, ref writer);
                }
            }
        }

        internal Object Deserialize(BinaryReader reader)
        {
            if (DeserializeStart != null)
                DeserializeStart();

            ReferenceMap.Clear();
            _referenceKey = 0;
            ComponentMap.Clear();

            GameObject root = null;

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var chunkID = reader.ReadInt32();
                
                if (chunkID == HEIARCHY_ID)
                {
                    var heiSize = reader.ReadInt64();
                    var origipos = reader.BaseStream.Position;
                    root = HeiDeserialize(null, ref reader);
                    //jump to point it should be at after doing heiarchy serialization
                    var correctPos = origipos + heiSize;
                    var actualPos = reader.BaseStream.Position;
                    if (actualPos != correctPos)
                    {
                        if (actualPos > correctPos)
                        {
                            throw new Exception(
                                string.Format("heiarchy overflowed to position {0}, should have gone to {1}", actualPos,
                                              correctPos));
                        }

                        if (DebugEnabled)
                        {
                            Debug.LogWarning(string.Format("heirarchy under at position {0}, should be at {1}",
                                                           actualPos, correctPos));
                        }
                        reader.BaseStream.Position = correctPos;
                    }
                }
                else
                {
                    Deserialize(ref reader, null, chunkID);
                }
            }

            return root;
        }

        internal void HeiSerialize(Transform trans, ref BinaryWriter writer)
        {
            var id = _referenceKey++;
            ReferenceMap[id] = trans;
            
            writer.Write(id);
            writer.Write(trans.gameObject.name);
            writer.Write(trans.localPosition);
            writer.Write(trans.localRotation);
            writer.Write(trans.localScale);

            //accumulate components
            ComponentMap[trans.gameObject] = trans.GetComponents(typeof (Component));

            //and recurse to children
            var childCount = trans.childCount;
            writer.Write(childCount);
            foreach (Transform child in trans)
            {
                HeiSerialize(child, ref writer);
            }
        }

        GameObject HeiDeserialize(Transform parent, ref BinaryReader reader)
        {
            var id = reader.ReadInt32();
            var name = reader.ReadString();

            var gobj = new GameObject(name);
            var trans = gobj.transform;
            ReferenceMap[id] = trans;
            
            trans.parent = parent;

            trans.localPosition = reader.ReadVector3();
            trans.localRotation = reader.ReadQuaternion();
            trans.localScale = reader.ReadVector3();

            var childCount = reader.ReadInt32();
            //recurse to children
            for (int i = 0; i < childCount; i++)
            {
                HeiDeserialize(trans, ref reader);
            }
            return gobj;
        }

        internal void InternalSerialize(Object obj, ref BinaryWriter writer)
        {
            IObjectSerializer serializer;

            ReferenceMap[_referenceKey++] = obj;
            if (!_serializers.TryGetValue(obj.GetType(), out serializer))
            {
                //no type
                writer.Write(UNKNOWN_ID);
                //no bytes
                writer.Write(0L);
                if (DebugEnabled)
                    Debug.LogWarning(string.Format("Encountered unknown type {0}. Wrote id of -1", obj.GetType().Name));
                return;
            }

            writer.Write(serializer.TypeID);

            //allocate space for the byte size
            writer.Write(0L);
            
            //do the writing
            var beginPosition = writer.BaseStream.Position;
            serializer.Serialize(obj, ref writer, this);
            var endPosition = writer.BaseStream.Position;

            //write the size
            writer.BaseStream.Position = beginPosition - sizeof(long);
            writer.Write(endPosition - beginPosition);

            //and go forward again to where we should be at
            writer.BaseStream.Position = endPosition;
        }

        internal Object InternalDeserialize(ref BinaryReader reader, Object parent)
        {
            var typeID = reader.ReadInt32();
            return Deserialize(ref reader, parent, typeID);
        }

        private Object Deserialize(ref BinaryReader reader, Object parent, int typeID)
        {
            var size = reader.ReadInt64();
            var origiPos = reader.BaseStream.Position;
            var jumpPoint = size + origiPos;
            
            Type type;

            Object ret = null;
            if (typeID < 0)
            {
                if (DebugEnabled)
                    Debug.LogWarning("Encountered a typeid of " + typeID + ". This usually means the serializer did not recognize the type during serialization");
            }
            else if (_typeMap.TryGetValue(typeID, out type))
            {
                ret = _serializers[type].Deserialize(ref reader, parent, this);
            }
            
            var currentPos = reader.BaseStream.Position;
            
            //only jump if we have to
            if (currentPos != jumpPoint)
            {
                if (currentPos > jumpPoint)
                {
                    //aaaaaaaaaaaaa
                    throw new Exception(string.Format("Current position {0} went past the serialized jump point {1}", currentPos, jumpPoint));
                }
                
                if (DebugEnabled)
                {
                    Debug.LogWarning(string.Format("Current position {0} is behind serialized jump point {1}. Jumping.", currentPos, jumpPoint));
                }
                reader.BaseStream.Position = jumpPoint;
            }

            return ret;
        }
    }
}

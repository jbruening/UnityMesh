using System.IO;
using UnityEngine;
using UnityModel;
using Component = UnityModel.Component;
using GameObject = UnityModel.GameObject;
using Transform = UnityModel.Transform;

public class RoundTripModelTest : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.GameObject _testObject = null;
    
    [ContextMenu("Run Test")]
    void DoTest()
    {
        var factory = new SerializerFactory();
        factory.RegisterSerializer(new GameObject());
        factory.RegisterSerializer(new Transform());

        using (var ms = new MemoryStream(4096))
        {
            factory.Serialize(_testObject, new BinaryWriter(ms));

            ms.Position = 0;
            factory.Deserialize(new BinaryReader(ms));
        }
    }
}

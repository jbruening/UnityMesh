using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityModel;
using Debug = UnityEngine.Debug;

public class RoundTripModelTest : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.GameObject _testObject = null;
    
    [ContextMenu("Run Test")]
    void DoTest()
    {
        var file = new UnityModelFile();

        using (var ms = new MemoryStream(4096))
        {
            var watch = new Stopwatch();
            
            watch.Start();
            file.Serialize(_testObject, ms);
            watch.Stop();
            
            Debug.Log("Serialization took " + watch.Elapsed.ToString());
            
            watch.Start();
            ms.Position = 0;
            file.Deserialize(ms);
            watch.Stop();
            
            Debug.Log(string.Format("round trip took {0}. Size is {1}", watch.Elapsed.ToString(), ms.Length));
        }
    }
}

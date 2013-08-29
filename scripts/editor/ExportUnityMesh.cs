using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityMesh;
using System;
using System.Collections;
using System.IO;
using System.Text;
using Object = UnityEngine.Object;
using System.Linq;

class ExportUnityMesh : EditorWindow
{
    private static List<MeshFilter> mfList;
    private static Mesh _selection;
    [MenuItem("File/Export UnityMesh...")]
    static void ExportSelectedMesh()
    {
        if (_selection == null) return;

        GetWindow<ExportUnityMesh>().Show();
    }

    [MenuItem("File/Import UnityMesh...")]
    static void ImportUnityMesh()
    {
        Import();
    }

    [MenuItem("File/Export UnityMesh...", true)]
    static bool ValidateExportSelectedMesh()
    {
        _selection = Selection.activeObject as Mesh;

        return _selection != null;
    }

   static void Init()
   {
      //terrain = null;
      //Terrain terrainObject = Selection.activeObject as Terrain;
      //if (!terrainObject)
      //{
      //   terrainObject = Terrain.activeTerrain;
      //}
      //if (terrainObject)
      //{
      //   terrain = terrainObject.terrainData;
      //   terrainPos = terrainObject.transform.position;
      //}

      //EditorWindow.GetWindow<ExportTerrain>().Show();
   }

   void OnGUI()
   {
       if (_selection == null)
       {
           Close();
           return;
       }
       GUILayout.Label(_selection.name);

      if (GUILayout.Button("Export"))
      {
         Export();
      }
   }

   void Export()
   {
       var fileName = EditorUtility.SaveFilePanel("Export .unm file", "", _selection.name, "unm");
       if (string.IsNullOrEmpty(fileName)) return;
       using (Stream stream = new FileStream(fileName, FileMode.OpenOrCreate))
       {
           UnityMeshFile.Write(stream, _selection);
       }
   }

    static void Import()
    {
        var filePath = EditorUtility.OpenFilePanel("Import .unm file", "", "unm");
        if (string.IsNullOrEmpty(filePath)) return;
        Mesh mesh;
        using (Stream stream = new FileStream(filePath, FileMode.Open))
        {
            mesh = UnityMeshFile.Read(stream);
        }

        var filename = Path.GetFileNameWithoutExtension(filePath);
        filePath = EditorUtility.SaveFilePanelInProject("Save asset", filename, "asset", "");
        if (string.IsNullOrEmpty(filePath)) return;
        AssetDatabase.CreateAsset(mesh, filePath);
    }
}
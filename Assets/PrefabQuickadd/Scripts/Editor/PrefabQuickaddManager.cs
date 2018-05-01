/*
 * Created by Shelley Lowe
 * www.shelleylowe.com
 */
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class PrefabQuickaddManager
{
    /// <summary>
    /// Add a new database asset for setting up prefab items in Add menu
    /// </summary>
    [MenuItem("Assets/Create/PrefabQuickadd Database Asset")]
    public static void CreateDatabase()
    {
        var asset = ScriptableObject.CreateInstance<PrefabQuickaddDatabase>();

        var rootFolder = GetRootFolderPath();
        Directory.CreateDirectory(string.Format("{0}/Databases", rootFolder));
        var relativeFolderPath = string.Format("{0}/PrefabQuickadd/Databases", GetRelativeToRootPath());
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(relativeFolderPath + "/NewDatabase.asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    /// <summary>
    /// Generate new cs file with MenuItem method for each database entry
    /// </summary>
    /// <param name="database"></param>
    public static void GenerateAddPrefabMenuItems(PrefabQuickaddDatabase database)
    {
        var rootFolder = GetRootFolderPath();
        Directory.CreateDirectory(string.Format("{0}/Scripts/Editor/Generated", rootFolder));

        var className = string.Format("GeneratedMenuItems_{0}", database.name.Replace(" ", ""));
        var relativeFilePath = string.Format("{0}/PrefabQuickadd/Scripts/Editor/Generated/{1}.cs", GetRelativeToRootPath(), className);
        var script = string.Format("{0}/Scripts/Editor/Generated/{1}.cs", rootFolder, className);
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("// AUTO-GENERATED CLASS. DO NOT EDIT.");
        sb.AppendLine("// This is an automatically generated script, any changes made will be overwritten.");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using UnityEditor;");
        sb.AppendLine("");
        sb.AppendLine("public static class " + className);
        sb.AppendLine("{");

        // Write cs method code that will add the prefab to the scene 
        var items = new List<PrefabQuickaddDatabase.PrefabQuickaddEntry>(database.Items);
        items.Sort((a, b) => string.Compare(a.Name, b.Name));
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];

            if (item.Name == "")
            {
                Debug.LogWarning("Missing prefab name at index " + i);
                continue;
            }

            if (item.Prefab == null)
            {
                Debug.LogWarning("Missing prefab object for " + item.Name);
                continue;
            }

            var menuName = item.Name;
            if (database.Submenu != "")
                menuName = string.Format("{0}/{1}", database.Submenu, item.Name);

            sb.AppendLine("    [MenuItem(\"GameObject/Add Prefab/" + menuName + "\", false, priority = -1)]");
            sb.AppendLine("    private static void MenuItem" + i.ToString() + "()");
            sb.AppendLine("    {");
            sb.AppendLine("        var asset = (GameObject)AssetDatabase.LoadAssetAtPath(\"" + AssetDatabase.GetAssetPath(item.Prefab) + "\", typeof(GameObject));");
            sb.AppendLine("        var go = (GameObject)PrefabUtility.InstantiatePrefab(asset);");
            sb.AppendLine("        go.transform.SetParent(Selection.activeTransform);");
            sb.AppendLine("        go.transform.localPosition = asset.transform.position;");
            sb.AppendLine("        Selection.activeGameObject = go;");
            sb.AppendLine("    }");
            sb.AppendLine("");
        }
        
        sb.AppendLine("}");

        // Remove old file if there is one and reimport script
        File.Delete(script);
        File.WriteAllText(script, sb.ToString(), Encoding.UTF8);
        AssetDatabase.ImportAsset(relativeFilePath);
    }

    public static void RemoveMenuItems(PrefabQuickaddDatabase database)
    {
        var className = string.Format("GeneratedMenuItems_{0}", database.name.Replace(" ", ""));
        var script = string.Format("{0}/Scripts/Editor/Generated/{1}.cs", GetRootFolderPath(), className);
        File.Delete(script);
        File.Delete(string.Format("{0}.meta", script));
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Gets the absolute path to the root folder of this extension in the Unity project
    /// </summary>
    /// <returns></returns>
    private static string GetRootFolderPath()
    {
        string[] res = Directory.GetDirectories(Application.dataPath, "PrefabQuickadd", SearchOption.AllDirectories);
        if (res.Length == 0)
        {
            Debug.LogError("Can't find root folder ....");
            return "";
        }
        return res[0].Replace("\\", "/");
    }

    /// <summary>
    /// Get the relative path of the root folder within the Unity Assets folder
    /// </summary>
    /// <returns></returns>
    private static string GetRelativeToRootPath()
    {
        var rootFolder = GetRootFolderPath();
        var relativeStart = rootFolder.IndexOf("Assets/");
        var relativeEnd = rootFolder.IndexOf("/PrefabQuickadd");

        return rootFolder.Substring(relativeStart, relativeEnd - relativeStart);
    }
}

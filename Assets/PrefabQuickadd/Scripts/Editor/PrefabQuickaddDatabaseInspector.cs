/*
 * Created by Shelley Lowe
 * www.shelleylowe.com
 */
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabQuickaddDatabase))]
public class PrefabQuickaddDatabaseInspector : Editor
{
    Vector2 _scroll;
    GameObject _newEntryPrefab;

   public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var database = (PrefabQuickaddDatabase)target;
        
        if (GUILayout.Button("Generate Menu Items"))
        {
            PrefabQuickaddManager.GenerateAddPrefabMenuItems(database);
        }
        if (GUILayout.Button("Delete Menu Items"))
        {
            PrefabQuickaddManager.RemoveMenuItems(database);
        }
        var miniWrappedStyle = new GUIStyle(EditorStyles.miniLabel);
        miniWrappedStyle.wordWrap = true;
        GUILayout.Label("Note: Deleting only removes the menu items from the hierarchy menu, all settings below will remain.", miniWrappedStyle);

        GUILayout.Space(10);

        GUILayout.Label("Settings:", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Group as:", "If set, all entries on this database will be grouped into a submenu in the final quickadd menu."), GUILayout.ExpandWidth(false));
        database.Submenu = GUILayout.TextField(database.Submenu);
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        _newEntryPrefab = (GameObject)EditorGUILayout.ObjectField(_newEntryPrefab, typeof(GameObject), false);
        if (GUILayout.Button("Add New", GUILayout.ExpandWidth(false)))
        {
            var newEntry = new PrefabQuickaddDatabase.PrefabQuickaddEntry()
            {
                Name = _newEntryPrefab != null ? _newEntryPrefab.name : "New Entry",
                Prefab = _newEntryPrefab,
                ExpandedInInspector = true
            };
            database.Items.Add(newEntry);
            _scroll = Vector2.up * 1000f; // jump to bottom 
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.Label("Prefabs:", EditorStyles.boldLabel);

        if (database.Items.Count > 0)
        {
            _scroll = GUILayout.BeginScrollView(_scroll);
            for (int i = 0; i < database.Items.Count; i++)
            {
                var item = database.Items[i];

                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                item.ExpandedInInspector = EditorGUILayout.Foldout(item.ExpandedInInspector, item.Name, true);
                if (EditorGUILayout.BeginFadeGroup(item.ExpandedInInspector ? 1 : 0))
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    item.Name = GUILayout.TextField(item.Name);
                    item.Prefab = (GameObject)EditorGUILayout.ObjectField(item.Prefab, typeof(GameObject), false);
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
                EditorGUILayout.EndFadeGroup();
                if (GUILayout.Button("-", GUILayout.Width(22)))
                {
                    database.Items.RemoveAt(i);
                    i--;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("No prefabs linked.", miniWrappedStyle);
        }
    }
}

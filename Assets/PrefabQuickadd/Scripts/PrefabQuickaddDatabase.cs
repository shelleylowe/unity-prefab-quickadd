/*
 * Created by Shelley Lowe
 * www.shelleylowe.com
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabQuickaddDatabase : ScriptableObject
{
    [System.Serializable]
    public class PrefabQuickaddEntry
    {
        public string Name;
        public GameObject Prefab;

        public bool ExpandedInInspector;
    }

    [SerializeField]
    private List<PrefabQuickaddEntry> _items = new List<PrefabQuickaddEntry>();

    [SerializeField]
    private string _submenu = "";

    public List<PrefabQuickaddEntry> Items {  get { return _items; } }

    public string Submenu { get { return _submenu; } set { _submenu = value; } }

}

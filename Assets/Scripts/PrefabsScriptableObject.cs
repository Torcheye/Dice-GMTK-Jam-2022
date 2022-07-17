using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create PrefabsScriptableObject", fileName = "PrefabsScriptableObject", order = 0)]
public class PrefabsScriptableObject : ScriptableObject
{
    public GameObject[] gems = new GameObject[7];
}
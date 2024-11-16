using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "(ScriptableObject)CharacterItem")]
public class GameData : ScriptableObject 
{
    public List<DiskInfo> InfoList = new List<DiskInfo>();
}
[System.Serializable]
public class DiskInfo
{
    public int ID;
    public string Name;
    public string Desc;
    [Tooltip("角色血量")]
    public int hp;
    [Tooltip("行动速度")]
    public float spd;
    public GameObject diskPrefabs;
}
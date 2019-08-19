using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkillTable
{
    public string name;
    public string tag;
    public GameObject prefab;
    public float coolTime;
    public int useMp;
    public GameObject paticle;
    public string skillInfo;
    public bool skillBool;
}

[CreateAssetMenu(fileName = "SkillData", menuName = "Skill/Info", order = 2)]
public class SkillScriptsTable : ScriptableObject
{
    public SkillTable[] skillTables;
}

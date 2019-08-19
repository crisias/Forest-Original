using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Status
{
    public int level;
    public int hp;
    public int mp;
    public float str;
    public int def;
    public int dex;
    public float cripro;
    public float cridem;
    public int exp;
}

[CreateAssetMenu(fileName = "Data", menuName = "Character/Status", order = 1)]
public class ScriptTable : ScriptableObject
{
    public GameObject magiPrefabs;

    public AnimationClip[] animations = new AnimationClip[4];

    public Status[] status;
}
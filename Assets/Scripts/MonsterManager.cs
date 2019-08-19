using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterInfo
{
    public string Name;
    public int Hp;
    public int Atk;
    public int Def;
    public int Dex;
    public int Exp;
    public GameObject Prefab;
    public List<AnimationClip> MonsterAni;

    public MonsterInfo(string _name, int _hp, int _atk, int _def, int _dex, int _exp, GameObject _prefab, List<AnimationClip> _monsterAni)
    {
        Name = _name;
        Hp = _hp;
        Atk = _atk;
        Def = _def;
        Dex = _dex;
        Exp = _exp;
        Prefab = _prefab;
        MonsterAni = _monsterAni;
    }
}

public class MonsterManager : MonoBehaviour
{
    public GameObject bDragonPrefab;
    public GameObject FrogPrefab;

    public List<AnimationClip> bDragonAnimations = new List<AnimationClip>();

    public List<AnimationClip> frogAnimations = new List<AnimationClip>();

    public List<MonsterInfo> monsterInfos = new List<MonsterInfo>();

    void Awake()
    {
        monsterInfos.Add(new MonsterInfo("BabyDragon", 30, 10, 0, 10, 10, bDragonPrefab, bDragonAnimations));
        monsterInfos.Add(new MonsterInfo("Frog", 30, 10, 0, 10, 10, FrogPrefab, frogAnimations));
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

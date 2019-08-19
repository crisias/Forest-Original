using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemInfo
{
    public float id;
    public Sprite cuberImage;
    public int type;
    public string name;
    public Sprite material1;
    public int material1Count;
    public Sprite material2;
    public int material2Count;
    public Sprite material3;
    public int material3Count;
    public int level;
    public int hp;
    public int mp;
    public int str;
    public int def;
    public float dex;
    public float cripro;
    public float cridem;
    public int gold;
    public int buyGold;
    public int count;
    public string info;
    public bool lockBool;
    public int reinForce;

    public ItemInfo(ItemInfo _info)
    {
        id = _info.id;
        cuberImage = _info.cuberImage;
        type = _info.type;
        name = _info.name;
        level = _info.level;
        hp = _info.hp;
        mp = _info.mp;
        str = _info.str;
        def = _info.def;
        dex = _info.dex;
        cripro = _info.cripro;
        cridem = _info.cridem;
        gold = _info.gold;
        buyGold = _info.buyGold;
        count = _info.count;
        info = _info.info;
        lockBool = _info.lockBool;
        material1 = _info.material1;
        material1Count = _info.material1Count;
        material2 = _info.material2;
        material2Count = _info.material2Count;
        material3 = _info.material3;
        material3Count = _info.material3Count;
        reinForce = _info.reinForce;
    }
}

[CreateAssetMenu(fileName = "ItemInfos", menuName = "Item/Info", order = 3)]
public class ItemTable : ScriptableObject
{
    public ItemInfo[] itemInfos;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SaveList
{
    //캐릭터 스텟
    public string job;
    public int lv;
    public int hp;
    public int mp;
    public float str;
    public int def;
    public float dex;
    public float cripro;
    public float cridem;
    public int exp;
    //스킬이미지
    public Sprite[] skillIconImage = new Sprite[4];
    public ItemInfo[] saveInven = new ItemInfo[60];

    public SaveList(SaveList saveList)
    {
        job = saveList.job;
        lv = saveList.lv;
        hp = saveList.hp;
        mp = saveList.mp;
        str = saveList.str;
        def = saveList.def;
        dex = saveList.dex;
        cripro = saveList.cripro;
        cridem = saveList.cridem;
        exp = saveList.exp;
        skillIconImage = saveList.skillIconImage;
        saveInven = saveList.saveInven;
    }
}

[CreateAssetMenu(fileName = "SaveData", menuName = "save/data", order = 4)]
public class SaveLoad : ScriptableObject
{
    public SaveList saveData;
}

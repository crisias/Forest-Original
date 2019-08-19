using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Xml;

//현재 캐릭터가 갔고 있는 쉽게 관리하기 위해 
//정보를 모아 뒀다
[System.Serializable]
public class CurrentPlayer
{
    public int job;
    public int level;
    public int hp;
    public int mp;
    public float str;
    public int dex;
    public int def;
    public float cripro;
    public float cridem;
    public int exp;

    public GameObject render;

    public AnimationClip[] ani;
}

public class Player : MonoBehaviour
{
    public string currentJob;

    public CurrentPlayer currentPlayerInfo;

    public CurrentPlayer LoadPlayerData;

    public ScriptTable magiMaxStatus;

    public Status currentMaxStatus;

    bool levelUp;

    public SaveLoad saveLoad;

    string dataBaseTag;
    Transform dataBaseTr;
    ItemManager itemManagerSc;
    SaveLoadData saveLoadDataSc;

    string xmlName = "playerData.xml";
    //스킬 버튼 이미지(드래그앤 드랍)
    public Sprite[] skillButtonImage = new Sprite[5];
    //스킬 버튼 오브젝트(드래그앤 드랍)
    public GameObject[] skillButton = new GameObject[4];
    //스킬창 스킬 장착칸 이미지
    public Sprite[] skillSlotImage = new Sprite[5];
    //스킬창 스킬 장착칸 오브젝트
    public GameObject[] skillSlot = new GameObject[4];
    //로드시 아이템의 정보를 받아올곳
    public ItemTable itemInfo;
    //로드시 장착아이템의 정보 넣어줄곳
    public GameObject[] equipSlots = new GameObject[9];
    //로드시 인벤토리 아이템의 정보를 넣어주기 위한 상위 오브젝트(드래그앤 드롭)
    public GameObject content;
    //로드시 인벤토리 아이템의 정보 넣어줄곳
    GameObject[] inventorySlots = new GameObject[60];

    string npcShopTag;
    Transform npcShopTr;
    NpcShop npcShopSc;



    //test text
    public Text[] testText = new Text[6];

    void Awake()
    {
        //currentJob = SelectSceneCtrl.selectJob;
        //test 한직업으로 고정
        currentJob = "Magician";

        dataBaseTag = FindTag.getInstance().dataBase;
        dataBaseTr = Find.getInstance().FindTagTransform(dataBaseTag);
        itemManagerSc = dataBaseTr.GetComponent<ItemManager>();
        saveLoadDataSc = dataBaseTr.GetComponent<SaveLoadData>();

        npcShopTag = FindTag.getInstance().shopNpc;
        npcShopTr = Find.getInstance().FindTagTransform(npcShopTag);
        npcShopSc = npcShopTr.GetComponent<NpcShop>();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i] = content.transform.GetChild(i + 1).gameObject;
        }
    }

    void Start()
    {
        //xml있을때
        if(SelectSceneUiManager.result)
        {
            Load();
        }
        //xml없을때
        else if(!SelectSceneUiManager.result)
        {
            //시작시 그 직업의 1레벨 스텟으로 고정하고 그직업의 프리펩과 외형을 받는다
            if (currentJob == "Magician")
            {
                //테스트
                currentPlayerInfo.level = 10;
                ItemManager.Gold = 100000000;

                LevelUp(currentPlayerInfo.level);
                maxHpMp(currentPlayerInfo.level);
            }
        }

        currentPlayerInfo.render = magiMaxStatus.magiPrefabs;
        currentPlayerInfo.ani = magiMaxStatus.animations;

        //테스트용
        //currentPlayerInfo.str = 100;
        magiMaxStatus.status[currentPlayerInfo.level - 1].exp = 30;

        GameObject a = Instantiate(currentPlayerInfo.render);
        a.transform.SetParent(transform);
        a.transform.localPosition = Vector3.zero;
    }

    void Update()
    {
        LevelCount();

        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }

    public void LevelUp(int currentLevel)
    {
        currentPlayerInfo.hp = magiMaxStatus.status[currentLevel - 1].hp;
        currentPlayerInfo.mp = magiMaxStatus.status[currentLevel - 1].mp;
        currentPlayerInfo.str = magiMaxStatus.status[currentLevel - 1].str;
        currentPlayerInfo.dex = magiMaxStatus.status[currentLevel - 1].dex;
        currentPlayerInfo.def = magiMaxStatus.status[currentLevel - 1].def;
        currentPlayerInfo.cripro = magiMaxStatus.status[currentLevel - 1].cripro;
        currentPlayerInfo.cridem = magiMaxStatus.status[currentLevel - 1].cridem;
        currentPlayerInfo.exp = 0;
    }

    public void LevelCount()
    {
        if (currentPlayerInfo.exp >= 30/*magiMaxStatus.status[currentPlayerInfo.level - 1].exp*/)
        {
            int a = currentPlayerInfo.exp - 30/*magiMaxStatus.status[currentPlayerInfo.level - 1].exp*/;
            if (currentPlayerInfo.level != 40)
            {
                currentPlayerInfo.level += 1;
                LevelUp(currentPlayerInfo.level);
                maxHpMp(currentPlayerInfo.level);
                currentPlayerInfo.exp += a;
            }
            else
                currentPlayerInfo.exp -= 1;
        }
    }

    public void maxHpMp(int lv)
    {
        currentMaxStatus = magiMaxStatus.status[lv - 1];
        //테스트용
        magiMaxStatus.status[currentPlayerInfo.level - 1].exp = 30;
    }

    Sprite SelectSprite(string spriteName)
    {
        for(int i = 0; i < skillButtonImage.Length; i++)
        {
            if (spriteName == skillButtonImage[i].name)
                return skillButtonImage[i];
        }
        return null;
    }

    Sprite SelectSlotsprite(string spriteName)
    {
        switch(spriteName)
        {
            case "ForcewaveIconImg":
                return skillSlotImage[0];
            case "HealIconImg":
                return skillSlotImage[1];
            case "LightningBallIconImg":
                return skillSlotImage[2];
            case "ShiningIconImg":
                return skillSlotImage[3];
            case "SparkIconImg":
                return skillSlotImage[4];
        }
        return null;
    }

    ItemInfo FindInfo(string imageName)
    {
        for(int i = 0; i < itemInfo.itemInfos.Length; i++)
        {
            if (itemInfo.itemInfos[i].cuberImage.name == imageName)
            {
                return itemInfo.itemInfos[i];
            }
        }
        return null;
    }
    //장착칸 저장된 이름으로 아이템 정보가져온다
    void EquipFindInfo(XmlReader xmlReader, string findName, int equipSlotArrayNum)
    {
        if (xmlReader.Read())
        {
            if (xmlReader.Value.Trim() != findName)
            {
                itemManagerSc.equipSlotInfos[equipSlotArrayNum] = new ItemInfo(FindInfo(xmlReader.Value.Trim()));
                equipSlots[equipSlotArrayNum - 1].GetComponent<Image>().sprite = itemManagerSc.equipSlotInfos[equipSlotArrayNum].cuberImage;
            }
        }
    }
    //장착칸 슬롯 bool값 처리
    void EquipBool(XmlReader xmlReader, int equipSlotArrayNum)
    {
        if(xmlReader.Read())
        {
            itemManagerSc.equipSlotBool[equipSlotArrayNum] = bool.Parse(xmlReader.Value.Trim());
        }
    }
    //장착칸 저장된 string string to bool 처리해서 잠금 이미지 처리
    void EquipLock(XmlReader xmlReader, int equipSlotArrayNum)
    {
        if (xmlReader.Read())
        {
            itemManagerSc.equipLockBool[equipSlotArrayNum] = bool.Parse(xmlReader.Value.Trim());
            equipSlots[equipSlotArrayNum - 1].transform.GetChild(0).gameObject.SetActive(bool.Parse(xmlReader.Value.Trim()));
        }
    }
    //장착칸 저장된 이름으로 강화수치 불러오기
    void EquipRein(XmlReader xmlReader, int equipSlotArrayNum)
    {
        if (xmlReader.Read())
        {
            itemManagerSc.equipSlotInfos[equipSlotArrayNum].reinForce = int.Parse(xmlReader.Value.Trim());
            if (itemManagerSc.equipSlotInfos[equipSlotArrayNum].reinForce > 0)
                equipSlots[equipSlotArrayNum - 1].transform.GetChild(1).GetComponent<Text>().text = "+" + itemManagerSc.equipSlotInfos[equipSlotArrayNum].reinForce.ToString();
        }
    }
    //인벤토리칸 정보 로드
    void InventorySlotInfo(XmlReader xmlReader, string compareName, int num)
    {
        if(xmlReader.Name == compareName)
        {
            if (xmlReader.Read())
            {
                if(xmlReader.Value.Trim() != "slotCuber")
                {
                    itemManagerSc.inventorySlotInfos[num] = new ItemInfo(FindInfo(xmlReader.Value.Trim()));
                    inventorySlots[num].GetComponent<Image>().sprite = itemManagerSc.inventorySlotInfos[num].cuberImage;
                }
            }
        }
    }
    //인벤토리칸 아이템 유무 로드
    void InventorySlotBool(XmlReader xmlReader, string compareName, int num)
    {
        if(xmlReader.Name == compareName)
        {
            if (xmlReader.Read())
            {
                itemManagerSc.inventorySlotBool[num] = bool.Parse(xmlReader.Value.Trim());
            }
        }
    }
    //인벤토리칸 아이템 잠금
    void InventorySlotLock(XmlReader xmlReader, string compareName, int num)
    {
        if(xmlReader.Name == compareName)
        {
            if (xmlReader.Read())
            {
                itemManagerSc.inventoryLockBool[num] = bool.Parse(xmlReader.Value.Trim());
                inventorySlots[num].transform.GetChild(1).gameObject.SetActive(bool.Parse(xmlReader.Value.Trim()));
            }
        }
    }
    //인벤토리칸 아이템 강화수치
    void InventorySlotRein(XmlReader xmlReader, string comparName, int num)
    {
        if(xmlReader.Name == comparName)
        {
            if (xmlReader.Read())
            {
                itemManagerSc.inventorySlotInfos[num].reinForce = int.Parse(xmlReader.Value.Trim());
                if (itemManagerSc.inventorySlotInfos[num].reinForce > 0)
                    inventorySlots[num].transform.GetChild(2).GetComponent<Text>().text = "+" + itemManagerSc.inventorySlotInfos[num].reinForce.ToString();
            }
        }
    }
    //인벤토리칸 아이템 개수
    void InventorySlotCount(XmlReader xmlreader, string comparName, int num)
    {
        if(xmlreader.Name == comparName)
        {
            if (xmlreader.Read())
            {
                itemManagerSc.inventorySlotInfos[num].count = int.Parse(xmlreader.Value.Trim());
                if (itemManagerSc.inventorySlotInfos[num].count > 0)
                    inventorySlots[num].transform.GetChild(0).GetComponent<Text>().text = itemManagerSc.inventorySlotInfos[num].count.ToString();
            }
        }
    }

    public void Load()
    {
        string xmlFilePath = Application.persistentDataPath + "/" + xmlName;

        using (XmlReader xmlReader = XmlReader.Create(xmlFilePath))
        {
            while (xmlReader.Read())
            {
                if (xmlReader.IsStartElement())
                {
                    switch (xmlReader.Name)
                    {
                        case "Job":
                            if (xmlReader.Read())
                                currentPlayerInfo.job = int.Parse(xmlReader.Value.Trim());
                            break;
                        case "Level":
                            if (xmlReader.Read())
                                currentPlayerInfo.level = int.Parse(xmlReader.Value.Trim());
                            break;
                        case "Hp":
                            if (xmlReader.Read())
                                currentPlayerInfo.hp = int.Parse(xmlReader.Value.Trim());
                            break;
                        case "Mp":
                            if (xmlReader.Read())
                                currentPlayerInfo.mp = int.Parse(xmlReader.Value.Trim());
                            break;
                        case "Str":
                            if (xmlReader.Read())
                                currentPlayerInfo.str = float.Parse(xmlReader.Value.Trim());
                            break;
                        case "Dex":
                            if (xmlReader.Read())
                                currentPlayerInfo.dex = int.Parse(xmlReader.Value.Trim());
                            break;
                        case "Def":
                            if (xmlReader.Read())
                                currentPlayerInfo.def = int.Parse(xmlReader.Value.Trim());
                            break;
                        case "Cripro":
                            if (xmlReader.Read())
                                currentPlayerInfo.cripro = float.Parse(xmlReader.Value.Trim());
                            break;
                        case "Cridem":
                            if (xmlReader.Read())
                                currentPlayerInfo.cridem = float.Parse(xmlReader.Value.Trim());
                            break;
                        case "Exp":
                            if (xmlReader.Read())
                                currentPlayerInfo.exp = int.Parse(xmlReader.Value.Trim());
                            break;
                        case "Gold":
                            if (xmlReader.Read())
                                ItemManager.Gold = int.Parse(xmlReader.Value.Trim());
                            break;
                        case "Skill1":
                            if (xmlReader.Read())
                            {
                                if (xmlReader.Value.Trim() != "EmptySkill")
                                {
                                    skillButton[0].GetComponent<Image>().sprite = SelectSprite(xmlReader.Value.Trim());
                                    skillSlot[0].transform.GetChild(0).gameObject.SetActive(true);
                                    skillSlot[0].transform.GetChild(0).GetComponent<Image>().sprite = SelectSlotsprite(xmlReader.Value.Trim());
                                }
                            }
                            break;
                        case "Skill2":
                            if (xmlReader.Read())
                            {
                                if (xmlReader.Value.Trim() != "EmptySkill")
                                {
                                    skillButton[1].GetComponent<Image>().sprite = SelectSprite(xmlReader.Value.Trim());
                                    skillSlot[1].transform.GetChild(0).gameObject.SetActive(true);
                                    skillSlot[1].transform.GetChild(0).GetComponent<Image>().sprite = SelectSlotsprite(xmlReader.Value.Trim());
                                }
                            }
                            break;
                        case "Skill3":
                            if (xmlReader.Read())
                            {
                                if (xmlReader.Value.Trim() != "EmptySkill")
                                {
                                    skillButton[2].GetComponent<Image>().sprite = SelectSprite(xmlReader.Value.Trim());
                                    skillSlot[2].transform.GetChild(0).gameObject.SetActive(true);
                                    skillSlot[2].transform.GetChild(0).GetComponent<Image>().sprite = SelectSlotsprite(xmlReader.Value.Trim());
                                }
                            }
                            break;
                        case "Skill4":
                            if (xmlReader.Read())
                            {
                                if (xmlReader.Value.Trim() != "EmptySkill")
                                {
                                    skillButton[3].GetComponent<Image>().sprite = SelectSprite(xmlReader.Value.Trim());
                                    skillSlot[3].transform.GetChild(0).gameObject.SetActive(true);
                                    skillSlot[3].transform.GetChild(0).GetComponent<Image>().sprite = SelectSlotsprite(xmlReader.Value.Trim());
                                }
                            }
                            break;
                        case "Shoulder":
                            EquipFindInfo(xmlReader, "Shoulder", 1);
                            break;
                        case "ShoulderBool":
                            EquipBool(xmlReader, 1);
                            break;
                        case "ShoulderLock":
                            EquipLock(xmlReader, 1);
                            break;
                        case "ShoulderRein":
                            EquipRein(xmlReader, 1);
                            break;
                        case "Cap":
                            EquipFindInfo(xmlReader, "Cap", 2);
                            break;
                        case "CapBool":
                            EquipBool(xmlReader, 2);
                            break;
                        case "CapLock":
                            EquipLock(xmlReader, 2);
                            break;
                        case "CapRein":
                            EquipRein(xmlReader, 2);
                            break;
                        case "Cape":
                            EquipFindInfo(xmlReader, "Cape", 3);
                            break;
                        case "CapeBool":
                            EquipBool(xmlReader, 3);
                            break;
                        case "CapeLock":
                            EquipLock(xmlReader, 3);
                            break;
                        case "CapeRein":
                            EquipRein(xmlReader, 3);
                            break;
                        case "Weapon":
                            EquipFindInfo(xmlReader, "Weapon", 4);
                            break;
                        case "WeaponBool":
                            EquipBool(xmlReader, 4);
                            break;
                        case "WeaponLock":
                            EquipLock(xmlReader, 4);
                            break;
                        case "WeaponRein":
                            EquipRein(xmlReader, 4);
                            break;
                        case "Arrmor":
                            EquipFindInfo(xmlReader, "Arrmor", 5);
                            break;
                        case "ArrmorBool":
                            EquipBool(xmlReader, 5);
                            break;
                        case "ArrmorLock":
                            EquipLock(xmlReader, 5);
                            break;
                        case "ArrmorRein":
                            EquipRein(xmlReader, 5);
                            break;
                        case "Glove":
                            EquipFindInfo(xmlReader, "Glove", 6);
                            break;
                        case "GloveBool":
                            EquipBool(xmlReader, 6);
                            break;
                        case "GloveLock":
                            EquipLock(xmlReader, 6);
                            break;
                        case "GloveRein":
                            EquipRein(xmlReader, 6);
                            break;
                        case "Pet":
                            EquipFindInfo(xmlReader, "Pet", 7);
                            break;
                        case "PetBool":
                            EquipBool(xmlReader, 7);
                            break;
                        case "PetLock":
                            EquipLock(xmlReader, 7);
                            break;
                        case "PetRein":
                            EquipRein(xmlReader, 7);
                            break;
                        case "Shose":
                            EquipFindInfo(xmlReader, "Shose", 8);
                            break;
                        case "ShoseBool":
                            EquipBool(xmlReader, 8);
                            break;
                        case "ShoseLock":
                            EquipLock(xmlReader, 8);
                            break;
                        case "ShoseRein":
                            EquipRein(xmlReader, 8);
                            break;
                        case "Costum":
                            EquipFindInfo(xmlReader, "Costum", 9);
                            break;
                        case "CostumBool":
                            EquipBool(xmlReader, 9);
                            break;
                        case "CostumLock":
                            EquipLock(xmlReader, 9);
                            break;
                        case "CostumRein":
                            EquipRein(xmlReader, 9);
                            break;
                        case "Time":
                            if (xmlReader.Read())
                                npcShopSc.time = float.Parse(xmlReader.Value.Trim());
                            break;
                    }
                    for (int i = 0; i < inventorySlots.Length; i++)
                    {
                        InventorySlotInfo(xmlReader, saveLoadDataSc.inventoryImageName[i], i);
                        InventorySlotBool(xmlReader, saveLoadDataSc.inventorySlotBool[i], i);
                        InventorySlotLock(xmlReader, saveLoadDataSc.inventorySlotLock[i], i);
                        InventorySlotCount(xmlReader, saveLoadDataSc.inventorySlotCount[i], i);
                        InventorySlotRein(xmlReader, saveLoadDataSc.inventorySlotReinForce[i], i);
                    }
                }
            }
            maxHpMp(currentPlayerInfo.level);
        }
    }
}

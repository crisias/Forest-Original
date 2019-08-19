using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class SaveLoadData : MonoBehaviour
{
    string playerTag;
    Transform playerTr;
    Player playerSc;

    string dataBaseTag;
    Transform dataBaseTr;
    ItemManager itemManagerSc;

    //xml 파일 이름
    string xmlName = "playerData.xml";
    //코루틴 제어할 bool값
    bool autoSave;
    //장착된 스킬 저장하기위해 스킬이미지 가져오기 위해 사용한 오브젝트(드래그앤 드랍)
    public GameObject[] skillButton = new GameObject[4];
    //장착칸 아이템의 저장할 이미지 이름
    string[] equipImageName = { "Shoulder", "Cap", "Cape", "Weapon", "Arrmor", "Glove", "Pet", "Shose", "Costum" };
    string[] equipSlotBool = { "ShoulderBool", "CapBool", "CapeBool", "WeaponBool", "ArrmorBool", "GloveBool", "PetBool", "ShoseBool", "CostumBool" };
    string[] equipLock = { "ShoulderLock", "CapLock", "CapeLock", "WeaponLock", "ArrmorLock", "GloveLock", "PetLock", "ShoseLock", "CostumLock" };
    string[] equipReinForce = { "ShoulderRein", "CapRein", "CapeRein", "WeaponRein", "ArrmorRein", "GloveRein", "PetRein", "ShoseRein", "CostumRein" };
    //장착칸에 저장될 아이템의 이미지 주기위해(info로 잡으면 장착된 이미지가 없으므로 빈슬롯 이미지라도 저장되게 하기위해)(드래그앤 드랍)
    public GameObject[] equipSlots = new GameObject[9];
    //장착칸 아이템의 정보를 저장할 이름(inspecter창에서 작성)
    public string[] inventoryImageName;
    public string[] inventorySlotBool;
    public string[] inventorySlotLock;
    public string[] inventorySlotReinForce;
    public string[] inventorySlotCount;
    //인벤토리 오브젝트에 접근하기위해 상위 오브젝트(드래그앤 드롭)
    public GameObject content;
    //장착칸에 저장될 아이템에 정보 변경을 위한 오브젝트
    GameObject[] inventorySlots = new GameObject[60];

    string npcShopTag;
    Transform npcShopTr;
    NpcShop npcShopSc;

    //랜덤 생성 아이템 찾을 이름
    public string[] randomImageNames = new string[15];
    public string[] randomSlotBools = new string[15];
    public string randomImageName = "randName";
    public string randomSlotBool = "randBool";
    
    //잡화상점 랜덤 이미지 이름 저장하기위해 상위 오브젝트(드래그앤 드롭)
    public GameObject randomContents;
    //잡화상점 칸의 오브젝트들
    GameObject[] randomSlots = new GameObject[15];
    private void Awake()
    {
        //오브젝트 지정
        for (int i = 0; i < inventorySlots.Length; i++)
            inventorySlots[i] = content.transform.GetChild(i + 1).gameObject;

        npcShopTag = FindTag.getInstance().shopNpc;
        npcShopTr = Find.getInstance().FindTagTransform(npcShopTag);
        npcShopSc = npcShopTr.GetComponent<NpcShop>();

        for(int i = 0; i < randomSlotBools.Length; i++)
        {
            randomImageNames[i] = randomImageName + i;
            randomSlotBools[i] = randomSlotBool + i;
        }

        for(int i = 0; i < randomSlots.Length; i++)
            randomSlots[i] = randomContents.transform.GetChild(i).gameObject;
    }
    void Start()
    {
        playerTag = FindTag.getInstance().player;
        playerTr = Find.getInstance().FindTagTransform(playerTag);
        playerSc = playerTr.GetComponent<Player>();

        dataBaseTag = FindTag.getInstance().dataBase;
        dataBaseTr = Find.getInstance().FindTagTransform(dataBaseTag);
        itemManagerSc = dataBaseTr.GetComponent<ItemManager>();
    }

    void Update()
    {
        Save();
    }

    //저장 정보 초기화
    public void ReSet()
    {
        XmlDocument doc = new XmlDocument();
        // xml 버전, 인코틔딩, Standalone
        XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        // 해당 정보를 xml에 추가.
        doc.AppendChild(xmlDeclaration);
        // 루트를 만듭니다.
        XmlElement Root = doc.CreateElement("Player");
        // 루트도 추가.
        doc.AppendChild(Root);
        // 루트 하위로 데이터를 넣을꺼에요. 이번엔 약식으로 넣을께요.
        // Field라는 이름으로 하나 만들고 
        XmlElement status = (XmlElement)Root.AppendChild(doc.CreateElement("Status"));
        XmlElement skill = (XmlElement)Root.AppendChild(doc.CreateElement("Skill"));
        // Attribute를 추가. 이런식으로 넣으면 한개로 처리됩니다.
        // 즉, <A value=10 /> 이렇게.
        status.AppendChild(doc.CreateElement("Job")).InnerText = "0";
        status.AppendChild(doc.CreateElement("Level")).InnerText = "0";
        status.AppendChild(doc.CreateElement("Hp")).InnerText = "0";
        status.AppendChild(doc.CreateElement("Mp")).InnerText = "0";
        status.AppendChild(doc.CreateElement("Str")).InnerText = "0";
        status.AppendChild(doc.CreateElement("Dex")).InnerText = "0";
        status.AppendChild(doc.CreateElement("Def")).InnerText = "0";
        status.AppendChild(doc.CreateElement("Cripro")).InnerText = "0";
        status.AppendChild(doc.CreateElement("Cridem")).InnerText = "0";
        status.AppendChild(doc.CreateElement("Exp")).InnerText = "0";
        skill.AppendChild(doc.CreateElement("Skill1")).InnerText = "EmptySkill";
        skill.AppendChild(doc.CreateElement("Skill2")).InnerText = "EmptySkill";
        skill.AppendChild(doc.CreateElement("Skill3")).InnerText = "EmptySkill";
        skill.AppendChild(doc.CreateElement("Skill4")).InnerText = "EmptySkill";

        File.WriteAllText(Application.persistentDataPath + "/playerData.xml", doc.OuterXml, System.Text.Encoding.UTF8);

        Debug.Log(doc.OuterXml);
    }

    public void Save()
    {
        XmlDocument doc = new XmlDocument();
        // xml 버전, 인코틔딩, Standalone
        XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        // 해당 정보를 xml에 추가.
        doc.AppendChild(xmlDeclaration);
        // 루트를 만듭니다.
        XmlElement Root = doc.CreateElement("Player");
        // 루트도 추가.
        doc.AppendChild(Root);
        // 루트 하위로 데이터를 넣을꺼에요. 이번엔 약식으로 넣을께요.
        // Field라는 이름으로 하나 만들고 
        XmlElement status = (XmlElement)Root.AppendChild(doc.CreateElement("Status"));
        XmlElement skill = (XmlElement)Root.AppendChild(doc.CreateElement("Skill"));
        XmlElement equip = (XmlElement)Root.AppendChild(doc.CreateElement("Equip"));
        XmlElement inven = (XmlElement)Root.AppendChild(doc.CreateElement("Inven"));
        XmlElement shop = (XmlElement)Root.AppendChild(doc.CreateElement("Shop"));
        //캐릭터 스텟 저장
        status.AppendChild(doc.CreateElement("Job")).InnerText = playerSc.currentPlayerInfo.job.ToString();
        status.AppendChild(doc.CreateElement("Level")).InnerText = playerSc.currentPlayerInfo.level.ToString();
        status.AppendChild(doc.CreateElement("Hp")).InnerText = playerSc.currentPlayerInfo.hp.ToString();
        status.AppendChild(doc.CreateElement("Mp")).InnerText = playerSc.currentPlayerInfo.mp.ToString();
        status.AppendChild(doc.CreateElement("Str")).InnerText = playerSc.currentPlayerInfo.str.ToString();
        status.AppendChild(doc.CreateElement("Dex")).InnerText = playerSc.currentPlayerInfo.dex.ToString();
        status.AppendChild(doc.CreateElement("Def")).InnerText = playerSc.currentPlayerInfo.def.ToString();
        status.AppendChild(doc.CreateElement("Cripro")).InnerText = playerSc.currentPlayerInfo.cripro.ToString();
        status.AppendChild(doc.CreateElement("Cridem")).InnerText = playerSc.currentPlayerInfo.cridem.ToString();
        status.AppendChild(doc.CreateElement("Exp")).InnerText = playerSc.currentPlayerInfo.exp.ToString();
        status.AppendChild(doc.CreateElement("Gold")).InnerText = ItemManager.Gold.ToString();
        //스킬 장착상태 저장
        skill.AppendChild(doc.CreateElement("Skill1")).InnerText = skillButton[0].GetComponent<Image>().sprite.name;
        skill.AppendChild(doc.CreateElement("Skill2")).InnerText = skillButton[1].GetComponent<Image>().sprite.name;
        skill.AppendChild(doc.CreateElement("Skill3")).InnerText = skillButton[2].GetComponent<Image>().sprite.name;
        skill.AppendChild(doc.CreateElement("Skill4")).InnerText = skillButton[3].GetComponent<Image>().sprite.name;
        //장착칸의 장착 아이템 상태 저장
        for (int i = 0; i < equipImageName.Length; i++)
        {
            equip.AppendChild(doc.CreateElement(equipImageName[i])).InnerText = equipSlots[i].GetComponent<Image>().sprite.name;
            equip.AppendChild(doc.CreateElement(equipSlotBool[i])).InnerText = itemManagerSc.equipSlotBool[i + 1].ToString();
            equip.AppendChild(doc.CreateElement(equipLock[i])).InnerText = itemManagerSc.equipLockBool[i + 1].ToString();
            if (itemManagerSc.equipSlotBool[i + 1])
                equip.AppendChild(doc.CreateElement(equipReinForce[i])).InnerText = itemManagerSc.equipSlotInfos[i + 1].reinForce.ToString();
        }
        //인벤토리칸의 아이템 상태 저장
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            inven.AppendChild(doc.CreateElement(inventoryImageName[i])).InnerText = inventorySlots[i].GetComponent<Image>().sprite.name;
            inven.AppendChild(doc.CreateElement(inventorySlotBool[i])).InnerText = itemManagerSc.inventorySlotBool[i].ToString();
            inven.AppendChild(doc.CreateElement(inventorySlotLock[i])).InnerText = itemManagerSc.inventoryLockBool[i].ToString();
            if (itemManagerSc.inventorySlotBool[i])
            {
                inven.AppendChild(doc.CreateElement(inventorySlotReinForce[i])).InnerText = itemManagerSc.inventorySlotInfos[i].reinForce.ToString();
                inven.AppendChild(doc.CreateElement(inventorySlotCount[i])).InnerText = itemManagerSc.inventorySlotInfos[i].count.ToString();
            }
        }
        //상점 랜덤 테이블 저장
        shop.AppendChild(doc.CreateElement("Time")).InnerText = npcShopSc.time.ToString();
        for(int i = 0; i < randomSlotBools.Length; i++)
        {
            shop.AppendChild(doc.CreateElement(randomImageNames[i])).InnerText = randomSlots[i].GetComponent<Image>().sprite.name;
            shop.AppendChild(doc.CreateElement(randomSlotBools[i])).InnerText = npcShopSc.randomInvenBools[i].ToString();
        }

        File.WriteAllText(Application.persistentDataPath + "/playerData.xml", doc.OuterXml, System.Text.Encoding.UTF8);

        Debug.Log(doc.OuterXml);
    }
    
    public void Load()
    {
        string xmlFilePath = Application.persistentDataPath + "/" + xmlName;
        
        using (XmlReader xmlReader = XmlReader.Create(xmlFilePath))
        {
            while (xmlReader.Read())
            {
                if(xmlReader.IsStartElement())
                {
                    switch (xmlReader.Name)
                    {
                        case "Job":
                            if (xmlReader.Read())
                                playerSc.currentPlayerInfo.job = int.Parse(xmlReader.Value.Trim());
                            break;
                        case "Level":
                            if (xmlReader.Read())
                                playerSc.currentPlayerInfo.level = int.Parse(xmlReader.Value.Trim());
                            break;
                        case "Hp":
                            if (xmlReader.Read())
                                playerSc.currentPlayerInfo.hp = int.Parse(xmlReader.Value.Trim());
                            break;
                        case "Mp":
                            if (xmlReader.Read())
                                playerSc.currentPlayerInfo.mp = int.Parse(xmlReader.Value.Trim());
                            break;
                        case "Str":
                            if (xmlReader.Read())
                                playerSc.currentPlayerInfo.str = float.Parse(xmlReader.Value.Trim());
                            break;
                        case "Dex":
                            if (xmlReader.Read())
                                playerSc.currentPlayerInfo.dex = int.Parse(xmlReader.Value.Trim());
                            break;
                        case "Def":
                            if (xmlReader.Read())
                                playerSc.currentPlayerInfo.def = int.Parse(xmlReader.Value.Trim());
                            break;
                        case "Cripro":
                            if (xmlReader.Read())
                                playerSc.currentPlayerInfo.cripro = float.Parse(xmlReader.Value.Trim());
                            break;
                        case "Cridem":
                            if (xmlReader.Read())
                                playerSc.currentPlayerInfo.cridem = float.Parse(xmlReader.Value.Trim());
                            break;
                        case "Exp":
                            if (xmlReader.Read())
                                playerSc.currentPlayerInfo.exp = int.Parse(xmlReader.Value.Trim());
                            break;
                    }
                }
            }
            playerSc.maxHpMp(playerSc.currentPlayerInfo.level);
        }
    }
}

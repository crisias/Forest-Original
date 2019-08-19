using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTag : MonoBehaviour
{
    //현재 사용중인 tag들
    public string player = "Player";
    public string pet = "Pet";
    public string playerManager = "PlayerManager";
    public string petManager = "PetManager";
    public string rideButton = "RideButton";
    public string attackButton = "AttackButton";
    public string target = "Target";
    public string skillUi = "SkillUi";
    public string canvas = "Canvas";
    public string targetButton = "TargetButton";
    public string dragonSpawnPoint = "DragonSpawnPoint";
    public string monsterManager = "MonsterManager";
    public string bDragonHive = "BDragonHive";
    public string bDragonDead = "BDragonDead";
    public string skillButton1 = "SkillButton1";
    public string skillButton2 = "SkillButton2";
    public string skillButton3 = "SkillButton3";
    public string skillButton4 = "SkillButton4";
    public string itemInfoWindow = "ItemInfoWindow";
    public string equipSlot = "EquipSlot";
    public string dataBase = "DataBase";
    public string shopNpc = "ShopNpc";
    public string uiEffect = "UiEffect";
    public string stage = "Stage";
    public string field1 = "Field1";
    public string field2 = "Field2";
    public string frogHive = "FrogHive";
    public string frogSpawnPoint = "FrogSpawnPoint";
    public string frog = "Frog";
    public string graveStone = "GraveStone";

    //현재 클래스를 instance라는 이름으로 담을 곳을 만든다
    private static FindTag instance;
    public static FindTag getInstance()
    {
        //instance가 비어 있으면
        if(!instance)
        {
            //instance에 findobjectoftype로 찾아서 넣는다
            instance = GameObject.FindObjectOfType(typeof(FindTag)) as FindTag;

            //그래도 instance가 비었다면
            if(!instance)
            {
                //GameObject를 하나 만들고 컴퍼넌트로 FindTag를 넣는다
                GameObject a = new GameObject("FindTag");
                instance = a.AddComponent<FindTag>();
            }
        }
        return instance;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerCtrl : MonoBehaviour
{
    //player tag 오브젝트에 접근하기 위한 tag , transform, 클래스를 받을곳
    string playerTag;
    Transform playerTr;
    Player playerSc;
    //pet tag 오브젝트에 접근하기 위한 tag tr Pet
    string petTag;
    Transform petTr;
    Pet petSc;
    //player animation에 접근할곳
    Animation ani;
    //database 오브젝트에 접근하기 위한 tag tr sc
    string dataBaseTag;
    Transform dataBaseTr;
    ItemManager itemManagerSc;
    //캐릭터 좌표 조정을 위해서 필요함
    string petManagerTag;
    Transform petManagerTr;
    //이동에 사용할 bool값
    public static bool portalBool;
    //캐릭터가 죽은지 확인해줄 bool
    public static bool playerDead;

    string cos;
    string pet;

    public static bool cosSwap;
    public static bool petSwap;
    //플레이어 죽으면 나올 묘비 오브젝트 접근 준비
    string graveStoneTag;
    Transform graveStoneTr;
    //캔버스 오브젝트에 접근할 준비
    string canvasTag;
    Transform canvasTr;
    //플레이어 리젠 장소(드래그앤 드롭)
    public GameObject reSetPoint;
    //부활시 카메라 로테이션 조정을 위해
    public GameObject followCam;
    //자동 회복위해서 사용
    bool auto;
    //재화부활시 겸치 보호
    bool SaveExp;
    void Awake()
    {
        //각각 정보를 연결
        playerTag = FindTag.getInstance().player;
        playerTr = Find.getInstance().FindTagTransform(playerTag);
        playerSc = playerTr.GetComponent<Player>();

        petTag = FindTag.getInstance().pet;
        petTr = Find.getInstance().FindTagTransform(petTag);
        petSc = petTr.GetComponent<Pet>();

        dataBaseTag = FindTag.getInstance().dataBase;
        dataBaseTr = Find.getInstance().FindTagTransform(dataBaseTag);
        itemManagerSc = dataBaseTr.GetComponent<ItemManager>();

        petManagerTag = FindTag.getInstance().petManager;
        petManagerTr = Find.getInstance().FindTagTransform(petManagerTag);

        canvasTag = FindTag.getInstance().canvas;
        canvasTr = Find.getInstance().FindTagTransform(canvasTag);

        graveStoneTag = FindTag.getInstance().graveStone;
        graveStoneTr = Find.getInstance().FindTagTransform(graveStoneTag);
    }

    void Start()
    {
        //player의 시작 시 animation을 정해주고 시작
        ani = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Animation>();
        //ani.clip = currentPlayerInfo.playerInfo.Anim[0];
        ani.Play();
    }

    void Update()
    {
        if (!playerDead)
        {
            PlayerPositionRota();
            PlayerCurrentAni();
        }
        //의상템 장착 했을시(itemManager에서 true)
        if (cosSwap)
        {
            SwapCloths();
        }
        //펫템 장착했을시(itemManager에서 true)
        if (petSwap)
        {
            SwapPets();
        }
        if(playerSc.currentPlayerInfo.hp <= 0)
        {
            playerDead = true;
            Dead();
        }
        //가만히 있으면 회복한다
        if (!auto)
            StartCoroutine(WaitHeal());
    }
    //의상 교체 함수
    void SwapCloths()
    {
        //현재 장착된 의상템의 id를 담는다
        cos = itemManagerSc.equipSlotInfos[9].name;   
        switch (cos)
        {
            case "노랑"://노랑
                OffCloths();
                playerTr.GetChild(0).transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(true);
                playerTr.GetChild(0).transform.GetChild(0).transform.GetChild(5).gameObject.SetActive(true);
                cosSwap = false;
                break;
            case "네이비"://네이비
                OffCloths();
                playerTr.GetChild(0).transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
                playerTr.GetChild(0).transform.GetChild(0).transform.GetChild(4).gameObject.SetActive(true);
                cosSwap = false;
                break;
            case "핑크"://핑크
                OffCloths();
                playerTr.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
                playerTr.GetChild(0).transform.GetChild(0).transform.GetChild(6).gameObject.SetActive(true);
                cosSwap = false;
                break;
        }
    }
    //모든 의상 끄기
    void OffCloths()
    {
        playerTr.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
        playerTr.GetChild(0).transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
        playerTr.GetChild(0).transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(false);
        playerTr.GetChild(0).transform.GetChild(0).transform.GetChild(4).gameObject.SetActive(false);
        playerTr.GetChild(0).transform.GetChild(0).transform.GetChild(5).gameObject.SetActive(false);
        playerTr.GetChild(0).transform.GetChild(0).transform.GetChild(6).gameObject.SetActive(false);
    }
    //펫 외형 교체 함수
    void SwapPets()
    {
        pet = itemManagerSc.equipSlotInfos[7].name;

        switch (pet)
        {
            case "펫 : 강아지"://개
                OffPet();
                petTr.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
                petSwap = false;
                break;
            case "펫 : 레서판더"://팬더
                OffPet();
                petTr.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                petSwap = false;
                break;
            case "펫 : 여우"://여우
                OffPet();
                petTr.GetChild(0).transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(true);
                petSwap = false;
                break;
            case "펫 : 곰"://곰
                OffPet();
                petTr.GetChild(0).transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
                petSwap = false;
                break;
        }
    }
    //모든 펫의 외형을 끈다
    void OffPet()
    {
        petTr.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
        petTr.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
        petTr.GetChild(0).transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
        petTr.GetChild(0).transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(false);
    }
    //player의 상황에 따라 position과 rotation을 정해주는 함수
    void PlayerPositionRota()
    {
        //탑승중일때
        if (UiManager.ride)
        {
            playerTr.position = petTr.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(10).gameObject.transform.position;
            //공격중이 아닐때의 rotation
            if (!Skill.boolRotation && !Skill.skillRote)
                playerTr.rotation = petTr.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(10).gameObject.transform.rotation;
        }
        //탑승중이 아닐때
        else
        {
            playerTr.position = petTr.position;
            playerTr.rotation = petTr.rotation;
        }
    }

    //player의 상황에 따라 animation을 실행시켜줄 함수
    void PlayerCurrentAni()
    {
        //탑승중이고 공격모션 중이 아닐때
        if (UiManager.ride && !Skill.boolRotation && !Skill.skillRote)
        {
            ani.CrossFade("run", 0.1f);
        }
        //탑승중이고 공격모션중일때
        else if(UiManager.ride && Skill.boolRotation)
        {
            ani.CrossFade("attack", 0.1f);
        }
        else if(UiManager.ride && Skill.skillRote)
        {
            ani.CrossFade("skill", 0.1f);
        }
        //탑승중이 아닐때
        else
        { 
            ani.CrossFade("idle", 0.05f);
        }
    }
    //이동 버튼에 사용
    public void PortalButton()
    {
        portalBool = true;
    }

    void Dead()
    {
        if(playerDead)
        {
            canvasTr.transform.GetChild(19).gameObject.SetActive(true);
            graveStoneTr.GetChild(0).gameObject.SetActive(true);
            graveStoneTr.position = petTr.position;
            graveStoneTr.rotation = petTr.rotation;
            playerTr.transform.GetChild(0).gameObject.SetActive(false);
            petTr.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    //부활
    public void ReSet()
    {
        playerSc.currentPlayerInfo.hp = playerSc.currentMaxStatus.hp;
        //묘지 오브젝트 끄고
        graveStoneTr.GetChild(0).gameObject.SetActive(false);
        //부활 설명창 끄고
        canvasTr.transform.GetChild(19).gameObject.SetActive(false);
        //플레이어 오브젝트 켜고
        playerTr.transform.GetChild(0).gameObject.SetActive(true);
        //플레이어와 펫 위치 변경
        petTr.transform.position = reSetPoint.transform.position;
        playerTr.transform.position = reSetPoint.transform.position;
        petTr.transform.rotation = reSetPoint.transform.rotation;
        playerTr.transform.rotation = reSetPoint.transform.rotation;
        //캐릭터가 탑승 상태에서 죽었을시 탑승상태 해제
        UiManager.ride = false;
        //카메라 위치 변경
        followCam.transform.rotation = reSetPoint.transform.rotation;
        //다시 죽을수있게
        playerDead = false;
        //부활 패널티
        if(!SaveExp)
        {
            if (playerSc.currentPlayerInfo.exp >= /*(playerSc.magiMaxStatus.status[playerSc.currentPlayerInfo.level - 1].exp / 10)*/10)
            {
                playerSc.currentPlayerInfo.exp -= /*(playerSc.magiMaxStatus.status[playerSc.currentPlayerInfo.level - 1].exp / 10)*/10;
            }
            else
                playerSc.currentPlayerInfo.exp = 0;
        }
    }
    //유료 부활창
    public void BuyReset()
    {
        //소지금이 충분하다면
        if(ItemManager.Gold >= 100000)
        {
            //소지금 깍고
            ItemManager.Gold -= 100000;
            SaveExp = true;
            //부활
            ReSet();
            //죽은상태 false
            playerDead = false;
            SaveExp = false;
        }
        else
        {
            //부활 실패창 켜준다
            canvasTr.GetChild(19).transform.GetChild(4).gameObject.SetActive(true);
        }
    }
    //부활 실패창 ui 꺼준다
    public void CloseFailReset()
    {
        canvasTr.GetChild(19).transform.GetChild(4).gameObject.SetActive(false);
    }
    //시간당 체력 마력 5% 자동회복 넘칠시 최대값 고정 움직이지 않으면 회복
    //test 3초 마다 5%
    IEnumerator WaitHeal()
    {
        auto = true;
        yield return new WaitForSeconds(1f);
        if (!JoyStickCtrl.moveFlag && !playerDead)
        {
            //float int 형문제로 소수점에 1레벨때 치료가 안되는 현상 발견후 수정
            float tempHp = ((float)playerSc.currentMaxStatus.hp / 100) * 5;
            float tempMp = ((float)playerSc.currentMaxStatus.mp / 100) * 5;

            playerSc.currentPlayerInfo.hp += (int)tempHp;
            playerSc.currentPlayerInfo.mp += (int)tempMp;

            if (playerSc.currentPlayerInfo.hp > playerSc.currentMaxStatus.hp)
                playerSc.currentPlayerInfo.hp = playerSc.currentMaxStatus.hp;
            if (playerSc.currentPlayerInfo.mp > playerSc.currentMaxStatus.mp)
                playerSc.currentPlayerInfo.mp = playerSc.currentMaxStatus.mp;
        }
        auto = false;
    }
}

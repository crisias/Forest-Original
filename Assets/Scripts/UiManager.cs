using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    //ride 버튼
    public static bool ride = false;
    //스킬창 버튼
    public static bool skillUiSwitch = false;
    //인벤토리창 버튼
    public static bool inventorySwitch = false;
    //canvas에 접근하기위한 tag tr
    string canvasTag;
    Transform canvasTr;
    //ride 버튼 클릭 이미지
    public Sprite on;
    public Sprite off;
    //ridebutton에 접근하기위한 tag tr Image
    string rideButtonTag;
    Transform rideButtonTr;
    Image rideButton;
    //Pet에 접근하기위한 tag tr
    string petTag;
    Transform petTr;
    //player객체에 접근하기 위한 tag tr sc
    string playerTag;
    Transform playerTr;
    Player playerSc;
    //직업 한글로 띄우기위해 사용
    string job;
    //database객체에 접근하기위해 사용 tag tr sc
    string dataBaseTag;
    Transform dataBaseTr;
    ItemManager itemManagerSc;
    //스킬창 contents오브젝트(드래그앤 드롭) 스킬창 껐다켰을때 스크롤 위치 재위치할때 쓴다
    public GameObject skillContents;
    //인벤토리창 contents오브젝트(드래그앤 드롭) 인벤토리창 껐다켰을때 스크롤 위치 재위치할때 쓴다
    public GameObject inventoryContents;

    void Awake()
    {
        //각각의 정보를 받아온다
        rideButtonTag = FindTag.getInstance().rideButton;
        rideButtonTr = Find.getInstance().FindTagTransform(rideButtonTag);
        rideButton = rideButtonTr.GetComponent<Image>();

        canvasTag = FindTag.getInstance().canvas;
        canvasTr = Find.getInstance().FindTagTransform(canvasTag);

        petTag = FindTag.getInstance().pet;
        petTr = Find.getInstance().FindTagTransform(petTag);

        playerTag = FindTag.getInstance().player;
        playerTr = Find.getInstance().FindTagTransform(playerTag);
        playerSc = playerTr.GetComponent<Player>();

        dataBaseTag = FindTag.getInstance().dataBase;
        dataBaseTr = Find.getInstance().FindTagTransform(dataBaseTag);
        itemManagerSc = dataBaseTr.GetComponent<ItemManager>();
    }

    void Start()
    {
        //각각의 직업을 받아서 한글로 변환
        switch (playerSc.currentJob)
        {
            case "Magician":
                job = "마법사";
                break;
        }
    }

    void Update()
    {
        //스킬창 켰을때
        if (skillUiSwitch)
        {
            //스킬창 ui
            canvasTr.GetChild(4).gameObject.SetActive(true);
            //스킬창 판넬
            canvasTr.GetChild(3).transform.GetChild(2).gameObject.SetActive(true);
        }
        //스킬창 껐을때
        else
        {
            //스킬창 ui
            canvasTr.GetChild(4).gameObject.SetActive(false);
            //스킬창 판넬
            canvasTr.GetChild(3).transform.GetChild(2).gameObject.SetActive(false);
            //슬라이드바를 원위치로
            skillContents.GetComponent<RectTransform>().localPosition = new Vector3(skillContents.GetComponent<RectTransform>().localPosition.x, 0, 0);
        }

        LevelDisplay();
        StatusDisplay(playerSc.currentPlayerInfo.level);
        HpMpGause();
        ExpGause();
        ShowGoldWindow();
    }
    //스킬창 버튼 함수
    public void SkillUi()
    {
        skillUiSwitch = !skillUiSwitch;
    }
    //ride 버튼 함수
    public void RideButtonSprite()
    {
        ride = !ride;
        if (ride)
        {
            rideButton.sprite = on;
            //펫오브젝트 켜준다
            petTr.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            rideButton.sprite = off;
            //펫 오브젝트 꺼준다
            petTr.GetChild(0).gameObject.SetActive(false);
        }
    }
    //스텟, 인벤토리창 함수
    public void StatusInventory()
    {
        inventorySwitch = !inventorySwitch;

        if (inventorySwitch)
        {
            //스텟창 ui
            canvasTr.GetChild(5).gameObject.SetActive(true);
            //인벤토리창 ui
            canvasTr.GetChild(6).gameObject.SetActive(true);
            //인벤토리창 판넬 ui
            canvasTr.GetChild(3).GetChild(3).gameObject.SetActive(true);
        }
        else
        {
            //스텟창 ui
            canvasTr.GetChild(5).gameObject.SetActive(false);
            //인벤토리창 ui
            canvasTr.GetChild(6).gameObject.SetActive(false);
            //인벤토리창 판넬 ui
            canvasTr.GetChild(3).GetChild(3).gameObject.SetActive(false);
            //인벤토리창 스크롤 위치 초기화
            inventoryContents.GetComponent<RectTransform>().localPosition = new Vector3(inventoryContents.GetComponent<RectTransform>().localPosition.x, 0, 0);
        }
    }
    //레벨창 함수
    void LevelDisplay()
    {
        //레벨창 ui
        canvasTr.GetChild(2).transform.GetChild(6).transform.GetChild(0).GetComponent<Text>().text = "Lv. " + playerSc.currentPlayerInfo.level;
    }
    //스텟, 인벤토리 창에 띄어줄 스텟 설명 텍스트
    void StatusDisplay(int currentLevel)
    {
        canvasTr.GetChild(5).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = "레벨 : " + playerSc.currentPlayerInfo.level;
        canvasTr.GetChild(5).transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text = "체력 : " + playerSc.currentPlayerInfo.hp + " / " + playerSc.currentMaxStatus.hp;
        canvasTr.GetChild(5).transform.GetChild(1).transform.GetChild(2).GetComponent<Text>().text = "공격력 : " + playerSc.currentPlayerInfo.str;
        canvasTr.GetChild(5).transform.GetChild(1).transform.GetChild(3).GetComponent<Text>().text = "회피율 : " + playerSc.currentPlayerInfo.dex + "%";
        canvasTr.GetChild(5).transform.GetChild(1).transform.GetChild(4).GetComponent<Text>().text = "치명타 확률 : " + playerSc.currentPlayerInfo.cripro + "%";
        canvasTr.GetChild(5).transform.GetChild(1).transform.GetChild(5).GetComponent<Text>().text = "치명타 데미지 : " + playerSc.currentPlayerInfo.cridem + "%";
        canvasTr.GetChild(5).transform.GetChild(1).transform.GetChild(6).GetComponent<Text>().text = "직업 : " + job;
        canvasTr.GetChild(5).transform.GetChild(1).transform.GetChild(7).GetComponent<Text>().text = "마력 : " + playerSc.currentPlayerInfo.mp + " / " + playerSc.currentMaxStatus.mp;
        canvasTr.GetChild(5).transform.GetChild(1).transform.GetChild(8).GetComponent<Text>().text = "방어력 : " + playerSc.currentPlayerInfo.def;
    }
    //hp, mp게이지 증감 함수
    void HpMpGause()
    {
        float h = (float)playerSc.currentPlayerInfo.hp / ((float)playerSc.currentMaxStatus.hp);
        //hp창 ui
        canvasTr.GetChild(2).transform.GetChild(2).GetComponent<Image>().fillAmount = h;
        float m = (float)playerSc.currentPlayerInfo.mp / ((float)playerSc.currentMaxStatus.mp);
        //mp창 ui
        canvasTr.GetChild(2).transform.GetChild(3).GetComponent<Image>().fillAmount = m;
    }
    //exp게이지 증감 함수
    void ExpGause()
    {
        float exp = (float)playerSc.currentPlayerInfo.exp / (float)playerSc.magiMaxStatus.status[playerSc.currentPlayerInfo.level - 1].exp;
        //exp창 ui
        canvasTr.GetChild(2).transform.GetChild(8).GetComponent<Image>().fillAmount = exp;
    }
    //레벨부족시 레벨 부족 안내창 띄운다
    public void EndFailWindow()
    {
        //레벨 안내창 판넬
        canvasTr.GetChild(9).gameObject.SetActive(false);
        //레벨 안내창 ui
        canvasTr.GetChild(10).gameObject.SetActive(false);
    }
    //player gold 표시
    public void ShowGoldWindow()
    {
        //캔버스 서순 때문에 3개 의 소지금 창을 운용(아이템 정보창 켰을땐 판넬 밑에 상점창들을 켰을땐 판넬위에 두고 싶은데 서순때문에 안되는 문제 그래서 2개 사용)
        //플레이어 인벤토리 소지금 ui
        canvasTr.GetChild(3).transform.GetChild(4).transform.GetChild(1).GetComponent<Text>().text = ItemManager.Gold + "원";
        //잡화상점 소지금 ui
        canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(0).transform.GetChild(3).transform.GetChild(1).GetComponent<Text>().text = ItemManager.Gold + "원";
        //캐쉬상점 소지금 ui
        canvasTr.GetChild(16).transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text = ItemManager.Gold + "원";
        //제작상점 소지금 ui
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = ItemManager.Gold + "원";
        //강화상점 소지금 ui
        canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = ItemManager.Gold + "원";
        //플레이어 인벤토리 켰다 끌때
        if (inventorySwitch)
            canvasTr.GetChild(3).transform.GetChild(4).gameObject.SetActive(true);
        else if (!inventorySwitch)
            canvasTr.GetChild(3).transform.GetChild(4).gameObject.SetActive(false);
    }
}

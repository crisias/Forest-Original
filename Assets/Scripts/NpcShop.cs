using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Xml;

public class NpcShop : MonoBehaviour
{
    //canvas에 접근하기 위한 tag, tr
    string canvasTag;
    Transform canvasTr;
    //database에 있는 itemmanager에 접근하기위한 tag, tr, sc
    string dataBaseTag;
    Transform dataBaseTr;
    ItemManager itemManagerSc;
    SaveLoadData saveLoadDataSc;
    //npc주위에 가면 player머리에 띄어줄 버튼의 이미지
    Image buttonImage;
    //player 오브젝트에 접근(드래그앤 드롭)
    public GameObject player;
    //npc 오브젝트에 접근(드래근앤 드롭)
    public GameObject shopNpc;
    public GameObject cashNpc;
    public GameObject blackNpc;
    //npc 버튼에 사용할 각각의 이미지(드래그앤 드롭)
    public Sprite shopImage;
    public Sprite cashImage;
    public Sprite blackImage;
    //npc와 player간의 거리를 담을곳
    public float shopDist;
    public float cashDist;
    public float blackDist;
    //버튼 사용여부
    public static bool shopBool;
    //잡화 상점 이용시 잡화상점 인벤의 각각의 칸 오브젝트
    GameObject[] shopInvenInfo = new GameObject[60];
    //현재 선택된 잡화상점 인벤토리의 번호
    int currentIndex;
    //잡화상점 인벤토리 칸이미지 사용후 다시돌려 놓을때 쓸 이미지
    public Sprite slotCuber;
    //랜덤 아이템 셋팅칸 오브젝트 담을곳
    public GameObject[] randomInvenSlots = new GameObject[15];
    //랜덤 아이템 정보를 담을 곳
    public ItemInfo[] randomInvenInfos = new ItemInfo[15];
    //랜덤 아이템 유무 확인 bool
    public bool[] randomInvenBools = new bool[15];
    //아이템 정보 원본이 담긴곳(드래그앤 드롭)
    public ItemTable itemTable;
    //잡화상점 랜덤 아이템 생성할때 쓸 int
    int rand;
    int rand2;
    //랜덤 아이템 변경 쿨타임 조절한 bool값
    bool wait;
    //구매 아이템칸의 순서 번호
    int buyCurrentIndex;
    //아이템 구매시에 쓸 bool값
    bool check;
    //아이템 구매시에 쓸 int값
    int checkNum;
    //리셋버튼에 사용하는 bool값
    bool resetBool;
    //잡화상점 타이머에 사용할 값
    public float time;
    float maxTime = 3601;
    int temporaryTime;
    //캐쉬 상점 구매 번호
    int cashIndex;
    int selectNum;
    //랜덤뽑기 이펙트에 사용할 bool값
    public bool randomBool;
    bool randomEnd;
    //랜덤 칸 마다의 bool값
    public bool[] randomImageBool = new bool[10];
    //뽑은 아이템 정보값 저장할곳
    public List<ItemInfo> randomItem = new List<ItemInfo>();
    //뽑은 아이템을 보여줄 오브젝트 접근
    GameObject[] randomObjects = new GameObject[10];
    //캐쉬상점 뽑기 이펙트 초기화해주기 위해 접근 준비
    string uiEffectTag;
    Transform uiEffectTr;
    UiEffect uiEffectSc;
    //잡화상점 인벤토리 재위치를 하기위해서 사용(드래그앤 드롭)
    public GameObject shopInventoryContents;
    //캐쉬상점 목록 재위치를 하기위해서 사용(드래그앤 드롭)
    public GameObject cashShopContents;

    string xmlName = "playerData.xml";

    public Text testtext;

    void Start()
    {
        //canvas 오브젝트에 접근
        canvasTag = FindTag.getInstance().canvas;
        canvasTr = Find.getInstance().FindTagTransform(canvasTag);
        //itemmanager에 접근
        dataBaseTag = FindTag.getInstance().dataBase;
        dataBaseTr = Find.getInstance().FindTagTransform(dataBaseTag);
        itemManagerSc = dataBaseTr.GetComponent<ItemManager>();
        saveLoadDataSc = dataBaseTr.GetComponent<SaveLoadData>();
        //npc버튼 오브젝트에 접근
        buttonImage = canvasTr.GetChild(15).GetComponent<Image>();
        //shop 인벤토리 칸 오브젝트들에 접근
        for(int i = 0; i < shopInvenInfo.Length; i++)
            shopInvenInfo[i] = canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(i + 1).gameObject;
        //잡화상점 아이템 랜덤 생성을 위한 랜덤칸 오브젝트들에 접근
        for (int i = 0; i < randomInvenSlots.Length; i++)
            randomInvenSlots[i] = canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(2).transform.GetChild(i).gameObject;
        if(SelectSceneUiManager.result)
        {
            testtext.text = "있음";
            Load();
        }
        else if (!SelectSceneUiManager.result)
        {
            testtext.text = "없음";
            //잡화상점에서 판매할 랜덤 아이템 생성
            RandomItemSet();
        }
        //랜덤아이템 보여줄 오브젝트 연결
        for (int i = 0; i < randomObjects.Length; i++)
            randomObjects[i] = canvasTr.GetChild(16).transform.GetChild(2).transform.GetChild(4).transform.GetChild(1).transform.GetChild(i).gameObject;
        //uieffect에 접근
        uiEffectTag = FindTag.getInstance().uiEffect;
        uiEffectTr = Find.getInstance().FindTagTransform(uiEffectTag);
        uiEffectSc = uiEffectTr.GetComponent<UiEffect>();
        //제작창 ui 셋팅
        MakePageSet();
        //강화창에서 쓸 인벤토리 칸에 연결
        for (int i = 0; i < reinSlotBool.Length; i++)
            reinObjects[i] = canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(i + 1).gameObject;
    }

    void Update()
    {
        //타이머 계속 연산
        Timer();
        //계속해서 player와 npc간의 거리 연산
        shopDist = Vector3.Distance(player.transform.position, shopNpc.transform.position);
        cashDist = Vector3.Distance(player.transform.position, cashNpc.transform.position);
        blackDist = Vector3.Distance(player.transform.position, blackNpc.transform.position);
        //잡화상점 npc가 특정 거리 안에 있고 인벤토리버튼과 스킬창버튼이 사용되지 않았을때
        if (shopDist <= 10f && !UiManager.inventorySwitch && !UiManager.skillUiSwitch)
        {
            //버튼 오브젝트 활성화 하고 이미지 변경
            canvasTr.GetChild(15).gameObject.SetActive(true);
            buttonImage.sprite = shopImage;
        }
        //캐쉬상점 npc가 특정 거리 안에 있고 인벤토리 버튼과 스킬창 버튼이 사용되지 않았을때
        else if (cashDist <= 10f && !UiManager.inventorySwitch && !UiManager.skillUiSwitch)
        {
            //버튼 오브젝트 활성화 하고 이미지 변경
            canvasTr.GetChild(15).gameObject.SetActive(true);
            buttonImage.sprite = cashImage;
        }
        //대장간 npc가 특정 거리 안에 있고 인벤토리 버튼과 스킬창 버튼이 사용되지 않았을때
        else if (blackDist <= 10f && !UiManager.inventorySwitch && !UiManager.skillUiSwitch)
        {
            //버튼 오브젝트 활성화 하고 이미지 변경
            canvasTr.GetChild(15).gameObject.SetActive(true);
            buttonImage.sprite = blackImage;
        }
        //특정 거리안에 들어가 있지 않다면 버튼 오브젝트 비활성화
        else
            canvasTr.GetChild(15).gameObject.SetActive(false);
        //잡화상점 이용중이라면 계속해서 실행
        if (shopBool)
        {
            ShopPlayerItemInfo();
        }
        /*잡화상점에서 잡템을 한종류를 다팔고 정리를 했을때 제일 마지막 템의 이미지가 그다음칸에 잔류하는 버그를 발견
        그칸의 정보는 없는 것으로 나오지만 그칸 전의 이미지와 개수가 표기됨 그래서 강제로 계속해서 
        칸의 정보가 없다면 이미지와 개수를 없는 상태로 표시하게함*/
        for (int i = 0; i < shopInvenInfo.Length; i++)
        {
            if (!itemManagerSc.inventorySlotBool[i])
            {
                shopInvenInfo[i].GetComponent<Image>().sprite = slotCuber;
                shopInvenInfo[i].transform.GetChild(0).gameObject.GetComponent<Text>().text = " ";
                shopInvenInfo[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        //wait가 false면 랜덤아이템 바꿔줄 코르틴 시작
        if (!wait)
        {
            StartCoroutine(ReRandom());
        }
        //나중에 지우자(랜덤 뽑기 아이템 하씩나오게 하는 코루틴 근데 뽑고 커버로 가리니까 쓸모없다)
        if (randomBool)
        {
            StartCoroutine(effect());
        }
        //제조 이펙트를 제어
        if (flameEffect)
        {
            StartCoroutine(FlameSprite());
        }
        //
        if(reinBool)
        {
            UseReinForce();
        }
        if (reinEffectBool)
            StartCoroutine(ReinEffectSprite());
    }
    //npc주위에 다가가면 활성화되는 버튼(하나로 3개의 npc가 사용중)
    public void ShopButton()
    {
        shopBool = !shopBool;
        //현재 선택된 버튼의 오브젝트에 접근
        GameObject currentObject = EventSystem.current.currentSelectedGameObject;
        //panel 활성화
        canvasTr.GetChild(16).transform.GetChild(0).gameObject.SetActive(true);
        //오브젝트의 이미지 이름이 shopicon일고 잡화상점 이용중일때
        if (currentObject.GetComponent<Image>().sprite.name == "ShopIcon" && shopBool)
        {
            //잡화상점 ui 활성화
            canvasTr.GetChild(16).transform.GetChild(1).gameObject.SetActive(true);
        }
        //오브젝트의 이미지 이름이 cashshopicon일때
        else if (currentObject.GetComponent<Image>().sprite.name == "CashShopIcon")
        {
            //캐쉬상점 ui 활성화
            canvasTr.GetChild(16).transform.GetChild(2).gameObject.SetActive(true);
        }
        //오브젝트의 이미지 이름이 blackshopicon일때
        else if (currentObject.GetComponent<Image>().sprite.name == "BlackShopIcon")
        {
            //강화, 제작상점 선택창 ui 활성화
            canvasTr.GetChild(16).transform.GetChild(3).gameObject.SetActive(true);
        }
    }
    //잡화상점 꺼줄 함수(판넬(버튼)에 사용중)
    public void EndShop()
    {
        shopBool = !shopBool;
        if(!shopBool)
        {
            //판넬ui
            canvasTr.GetChild(16).transform.GetChild(0).gameObject.SetActive(false);
            //잡화상점ui
            canvasTr.GetChild(16).transform.GetChild(1).gameObject.SetActive(false);
            //캐쉬상점ui
            canvasTr.GetChild(16).transform.GetChild(2).gameObject.SetActive(false);
            //강화,제작 선택창 ui
            canvasTr.GetChild(16).transform.GetChild(3).gameObject.SetActive(false);
            //제작창 ui
            canvasTr.GetChild(16).transform.GetChild(4).gameObject.SetActive(false);
            //강화상점 ui
            canvasTr.GetChild(16).transform.GetChild(5).gameObject.SetActive(false);
            makeShopBool = false;
            reinBool = false;
            //강화상점 ui 끌때 메인재료창 서브재료창에 아이템이 있다면
            ReinForceCancle();
            //강화상점 ui 끌때 선택효과 있는 상태에서 끄면 닫히면서 효과 사라지게
            if(!reinBool)
            {
                for(int i = 0; i < reinSlotBool.Length; i++)
                {
                    if(itemManagerSc.inventorySlotBool[i])
                    {
                        reinObjects[i].transform.GetChild(3).gameObject.SetActive(false);
                    }
                }
            }
            //잡화상점 인벤토리 스크롤 위치 초기화
            shopInventoryContents.GetComponent<RectTransform>().localPosition = new Vector3(shopInventoryContents.GetComponent<RectTransform>().localPosition.x, 0, 0);
            //캐쉬상점 인벤토리 스크롤 위치 초기화
            cashShopContents.GetComponent<RectTransform>().localPosition = new Vector3(0, cashShopContents.GetComponent<RectTransform>().localPosition.y, 0);
            //강화상점 인벤토리 스크롤 위치 초기화
            reinForceContents.GetComponent<RectTransform>().localPosition = new Vector3(reinForceContents.GetComponent<RectTransform>().localPosition.x, 0, 0);
        }
    }
    //잡화상점 이용중일때 계속해서 실행
    void ShopPlayerItemInfo()
    {
        //잡화상점 인벤 칸수 만큼 반복
        for(int i = 0; i < itemManagerSc.inventorySlotInfos.Length; i++)
        {
            //칸정보가 true일때
            if(itemManagerSc.inventorySlotBool[i])
            {
                //잡화상점 인벤의 이미지 교체
                shopInvenInfo[i].GetComponent<Image>().sprite = itemManagerSc.inventorySlotInfos[i].cuberImage;
                //잡화상점 인벤의 잠금 오브젝트를 비활성화
                shopInvenInfo[i].transform.GetChild(1).gameObject.SetActive(false);
                //잡화상점 인벤의 개수 텍스트 공란 설정
                shopInvenInfo[i].transform.GetChild(0).GetComponent<Text>().text = " ";
                //잡화상점 인벤의 강화 텍스트 공란 설정
                shopInvenInfo[i].transform.GetChild(2).GetComponent<Text>().text = " ";
                //인벤토리 칸의 개수 정보가 0이 아니라면 칸정보의 개수대로 교체
                if (itemManagerSc.inventorySlotInfos[i].count != 0)
                    shopInvenInfo[i].transform.GetChild(0).gameObject.GetComponent<Text>().text = itemManagerSc.inventorySlotInfos[i].count.ToString();
                //인벤토리 칸이 잠금상태라면 잡화상점인벤의 칸도 잠금오브젝트를 활성화
                if (itemManagerSc.inventoryLockBool[i])
                    shopInvenInfo[i].transform.GetChild(1).gameObject.SetActive(true);
                //인벤토리 칸의 강화 정보가 0이 아니라면 칸정보의 개수대로 교체
                if (itemManagerSc.inventorySlotInfos[i].reinForce != 0)
                    shopInvenInfo[i].transform.GetChild(2).GetComponent<Text>().text = "+" + itemManagerSc.inventorySlotInfos[i].reinForce.ToString();
            }
        }
    }
    //잡화상점 인벤에서 안내창 띄울 버튼(잡화상점 인벤 칸 전부가 사용중)
    public void ShopInvenInfoWindowButton()
    {
        //현재 버튼이 사용된 오브젝트를 구별하기 위해사용
        GameObject currentObject = EventSystem.current.currentSelectedGameObject;
        //현재 버튼이 사용된 오브젝트의 이름을 담는다
        string currentName = currentObject.name;
        //현재 버튼의 순서를 파악
        currentIndex = (canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).Find(currentName).GetSiblingIndex()) - 1;
        //현재 칸에 아이템이 있다면
        if(itemManagerSc.inventorySlotBool[currentIndex])
        {   
            //현재 아이템이 잠금 상태라면
            if (currentObject.transform.GetChild(1).gameObject.activeInHierarchy)
            {
                //잠금안내 ui
                canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(4).gameObject.SetActive(true);
            }
            //잠금상태가 아니라면
            else
            {
                //판매 선택창 ui
                canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(5).gameObject.SetActive(true);
                //판매할 아이템이 강화가 되있을때
                if(itemManagerSc.inventorySlotInfos[currentIndex].reinForce > 0)
                {
                    float calcu = itemManagerSc.inventorySlotInfos[currentIndex].gold + ((itemManagerSc.inventorySlotInfos[currentIndex].gold / 10) * itemManagerSc.inventorySlotInfos[currentIndex].reinForce) * (itemManagerSc.inventorySlotInfos[currentIndex].level / 10);
                    //판매 선택창 ui 텍스트
                    canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(5).transform.GetChild(1).transform.GetChild(0).
                        gameObject.GetComponent<Text>().text = "+" + itemManagerSc.inventorySlotInfos[currentIndex].reinForce + " " + itemManagerSc.inventorySlotInfos[currentIndex].name + "을(를) " +
                        (int)calcu + "원에 판매하시겠습니까?";
                }
                //판매할 아이템이 강화가 안되있을때
                else
                {
                    //판매 선택창 ui 텍스트
                    canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(5).transform.GetChild(1).transform.GetChild(0).
                        gameObject.GetComponent<Text>().text = itemManagerSc.inventorySlotInfos[currentIndex].name + "을(를) " +
                        itemManagerSc.inventorySlotInfos[currentIndex].gold + "원에 판매하시겠습니까?";
                }
            }
        }
    }
    //잡화상점 안내창을 꺼줄 함수(판넬, 아니요버튼에 사용중)
    public void CloseShopInvenInfoWindow()
    {
        //잠금안내 ui
        canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(4).gameObject.SetActive(false);
        //판매 선택창 ui
        canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(5).gameObject.SetActive(false);
        //구매 선택창 ui
        canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(6).gameObject.SetActive(false);
        //구매 실패 ui
        canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(7).gameObject.SetActive(false);
    }
    //장비 및 잡템 판매 함수(예 버튼에 사용중)
    public void SellItem()
    {
        //인벤토리에 있는 아이템이 잡템이고 개수가 1보다 많을때
        if (itemManagerSc.inventorySlotInfos[currentIndex].id > 1000 && itemManagerSc.inventorySlotInfos[currentIndex].count > 1)
        {
            //player의 gold에 인벤토리의 gold정보 만큼 더해준다
            ItemManager.Gold += itemManagerSc.inventorySlotInfos[currentIndex].gold;
            //판매 선택창 ui 비활성화
            canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(5).gameObject.SetActive(false);
            //인벤토리에 있는 아이템 정보의 개수 1감소
            itemManagerSc.inventorySlotInfos[currentIndex].count -= 1;
            //인벤토리에 있는 인벤토리칸 오브젝트의 개수 텍스트 수정
            itemManagerSc.inventorySlots[currentIndex].transform.GetChild(0).gameObject.GetComponent<Text>().text = itemManagerSc.inventorySlotInfos[currentIndex].count.ToString();
            //잡화상점 인벤토리에 있는 오브젝트의 개수 텍스트 수정
            shopInvenInfo[currentIndex].transform.GetChild(0).GetComponent<Text>().text = itemManagerSc.inventorySlotInfos[currentIndex].count.ToString();
        }           
        //인벤토리에 있는 아이템이 장비이고 
        else if(itemManagerSc.inventorySlotInfos[currentIndex].id < 1000 || itemManagerSc.inventorySlotInfos[currentIndex].id > 1000 && itemManagerSc.inventorySlotInfos[currentIndex].count <= 1)
        {
            //판매 선택창 ui 비활성화
            canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(5).gameObject.SetActive(false);
            //아이템 강화 수치와 종류에 따라 판매 가격 연산
            float calcu = itemManagerSc.inventorySlotInfos[currentIndex].gold + ((itemManagerSc.inventorySlotInfos[currentIndex].gold / 10) * itemManagerSc.inventorySlotInfos[currentIndex].reinForce) * (itemManagerSc.inventorySlotInfos[currentIndex].level / 10);
            //player의 gold에 인벤토리의 gold정보 만큼 더해준다
            ItemManager.Gold += (int)calcu;
            //잡화상점 인벤토리의 개수 텍스트를 공란으로 수정
            shopInvenInfo[currentIndex].transform.GetChild(0).gameObject.GetComponent<Text>().text = " ";
            //인벤토리의 개수 텍스트를 공란으로 수정
            itemManagerSc.inventorySlots[currentIndex].transform.GetChild(0).gameObject.GetComponent<Text>().text = " ";
            //잡화상점 인벤토리의 강화 텍스트를 공란으로 수정
            shopInvenInfo[currentIndex].transform.GetChild(2).gameObject.GetComponent<Text>().text = " ";
            //이벤토리의 강화 텍스트를 공란으로 수정
            itemManagerSc.inventorySlots[currentIndex].transform.GetChild(2).gameObject.GetComponent<Text>().text = " ";
            //잡화상점 인벤토리의 이미지를 공란 이미지로 수정
            shopInvenInfo[currentIndex].GetComponent<Image>().sprite = slotCuber;
            //인벤토리의 이미지를 공란 이미지로 수정
            itemManagerSc.inventorySlots[currentIndex].GetComponent<Image>().sprite = slotCuber;
            //인벤토리의 유무확인 bool값을 false로 수정
            itemManagerSc.inventorySlotBool[currentIndex] = false;
            //인벤토리의 아이템 정보를 초기화
            itemManagerSc.inventorySlotInfos[currentIndex] = null;
        }
    }
    //잡화상점에 판매할 랜덤아이템을 생성하는 함수
    void RandomItemSet()
    {
        for(int i = 0; i < randomInvenInfos.Length; i++)
        {
            //판매할 아이템의 순번을 뽑는다
            rand = Random.Range(0, 46);
            //코스튬 아이템과 펫외형 아이템은 생성되지 않도록 예외 처리 한다
            if(rand > 20 && rand < 28)
            {
                //일정 수를 랜덤 돌려서 나온값을 더해준다(다시 0,46 범위로 랜덤을 돌릴수 있지만 랜덤돌려도 그값이 다시나올수 있어서 그냥 더했다)
                rand2 = Random.Range(7, 19);
                rand += rand2;
            }
            //아이템 유무 bool true로
            randomInvenBools[i] = true;
            //아이템 정보에 원본값을 넘겨준다
            randomInvenInfos[i] = new ItemInfo(itemTable.itemInfos[rand]);
            //그칸의 이미지를 정보의 이미지로 변경해준다
            randomInvenSlots[i].GetComponent<Image>().sprite = randomInvenInfos[i].cuberImage;
        }
    }
    //랜덤아이템 바꿔줄 코르틴
    IEnumerator ReRandom()
    {
        //wait를 true로 해서 코르틴에 재진입 하는 것을 막는다
        wait = true;
        //특정 시간 만큼 wait
        yield return new WaitForSeconds(maxTime);
        //구매중에 재셋팅 될경우 구매창을 꺼버린다
        canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(6).gameObject.SetActive(false);
        //랜덤 아이템 셋팅 함수 시작
        RandomItemSet();
        //다시 코르틴에 진입할수 있도록 false
        wait = false;
    }
    //잡화상점 구매 윈도우
    public void BuyButton()
    {
        //현재 선택한 버튼의 정보를 가져온다
        GameObject currentObject = EventSystem.current.currentSelectedGameObject;
        //현재 선택한 버튼의 이름
        string currentName = currentObject.name;
        //현재 선택한 버튼의 순서를 파악한다
        buyCurrentIndex = canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(2).Find(currentName).GetSiblingIndex();
        //현재 선택한 버튼(칸)에 아이템이 있으면
        if(randomInvenBools[buyCurrentIndex])
        {
            //구매 선택창 ui
            canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(6).gameObject.SetActive(true);
            //구매 선택창 text
            canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(6).transform.GetChild(1).transform.GetChild(0).
                gameObject.GetComponent<Text>().text = randomInvenInfos[buyCurrentIndex].name + "을(를) " +
                randomInvenInfos[buyCurrentIndex].buyGold + "에 구매하시겠습니까?";
        }
    }
    //잡화상점 구매 함수
    public void BuyItem()
    {
        //bool값 초기화
        check = false;
        //player의 소지금이 아이템의 가격 보다 많다면
        if (ItemManager.Gold >= randomInvenInfos[buyCurrentIndex].gold)
        {
            //소지금을 깍는다
            ItemManager.Gold -= randomInvenInfos[buyCurrentIndex].buyGold;
            //구매하는 아이템이 잡템일 경우
            if (randomInvenInfos[buyCurrentIndex].id > 1000)
            {
                for (int i = 0; i < itemManagerSc.inventorySlotInfos.Length; i++)
                {
                    //인벤토리 칸에 아이템이 있다면
                    if (itemManagerSc.inventorySlotBool[i])
                    {
                        Debug.Log("같은잡템이 있다");
                        //인벤토리 칸에 있는 아이템의 id와 랜덤칸에서 구매한 아이템의 id가 같다면
                        if (itemManagerSc.inventorySlotInfos[i].id == randomInvenInfos[buyCurrentIndex].id)
                        {
                            //같은 템이 생성 되지 않도록 막는다
                            check = true;
                            //구매창 ui닫는다
                            canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(6).gameObject.SetActive(false);
                            //랜덤칸의 유무를 false로 바꿔준다
                            randomInvenBools[buyCurrentIndex] = false;
                            //랜덤칸의 이미지를 빈이미지로 바꿔준다
                            randomInvenSlots[buyCurrentIndex].GetComponent<Image>().sprite = slotCuber;
                            //랜덤칸의 정보를 초기화 해준다
                            randomInvenInfos[buyCurrentIndex] = null;
                            //인벤토리 칸의 개수 정보를 +1해준다
                            itemManagerSc.inventorySlotInfos[i].count += 1;
                            //인벤토리 칸의 개수 text를 수정한다
                            itemManagerSc.inventorySlots[i].transform.GetChild(0).gameObject.GetComponent<Text>().text = itemManagerSc.inventorySlotInfos[i].count.ToString();
                            break;
                        }
                    }
                }
                //id가 같은 템이 없다면
                if (!check)
                {
                    Debug.Log("같은 잡템이 없다");
                    for (int i = 0; i < itemManagerSc.inventorySlotInfos.Length; i++)
                    {
                        //인벤토리 빈칸을 찾는다
                        if (!itemManagerSc.inventorySlotBool[i])
                        {
                            //판매창 ui 닫는다
                            canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(6).gameObject.SetActive(false);
                            //인벤토리 칸 유무를 true로 바꿔준다
                            itemManagerSc.inventorySlotBool[i] = true;
                            //인벤토리 칸에 정보를 넘겨준다
                            itemManagerSc.inventorySlotInfos[i] = new ItemInfo(randomInvenInfos[buyCurrentIndex]);
                            //인벤토리 칸 이미지를 정보대로 바꿔준다
                            itemManagerSc.inventorySlots[i].GetComponent<Image>().sprite = itemManagerSc.inventorySlotInfos[i].cuberImage;
                            //인벤토리 칸의 개수 text를 수정한다
                            itemManagerSc.inventorySlots[i].transform.GetChild(0).gameObject.GetComponent<Text>().text = itemManagerSc.inventorySlotInfos[i].count.ToString();
                            //랜덤 칸 유무를 false로 바꿔준다
                            randomInvenBools[buyCurrentIndex] = false;
                            //랜덤 칸 이미지를 빈이미지로 바꿔준다
                            randomInvenSlots[buyCurrentIndex].GetComponent<Image>().sprite = slotCuber;
                            //랜덤 칸 정보를 초기화 한다
                            randomInvenInfos[buyCurrentIndex] = null;
                            break;
                        }
                    }
                }
            }
            //구매하는 아이템이 장비일 경우
            else if (randomInvenInfos[buyCurrentIndex].id < 1000)
            {
                for (int i = 0; i < itemManagerSc.inventorySlotInfos.Length; i++)
                {
                    //인벤토리 칸에서 빈공간을 찾는다
                    if (!itemManagerSc.inventorySlotBool[i])
                    {
                        Debug.Log("장비템이다");
                        //판매창 ui 닫는다
                        canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(6).gameObject.SetActive(false);
                        //인벤토리 칸 유무를 true로 바꿔준다
                        itemManagerSc.inventorySlotBool[i] = true;
                        //인벤토리 칸에 정보를 넘겨준다
                        itemManagerSc.inventorySlotInfos[i] = new ItemInfo(randomInvenInfos[buyCurrentIndex]);
                        //인벤토리 칸 이미지를 정보대로 바꿔준다
                        itemManagerSc.inventorySlots[i].GetComponent<Image>().sprite = itemManagerSc.inventorySlotInfos[i].cuberImage;
                        //랜덤 칸 유무를 false로 바꿔준다
                        randomInvenBools[buyCurrentIndex] = false;
                        //랜덤 칸 이미지를 빈이미지로 바꿔준다
                        randomInvenSlots[buyCurrentIndex].GetComponent<Image>().sprite = slotCuber;
                        //랜덤 칸 정보를 초기화 한다
                        randomInvenInfos[buyCurrentIndex] = null;
                        break;
                    }
                }
            }
        }
        //소지금이 부족할시
        else
        {
            //구매 선택창 ui 비활성화
            canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(6).gameObject.SetActive(false);
            //실패 ui 활성화
            canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(7).gameObject.SetActive(true);
            //실패 ui text 변경
            canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(7).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = "소지금이 부족하여 구매할수없습니다.";
        }
    }
    //잡화상점 판매상품 리셋
    public void ReSetButton()
    {
        resetBool = !resetBool;
        //리셋 선택창 on
        if(resetBool)
            canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(true);
        //리셋 선택창 off
        if (!resetBool)
            canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(false);
    }
    //아이템 리셋 버튼
    public void ReSetItem()
    {
        resetBool = false;
        //리셋 ui끈다
        if (!resetBool)
            canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(false);
        //소지금이 300000 보다 많다면
        if (ItemManager.Gold >= 300000)
        { 
            //비용 차감
            ItemManager.Gold -= 300000;
            //시간리셋
            time = 0;
            //아이템 리셋
            RandomItemSet();
        }
        //소지금이 300000 보다 적다면
        else
        {
            //소지금 부족 ui 띄운다
            canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(3).transform.GetChild(3).gameObject.SetActive(true);
        }
    }
    //소지금 부족 ui 끈다
    public void LowGold()
    {
        canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(3).transform.GetChild(3).gameObject.SetActive(false);
    }
    //잡화상점 아이템 초기화 타이머
    void Timer()
    {
        time += Time.deltaTime;
        temporaryTime = (int)(maxTime - time) / 60;
        canvasTr.GetChild(16).transform.GetChild(1).transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().text = temporaryTime.ToString() + "분뒤에 갱신됩니다.";
    }







    //캐쉬상점 함수

    //구매 선택창
    public void BuyInfo()
    {
        //현재 사용한 버튼의 오브젝트를 찾는다
        GameObject currentButton = EventSystem.current.currentSelectedGameObject;
        //버튼의 이름으로 현재 사용할 캐쉬상점 순번을 받는다
        switch (currentButton.name)
        {
            case "Button1":
                cashIndex = 1;
                break;
            case "Button2":
                cashIndex = 2;
                break;
            case "Button3":
                cashIndex = 3;
                break;
        }
        //구매 선택창 ui
        canvasTr.GetChild(16).transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(true);
    }
    //캐쉬 상점 ui 닫기(구매 선택창 no에 사용중)
    public void CashShopEnd()
    {
        //구매 선택창 ui
        canvasTr.GetChild(16).transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
        //구매 실패창 ui
        canvasTr.GetChild(16).transform.GetChild(2).transform.GetChild(3).gameObject.SetActive(false);
        //이미지가 다 바뀌고 이펙트효과가 다끝났다면 창을 끌수있게 해준다
        if (randomEnd && uiEffectSc.effectCtrlBool[uiEffectSc.effectCtrlBool.Length - 1])
        {
            //뽑기 이펙트창 ui
            canvasTr.GetChild(16).transform.GetChild(2).transform.GetChild(4).gameObject.SetActive(false);
            //이펙트효과 bool값을 false로해서 다시 쓸수있게 준비한다
            for (int i = 0; i < uiEffectSc.effectCtrlBool.Length; i++)
                uiEffectSc.effectCtrlBool[i] = false;
        }
    }

    ////뽑기시 하나씩나오게 해줄 함수
    IEnumerator effect()
    {
        //일정 시간후 재진입을 하게 하기위해 false로
        randomBool = false;
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < randomObjects.Length; i++)
        {
            //이미지가 변하지 않은곳을 찾는다
            if (!randomImageBool[i])
            {
                //이미지가 바뀐것을 알리기위해 true로
                randomImageBool[i] = true;
                //이미지를 바꾼다
                randomObjects[i].GetComponent<Image>().sprite = randomItem[i].cuberImage;
                //생성후에 가려놓는다(이펙트와 속도를 맞추기 어려워서 이펙트가 끝나면 다시 보여주게한다)
                canvasTr.GetChild(17).transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(true);
                //마지막 반복이 아니라면 true로 바꿔서 재진입 가능하게 한다
                if (i != randomObjects.Length - 1)
                    randomBool = true;
                //마지막 반복이라면 끝났음을 알리기 위해 bool값을 true로 한다
                else if (i == randomObjects.Length - 1)
                    randomEnd = true;
                break;
            }
        }
    }

    //캐쉬 상점 ui(구매 선택창 Yes에 사용중)
    public void CashShopBuy()
    {
        if(ItemManager.Gold >= 50000)
        {
            //구매 선택창 ui꺼준다
            canvasTr.GetChild(16).transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
            ItemManager.Gold -= 50000;
            RandomBuyEffect();
        }
        else
        {
            //구매 선택창 ui꺼준다
            canvasTr.GetChild(16).transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
            //구매 실패창 ui켜준다
            canvasTr.GetChild(16).transform.GetChild(2).transform.GetChild(3).gameObject.SetActive(true);
        }
    }
    //구매했을때 효과 보여줄 함수
    void RandomBuyEffect()
    {
        //뽑았을때 저장된 아이템 정보 말소
        randomItem.Clear();
        int temp;
        //ui창 조건줄 bool값
        randomEnd = false;
        //뽑기전에 칸이미지를 빈공간 이미지로 초기화
        for (int i = 0; i < randomObjects.Length; i++)
            randomObjects[i].GetComponent<Image>().sprite = slotCuber;
        //랜덤 뽑기창 ui
        canvasTr.GetChild(16).transform.GetChild(2).transform.GetChild(4).gameObject.SetActive(true);
        //랜덤 뽑기창에 정보를 변경
        for (int i = 0; i < randomObjects.Length; i++)
        {
            //랜덤칸 이미지 bool값 초기화
            randomImageBool[i] = false;
            //뽑기시 랜덤값을 뽑아 온다
            int num = Random.Range(1, 101);
            //뽑기 종류 선택
            switch (cashIndex)
            {
                case 1:
                    temp = RandomNumSelect(cashIndex, num);
                    randomItem.Add(new ItemInfo(itemTable.itemInfos[temp]));
                    break;
                case 2:
                    temp = RandomNumSelect(cashIndex, num);
                    randomItem.Add(new ItemInfo(itemTable.itemInfos[temp]));
                    break;
                case 3:
                    temp = RandomNumSelect(cashIndex, num);
                    randomItem.Add(new ItemInfo(itemTable.itemInfos[temp]));
                    break;
            }
            //마지막 반복이면 bool값을 바꿔 알린다
            if (i == randomObjects.Length - 1)
                randomBool = true;
        }
        //인벤토리의 정보를 변경
        for(int i = 0; i < randomObjects.Length; i++)
        {
            //아이템이 장비템일 경우
            if (randomItem[i].id < 1000)
            {
                for (int j = 0; j < itemManagerSc.inventorySlotBool.Length; j++)
                {
                    //인벤토리칸이 비어있다면
                    if(!itemManagerSc.inventorySlotBool[j])
                    {
                        //bool값 바꿔서 아이템 유무 바꾼다
                        itemManagerSc.inventorySlotBool[j] = true;
                        //인벤토리 정보 배열에 아이템 정보를 넘겨준다
                        itemManagerSc.inventorySlotInfos[j] = new ItemInfo(randomItem[i]);
                        //인벤토리 칸의 이미지를 아이템 이미지로 바꿔준다
                        itemManagerSc.inventorySlots[j].GetComponent<Image>().sprite = itemManagerSc.inventorySlotInfos[j].cuberImage;
                        break;
                    }
                }
            }
            //아이템이 잡템일때
            else if (randomItem[i].id > 1000)
            {
                //2가지 상황을 구별하기 위한 bool값(인벤토리에 같은 잡템이 있을 경우 or 없을 경우)
                bool check2 = false;

                for(int j = 0; j < itemManagerSc.inventorySlotBool.Length; j++)
                {
                    //인벤토리에서 아이템이 있는 칸을 찾는다
                    if(itemManagerSc.inventorySlotBool[j])
                    {
                        //인벤토리에 같은 템이 있는 상황이라면
                        if (itemManagerSc.inventorySlotInfos[j].id == randomItem[i].id)
                        {
                            //bool값을 true로 해서 (같지 않은 상황)이 실행되지 않게 막는다
                            check2 = true;
                            //인벤토리 아이템의 개수를 올려준다
                            itemManagerSc.inventorySlotInfos[j].count += 1;
                            //인벤토리 아이템의 개수 텍스트를 변경해준다
                            itemManagerSc.inventorySlots[j].transform.GetChild(0).gameObject.GetComponent<Text>().text = itemManagerSc.inventorySlotInfos[j].count.ToString();
                            break;
                        }
                    }
                }
                //인벤토리에 같은 아이템이 없다면
                if(!check2)
                {
                    for (int l = 0; l < itemManagerSc.inventorySlotBool.Length; l++)
                    {
                        //인벤토리의 빈공간을 찾는다
                        if (!itemManagerSc.inventorySlotBool[l])
                        {
                            //공간 유무 bool값 true로 해준다
                            itemManagerSc.inventorySlotBool[l] = true;
                            //인벤토리 정보 배열에 정보를 넘겨준다
                            itemManagerSc.inventorySlotInfos[l] = new ItemInfo(randomItem[i]);
                            //인벤토리 칸의 이미지를 정보 이미지로 바꿔준다
                            itemManagerSc.inventorySlots[l].GetComponent<Image>().sprite = itemManagerSc.inventorySlotInfos[l].cuberImage;
                            //인벤토리 칸의 텍스트를 바꿔준다
                            itemManagerSc.inventorySlots[l].transform.GetChild(0).gameObject.GetComponent<Text>().text = itemManagerSc.inventorySlotInfos[l].count.ToString();
                            break;
                        }
                    }
                }
            }
        }
    }
    //각 캐쉬 상점 별로 랜덤 번호별 아이템 번호 연결
    int RandomNumSelect(int cash, int num)
    {
        //캐쉬 상점 1, 2, 3 공용 잡텝 15(75%), 책3(20%)이 나온다
        if (0 < num && num < 6)
            return 31;
        else if (5 < num && num < 11)
            return 32;
        else if (10 < num && num < 16)
            return 33;
        else if (15 < num && num < 21)
            return 34;
        else if (20 < num && num < 26)
            return 35;
        else if (25 < num && num < 31)
            return 36;
        else if (30 < num && num < 36)
            return 37;
        else if (35 < num && num < 41)
            return 38;
        else if (40 < num && num < 46)
            return 39;
        else if (45 < num && num < 51)
            return 40;
        else if (50 < num && num < 56)
            return 41;
        else if (55 < num && num < 61)
            return 42;
        else if (60 < num && num < 66)
            return 43;
        else if (65 < num && num < 71)
            return 44;
        else if (70 < num && num < 76)
            return 45;
        else if (75 < num && num < 83)
            return 28;
        else if (82 < num && num < 90)
            return 29;
        else if (89 < num && num < 96)
            return 30;
        //캐쉬상점 별로 설정
        if (cash == 1)
            return 22;
        else if (cash == 2)
            return 23;
        else if (cash == 3)
        {
            if (95 < num && num < 98)
                return 25;
            else if (97 < num && num < 100)
                return 26;
            else
                return 27;
        }
        return 0;
    }






    //제조창 슬롯들
    public GameObject[] makeSlots = new GameObject[21];
    //혹시 모르니까 원본 정보를 바로받지 말고 걸쳐서 받자
    public ItemInfo[] makeSlotInfos = new ItemInfo[21];
    //제조 가능 한가에 대한 bool값
    bool[] makeOk = new bool[21];
    //제조 가능 한지 확인할 bool
    bool check1;
    bool check2;
    bool check3;
    //제작창 스크롤 오브젝트(드래그앤 드롭)
    public GameObject makeShopContents;
    //제작창에 소지금창 띄울때 쓸 bool값
    public static bool makeShopBool;
    //제작창 제조가능창에 개수 나타낼때쓴다
    int count;
    //현재 클릭한 버튼검출할때 사용
    GameObject currentButtonMake;
    //현재 클릭한 버튼 몇번째 자식인지 검출할때 사용
    int currentIndexMake;
    //제작 애니메이션 마지막을 맞추기위해
    bool endAni;
    //플레임 애니메이션 에서 쓸 bool값
    bool flameEffect;
    //플레임 애니메이션 스프라이트
    public Sprite[] flame = new Sprite[6];
    //플레임 스프라이트 인덱스
    int flameIndex;
    //세번째 소지금창 관리를 위한 bool값
    public static bool effectBool;
    //소지금창 관리를 위한 제조선택창 bool값
    public static bool makeSelectBool;
    //제조완료후 띄어줄 이미지(드래그앤 드롭)
    public Sprite[] makeEndImage = new Sprite[21];

    //강화 제작 함수
    public void OpenMakeUi()
    {
        makeShopBool = true;
        //슬라이드바를 원위치로
        makeShopContents.GetComponent<RectTransform>().localPosition = new Vector3(makeShopContents.GetComponent<RectTransform>().localPosition.x, 0, 0);
        //panel 활성화
        canvasTr.GetChild(16).transform.GetChild(0).gameObject.SetActive(true);
        //제작 ui 활성화
        canvasTr.GetChild(16).transform.GetChild(4).gameObject.SetActive(true);
        //제작 상점에 들어갈때 마다 검사한다
        FindMakeItem();
        //제작 상점 들어갈때, 제작후에 수정해서 띄어줄 제작가능 개수 텍스트
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().text = makePossibleCount().ToString() + "개";
    }
    //제조창 ui 셋팅
    void MakePageSet()
    {
        for(int i = 0; i < makeSlots.Length; i++)
        {
            //오브젝트와 정보 배열에 값을 지정해준다
            makeSlots[i] = canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(i).gameObject;
            makeSlotInfos[i] = new ItemInfo(itemTable.itemInfos[i]);
        }

        //0:제작템이미지 1:아이템이름 2:아이템제조가격 3:재료(이미지)자식(텍스트) 4:재료 5:재료 6:커버
        for(int i = 0; i < makeSlots.Length; i++)
        {
            makeSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = makeSlotInfos[i].cuberImage;
            makeSlots[i].transform.GetChild(1).GetComponent<Text>().text = makeSlotInfos[i].name;
            makeSlots[i].transform.GetChild(2).GetComponent<Text>().text = makeSlotInfos[i].buyGold.ToString();
            makeSlots[i].transform.GetChild(3).GetComponent<Image>().sprite = makeSlotInfos[i].material1;
            makeSlots[i].transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().text = makeSlotInfos[i].material1Count.ToString();
            makeSlots[i].transform.GetChild(4).GetComponent<Image>().sprite = makeSlotInfos[i].material2;
            makeSlots[i].transform.GetChild(4).transform.GetChild(0).GetComponent<Text>().text = makeSlotInfos[i].material2Count.ToString();
            makeSlots[i].transform.GetChild(5).GetComponent<Image>().sprite = makeSlotInfos[i].material3;
            makeSlots[i].transform.GetChild(5).transform.GetChild(0).GetComponent<Text>().text = makeSlotInfos[i].material3Count.ToString();
        }
    }
    //제작 할수 있는 아이템 검사 함수
    void FindMakeItem()
    {
        for(int i = 0; i < makeSlots.Length; i++)
        {
            //각 재료 아이템의 재료 개수가 충분한지 확인할때 쓸 bool값
            check1 = false;
            check2 = false;
            check3 = false;

            for(int j = 0; j < itemManagerSc.inventorySlotBool.Length; j++)
            {
                //인벤토리 칸에 아이템이 있고 id가 잡화템이라면
                if(itemManagerSc.inventorySlotBool[j] && itemManagerSc.inventorySlotInfos[j].id > 1000)
                {
                    //재료1의 이미지 이름과 인벤토리창에 있는 아이템의 이미지 이름이 같다면
                    if (makeSlotInfos[i].material1.name == itemManagerSc.inventorySlotInfos[j].cuberImage.name)
                    {
                        //인벤토리 칸의 아이템개수가 재료1의 필요 개수 보다 많으면
                        if (itemManagerSc.inventorySlotInfos[j].count >= makeSlotInfos[i].material1Count)
                            check1 = true;   
                    }
                    if(makeSlotInfos[i].material2.name == itemManagerSc.inventorySlotInfos[j].cuberImage.name)
                    {
                        if (itemManagerSc.inventorySlotInfos[j].count >= makeSlotInfos[i].material2Count)
                            check2 = true;
                    }
                    if(makeSlotInfos[i].material3.name == itemManagerSc.inventorySlotInfos[j].cuberImage.name)
                    {
                        if (itemManagerSc.inventorySlotInfos[j].count >= makeSlotInfos[i].material3Count)
                            check3 = true;
                    }
                    //재료1,2,3의 개수가 다있다면 커버를 꺼준다
                    if (check1 && check2 && check3)
                    {
                        makeSlots[i].transform.GetChild(6).gameObject.SetActive(false);
                        makeOk[i] = true;
                    }
                    //재료가 모자라다면 커버를 켜준다
                    else
                    {
                        makeSlots[i].transform.GetChild(6).gameObject.SetActive(true);
                        makeOk[i] = false;
                    }
                }
            }
        }
    }
    //제조 가능 개수 파악 함수
    int makePossibleCount()
    {
        count = 0;
        for(int i = 0; i < makeSlotInfos.Length; i++)
        {
            if (makeOk[i])
                count++;
        }
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().text = count.ToString() + "개";
        return count;
    }
    //제조 확인창(버튼 21개에 사용)
    public void MakeSelect()
    {
        //현재 사용한 버튼 오브젝트를 파악
        currentButtonMake = EventSystem.current.currentSelectedGameObject;
        //현재 오브젝트가 몇번째 자식인지 파악
        currentIndexMake = canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).Find(currentButtonMake.name).GetSiblingIndex();
        //현재 오브젝트가 제조 가능하다면
        if(makeOk[currentIndexMake])
        {
            makeSelectBool = true;
            //제조 선택창 ui
            canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(3).gameObject.SetActive(true);
            //제조 선택창 ui text변경
            canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(3).transform.GetChild(2).GetComponent<Text>().text = makeSlotInfos[currentIndexMake].name + "를 제작하시겠습니까?";
        }
    }
    //제조확인창 no선택시
    public void MakeSelectNo()
    {
        makeSelectBool = false;
        //제조 선택창 ui
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(3).gameObject.SetActive(false);
        //실패창 ui 비활성화
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(5).gameObject.SetActive(false);
        //제조 이펙트가 다 끝났을때(마지막 제조 아이템 이미지가 활성화 됐다면)
        if(canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(5).gameObject.activeInHierarchy)
        {
            //제조 이펙트 ui
            canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).gameObject.SetActive(false);
            //슬롯 이미지
            canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(5).gameObject.SetActive(false);
            //제조 아이템 이미지
            canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(6).gameObject.SetActive(false);
            effectBool = false;
        }
    }
    //아이템 제조 함수(yes버튼에 사용)
    public void MakeSelectYes()
    {
        //제조할 비용이 있다면
        if(ItemManager.Gold > makeSlotInfos[currentIndexMake].buyGold)
        {
            makeSelectBool = false;
            effectBool = true;
            MakePay();
            for (int i = 0; i < itemManagerSc.inventoryLockBool.Length; i++)
            {
                //인벤토리 빈공간을 찾는다
                if (!itemManagerSc.inventorySlotBool[i])
                {
                    //인벤토리 칸 아이템 유무를 true로
                    itemManagerSc.inventorySlotBool[i] = true;
                    //인벤토리 칸 아이템 정보를 넘겨준다
                    itemManagerSc.inventorySlotInfos[i] = new ItemInfo(makeSlotInfos[currentIndexMake]);
                    //인벤토리칸 이미지를 정보 이미지로 바꿔준다
                    itemManagerSc.inventorySlots[i].GetComponent<Image>().sprite = itemManagerSc.inventorySlotInfos[i].cuberImage;
                    break;
                }
            }
            //제조 선택창 ui
            canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(3).gameObject.SetActive(false);
            FindMakeItem();
            makePossibleCount();
            MakeEffect();
        }
        //제조할 비용이 없다면
        else
        {
            //제조 선택창 ui 비활성화
            canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(3).gameObject.SetActive(false);
            //실패창 ui 활성화
            canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(5).gameObject.SetActive(true);
        }
    }
    //제조 비용및 재료 지불
    public void MakePay()
    {
        //제조 비용을 빼준다
        ItemManager.Gold -= makeSlotInfos[currentIndexMake].buyGold;
        for (int i = 0; i < itemManagerSc.inventorySlotBool.Length; i++)
        {
            //인벤토리 칸에 아이템이 있고 그아이템 id가 1000이상일때 (잡템일때)
            if (itemManagerSc.inventorySlotBool[i] && itemManagerSc.inventorySlotInfos[i].id > 1000)
            {
                //제조상점 목록 칸의 재료1의 이미지 이름과 인벤토리칸의 이미지 이름이 같다면
                if (makeSlotInfos[currentIndexMake].material1.name == itemManagerSc.inventorySlotInfos[i].cuberImage.name)
                {
                    //인벤토리칸의 개수를 필요개수만큼 빼준다
                    itemManagerSc.inventorySlotInfos[i].count -= makeSlotInfos[currentIndexMake].material1Count;
                    //개수를 빼주고 난뒤 텍스트 수정
                    itemManagerSc.inventorySlots[i].transform.GetChild(0).GetComponent<Text>().text = itemManagerSc.inventorySlotInfos[i].count.ToString();
                    //만약 빼고난뒤에 개수가 0이라면
                    if (itemManagerSc.inventorySlotInfos[i].count <= 0)
                    {
                        //인벤토리 칸 아이템 정보 유무 false
                        itemManagerSc.inventorySlotBool[i] = false;
                        //인벤토리 칸 아이템 정보를 null
                        itemManagerSc.inventorySlotInfos[i] = null;
                        //인벤토리 칸 이미지를 빈 이미지로
                        itemManagerSc.inventorySlots[i].GetComponent<Image>().sprite = slotCuber;
                        //인벤토리 칸 개수 text를 공란으로
                        itemManagerSc.inventorySlots[i].transform.GetChild(0).GetComponent<Text>().text = " ";
                    }
                }
                else if (makeSlotInfos[currentIndexMake].material2.name == itemManagerSc.inventorySlotInfos[i].cuberImage.name)
                {
                    itemManagerSc.inventorySlotInfos[i].count -= makeSlotInfos[currentIndexMake].material2Count;
                    itemManagerSc.inventorySlots[i].transform.GetChild(0).GetComponent<Text>().text = itemManagerSc.inventorySlotInfos[i].count.ToString();
                    if (itemManagerSc.inventorySlotInfos[i].count <= 0)
                    {
                        itemManagerSc.inventorySlotBool[i] = false;
                        itemManagerSc.inventorySlotInfos[i] = null;
                        itemManagerSc.inventorySlots[i].GetComponent<Image>().sprite = slotCuber;
                        itemManagerSc.inventorySlots[i].transform.GetChild(0).GetComponent<Text>().text = " ";
                    }
                }
                else if (makeSlotInfos[currentIndexMake].material3.name == itemManagerSc.inventorySlotInfos[i].cuberImage.name)
                {
                    itemManagerSc.inventorySlotInfos[i].count -= makeSlotInfos[currentIndexMake].material3Count;
                    itemManagerSc.inventorySlots[i].transform.GetChild(0).GetComponent<Text>().text = itemManagerSc.inventorySlotInfos[i].count.ToString();
                    if (itemManagerSc.inventorySlotInfos[i].count <= 0)
                    {
                        itemManagerSc.inventorySlotBool[i] = false;
                        itemManagerSc.inventorySlotInfos[i] = null;
                        itemManagerSc.inventorySlots[i].GetComponent<Image>().sprite = slotCuber;
                        itemManagerSc.inventorySlots[i].transform.GetChild(0).GetComponent<Text>().text = " ";
                    }
                }
            }
        }
    }
    //제조 이펙트 함수
    public void MakeEffect()
    {
        //제조 이펙트 ui
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).gameObject.SetActive(true);
        //제조 이펙트 판넬
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(0).gameObject.SetActive(true);
        //제조 이펙트 재료1 이미지
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(1).gameObject.SetActive(true);
        //제조 이펙트 재료2 이미지
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(2).gameObject.SetActive(true);
        //제조 이펙트 재료3 이미지
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(3).gameObject.SetActive(true);
        //제조 이펙트 재료1 이미지 변경
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(1).GetComponent<Image>().sprite = makeSlotInfos[currentIndexMake].material1;
        //제조 이펙트 재료2 이미지 변경
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(2).GetComponent<Image>().sprite = makeSlotInfos[currentIndexMake].material2;
        //제조 이펙트 재료3 이미지 변경
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(3).GetComponent<Image>().sprite = makeSlotInfos[currentIndexMake].material3;
        //제조 애니메이션 실행
        Animation ani = canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).GetComponent<Animation>();
        ani.Play();
        endAni = true;
        //제조 애니메이션 끝날때 쯤에 맞춰서 딜레이를 준다
        if (endAni)
            StartCoroutine(EndAni());
    }
    //제조 애니메이션 끝나면 다음게 실행될수있게 제조 애니메이션 길이정도로 딜레이
    IEnumerator EndAni()
    {
        yield return new WaitForSeconds(0.8f);
        //제조 이펙트 재료1 이미지
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(1).gameObject.SetActive(false);
        //제조 이펙트 재료2 이미지
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(2).gameObject.SetActive(false);
        //제조 이펙트 재료3 이미지
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(3).gameObject.SetActive(false);
        flameEffect = true;
    }
    //플레임 이펙트 스프라이트를 돌려줄 코루틴
    IEnumerator FlameSprite()
    {
        //일정시간동안 재진입을 막는다
        flameEffect = false;
        yield return new WaitForSeconds(0.1f);
        //플레임 이펙트 ui
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(4).gameObject.SetActive(true);
        //플레임 이펙트 ui의 이미지를 flameindex번째의 이미지로
        canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(4).GetComponent<Image>().sprite = flame[flameIndex];
        //다음이미지로
        flameIndex++;
        //재진입 할수있게 true로
        flameEffect = true;
        //플레임 이펙트 스프라이트가 마지막장을 넘었다면
        if (flameIndex > 5)
        {
            //0초기화
            flameIndex = 0;
            //재진입 못하게 막는다
            flameEffect = false;
            //플레임 이펙트 ui
            canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(4).gameObject.SetActive(false);
            //슬롯 이미지
            canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(5).gameObject.SetActive(true);
            //완성 제조 아이템 이미지
            canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(6).gameObject.SetActive(true);
            //완성 제조 아이템 이미지 변경
            canvasTr.GetChild(16).transform.GetChild(4).transform.GetChild(4).transform.GetChild(6).GetComponent<Image>().sprite = makeEndImage[currentIndexMake];
        }
    }







    //강화 상점 함수
    public static bool reinBool;
    //강화 상점 인벤토리 칸 오브젝트
    GameObject[] reinObjects = new GameObject[60];
    //강화 상점 인벤토리 칸 아이템 유무 bool
    bool[] reinSlotBool = new bool[60];
    //임시적으로 아이템 정보를 저장해줄 공간
    public ItemInfo reinTemp;
    //메인재료창의 아이템 정보
    public ItemInfo mate1;
    //보조재료창의 아이템 정보
    public ItemInfo mate2;
    //강화상점에서 쓸 현재 선택된 아이템의 인벤토리 위치 번호
    int currentIndexRein;
    //메인재료창 오브젝트
    public GameObject objectMate1;
    //보조재료창 오브젝트
    public GameObject objectMate2;
    //강화 이펙트 스프라이트 배열
    public Sprite[] criticalReinForce = new Sprite[19];
    //강화 이펙트에 쓰일 bool값
    bool reinEffectBool;
    //강화 이펙트 스프라이트 번호
    int reinEffectSpriteIndex;
    //강화 이펙트 이미지 오브젝트에 접근하기 위해
    public GameObject reinEffect;
    //메인재료와 보조재료 들어왔는지 확인할 bool값
    bool setReinBool;
    bool setMateBool;
    //강화상점 인벤토리 스크롤 초기화 할때 쓸 오브젝트(드래그앤 드롭)
    public GameObject reinForceContents;
    //강화 금액 배열
    int[] reinGold = { 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 10000 };
    //소지금 강화 가능 여부 조건 검사시 사용
    int temp;

    //강화 상점 ui
    public void OpenReinForceShop()
    {
        reinBool = !reinBool;
        if(reinBool)
        {
            //상점 판넬 ui 활성화
            canvasTr.GetChild(16).transform.GetChild(0).gameObject.SetActive(true);
            //강화 상점 ui 활성화
            canvasTr.GetChild(16).transform.GetChild(5).gameObject.SetActive(true);
        }
    }
    //강화상점 이용중일때 계속띄운다
    void UseReinForce()
    {
        for(int i = 0; i < reinSlotBool.Length; i++)
        {
            //인벤토리 칸 유무가 true라면 
            if (itemManagerSc.inventorySlotBool[i])
            {
                //강화 인벤토리칸 유무 true
                reinSlotBool[i] = true;
                //강화 인벤토리칸 이미지를 인벤정보 이미지로
                reinObjects[i].GetComponent<Image>().sprite = itemManagerSc.inventorySlotInfos[i].cuberImage;
                //인벤토리 개수 정보가 0보다 많다면 개수 수정
                if (itemManagerSc.inventorySlotInfos[i].count > 0)
                    reinObjects[i].transform.GetChild(0).GetComponent<Text>().text = itemManagerSc.inventorySlotInfos[i].count.ToString();
                //인벤토리 개수 정보가 없다면 공란으로
                else
                    reinObjects[i].transform.GetChild(0).GetComponent<Text>().text = " ";
                //인벤토리 잠금 정보가 true라면 잠금이미지 활성화
                if (itemManagerSc.inventoryLockBool[i])
                    reinObjects[i].transform.GetChild(1).gameObject.SetActive(true);
                //인벤토리 잠금 정보가 false라면 잠금이미지 비활성화
                else
                    reinObjects[i].transform.GetChild(1).gameObject.SetActive(false);
                //인벤토리 강화 정보가 0보다 크다면 강화 텍스트 수정
                if (itemManagerSc.inventorySlotInfos[i].reinForce > 0)
                    reinObjects[i].transform.GetChild(2).GetComponent<Text>().text = "+" + itemManagerSc.inventorySlotInfos[i].reinForce.ToString();
                //인벤토리 강화 정보가 없다면 공란으로
                else
                    reinObjects[i].transform.GetChild(2).GetComponent<Text>().text = " ";
            }
            //인벤토리칸 유무가 false라면
            else
            {
                //강화인벤토리칸 유무 false로
                reinSlotBool[i] = false;
                //강화인벤토리칸 이미지를 빈이미지로
                reinObjects[i].GetComponent<Image>().sprite = slotCuber;
                //강화 정보 공란으로
                reinObjects[i].transform.GetChild(2).GetComponent<Text>().text = " ";
                //개수 정보 공란으로
                reinObjects[i].transform.GetChild(0).GetComponent<Text>().text = " ";
                //잠금 이미지 비활성화
                reinObjects[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
    //강화상점 인벤토리 칸 버튼에 사용
    public void ReinForceSet()
    {
        //현재 사용된 버튼을 찾는다
        GameObject currentobjectRein = EventSystem.current.currentSelectedGameObject;
        //현재 사용된 버튼이 몇번째 자식인지 파악
        currentIndexRein = canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.Find(currentobjectRein.name).GetSiblingIndex() - 1;
        //전에 사용한 흔적을 지워준다
        for (int i = 0; i < reinSlotBool.Length; i++)
        {   
            //강화 인벤에 아이템이 있다면
            if (reinSlotBool[i])
            {
                //선택 효과를 꺼준다
                reinObjects[i].transform.GetChild(3).gameObject.SetActive(false);
                //임시 정보를 초기화
                reinTemp = null;
            }
        }
        //현재 선택된 버튼에 아이템이 있다면
        if (reinSlotBool[currentIndexRein])
        {
            Debug.Log("test");
            //아이템이 잠겨있는지확인
            if (itemManagerSc.inventoryLockBool[currentIndexRein])
            {
                //실패창 ui
                canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).gameObject.SetActive(true);
                //실패창 ui text 변경
                canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).transform.GetChild(2).GetComponent<Text>().text = "잠겨있는 아이템은\n 사용할수 없습니다";
            }
            //아이템의 종류를 확인(1000이상은 잡화)
            else if (itemManagerSc.inventorySlotInfos[currentIndexRein].id > 1000)
            {
                canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).gameObject.SetActive(true);
                canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).transform.GetChild(2).GetComponent<Text>().text = "잡화 아이템은 \n 사용할수 없습니다";
            }
            //아이템의 종류를 확인(800이상은 외형)
            else if (itemManagerSc.inventorySlotInfos[currentIndexRein].id > 800)
            {
                canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).gameObject.SetActive(true);
                canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).transform.GetChild(2).GetComponent<Text>().text = "의상 아이템은 \n 사용할수 없습니다";
            }
            //아이템의 종류를 확인(800이하는 장비)
            else
            {
                //선택효과를 켜준다
                currentobjectRein.transform.GetChild(3).gameObject.SetActive(true);
                //임시 정보에 현재 칸의 아이템 정보를 넘겨준다
                reinTemp = new ItemInfo(itemManagerSc.inventorySlotInfos[currentIndexRein]);
            }
        }
    }
    //강화상점 ui 꺼줄때 사용
    public void ReinEnd()
    {
        //실패창 ui 비활성화
        canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).gameObject.SetActive(false);
        //이펙트가 다끝나면 끌수 있게 조절 한다
        if(canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(5).gameObject.activeInHierarchy)
        {
            //이펙트 ui 강화아이템 슬롯 비활성화
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(4).gameObject.SetActive(false);
            //이펙트 ui 강화아이템 이미지 비활성화
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(5).gameObject.SetActive(false);
            //이펙트 ui 비활성화
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).gameObject.SetActive(false);
        }
        //강화 선택창 ui 비활성화
        canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(6).gameObject.SetActive(false);
        //소지금 부족 ui 비활성화
        canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(7).gameObject.SetActive(false);
    }
    //메인재료버튼 재료버튼에 사용
    public void ReinMaterial()
    {
        //현재 사용된 버튼을 찾는다
        GameObject currentObject = EventSystem.current.currentSelectedGameObject;
        if(reinTemp != null)
        {
            //현재 사용된 버튼의 이름이 ReinItem이라면
            if (currentObject.name == "ReinItem")
            {
                //메인 재료창에 넣으려는 아이템의 강화가 최대일때
                if(reinTemp.reinForce == 9)
                {
                    //실패창 ui 활성화
                    canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).gameObject.SetActive(true);
                    //실패창 ui text 변경
                    canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).transform.GetChild(2).GetComponent<Text>().text = "장비의 강화수치가 최대치에 도달했습니다";
                    //선택효과 해제
                    reinObjects[currentIndexRein].transform.GetChild(3).gameObject.SetActive(false);
                }
                //메인재료 창이 비었다면
                else if (!setReinBool)
                {
                    //메인재료 창 아이템 유무 true
                    setReinBool = true;
                    //메인재료 정보에 임시정보를 넘겨준다
                    mate1 = new ItemInfo(reinTemp);
                    //메인재료 이미지를 정보대로 바꿔준다
                    currentObject.transform.GetChild(0).GetComponent<Image>().sprite = mate1.cuberImage;
                    //강화 정보가 있다면 표시 없다면 표시하지 않는다
                    if (mate1.reinForce > 0)
                        currentObject.transform.GetChild(1).GetComponent<Text>().text = "+" + mate1.reinForce.ToString();
                    else
                        currentObject.transform.GetChild(1).GetComponent<Text>().text = " ";
                    //인벤토리 슬롯 유무 false로
                    itemManagerSc.inventorySlotBool[currentIndexRein] = false;
                    //인벤토리 이미지 빈이미지로 바꾼다
                    itemManagerSc.inventorySlots[currentIndexRein].GetComponent<Image>().sprite = slotCuber;
                    //인벤토리 정보 지운다
                    itemManagerSc.inventorySlotInfos[currentIndexRein] = null;
                    //인벤토리 강화 정보를 지운다
                    itemManagerSc.inventorySlots[currentIndexRein].transform.GetChild(2).GetComponent<Text>().text = " ";
                    //선택효과 꺼준다
                    reinObjects[currentIndexRein].transform.GetChild(3).gameObject.SetActive(false);
                    //임시 정보 초기화
                    reinTemp = null;
                }
                //메인재료창은 보조재료를 넣기 전까지 교환이 가능하게 한다
                else if (setReinBool && !setMateBool)
                {
                    //다른템으로 교체하기 위해 현재 있는 아이템을 인벤토리로 빼준다
                    for (int i = 0; i < itemManagerSc.inventorySlotBool.Length; i++)
                    {
                        //인벤토리의 빈공간을 찾는다
                        if (!itemManagerSc.inventorySlotBool[i])
                        {
                            //인벤토리 공간 유무를 true
                            itemManagerSc.inventorySlotBool[i] = true;
                            //강화칸에 있던 아이템의 정보를 넘겨준다
                            itemManagerSc.inventorySlotInfos[i] = new ItemInfo(mate1);
                            //인벤토리 칸의 이미지를 정보 이미지로 바꿔준다
                            itemManagerSc.inventorySlots[i].GetComponent<Image>().sprite = itemManagerSc.inventorySlotInfos[i].cuberImage;
                            break;
                        }
                    }
                    //메인재료 정보에 임시정보를 넘겨준다
                    mate1 = new ItemInfo(reinTemp);
                    //메인재료 이미지를 정보대로 바꿔준다
                    currentObject.transform.GetChild(0).GetComponent<Image>().sprite = mate1.cuberImage;
                    //강화 정보가 있다면 표시 없다면 표시하지 않는다
                    if (mate1.reinForce > 0)
                        currentObject.transform.GetChild(1).GetComponent<Text>().text = "+" + mate1.reinForce.ToString();
                    else
                        currentObject.transform.GetChild(1).GetComponent<Text>().text = " ";
                    //인벤토리 슬롯 유무 false로
                    itemManagerSc.inventorySlotBool[currentIndexRein] = false;
                    //인벤토리 이미지 빈이미지로 바꾼다
                    itemManagerSc.inventorySlots[currentIndexRein].GetComponent<Image>().sprite = slotCuber;
                    //인벤토리의 강화 정보를 초기화한다
                    itemManagerSc.inventorySlots[currentIndexRein].transform.GetChild(2).GetComponent<Text>().text = " ";
                    //인벤토리 정보 지운다
                    itemManagerSc.inventorySlotInfos[currentIndexRein] = null;
                    //선택효과 꺼준다
                    reinObjects[currentIndexRein].transform.GetChild(3).gameObject.SetActive(false);
                    //임시 정보 초기화
                    reinTemp = null;
                }
                //메인 재료창 보조재료창 둘다 차있다면
                else
                {
                    //선택 효과 꺼준다
                    reinObjects[currentIndexRein].transform.GetChild(3).gameObject.SetActive(false);
                    //실패 안내창 ui 활성화
                    canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).gameObject.SetActive(true);
                    //실패 안내창 text 변경
                    canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).transform.GetChild(2).GetComponent<Text>().text = "강화 대기 상태입니다 \n 변경하시려면 취소를 눌러 주세요";
                }
            }
            //현재 사용된 버튼의 이름이 MateItem이라면
            else if (currentObject.name == "MateItem")
            {
                //메인재료가 설정이 안되어있다면
                if (!setReinBool)
                {
                    //선택효과 비활성화
                    reinObjects[currentIndexRein].transform.GetChild(3).gameObject.SetActive(false);
                    //실패창 ui 활성화
                    canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).gameObject.SetActive(true);
                    //실패창 ui text 변경
                    canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).transform.GetChild(2).GetComponent<Text>().text = "메인 재료를 먼저 설정해 주세요";
                }
                //강화창에 있는 아이템과 재료창에 넣으려는 아이템의 이름이 같다면
                else if (mate1.name == reinTemp.name)
                {
                    //넣으려는 아이템에 강화 정보가 없다면
                    if (reinTemp.reinForce == 0)
                    {
                        //재료창이 비었다면
                        if (!setMateBool)
                        {
                            //보조 재료 아이템 유무 true
                            setMateBool = true;
                            //보조재료 정보에 임시정보를 넘겨준다
                            mate2 = new ItemInfo(reinTemp);
                            //보조재료 이미지를 정보대로 바꿔준다
                            currentObject.transform.GetChild(0).GetComponent<Image>().sprite = mate2.cuberImage;
                            //인벤토리 슬롯 유무 false로
                            itemManagerSc.inventorySlotBool[currentIndexRein] = false;
                            //인벤토리 이미지 빈이미지로 바꾼다
                            itemManagerSc.inventorySlots[currentIndexRein].GetComponent<Image>().sprite = slotCuber;
                            //인벤토리 정보 지운다
                            itemManagerSc.inventorySlotInfos[currentIndexRein] = null;
                            //선택효과 꺼준다
                            reinObjects[currentIndexRein].transform.GetChild(3).gameObject.SetActive(false);
                            //임시 정보 초기화
                            reinTemp = null;
                        }
                        else
                        {
                            //선택효과 비활성화
                            reinObjects[currentIndexRein].transform.GetChild(3).gameObject.SetActive(false);
                            //실패창 ui 활성화
                            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).gameObject.SetActive(true);
                            //실패창 ui text 변경
                            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).transform.GetChild(2).GetComponent<Text>().text = "강화 대기 상태입니다 \n 변경하시려면 취소를 눌러 주세요";
                        }
                    }
                    //넣으려는 아이템에 강화 정보가 있다면
                    else
                    {
                        //선택 효과 비활성화
                        reinObjects[currentIndexRein].transform.GetChild(3).gameObject.SetActive(false);
                        //실패창 ui 활성화
                        canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).gameObject.SetActive(true);
                        //실패창 ui text 변경
                        canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).transform.GetChild(2).GetComponent<Text>().text = "강화가 된 아이템은 재료로 사용할수없습니다";
                    }
                }
                //두아이템다 셋팅 끝났다면
                else if (setReinBool && setMateBool)
                {
                    //선택효과 비활성화
                    reinObjects[currentIndexRein].transform.GetChild(3).gameObject.SetActive(false);
                    //실패창 ui 활성화
                    canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).gameObject.SetActive(true);
                    //실패창 ui text 변경
                    canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).transform.GetChild(2).GetComponent<Text>().text = "강화 대기 상태입니다 \n 변경하시려면 취소를 눌러 주세요";
                }
                else
                {
                    //실패창 ui 활성화
                    canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).gameObject.SetActive(true);
                    //실패창 ui text 변경
                    canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).transform.GetChild(2).GetComponent<Text>().text = "동일한 아이템만 강화 재료로 사용할수 있습니다";
                    //선택 효과 비활성화
                    reinObjects[currentIndexRein].transform.GetChild(3).gameObject.SetActive(false);
                }
            }
        }
    }
    //강화 준비 해제(취소 버튼에 사용)
    public void ReinForceCancle()
    {
        //메인재료에 아이템이 들어와 있다면
        if(setReinBool)
        {
            for (int i = 0; i < itemManagerSc.inventorySlotBool.Length; i++)
            {
                //인벤토리 칸에 빈공간을 찾는다
                if (!itemManagerSc.inventorySlotBool[i])
                {
                    //인벤토리 칸 유무 true로
                    itemManagerSc.inventorySlotBool[i] = true;
                    //인벤토리 칸 아이템 정보를 메인재료 정보로
                    itemManagerSc.inventorySlotInfos[i] = new ItemInfo(mate1);
                    //인벤토리 칸 이미지를 정보의 이미지로
                    itemManagerSc.inventorySlots[i].GetComponent<Image>().sprite = itemManagerSc.inventorySlotInfos[i].cuberImage;
                    //인벤토리 칸 강화정보가 있다면 텍스트를 수정한다
                    if (mate1.reinForce > 0)
                        itemManagerSc.inventorySlots[i].transform.GetChild(2).GetComponent<Text>().text = "+" + itemManagerSc.inventorySlotInfos[i].reinForce.ToString();
                    break;
                }
            }
            //메인재료 칸의 이미지를 빈슬롯 이미지로 변경
            objectMate1.transform.GetChild(0).GetComponent<Image>().sprite = slotCuber;
            //메인재료 칸의 강화 정보를 공란으로 설정
            objectMate1.transform.GetChild(1).GetComponent<Text>().text = " ";
            //메인재료 정보 초기화
            mate1 = null;
            //메인재료 유무 false
            setReinBool = false;
        }
        //보조재료에 아이템이 들어와 있다면
        if(setMateBool)
        {
            for (int i = 0; i < itemManagerSc.inventorySlotBool.Length; i++)
            {
                //인벤토리 칸의 빈공간을 찾는다
                if (!itemManagerSc.inventorySlotBool[i])
                {
                    //인벤토리칸의 아이템 유무 true
                    itemManagerSc.inventorySlotBool[i] = true;
                    //인벤토리칸에 재료칸의 정보를 넘긴다
                    itemManagerSc.inventorySlotInfos[i] = new ItemInfo(mate2);
                    //인벤토리칸의 이미지를 정보대로 바꿔준다
                    itemManagerSc.inventorySlots[i].GetComponent<Image>().sprite = itemManagerSc.inventorySlotInfos[i].cuberImage;
                    break;
                }
            }
            //재료칸 이미지를 빈슬롯이미지로 바꿔준다
            objectMate2.transform.GetChild(0).GetComponent<Image>().sprite = slotCuber;
            //재료칸 정보를 초기화
            mate2 = null;
            //재료칸 아이템 유무 false
            setMateBool = false;
        }
        //임시 아이템 정보를 초기화
        reinTemp = null;
    }
    //강화 버튼 yes 버튼에 사용
    public void ReinSelect()
    {
        //메인재료창, 보조재료창에 아이템이 있다면
        if(setReinBool && setMateBool)
        {
            //강화 선택창 ui 활성화
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(6).gameObject.SetActive(true);
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(6).transform.GetChild(2).GetComponent<Text>().text = "강화시 재료아이템은 사라집니다 강화하시겠습니까? \n비용 : " + (reinGold[mate1.reinForce] * (mate1.level / 10)) + "원";
        }
        //둘다 없거나, 둘중 하나가 없다면
        else
        {
            //실패창 ui 활성화
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).gameObject.SetActive(true);
            //실패창 ui 텍스트 변경
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(4).transform.GetChild(2).GetComponent<Text>().text = "강화 준비가 완료 되지 않았습니다";
        }
    }
    //강화 선택창 yes 버튼에 사용
    public void ReinForce()
    {
        //인덱스 -1 안되게 수정
        if (mate1.reinForce - 1 < 0)
            temp = 0;
        else
            temp = 1;
        //지불 금액 연산
        int a = (reinGold[mate1.reinForce - temp] * (mate1.level / 10));
        //소지금이 충분하다면
        if (ItemManager.Gold > a)
        {
            //강화 선택창 ui 비활성화
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(6).gameObject.SetActive(false);
            //강화 이펙트 ui 활성화
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).gameObject.SetActive(true);
            //강화 이펙트 ui 메인재료 이미지 활성화
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(1).gameObject.SetActive(true);
            //강화 이펙트 ui 메인재료 이미지 변경
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(1).GetComponent<Image>().sprite = FindSprite(mate1.cuberImage.name);
            //강화 이펙트 ui 보조재료 이미지 활성화
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(2).gameObject.SetActive(true);
            //강화 이펙트 ui 보조재료 이미지 변경
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(2).GetComponent<Image>().sprite = FindSprite(mate2.cuberImage.name);
            //메인 재료의 강화 정보가 있다면 텍스트 변경 강화정보가 없다면 공란으로
            if (mate1.reinForce > 0)
                canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = "+" + mate1.reinForce.ToString();
            else
                canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = " ";
            //강화 애니메이션 끝나면 다음 동작 실행 되도록 강화 애니메이션 길이 정도 만큼 wait
            StartCoroutine(ReinAniWait());
        }
        //소지금이 부족하다면
        else
        {
            //강화 선택창 ui 비활성화
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(6).gameObject.SetActive(false);
            //소지금 부족창 ui 활성화
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(7).gameObject.SetActive(true);
        }
    }
    //애니메이션 끝날때까지 기다려준다
    IEnumerator ReinAniWait()
    {
        yield return new WaitForSeconds(0.8f);
        //메인 재료의 이미지 비활성화
        canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(1).gameObject.SetActive(false);
        //서브 재료의 이미지 비활성화
        canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(2).gameObject.SetActive(false);
        //강화 이펙트 코루틴에 진입할수 있도록 true로 변경
        reinEffectBool = true;
        //강화 이펙트 활성화
        canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(3).gameObject.SetActive(true);
    }
    //강화 이펙트 코루틴
    IEnumerator ReinEffectSprite()
    {
        //재진입 안되도록 false
        reinEffectBool = false;
        yield return new WaitForSeconds(0.02f);
        //이펙트 스프라이트를 현재 스프라이트 번호로
        reinEffect.GetComponent<Image>().sprite = criticalReinForce[reinEffectSpriteIndex];
        //스프라이트 번호 증가
        reinEffectSpriteIndex++;
        //재진입 되도록 true
        reinEffectBool = true;
        //마지막 스프라이트를 넘어가면
        if(reinEffectSpriteIndex > 18)
        {
            //스프라이트 번호 초기화
            reinEffectSpriteIndex = 0;
            //재진입 안되도록 false
            reinEffectBool = false;
            //강화 이펙트 비활성화
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(3).gameObject.SetActive(false);
            //강화 1증가
            mate1.reinForce += 1;
            //강화 id 0.05빼준다(정렬했을때 같은 템들중에서 가장 앞에 나열되게 하려고)
            mate1.id -= 0.05f;
            int min = mate1.reinForce;
            ItemManager.Gold -= (reinGold[mate1.reinForce - 1] * (mate1.level / 10));
            //강화 완료 아이템 슬롯 활성화
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(4).gameObject.SetActive(true);
            //강화 완료 아이템 이미지 활성화
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(5).gameObject.SetActive(true);
            //강화 완료 아이템 이미지를 변경
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(5).GetComponent<Image>().sprite = FindSprite(mate1.cuberImage.name);
            //강화 완료 아이템의 강화 텍스트 변경
            canvasTr.GetChild(16).transform.GetChild(5).transform.GetChild(5).transform.GetChild(5).transform.GetChild(0).GetComponent<Text>().text = "+" + mate1.reinForce.ToString();
            //메인재료창의 강화 텍스트 공란으로
            objectMate1.transform.GetChild(1).GetComponent<Text>().text = " ";
            //메인재료창의 이미지를 빈슬롯 이미지로
            objectMate1.transform.GetChild(0).GetComponent<Image>().sprite = slotCuber;
            //보조재료창의 이미지를 빈슬롯 이미지로
            objectMate2.transform.GetChild(0).GetComponent<Image>().sprite = slotCuber;
            //메인재료 유무 false
            setReinBool = false;
            //보조재료 유무 false
            setMateBool = false;

            for(int i = 0; i < itemManagerSc.inventorySlotBool.Length; i++)
            {
                //인벤토리 빈칸을 찾는다
                if(!itemManagerSc.inventorySlotBool[i])
                {
                    //인벤토리칸의 유무를 true
                    itemManagerSc.inventorySlotBool[i] = true;
                    //인벤토리칸의 정보를 메인재료 정보로
                    itemManagerSc.inventorySlotInfos[i] = new ItemInfo(mate1);
                    //인벤토리칸의 이미지를 정보대로 변경한다
                    itemManagerSc.inventorySlots[i].GetComponent<Image>().sprite = itemManagerSc.inventorySlotInfos[i].cuberImage;
                    //인벤토리칸의 강화 텍스트를 수정한다
                    itemManagerSc.inventorySlots[i].transform.GetChild(2).GetComponent<Text>().text = "+" + itemManagerSc.inventorySlotInfos[i].reinForce.ToString();
                    break;
                }
            }
            //메인재료 정보 초기화
            mate1 = null;
            //보조재료 정보 초기화
            mate2 = null;
        }
    }
    //스프라이트 이름으로 같은 이름의 스프라이트를 찾는다(같은 이름의 다른이미지 사용하기위해)
    Sprite FindSprite(string name)
    {
        for (int i = 0; i < makeEndImage.Length; i++)
        {
            if (makeEndImage[i].name == name)
            {
                return makeEndImage[i];
            }
        }
        return slotCuber;
    }

    ItemInfo RandomFindInfo(string itemImageName)
    {
        for (int i = 0; i < itemTable.itemInfos.Length; i++)
        {
            if (itemImageName == itemTable.itemInfos[i].cuberImage.name)
                return itemTable.itemInfos[i];
        }
        return null;
    }

    void RandomItemLoad(XmlReader xmlReader, string compareName, int num)
    {
        if (xmlReader.Name == compareName)
        {
            if (xmlReader.Read())
            {
                if(xmlReader.Value.Trim() != "slotCuber")
                {
                    randomInvenInfos[num] = new ItemInfo(RandomFindInfo(xmlReader.Value.Trim()));
                    randomInvenSlots[num].GetComponent<Image>().sprite = randomInvenInfos[num].cuberImage;
                }
            }
        }
    }

    void RandomBoolLoad(XmlReader xmlReader, string compareName, int num)
    {
        if (xmlReader.Name == compareName)
        {
            if (xmlReader.Read())
            {
                randomInvenBools[num] = bool.Parse(xmlReader.Value.Trim());
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
                    for (int i = 0; i < randomInvenBools.Length; i++)
                    {
                        RandomItemLoad(xmlReader, saveLoadDataSc.randomImageNames[i], i);
                        RandomBoolLoad(xmlReader, saveLoadDataSc.randomSlotBools[i], i);
                    }
                }
            }
        }
    }
}

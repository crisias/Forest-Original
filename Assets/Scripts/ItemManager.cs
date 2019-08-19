using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemManager : MonoBehaviour
{
    //canvas 객체에 접근하기위한 tag, tr
    string canvasTag;
    Transform canvasTr;
    //player 객체에 접근하기위한 tag, tr, sc
    string playerTag;
    Transform playerTr;
    Player playerSc;
    //itemtable에 접근하기위한 선언(드래그앤 드롭)
    public ItemTable itemTable;
    //inventory 칸마다 템이 있는지 없는지를 표현할 bool값
    public bool[] inventorySlotBool = new bool[60];
    //inventory 칸마다 잠금 bool값
    public bool[] inventoryLockBool = new bool[60];
    //equip 칸마다 템이 있는지 없는지를 표현할 bool값
    public bool[] equipSlotBool = new bool[10];
    //equip 칸마다 잠금 bool 값
    public bool[] equipLockBool = new bool[10];
    //inventory 칸마다 있는 템을 표현하기 위해 칸마다의 오브젝트를 담을 리스트(이 리스트를 사용해서 인벤토리칸의 아이템이미지를 변경한다)
    public List<GameObject> inventorySlots = new List<GameObject>();
    //inventory 칸마다 있는 템의 정보를 같고있을 클래스 배열
    public ItemInfo[] inventorySlotInfos = new ItemInfo[60];
    //equip 칸마다 있는 템을 표현하기 위한 리스트(이 리스트를 사용해서 장착칸의 이미지를 변경한다)
    public List<GameObject> equipSlots = new List<GameObject>();
    //equip 칸마다 있는 템의 정보를 가지고 있을 클래스 배열
    public ItemInfo[] equipSlotInfos = new ItemInfo[10];
    //아이템 정보창을 위한 bool값
    public bool itemInfoWindowBool;
    //잡화템 정보창을 위한 bool값
    public bool etcInfoWindowBool;
    //클릭한 오브젝트의 정보를 받기위한 오브젝트
    GameObject currentObject;
    //현재 선택된 오브젝트(인벤칸,장착칸)의 순번을 파악하기위해 사용
    public int currentSlotIndex;
    //아이템 정보창 객체에 접근하기 위한 tr
    Transform itemInfoWindowTr;
    //잡화템 정보창 객체에 접근하기 위한 tr
    Transform etcInfoWindowTr;
    //템이 없는 곳의 이미지 (0 = 인벤토리, 1 ~ 9 = 장착칸)(드래그앤 드롭)
    public Sprite[] cuberImage = new Sprite[10];
    //장착, 해제를 위한 bool값
    bool equip;
    //삭제 창을 위한 bool값
    bool DeleteWindowBool;
    //스텟창에 띄울 현재 스텟값과 장비옵션스텟의 합산값
    public int hp;
    public int mp;
    public int str;
    public int def;
    public float dex;
    public float cripro;
    public float cridem;
    //템순서정리할때 쓸 리스트 배열
    public List<ItemInfo> temp = new List<ItemInfo>();
    //player의 소지금
    public static int Gold;

    void Awake()
    {
        //canvas tag, tr 정의
        canvasTag = FindTag.getInstance().canvas;
        canvasTr = Find.getInstance().FindTagTransform(canvasTag);
        //player tag, tr, sc 정의
        playerTag = FindTag.getInstance().player;
        playerTr = Find.getInstance().FindTagTransform(playerTag);
        playerSc = playerTr.GetComponent<Player>();
        //inventoryslots 리스트에 인벤토리 칸 오브젝트를 넣는다(-1은 맨앞에 칸과 상관 없는 자식이 하나있어서)
        for (int i = 1; i < canvasTr.GetChild(6).transform.GetChild(0).transform.GetChild(0).childCount; i++)
        {
            inventorySlots.Add(canvasTr.GetChild(6).transform.GetChild(0).transform.GetChild(0).transform.GetChild(i).gameObject);
            //중복템에 쓸 text정보 초기화 
            inventorySlots[i - 1].transform.GetChild(0).GetComponent<Text>().text = "";
        }
        //equipslots 리스트에 장착칸 오브젝트를 넣는다
        for (int i = 0; i < canvasTr.GetChild(5).transform.GetChild(0).childCount; i++)
        {
            equipSlots.Add(canvasTr.GetChild(5).transform.GetChild(0).transform.GetChild(i).gameObject);
        }
    }

    void Start()
    {
        //아이템정보창 오브젝트 연결
        itemInfoWindowTr = canvasTr.GetChild(8).transform;
        //잡화정보창 오브젝트 연결
        etcInfoWindowTr = canvasTr.GetChild(12).transform;
        //장착칸에 코스튬과 펫 아이템 미리 하나씩 생성
        equipSlotInfos[7] = itemTable.itemInfos[24];
        equipSlotInfos[9] = itemTable.itemInfos[21];
        equipSlotBool[7] = true;
        equipSlotBool[9] = true;
        equipSlots[6].GetComponent<Image>().sprite = equipSlotInfos[7].cuberImage;
        equipSlots[8].GetComponent<Image>().sprite = equipSlotInfos[9].cuberImage;

        //test 잡화템생성
        //for(int i = 0; i < 15; i++)
        //{
        //    inventorySlotInfos[i] = itemTable.itemInfos[31 + i];
        //    inventorySlotInfos[i].count = 50;
        //    inventorySlots[i].transform.GetChild(0).GetComponent<Text>().text = inventorySlotInfos[i].count.ToString();
        //    inventorySlotBool[i] = true;
        //    inventorySlots[i].GetComponent<Image>().sprite = inventorySlotInfos[i].cuberImage;
        //}
    }

    void Update()
    {
        //인벤토리 테스트를 위해 템생성을 할때쓸 키
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            int rand;
            rand = Random.Range(0, 3);
            ItemAdd(rand);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            int rand;
            rand = Random.Range(3, 6);
            ItemAdd(rand);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            int rand;
            rand = Random.Range(6, 9);
            ItemAdd(rand);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            int rand;
            rand = Random.Range(9, 12);
            ItemAdd(rand);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            int rand;
            rand = Random.Range(12, 15);
            ItemAdd(rand);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            int rand;
            rand = Random.Range(15, 18);
            ItemAdd(rand);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            int rand;
            rand = Random.Range(18, 21);
            ItemAdd(rand);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            int rand;
            rand = Random.Range(21, 24);
            ItemAdd(rand);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            int rand;
            rand = Random.Range(24, 28);
            ItemAdd(rand);
        }
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            int rand;
            rand = Random.Range(28, 46);
            ItemAdd(rand);
        }
        ShowLock();
    }

    //테스트를 위해 사용한 함수 번호를 받아 그번호의 아이템 정보와 이미지를 받는다
    public void ItemAdd(int rand)
    {
        ItemInfo addInfo = new ItemInfo(itemTable.itemInfos[rand]);

        if (addInfo.id < 1000)
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (!inventorySlotBool[i])
                {
                    inventorySlotBool[i] = true;
                    inventorySlotInfos[i] = new ItemInfo(addInfo);
                    inventorySlots[i].GetComponent<Image>().sprite = inventorySlotInfos[i].cuberImage;
                    break;
                }
            }
        }
        else if (addInfo.id > 1000)
        {
            if (EtcOverlap(addInfo) == true)
            {
                for(int i = 0; i < inventorySlots.Count; i++)
                {
                    if(inventorySlotBool[i] && inventorySlotInfos[i].id == addInfo.id)
                    {
                        inventorySlotInfos[i].count += 1;
                        inventorySlots[i].transform.GetChild(0).GetComponent<Text>().text = inventorySlotInfos[i].count.ToString();
                    }
                }
            }
            else if (EtcOverlap(addInfo) == false)
            {
                for (int i = 0; i < inventorySlots.Count; i++)
                {
                    if (!inventorySlotBool[i])
                    {
                        inventorySlotInfos[i] = new ItemInfo(addInfo);
                        //inventorySlotInfos[i].count += 1;
                        inventorySlotBool[i] = true;
                        inventorySlots[i].GetComponent<Image>().sprite = inventorySlotInfos[i].cuberImage;
                        inventorySlots[i].transform.GetChild(0).gameObject.GetComponent<Text>().text = inventorySlotInfos[i].count.ToString();
                        break;
                    }
                }
            }
        }
    }
    //테스트 같은템이 있으면 true 없으면 false
    bool EtcOverlap(ItemInfo num)
    {
        for(int i = 0; i < inventorySlotBool.Length; i++)
        {
            if(inventorySlotBool[i] && inventorySlotInfos[i].id == num.id)
            {
                return true;
            }
        }
        return false;
    }

    //아이템 정보창 윈도우 함수(인벤토리 안, 장착칸 모든 칸 버튼에 달려있다)
    public void ItemWindow()
    {
        //현재 오브젝트 정보를 파악
        currentObject = EventSystem.current.currentSelectedGameObject;
        //현재 오브젝트의 이름
        string currentObjectName = currentObject.name;
        //현재 선택된 오브젝트가 slot이라면(장착칸인지 인벤토리칸인지 구분)
        if (currentObject.tag == "Slot")
        {
            //현재 선택된 오브젝트가 몇번째 자식(칸)인지 파악
            currentSlotIndex = (canvasTr.GetChild(6).transform.GetChild(0).transform.GetChild(0).transform.Find(currentObjectName).GetSiblingIndex()) - 1;
            //그 순번의 bool값이 true일 경우(그칸에 아이템이 있을 경우)
            if (inventorySlotBool[currentSlotIndex])
            {
                //장비템인지 잡화템인지 구별 장비템이라면
                if(inventorySlotInfos[currentSlotIndex].id < 800)
                {
                    //아이템 정보창 bool값 true로  
                    itemInfoWindowBool = !itemInfoWindowBool;
                    //아이템 정보창 bool이 true면
                    if (itemInfoWindowBool)
                    {
                        //아이템 정보창 객체활성화
                        canvasTr.GetChild(7).gameObject.SetActive(true);
                        canvasTr.GetChild(8).gameObject.SetActive(true);
                        //equip true(장착 할때씀)
                        equip = true;
                        canvasTr.GetChild(8).transform.GetChild(10).transform.GetChild(0).GetComponent<Text>().text = "장착";

                        ShowInfo(inventorySlotInfos[currentSlotIndex]);
                    }
                }
                //잡화템이라면
                else if(inventorySlotInfos[currentSlotIndex].id > 800)
                {
                    //잡템 정보창 bool true로
                    etcInfoWindowBool = !etcInfoWindowBool;
                    //잡템 정보창 bool이 true면
                    if(etcInfoWindowBool)
                    {
                        //잡템 정보창 객체 활성화
                        canvasTr.GetChild(11).gameObject.SetActive(true);
                        canvasTr.GetChild(12).gameObject.SetActive(true);
                        //코스튬이나 펫외형일때는 장착버튼 활성화
                        equip = true;
                        canvasTr.GetChild(12).transform.GetChild(4).transform.GetChild(0).GetComponent<Text>().text = "장착";
                        ShowInfo(inventorySlotInfos[currentSlotIndex]);
                    }
                    //잡템일 경우 장착 버튼 비활성화
                    if (inventorySlotInfos[currentSlotIndex].id > 1000)
                        canvasTr.GetChild(12).transform.GetChild(4).gameObject.SetActive(false);
                }
            }
        }
        //tag가 equipslot이면
        else if(currentObject.tag == "EquipSlot")
        {
            //현재 선택한 장착칸이 몇번째 자식인지 파악
            currentSlotIndex = (canvasTr.GetChild(5).transform.GetChild(0).transform.Find(currentObjectName).GetSiblingIndex()) + 1;
            //그 순번의 bool값이 true라면(그 장착칸에 아이템이 있다면)
            if (equipSlotBool[currentSlotIndex])
            {
                //장비템이라면
                if(equipSlotInfos[currentSlotIndex].id < 800)
                {
                    //아이템정보창을 true로
                    itemInfoWindowBool = !itemInfoWindowBool;
                    //아이템정보창이 true면
                    if (itemInfoWindowBool)
                    {
                        //아이템 정보창 객체 활성화
                        canvasTr.GetChild(7).gameObject.SetActive(true);
                        canvasTr.GetChild(8).gameObject.SetActive(true);
                        //equip false(해제 할때씀)
                        equip = false;
                        canvasTr.GetChild(8).transform.GetChild(10).transform.GetChild(0).GetComponent<Text>().text = "해제";

                        ShowInfo(equipSlotInfos[currentSlotIndex]);
                    }
                }
                //외형템이라면
                else if(equipSlotInfos[currentSlotIndex].id > 800)
                {
                    //잡화템정보창을 true
                    etcInfoWindowBool = !etcInfoWindowBool;
                    //잡화템정보창 bool true면
                    if (etcInfoWindowBool)
                    {
                        //잡화템 정보창 활성화
                        canvasTr.GetChild(11).gameObject.SetActive(true);
                        canvasTr.GetChild(12).gameObject.SetActive(true);
                        //장착,해제 버튼을 비활성화(펫하고 코스튬템 장착시 해제 안되게 함 교환만 가능)
                        canvasTr.GetChild(12).transform.GetChild(4).gameObject.SetActive(false);

                        ShowInfo(equipSlotInfos[currentSlotIndex]);
                    }
                }
            }
        }
    }
    //아이템 정보창 끌때쓸 함수(정보창이 아닌 화면을 누르면 꺼지게 구현)
    public void ItemWindowClose()
    {
        //장비템창이 켜져있다면
        if (itemInfoWindowBool)
        {
            //false로
            itemInfoWindowBool = false;
            if (!itemInfoWindowBool)
            {
                //back하고 장비템창을 꺼준다
                canvasTr.GetChild(7).gameObject.SetActive(false);
                canvasTr.GetChild(8).gameObject.SetActive(false);
            }
            //장착,해제 버튼 활성화
            canvasTr.GetChild(8).transform.GetChild(10).gameObject.SetActive(true);
        }
        //잡템창이 켜져있다면
        else if (etcInfoWindowBool)
        {
            //false로 바꿔준다
            etcInfoWindowBool = false;
            if (!etcInfoWindowBool)
            {
                //back하고 잡템창을 꺼준다
                canvasTr.GetChild(11).gameObject.SetActive(false);
                canvasTr.GetChild(12).gameObject.SetActive(false);
            }
            //장착,해제 버튼 활성화
            canvasTr.GetChild(12).transform.GetChild(4).gameObject.SetActive(true);
        }
    }
    //장비템창, 잡템창에서 잠금 설정시 실시간으로 잠겼다 풀렸다하는 이미지를 위해 사용(각각을 나눠논이유는 각각의 정보창이 달라서)
    void ShowLock()
    {
        //장비템창이나 잡템창이 활성화 돼어있다면
        if(canvasTr.GetChild(8).gameObject.activeInHierarchy || canvasTr.GetChild(12).gameObject.activeInHierarchy)
        {
            //현재 클릭한 버튼의 tag가 slot이라면
            if (currentObject.tag == "Slot")
            {
                //id가 장비템이라면(코스튬, 펫외형 제외)
                if(inventorySlotInfos[currentSlotIndex].id < 800)
                {
                    //현재 칸의 아이템이 잠겨있다면 정보창의 잠긴이미지 활성화
                    if (inventoryLockBool[currentSlotIndex])
                        itemInfoWindowTr.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
                    //현재 칸의 아이템이 잠겨있지 않다면 정보창의 잠긴 이미지 비활성화
                    else if (!inventoryLockBool[currentSlotIndex])
                        itemInfoWindowTr.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                }
                //id가 잡템이라면(코스튬, 펫외형 포함)
                else if(inventorySlotInfos[currentSlotIndex].id > 800)
                {
                    //현재 칸의 장비가 잠겨있다면 정보창의 잠긴이미지 활성화
                    if (inventoryLockBool[currentSlotIndex])
                        etcInfoWindowTr.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
                    //현재 칸의 장비가 잠겨있지 않다면 정보창의 잠긴이미지 비활성화
                    else if (!inventoryLockBool[currentSlotIndex])
                        etcInfoWindowTr.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                }
            }
            //현재 클릭한 버튼의 tag가 equipslot이라면
            else if (currentObject.tag == "EquipSlot")
            {
                //id가 장비템이라면(코스튬, 펫외형 제외)
                if (equipSlotInfos[currentSlotIndex].id < 800)
                {
                    //현재칸의 아이템이 잠금 상태라면 정보창의 잠금 이미지를 활성화
                    if (equipLockBool[currentSlotIndex])
                        itemInfoWindowTr.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
                    //현재칸의 아이템이 잠근 상태가 아니라면 정보창의 잠금 이미지를 비활성화
                    else if (!equipLockBool[currentSlotIndex])
                        itemInfoWindowTr.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                }
                //id가 잡템이라면(코스튬, 펫외형 포함)
                else if (equipSlotInfos[currentSlotIndex].id > 800)
                {
                    //현재칸의 아이템이 잠금상태라면 정보창의 잠금 이미지를 활성
                    if (equipLockBool[currentSlotIndex])
                        etcInfoWindowTr.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
                    //현재칸의 아이템이 잠금상태가 아니라면 정보창의 정보이미지를 비활성화
                    else if (!equipLockBool[currentSlotIndex])
                        etcInfoWindowTr.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
    }

    //아이템정보창 객체의 텍스트와 이미지를 변경하는 함수(템 분류 별로 띄울 정보 변경)
    void ShowInfo(ItemInfo iteminfo)
    {
        //장비템일때(코스튬, 펫외형 제외)
        if(iteminfo.id < 800)
        {
            //아이템의 강화 수치가 있다면
            if (iteminfo.reinForce > 0)
            {
                //강화 수치 텍스트 변경
                itemInfoWindowTr.GetChild(0).transform.GetChild(2).GetComponent<Text>().text = "+" + iteminfo.reinForce.ToString();
            }
            //아이템의 강화 수치가 없다면 텍스트 공란으로
            else
                itemInfoWindowTr.GetChild(0).transform.GetChild(2).GetComponent<Text>().text = " ";
            itemInfoWindowTr.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = iteminfo.cuberImage;
            itemInfoWindowTr.GetChild(1).GetComponent<Text>().text = iteminfo.name;
            itemInfoWindowTr.GetChild(2).GetComponent<Text>().text = "장착 레벨 : " + iteminfo.level.ToString();
            itemInfoWindowTr.GetChild(3).GetComponent<Text>().text = "공격력 : " + iteminfo.str.ToString();
            itemInfoWindowTr.GetChild(4).GetComponent<Text>().text = "방어력 : " + iteminfo.def.ToString();
            itemInfoWindowTr.GetChild(5).GetComponent<Text>().text = "체력 : " + iteminfo.hp.ToString();
            itemInfoWindowTr.GetChild(6).GetComponent<Text>().text = "마력 : " + iteminfo.mp.ToString();
            itemInfoWindowTr.GetChild(7).GetComponent<Text>().text = "회피율 : " + iteminfo.dex.ToString() + "%";
            itemInfoWindowTr.GetChild(8).GetComponent<Text>().text = "치명타 확률 : " + iteminfo.cripro.ToString() + "%";
            itemInfoWindowTr.GetChild(9).GetComponent<Text>().text = "치명타 데미지 : " + iteminfo.cridem.ToString() + "%";
        }
        //잡템일때(코스튬, 펫외형 포함)
        else if(iteminfo.id > 800)
        {
            etcInfoWindowTr.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = iteminfo.cuberImage;
            etcInfoWindowTr.GetChild(1).GetComponent<Text>().text = iteminfo.name;
            //잡템일때
            if (iteminfo.id > 1000)
                etcInfoWindowTr.GetChild(2).GetComponent<Text>().text = "소지 개수 : " + iteminfo.count.ToString();
            //코스튬, 펫외형일때
            else if (iteminfo.id < 1000)
                etcInfoWindowTr.GetChild(2).GetComponent<Text>().text = " ";
            etcInfoWindowTr.GetChild(3).GetComponent<Text>().text = iteminfo.info;
        }
    }
    //아이템 잠금 해제 버튼
    public void ItemLockButton()
    {
        //현재 클릭한 버튼의 tag가 slot일때
        if(currentObject.tag == "Slot")
        {
            inventoryLockBool[currentSlotIndex] = !inventoryLockBool[currentSlotIndex];
            if (inventoryLockBool[currentSlotIndex])
            {
                //잠금이미지를 활성화 해주고 id에서 0.1 빼준다(정리할때 같은종류의 템중에서 잠겨있는 템이 앞쪽으로 몰리게하기위해서)
                inventorySlotInfos[currentSlotIndex].id -= 0.1f;
                inventorySlots[currentSlotIndex].transform.GetChild(1).gameObject.SetActive(true);
            }
            else if (!inventoryLockBool[currentSlotIndex])
            {
                //잠금이미지를 비활성화 해주고 id에서 0.1 더해준다
                inventorySlotInfos[currentSlotIndex].id += 0.1f;
                inventorySlots[currentSlotIndex].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        //현재 클릭한 버튼의 tag가 equipslot일때
        else if(currentObject.tag == "EquipSlot")
        {
            equipLockBool[currentSlotIndex] = !equipLockBool[currentSlotIndex];
            if (equipLockBool[currentSlotIndex])
            {
                //잠금이미지를 활성화 해주고 id 0.1빼준다
                equipSlotInfos[currentSlotIndex].id -= 0.1f;
                equipSlots[currentSlotIndex - 1].transform.GetChild(0).gameObject.SetActive(true);
            }
            else if (!equipLockBool[currentSlotIndex])
            {
                //잠금이미지 비활성화해주고 id 0.1더해준다
                equipSlotInfos[currentSlotIndex].id += 0.1f;
                equipSlots[currentSlotIndex - 1].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
    //장착, 해제 버튼 공용함수
    public void EquipButton()
    {
        //equip가 true면 장착, equip가 false면 해제
        if (equip)
        {
            Equip();
            StatusSum();
            StatusPlus(hp, mp, str, def, dex, cripro, cridem);
        }
        else if (!equip)
        {
            UnEquip();
            StatusMin(hp, mp, str, def, dex, cripro, cridem);
            StatusSum();
        }
    }
    void StatusPlus(int hp, int mp, int str, int def, float dex, float cripro, float cridem)
    {
        playerSc.currentMaxStatus.hp += hp;
        playerSc.currentMaxStatus.mp += mp;
        playerSc.currentPlayerInfo.str += str;
        playerSc.currentPlayerInfo.def += def;
        playerSc.currentPlayerInfo.dex += (int)dex;
        playerSc.currentPlayerInfo.cripro += cripro;
        playerSc.currentPlayerInfo.cridem += cridem;
    }
    void StatusMin(int hp, int mp, int str, int def, float dex, float cripro, float cridem)
    {
        playerSc.currentMaxStatus.hp -= hp;
        playerSc.currentMaxStatus.mp -= mp;
        playerSc.currentPlayerInfo.str -= str;
        playerSc.currentPlayerInfo.def -= def;
        playerSc.currentPlayerInfo.dex -= (int)dex;
        playerSc.currentPlayerInfo.cripro -= cripro;
        playerSc.currentPlayerInfo.cridem -= cridem;
    }
    //장착 함수
    public void Equip()
    {
        //현재 선택된 아이템의 type 파악
        int type = inventorySlotInfos[currentSlotIndex].type;
        //타입별로 함수 실행
        switch (type)
        {
            case 1:
                Swap(type, currentSlotIndex);
                break;
            case 2:
                Swap(type, currentSlotIndex);
                break;
            case 3:
                Swap(type, currentSlotIndex);
                break;
            case 4:
                Swap(type, currentSlotIndex);
                break;
            case 5:
                Swap(type, currentSlotIndex);
                break;
            case 6:
                Swap(type, currentSlotIndex);
                break;
            case 7:
                Swap(type, currentSlotIndex);
                //펫외형 교체 함수 실행을 위해 petSwap을 true로
                PlayerCtrl.petSwap = true;
                break;
            case 8:
                Swap(type, currentSlotIndex);
                break;
            case 9:
                Swap(type, currentSlotIndex);
                //의상 교체 함수 실행을 위해 cosSwap을 true로
                PlayerCtrl.cosSwap = true;
                break;
        }
    }
    //타입별 장착 함수
    void Swap(int typeNum, int currentSlotNum)
    {
        //player레벨이 아이템 제한 레벨과 같거나 크다면
        if(playerSc.currentPlayerInfo.level >= inventorySlotInfos[currentSlotIndex].level)
        { 
            //현재 장착칸(type)의 bool값이 false면(현재 장착칸에 장비가 없다면)
            if (!equipSlotBool[typeNum])
            {
                //인벤토리칸 아이템이 잠겨있다면
                if(inventoryLockBool[currentSlotNum])
                {
                    //인벤토리 잠금 bool값 false로 하고 자물쇠 이미지 오브젝트 꺼준다
                    inventoryLockBool[currentSlotNum] = false;
                    inventorySlots[currentSlotNum].transform.GetChild(1).gameObject.SetActive(false);
                    //장착칸 잠금 bool값 true로 해준뒤 자물쇠 이미지 오브젝트 켜준다
                    equipLockBool[typeNum] = true;
                    equipSlots[typeNum - 1].transform.GetChild(0).gameObject.SetActive(true);
                }

                //현재 장착칸 bool값 true로
                equipSlotBool[typeNum] = true;
                //장착칸 아이템 정보에 인벤토리에 있던 아이템 정보 넘겨줌
                equipSlotInfos[typeNum] = inventorySlotInfos[currentSlotNum];
                //장착칸 아이템 이미지를 정보에 있는 이미지로 바꿔줌
                equipSlots[typeNum - 1].GetComponent<Image>().sprite = equipSlotInfos[typeNum].cuberImage;
                //장착칸 강화 정보를 수정
                if (equipSlotInfos[typeNum].reinForce > 0)
                    equipSlots[typeNum - 1].transform.GetChild(1).GetComponent<Text>().text = "+" + equipSlotInfos[typeNum].reinForce.ToString();
                else
                    equipSlots[typeNum - 1].transform.GetChild(1).GetComponent<Text>().text = " ";
                //인벤토리 이미지를 빈이미지로 바꿔준다
                inventorySlots[currentSlotNum].GetComponent<Image>().sprite = cuberImage[0];
                //인벤토리 아이템칸 false
                inventorySlotBool[currentSlotNum] = false;
                //인벤토리 강화 정보 초기화
                inventorySlots[currentSlotNum].transform.GetChild(2).GetComponent<Text>().text = " ";
                //인벤토리 아이템칸 정보 초기화
                inventorySlotInfos[currentSlotNum] = null;
                //아이템 정보창 꺼준다
                itemInfoWindowBool = !itemInfoWindowBool;
                if (!itemInfoWindowBool)
                {
                    canvasTr.GetChild(7).gameObject.SetActive(false);
                    canvasTr.GetChild(8).gameObject.SetActive(false);
                }
            }
            //현재 장착칸(type)의 bool값이 true면(현재 장착칸에 장비가 있다면)
            else if (equipSlotBool[typeNum])
            {
                StatusMin(hp, mp, str, def, dex, cripro, cridem);

                //인벤토리나 장착칸 중에 잠긴 아이템이 있다면
                if(inventoryLockBool[currentSlotNum] || equipLockBool[typeNum])
                {
                    //장비, 인벤에 아이템 잠김 상태를 저장할 bool값
                    bool equipTemp;
                    bool invenTemp;
                    //잠김 상태를 저장
                    equipTemp = equipLockBool[typeNum];
                    invenTemp = inventoryLockBool[currentSlotNum];
                    //장비칸과 인벤토리칸 양쪽다 잠금 bool값 false로 바꾸고 잠금이미지 오브젝트 꺼준다
                    inventoryLockBool[currentSlotNum] = false;
                    equipLockBool[typeNum] = false;
                    inventorySlots[currentSlotNum].transform.GetChild(1).gameObject.SetActive(false);
                    equipSlots[typeNum - 1].transform.GetChild(0).gameObject.SetActive(false);
                    //잠긴 상태였던 아이템은 다시 잠금 상태로 만들어준다
                    //장비쪽이 잠긴상태였다면
                    if(equipTemp)
                    {
                        inventoryLockBool[currentSlotNum] = true;
                        inventorySlots[currentSlotNum].transform.GetChild(1).gameObject.SetActive(true);
                    }
                    //인벤쪽이 잠긴상태였다면
                    if(invenTemp)
                    {
                        equipLockBool[typeNum] = true;
                        equipSlots[typeNum - 1].transform.GetChild(0).gameObject.SetActive(true);
                    }
                }

                //서로의 정보를 교환하기 위해 별도의 공간을 하나 만든다
                ItemInfo temp;
                //temp에 인벤토리칸의 아이템 정보를 넘겨준다
                temp = inventorySlotInfos[currentSlotNum];
                //인벤토리칸의 정보에 장착칸의 아이템 정보를 넘겨준다
                inventorySlotInfos[currentSlotNum] = equipSlotInfos[typeNum];
                //장착칸에 temp에 있는 아이템 정보를 넘겨준다
                equipSlotInfos[typeNum] = temp;
                //장착칸 이미지를 정보대로 변경한다
                equipSlots[typeNum - 1].GetComponent<Image>().sprite = equipSlotInfos[typeNum].cuberImage;
                //장착칸 아이템에 강화 정보가 있다면
                if (equipSlotInfos[typeNum].reinForce > 0)
                    equipSlots[typeNum - 1].transform.GetChild(1).GetComponent<Text>().text = "+" + equipSlotInfos[typeNum].reinForce.ToString();
                else
                    equipSlots[typeNum - 1].transform.GetChild(1).GetComponent<Text>().text = " ";
                //인벤토리칸 아이템에 강화 정보가 있다면
                if (inventorySlotInfos[currentSlotNum].reinForce > 0)
                    inventorySlots[currentSlotNum].transform.GetChild(2).GetComponent<Text>().text = "+" + inventorySlotInfos[currentSlotNum].reinForce.ToString();
                else
                    inventorySlots[currentSlotNum].transform.GetChild(2).GetComponent<Text>().text = " ";
                //인벤토리칸 이미지를 정보대로 변경한다
                inventorySlots[currentSlotNum].GetComponent<Image>().sprite = inventorySlotInfos[currentSlotNum].cuberImage;
                //아이템 정보창을 꺼준다
                itemInfoWindowBool = !itemInfoWindowBool;
                if (!itemInfoWindowBool)
                {
                    canvasTr.GetChild(7).gameObject.SetActive(false);
                    canvasTr.GetChild(8).gameObject.SetActive(false);
                }
                //잡텝 정보창 꺼준다
                etcInfoWindowBool = !etcInfoWindowBool;
                if (!etcInfoWindowBool)
                {
                    canvasTr.GetChild(11).gameObject.SetActive(false);
                    canvasTr.GetChild(12).gameObject.SetActive(false);
                }
            }
        }
        //플레이어 레벨이 장착 레벨보다 낮다면
        else
        {
            //안내창을 띄어준다
            canvasTr.GetChild(9).gameObject.SetActive(true);
            canvasTr.GetChild(10).gameObject.SetActive(true);
        }
    }
    //장착 해제 함수
    public void UnEquip()
    {
        //인벤토리칸의 bool값이 false인 가장 빠른곳을 찾는다
        for(int i = 0; i < inventorySlots.Count; i++)
        {
            //인벤토리 칸이 비었다면
            if (!inventorySlotBool[i])
            {
                //장착칸에 아이템이 잠겨있다면
                if (equipLockBool[currentSlotIndex])
                {
                    //장착칸 잠금 bool을 false로 바꾸고 이미지 오브젝트를 꺼준다
                    equipLockBool[currentSlotIndex] = false;
                    equipSlots[currentSlotIndex - 1].transform.GetChild(0).gameObject.SetActive(false);
                    //인벤칸 잠금을 true로 바꾸고 이미지 오브젝트를 켜준다
                    inventoryLockBool[i] = true;
                    inventorySlots[i].transform.GetChild(1).gameObject.SetActive(true);
                }

                //인벤토리 칸에 장착칸 아이템의 정보를 넘긴다
                inventorySlotInfos[i] = equipSlotInfos[currentSlotIndex];
                //인벤토리 칸 bool값을 true
                inventorySlotBool[i] = true;
                //장착칸 이미지를 빈이미지로 바꿔준다
                equipSlots[currentSlotIndex - 1].GetComponent<Image>().sprite = cuberImage[currentSlotIndex];
                //장착칸 bool값을 false로 바꿔준다
                equipSlotBool[currentSlotIndex] = false;
                //장착칸 강화 정보를 초기화 한다
                equipSlots[currentSlotIndex - 1].transform.GetChild(1).GetComponent<Text>().text = " ";
                //장착칸 정보를 초기화
                equipSlotInfos[currentSlotIndex] = null;
                //인벤토리칸 이미지를 정보 이미지로 바꿔준다
                inventorySlots[i].GetComponent<Image>().sprite = inventorySlotInfos[i].cuberImage;
                //인벤토리칸 강화정보가 있다면 켜준다
                if (inventorySlotInfos[i].reinForce > 0)
                    inventorySlots[i].transform.GetChild(2).GetComponent<Text>().text = "+" + inventorySlotInfos[i].reinForce.ToString();
                //아이템 정보창 꺼준다
                itemInfoWindowBool = !itemInfoWindowBool;
                if (!itemInfoWindowBool)
                {
                    canvasTr.GetChild(7).gameObject.SetActive(false);
                    canvasTr.GetChild(8).gameObject.SetActive(false);
                }
                break;
            }
        }
    }
    //삭제창 함수
    public void DeleteButton()
    {
        DeleteWindowBool = !DeleteWindowBool;
        if(DeleteWindowBool)
        {
            if(currentObject.tag == "Slot" && inventoryLockBool[currentSlotIndex])
            {
                //back하고 버리기 선택창 ui 켜준다
                canvasTr.GetChild(13).gameObject.SetActive(true);
                canvasTr.GetChild(14).gameObject.SetActive(true);
                canvasTr.GetChild(14).GetChild(0).gameObject.GetComponent<Text>().text = "잠금 상태에서는 버리실수 없습니다.";
                //버튼두개 꺼준다
                canvasTr.GetChild(14).GetChild(1).gameObject.SetActive(false);
                canvasTr.GetChild(14).GetChild(2).gameObject.SetActive(false);
            }
            else if(currentObject.tag == "EquipSlot" && equipSlotBool[currentSlotIndex])
            {
                //back하고 버리기 선택창 ui 켜준다
                canvasTr.GetChild(13).gameObject.SetActive(true);
                canvasTr.GetChild(14).gameObject.SetActive(true);
                canvasTr.GetChild(14).GetChild(0).gameObject.GetComponent<Text>().text = "장착 상태에서는 버리실수 없습니다.";
                //버튼두개 꺼준다
                canvasTr.GetChild(14).GetChild(1).gameObject.SetActive(false);
                canvasTr.GetChild(14).GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                //back하고 버리기 선택창 ui 켜준다
                canvasTr.GetChild(13).gameObject.SetActive(true);
                canvasTr.GetChild(14).gameObject.SetActive(true);
            }
        }
    }
    //예 버튼
    public void YesButton()
    {
        //장비템이라면(코스튬, 펫외형 포함)
        if(inventorySlotInfos[currentSlotIndex].id < 1000)
        {
            itemInfoWindowBool = !itemInfoWindowBool;
            etcInfoWindowBool = !etcInfoWindowBool;
            DeleteWindowBool = !DeleteWindowBool;
            //장비템 back 정보창 꺼준다
            canvasTr.GetChild(7).gameObject.SetActive(false);
            canvasTr.GetChild(8).gameObject.SetActive(false);
            //잡템 back 정보창 꺼준다
            canvasTr.GetChild(11).gameObject.SetActive(false);
            canvasTr.GetChild(12).gameObject.SetActive(false);
            //버리기 선택창 back 꺼준다
            canvasTr.GetChild(13).gameObject.SetActive(false);
            canvasTr.GetChild(14).gameObject.SetActive(false);
            //그칸의 유무 bool값 false로
            inventorySlotBool[currentSlotIndex] = false;
            //그칸의 이미지를 공란이미지로
            inventorySlots[currentSlotIndex].GetComponent<Image>().sprite = cuberImage[0];
            //그칸의 정보를 초기화한다
            inventorySlotInfos[currentSlotIndex] = null;
        }
        //잡템이라면
        else if(inventorySlotInfos[currentSlotIndex].id > 1000)
        {
            etcInfoWindowBool = !etcInfoWindowBool;
            DeleteWindowBool = !DeleteWindowBool;
            //잡템 back 정보창 꺼준다
            canvasTr.GetChild(11).gameObject.SetActive(false);
            canvasTr.GetChild(12).gameObject.SetActive(false);
            //버리기 선택창 back 꺼준다
            canvasTr.GetChild(13).gameObject.SetActive(false);
            canvasTr.GetChild(14).gameObject.SetActive(false);
            //잡템의 개수가 1보다 많다면
            if (inventorySlotInfos[currentSlotIndex].count > 1)
            {
                //개수만 1개 줄여준다
                inventorySlotInfos[currentSlotIndex].count -= 1;
                //텍스트를 수정한다
                inventorySlots[currentSlotIndex].transform.GetChild(0).gameObject.GetComponent<Text>().text = inventorySlotInfos[currentSlotIndex].count.ToString();
            }
            //잡템의 개수가 1이거나 적다면
            else if(inventorySlotInfos[currentSlotIndex].count <= 1)
            {
                //그칸의 유무 bool을 false로
                inventorySlotBool[currentSlotIndex] = false;
                //그칸의 이미지를 공란 이미지로
                inventorySlots[currentSlotIndex].GetComponent<Image>().sprite = cuberImage[0];
                //그칸의 text를 공란으로
                inventorySlots[currentSlotIndex].transform.GetChild(0).gameObject.GetComponent<Text>().text = " ";
                //그칸의 정보를 초기화한다
                inventorySlotInfos[currentSlotIndex] = null;
            }
        }
    }
    //아니요 버튼
    public void NoButton()
    {
        DeleteWindowBool = !DeleteWindowBool;
        //버리기 선택창 back을 꺼준다
        canvasTr.GetChild(13).gameObject.SetActive(false);
        canvasTr.GetChild(14).gameObject.SetActive(false);
        //text가 다르게 변경돼있을수 있으므로 디폴트 텍스트로 변경
        canvasTr.GetChild(14).GetChild(0).gameObject.GetComponent<Text>().text = "아이템을 버리시겠습니까?";
        //버튼이 꺼져 있을수도 있으므로 다시켜준다
        canvasTr.GetChild(14).GetChild(1).gameObject.SetActive(true);
        canvasTr.GetChild(14).GetChild(2).gameObject.SetActive(true);
    }
    //장비 스텟 계산 함수
    void StatusSum()
    {
        //장착칸 숫자만큼 배열 만든다
        int[] sumHp = new int[9];
        int[] sumMp = new int[9];
        int[] sumStr = new int[9];
        int[] sumDef = new int[9];
        float[] sumDex = new float[9];
        float[] sumCripro = new float[9];
        float[] sumCridem = new float[9];
        //장착칸수만큼 반복
        for(int i = 1; i < equipSlotBool.Length; i++)
        {
            //장착칸 bool이 false라면 스텟을 0으로 한다
            if(!equipSlotBool[i])
            {
                sumHp[i - 1] = 0;
                sumMp[i - 1] = 0;
                sumStr[i - 1] = 0;
                sumDef[i - 1] = 0;
                sumDex[i - 1] = 0;
                sumCripro[i - 1] = 0;
                sumCridem[i - 1] = 0;
            }
            //장착칸 bool이 true라면 스텟에 정보를 넘겨준다
            else if(equipSlotBool[i])
            {
                sumHp[i - 1] = equipSlotInfos[i].hp;
                sumMp[i - 1] = equipSlotInfos[i].mp;
                sumStr[i - 1] = equipSlotInfos[i].str;
                sumDef[i - 1] = equipSlotInfos[i].def;
                sumDex[i - 1] = equipSlotInfos[i].dex;
                sumCripro[i - 1] = equipSlotInfos[i].cripro;
                sumCridem[i - 1] = equipSlotInfos[i].cridem;
            }
        }
        //모든 스텟값 합산한다
        hp = sumHp[0] + sumHp[1] + sumHp[2] + sumHp[3] + sumHp[4] + sumHp[5] + sumHp[6] + sumHp[7] + sumHp[8];
        mp = sumMp[0] + sumMp[1] + sumMp[2] + sumMp[3] + sumMp[4] + sumMp[5] + sumMp[6] + sumMp[7] + sumMp[8];
        str = sumStr[0] + sumStr[1] + sumStr[2] + sumStr[3] + sumStr[4] + sumStr[5] + sumStr[6] + sumStr[7] + sumStr[8];
        def = sumDef[0] + sumDef[1] + sumDef[2] + sumDef[3] + sumDef[4] + sumDef[5] + sumDef[6] + sumDef[7] + sumDef[8];
        dex = sumDex[0] + sumDex[1] + sumDex[2] + sumDex[3] + sumDex[4] + sumDex[5] + sumDex[6] + sumDex[7] + sumDex[8];
        cripro = sumCripro[0] + sumCripro[1] + sumCripro[2] + sumCripro[3] + sumCripro[4] + sumCripro[5] + sumCripro[6] + sumCripro[7] + sumCripro[8];
        cridem = sumCridem[0] + sumCridem[1] + sumCridem[2] + sumCridem[3] + sumCridem[4] + sumCridem[5] + sumCridem[6] + sumCridem[7] + sumCridem[8];
    }
    //아이템 정리 함수
    public void SortItem()
    {
        for(int i = 0; i < inventorySlots.Count; i++)
        {
            //인벤토리칸 bool값이 true라면
            if (inventorySlotBool[i])
            {
                //값을 주기전에 false로 초기화
                inventorySlotInfos[i].lockBool = false;
                //아이템이 잠겨있다면
                if (inventoryLockBool[i])
                {
                    //임시 잠금 bool값을 true로
                    inventorySlotInfos[i].lockBool = true;
                    //잠금이미지를 꺼준다
                    inventorySlots[i].transform.GetChild(1).gameObject.SetActive(false);
                    //잠금bool값을 false로
                    inventoryLockBool[i] = false;
                }

                //강화 텍스트를 초기화 해준다
                inventorySlots[i].transform.GetChild(2).GetComponent<Text>().text = " ";
                //이미지를 빈 이미지로 바꿔준다
                inventorySlots[i].GetComponent<Image>().sprite = cuberImage[0];
                //개수를 공백으로 처리한다
                inventorySlots[i].transform.GetChild(0).GetComponent<Text>().text = " ";
                //인벤토리 칸 bool값을 false로 바꿔준다
                inventorySlotBool[i] = false;
                //temp리스트에 아이템 정보를 넘겨준다
                temp.Add(new ItemInfo(inventorySlotInfos[i]));
                //인벤토리 칸 정보를 초기화
                inventorySlotInfos[i] = null;
            } 
        }
        //리스트 sort함수를 사용해서 각 아이템정보의 id를 비교해서 순서 정리
        temp.Sort(delegate (ItemInfo A, ItemInfo B)
        {
            if (A.id > B.id) return 1;
            else if (A.id < B.id) return -1;
            return 0;
        });
        for (int i = 0; i < temp.Count; i++)
        {
            //인벤토리 칸 bool값이 false인곳에 temp에 담겨있는 아이템 정보를 넘겨준다
            if (!inventorySlotBool[i])
            {
                //인벤토리 칸 bool값 true로
                inventorySlotBool[i] = true;
                //인벤토리 칸에 temp 정보를 넘겨준다
                inventorySlotInfos[i] = new ItemInfo(temp[i]);
                //인벤토리 칸 이미지를 정보대로 바꿔준다
                inventorySlots[i].GetComponent<Image>().sprite = inventorySlotInfos[i].cuberImage;
                //개수정보가 있다면 띄어준다
                if (inventorySlotInfos[i].count > 0)
                    inventorySlots[i].transform.GetChild(0).GetComponent<Text>().text = inventorySlotInfos[i].count.ToString();
                //강화정보가 있다면 띄어준다
                if (inventorySlotInfos[i].reinForce > 0)
                    inventorySlots[i].transform.GetChild(2).GetComponent<Text>().text = "+" + inventorySlotInfos[i].reinForce.ToString();
                //정보에 잠겨있다고 한다면
                if (inventorySlotInfos[i].lockBool)
                {
                    //잠금이미지를 활성화 해준다
                    inventorySlots[i].transform.GetChild(1).gameObject.SetActive(true);
                    //잠금 bool값을 true 바꾼다
                    inventoryLockBool[i] = true;
                }
            }
        }
        //리스트 정보를 지운다
        temp.Clear();
    }
}

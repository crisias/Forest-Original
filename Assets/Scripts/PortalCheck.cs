using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortalCheck : MonoBehaviour
{
    //이동 버튼에 사용할 이미지(드래그앤 드롭)
    public Sprite portalImg;
    //캐릭터 머리에 버튼 활성화 하기위해서 사용
    string canvasTag;
    Transform canvasTr;
    //현재 포탈 이름
    string portalName;
    //이름마다 포탈위치 설정하기위해
    Transform portalTemp;
    //이동할 포탈 좌표(1.용입, 2.용출 3.개입, 4.개출)(드래그앤 드롭)
    public Transform[] portal = new Transform[4];
    //플레이어 위치를 바꿔주기 위해 위치를 바꿔줄 오브젝트들(프리펩이라 일일히 코드로 연결)
    string playerTag;
    Transform playerTr;
    string petTag;
    Transform petTr;
    string petManagerTag;
    Transform petManagerTr;
    //플레이어 옮길 좌표(프리펩이라 일일히 코드로 연결)
    string stageTag;
    Transform stageTr;
    string field1Tag;
    Transform field1Tr;
    string field2Tag;
    Transform field2Tr;

    void Start()
    {
        playerTag = FindTag.getInstance().player;
        playerTr = Find.getInstance().FindTagTransform(playerTag);

        petTag = FindTag.getInstance().pet;
        petTr = Find.getInstance().FindTagTransform(petTag);

        petManagerTag = FindTag.getInstance().petManager;
        petManagerTr = Find.getInstance().FindTagTransform(petManagerTag);

        canvasTag = FindTag.getInstance().canvas;
        canvasTr = Find.getInstance().FindTagTransform(canvasTag);

        stageTag = FindTag.getInstance().stage;
        stageTr = Find.getInstance().FindTagTransform(stageTag);

        field1Tag = FindTag.getInstance().field1;
        field1Tr = Find.getInstance().FindTagTransform(field1Tag);

        field2Tag = FindTag.getInstance().field2;
        field2Tr = Find.getInstance().FindTagTransform(field2Tag);

        portal[0] = field1Tr.GetChild(1).transform.GetChild(0).transform;
        portal[1] = stageTr.GetChild(1).transform.GetChild(0).transform;
        portal[2] = field2Tr.GetChild(1).transform.GetChild(0).transform;
        portal[3] = stageTr.GetChild(1).transform.GetChild(1).transform;
    }

    void Update()
    {
        //이동 버튼 사용시 진입
        if (PlayerCtrl.portalBool)
        {
            PortalUse();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Portal")
        {
            canvasTr.GetChild(18).gameObject.SetActive(true);
            canvasTr.GetChild(18).GetComponent<Image>().sprite = portalImg;

            if (other.name == "Home-Field1")
            {
                canvasTr.GetChild(18).transform.GetChild(1).GetComponent<Text>().text = "용의 둥지로 이동";
                portalName = "Home-Field1";
            }
            else if (other.name == "Field1-Home")
            {
                canvasTr.GetChild(18).transform.GetChild(1).GetComponent<Text>().text = "마을로 이동";
                portalName = "Field1-Home";
            }
            else if (other.name == "Home-Field2")
            {
                canvasTr.GetChild(18).transform.GetChild(1).GetComponent<Text>().text = "개구리 둥지로 이동";
                portalName = "Home-Field2";
            }
            else if (other.name == "Field2-Home")
            {
                canvasTr.GetChild(18).transform.GetChild(1).GetComponent<Text>().text = "마을로 이동";
                portalName = "Field2-Home";
            }
        }
    }
    //포탈에 근접해서 버튼떴을때 스킬창 혹은 인벤토리창 열면 버튼이 창을 가리는 현상 해결
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Portal")
        {
            if (UiManager.skillUiSwitch || UiManager.inventorySwitch)
                canvasTr.GetChild(18).gameObject.SetActive(false);
            else
                canvasTr.GetChild(18).gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Portal")
        {
            canvasTr.GetChild(18).gameObject.SetActive(false);
        }
    }

    void PortalUse()
    {
        switch (portalName)
        {
            case "Home-Field1":
                portalTemp = portal[0];
                break;
            case "Field1-Home":
                portalTemp = portal[1];
                break;
            case "Home-Field2":
                portalTemp = portal[2];
                break;
            case "Field2-Home":
                portalTemp = portal[3];
                break;
        }

        playerTr.position = new Vector3(portalTemp.position.x, portalTemp.position.y, portalTemp.position.z);
        petTr.position = new Vector3(portalTemp.position.x, portalTemp.position.y, portalTemp.position.z);
        petManagerTr.position = new Vector3(portalTemp.position.x, portalTemp.position.y, portalTemp.position.z);
        PlayerCtrl.portalBool = false;
    }
}

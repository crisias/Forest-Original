using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class SkillInfo
{
    //스킬별로 갖고 있는 정보
    public Sprite SkillIconImage;
    public Sprite SkillInfoImage;
    public string SkillInfoText;

    public SkillInfo(Sprite skillIconImage, Sprite skillInfoImage, string skillInfoText)
    {
        SkillIconImage = skillIconImage;
        SkillInfoImage = skillInfoImage;
        SkillInfoText = skillInfoText;
    }
}

public class SkillWindow : MonoBehaviour
{
    //SkillUi에 접근하기 위해 필요한 tag tr
    string skillUitag;
    Transform skillUiTr;
    //스킬의 버튼 아이콘 이미지를 관리(드래그앤드롭)
    public List<Sprite> skillIconImages = new List<Sprite>();
    //스킬창의 스킬이미지를 관리(드래그앤드롭)
    public List<Sprite> skillInfoImages = new List<Sprite>();
    //스킬 설명창의 15개 image컴퍼넌트를 관리(많아서 for문 사용해서 넣음)
    public List<GameObject> skillInfo = new List<GameObject>();
    //스킬 장착창의 4개 image컴퍼넌트를 관리(드래그앤드롭)
    public List<GameObject> setSkillInfo = new List<GameObject>();
    //버튼 4개 오브젝트(드래그앤드롭)
    public List<GameObject> setSkillSlotInfo = new List<GameObject>();
    //스킬창에서 사용할 각각의 스킬에 정보를 모아 이쪽에서 관리
    public List<SkillInfo> skillInfos = new List<SkillInfo>();
    //스킬슬롯에 저장하기 위한 스킬을 고를때 선택된 상태로 두기위해서
    public GameObject currentSkillInfo;

    bool check;
    //스킬들의 정보(드래그앤 드롭)
    public SkillScriptsTable skillTable;
    //선택된 스킬이 있는지 확인할 값
    bool click;


    void Start()
    {
        //각각의 정보를 받아온다
        skillUitag = FindTag.getInstance().skillUi;
        skillUiTr = Find.getInstance().FindTagTransform(skillUitag);
        //skillInfos에 각각의 정보를 넣어준다
        for (int i = 0; i < skillIconImages.Count; i++)
            skillInfos.Add(new SkillInfo(skillIconImages[i], skillInfoImages[i], skillTable.skillTables[i + 1].skillInfo));
        //skillInfo에 오브젝트를 넣는다
        for (int i = 0; i < skillUiTr.GetChild(0).transform.GetChild(0).childCount; i++)
            skillInfo.Add(skillUiTr.GetChild(0).transform.GetChild(0).transform.GetChild(i).gameObject);
        //스킬창에 현재 skillInfos에 들어 있는 스킬을 등록
        for(int i = 0; i < skillInfos.Count; i++)
        {
            skillInfo[i].transform.GetChild(0).gameObject.SetActive(true);
            skillInfo[i].transform.GetChild(0).GetComponent<Image>().sprite = skillInfos[i].SkillInfoImage;
            skillInfo[i].transform.GetChild(1).GetComponent<Text>().text = skillInfos[i].SkillInfoText;
        }
    }

    void Update()
    {
        //스킬창이 켜져 있고 현재 스킬 정보가 있다면 클릭 효과를 비활성화
        if(!UiManager.skillUiSwitch && currentSkillInfo != null)
            currentSkillInfo.transform.GetChild(2).gameObject.SetActive(false);
        //스킬창이 켜져 있다면
        if(UiManager.skillUiSwitch)
        {
            for (int i = 0; i < setSkillInfo.Count; i++)
            {
                //스킬창 스킬장착 칸에 스킬 이미지가 활성화 됐다면
                if (setSkillInfo[i].transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    //그이미지의 이름이 sparkimg이면 스킬버튼 이미지를 sparkiconimage로 바꿔준다
                    if (setSkillInfo[i].transform.GetChild(0).GetComponent<Image>().sprite.name == "SparkImg")
                        setSkillSlotInfo[i].GetComponent<Image>().sprite = skillIconImages[0];
                    //그이미지의 이름이 healimg이면 스킬버튼 이미지를 sparkiconimage로 바꿔준다
                    else if (setSkillInfo[i].transform.GetChild(0).GetComponent<Image>().sprite.name == "HealImg")
                        setSkillSlotInfo[i].GetComponent<Image>().sprite = skillIconImages[1];
                    //그이미지의 이름이 forcewaveimg이면 스킬버튼 이미지를 sparkiconimage로 바꿔준다
                    else if (setSkillInfo[i].transform.GetChild(0).GetComponent<Image>().sprite.name == "ForcewaveImg")
                        setSkillSlotInfo[i].GetComponent<Image>().sprite = skillIconImages[2];
                    //그이미지의 이름이 lightingimg이면 스킬버튼 이미지를 sparkiconimage로 바꿔준다
                    else if (setSkillInfo[i].transform.GetChild(0).GetComponent<Image>().sprite.name == "LightningImg")
                        setSkillSlotInfo[i].GetComponent<Image>().sprite = skillIconImages[3];
                    //그이미지의 이름이 shiningimg이면 스킬버튼 이미지를 sparkiconimage로 바꿔준다
                    else if (setSkillInfo[i].transform.GetChild(0).GetComponent<Image>().sprite.name == "ShiningImg")
                        setSkillSlotInfo[i].GetComponent<Image>().sprite = skillIconImages[4];
                }
            }
        }
    }

    //스킬창 오른쪽렬 버튼(클릭하면 선택 효과를 주고 다른번튼 사용시 선택 효과 교체)
    public void ClickSwap()
    {
        GameObject currentButton = EventSystem.current.currentSelectedGameObject;
        if(currentButton.transform.GetChild(0).gameObject.activeInHierarchy && currentSkillInfo == null)
        {
            currentButton.transform.GetChild(2).gameObject.SetActive(true);
            currentSkillInfo = currentButton;
            click = true;
        }
        else if(currentSkillInfo != null)
        {
            currentSkillInfo.transform.GetChild(2).gameObject.SetActive(false);
            currentButton.transform.GetChild(2).gameObject.SetActive(true);
            currentSkillInfo = currentButton;
            click = true;
        }
    }

    //스킬창 왼쪽렬 버튼
    public void SkillSet()
    {
        if (currentSkillInfo != null && click)
        {
            for (int i = 0; i < setSkillInfo.Count; i++)
            {
                Debug.Log("check3");
                if (setSkillInfo[i].transform.GetChild(0).gameObject.activeInHierarchy == true)
                {
                    Debug.Log("check2");
                    if (currentSkillInfo.transform.GetChild(0).GetComponent<Image>().sprite.name == setSkillInfo[i].transform.GetChild(0).GetComponent<Image>().sprite.name)
                    {
                        Debug.Log("check");
                        check = true;
                    }
                }
            }

            if (!check && !Skill.button1 && !Skill.button2 && !Skill.button3 && !Skill.button4)
            {
                Debug.Log("test");
                currentSkillInfo.transform.GetChild(2).gameObject.SetActive(false);
                GameObject currentSlot = EventSystem.current.currentSelectedGameObject;
                currentSlot.transform.GetChild(0).gameObject.SetActive(true);
                currentSlot.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = currentSkillInfo.transform.GetChild(0).GetComponent<Image>().sprite;
                currentSkillInfo = null;
                click = false;
            }
            else
            {
                currentSkillInfo.transform.GetChild(2).gameObject.SetActive(false);
                currentSkillInfo = null;
                check = false;
                click = false;
            }
        }
    }
}

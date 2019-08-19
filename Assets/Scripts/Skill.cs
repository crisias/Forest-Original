using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Skill : MonoBehaviour
{
    //player에 접근하기위한 tag tr
    string playerTag;
    Transform playerTr;

    Player playerSc;

    //point 공격 발사체가 생성될 지점
    Transform point;
    //attackButton에 접근하기위한 tag tr image
    string attackButtonTag;
    Transform attackButtonTr;
    Image attackButtonImg;
    //공격쿨타임(나중에 여러 스킬이 공유할수 있게 수정할것)
    float attackWaitTime = 1f;
    //공격 프리펩
    public GameObject attack;
    //여러 클래스와 공유할 bool값
    public static bool boolRotation = false;
    public static bool boolAttack = false;
    //target에 접근하기 위한 tag tr
    string targetTag;
    Transform targetTr;

    Vector3 test;

    public static bool button1;
    public static bool button2;
    public static bool button3;
    public static bool button4;

    float button2WaitTime = 0;
    float button3WaitTime = 0;
    float button4WaitTime = 0;
    float button1WaitTime = 0;

    public SkillScriptsTable skillScriptsTable;

    int currentSkillIndex;
    string spriteName;
    string buttonName;

    SkillTable currentSkill;

    GameObject currentSkillButton;

    string skillButton1Tag;
    Transform skillButton1Tr;
    Image skillButton1Img;

    string skillButton2Tag;
    Transform skillButton2Tr;
    Image skillButton2Img;

    string skillButton3Tag;
    Transform skillButton3Tr;
    Image skillButton3Img;

    string skillButton4Tag;
    Transform skillButton4Tr;
    Image skillButton4Img;

    public static bool skillRote;
    //발사체 파티클(드래그앤 드롭)
    public GameObject skill1;
    public GameObject skill2;
    public GameObject skill3;
    public GameObject skill4;

    void Awake()
    {
        //각각의 정보를 받아온다
        playerTag = FindTag.getInstance().player;
        playerTr = Find.getInstance().FindTagTransform(playerTag);

        playerSc = playerTr.GetComponent<Player>();

        attackButtonTag = FindTag.getInstance().attackButton;
        attackButtonTr = Find.getInstance().FindTagTransform(attackButtonTag);
        attackButtonImg = attackButtonTr.GetChild(0).GetComponent<Image>();

        targetTag = FindTag.getInstance().target;
        targetTr = Find.getInstance().FindTagTransform(targetTag);

        skillButton1Tag = FindTag.getInstance().skillButton1;
        skillButton1Tr = Find.getInstance().FindTagTransform(skillButton1Tag);
        skillButton1Img = skillButton1Tr.GetChild(0).GetComponent<Image>();

        skillButton2Tag = FindTag.getInstance().skillButton2;
        skillButton2Tr = Find.getInstance().FindTagTransform(skillButton2Tag);
        skillButton2Img = skillButton2Tr.GetChild(0).GetComponent<Image>();

        skillButton3Tag = FindTag.getInstance().skillButton3;
        skillButton3Tr = Find.getInstance().FindTagTransform(skillButton3Tag);
        skillButton3Img = skillButton3Tr.GetChild(0).GetComponent<Image>();

        skillButton4Tag = FindTag.getInstance().skillButton4;
        skillButton4Tr = Find.getInstance().FindTagTransform(skillButton4Tag);
        skillButton4Img = skillButton4Tr.GetChild(0).GetComponent<Image>();
    }

    void Start()
    {
    }

    void Update()
    {
        //발사체 생성될 위치 지정
        point = playerTr.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(7).gameObject.transform;

        //공격중일때
        if (boolAttack)
        {
            //쿨타임 이미지 키고 fillAmount로 쿨타임 구현
            attackButtonImg.fillAmount -= 1.0f / attackWaitTime * Time.deltaTime;
            attackButtonTr.GetChild(0).gameObject.SetActive(true);
        }
        //공격중이 아닐때
        else
        {
            //쿨타임 이미지 끈다
            attackButtonTr.GetChild(0).gameObject.SetActive(false);
        }
        if (button1)
        {
            skillButton1Img.fillAmount -= 1.0f / button1WaitTime * Time.deltaTime;
            skillButton1Tr.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            skillButton1Tr.GetChild(0).gameObject.SetActive(false);
        }
        if(button2)
        {
            skillButton2Img.fillAmount -= 1.0f / button2WaitTime * Time.deltaTime;
            skillButton2Tr.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            skillButton2Tr.GetChild(0).gameObject.SetActive(false);
        }
        if(button3)
        {
            skillButton3Img.fillAmount -= 1.0f / button3WaitTime * Time.deltaTime;
            skillButton3Tr.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            skillButton3Tr.GetChild(0).gameObject.SetActive(false);
        }
        if(button4)
        {
            skillButton4Img.fillAmount -= 1.0f / button4WaitTime * Time.deltaTime;
            skillButton4Tr.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            skillButton4Tr.GetChild(0).gameObject.SetActive(false);
        }
    }
    //공격 버튼 함수
    public void Attack()
    {
        if(playerSc.currentPlayerInfo.mp >= skillScriptsTable.skillTables[0].useMp)
        {
            //탑승중이고 공격중이 아닐때
            if (UiManager.ride && !boolAttack)
            {
                //테스트용
                playerSc.currentPlayerInfo.mp -= skillScriptsTable.skillTables[0].useMp;
                boolAttack = true;
                GameObject a = Instantiate(attack);
                //target오브젝트에 자식이 없을때
                if (targetTr.childCount == 0)
                {
                    boolRotation = true;
                    //쿨타임돌리고 발사체 위치를 point로
                    StartCoroutine(attackButtonCooltime(skillScriptsTable.skillTables[0].coolTime));
                    StartCoroutine(attackMotionDelay());
                    a.transform.position = point.position;
                    a.transform.rotation = point.rotation;
                }
                //target오브젝트에 자식이 있을때
                else
                {
                    boolRotation = true;
                    //캐릭터의 rotation을 target을 바라보게 바꾼다
                    Vector3 vec = targetTr.transform.GetChild(0).transform.position - playerTr.position;
                    vec.Normalize();
                    vec.y = 0;
                    Quaternion q = Quaternion.LookRotation(vec);
                    playerTr.rotation = q;
                    //스킬쿨과 모션쿨을 돌리고 발사체 위치를 point로
                    StartCoroutine(attackButtonCooltime(skillScriptsTable.skillTables[0].coolTime));
                    StartCoroutine(attackMotionDelay());
                    a.transform.position = point.position;
                    a.transform.rotation = point.rotation;
                }
                //쿨타임 이미지를 위해 값을 원위치로
                attackButtonImg.fillAmount = 1f;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
            }
        }
    }

    public void SkillUse()
    {
        currentSkillButton = EventSystem.current.currentSelectedGameObject;
        if(currentSkillButton.GetComponent<Image>().sprite.name != "EmptySkill")
        {
            buttonName = currentSkillButton.name;
            spriteName = currentSkillButton.GetComponent<Image>().sprite.name;
            for (int i = 0; i < skillScriptsTable.skillTables.Length; i++)
            {
                if (spriteName == skillScriptsTable.skillTables[i].tag)
                {
                    currentSkillIndex = i;
                }
            }
            CoroutineSelect(buttonName);
        }
    }

    void CoroutineSelect(string gameobjectname)
    {
        switch (gameobjectname)
        {
            case "SkillButton1":
                if (UiManager.ride && !button1)
                {
                    if (!skillRote)
                    {
                        skillRote = true;
                        playerRotation();
                        StartCoroutine(skillMotionDelay());
                    }
                    skillButton1Img.fillAmount = 1;
                    button1 = true;
                    button1WaitTime = skillScriptsTable.skillTables[currentSkillIndex].coolTime;
                    SkillSelect(spriteName);
                    StartCoroutine(Skillbutton1Cooltime(skillScriptsTable.skillTables[currentSkillIndex].coolTime));
                }
                break;
            case "SkillButton2":
                if(UiManager.ride && !button2)
                {
                    if (!skillRote)
                    {
                        skillRote = true;
                        playerRotation();
                        StartCoroutine(skillMotionDelay());
                    }
                    skillButton2Img.fillAmount = 1;
                    button2 = true;
                    button2WaitTime = skillScriptsTable.skillTables[currentSkillIndex].coolTime;
                    SkillSelect(spriteName);
                    StartCoroutine(Skillbutton2Cooltime(skillScriptsTable.skillTables[currentSkillIndex].coolTime));
                }
                break;
            case "SkillButton3":
                if(UiManager.ride && !button3)
                {
                    if (!skillRote)
                    {
                        skillRote = true;
                        playerRotation();
                        StartCoroutine(skillMotionDelay());
                    }
                    skillButton3Img.fillAmount = 1;
                    button3 = true;
                    button3WaitTime = skillScriptsTable.skillTables[currentSkillIndex].coolTime;
                    SkillSelect(spriteName);
                    StartCoroutine(Skillbutton3Cooltime(skillScriptsTable.skillTables[currentSkillIndex].coolTime));
                }
                break;
            case "SkillButton4":
                if(UiManager.ride && !button4)
                {
                    if (!skillRote)
                    {
                        skillRote = true;
                        playerRotation();
                        StartCoroutine(skillMotionDelay());
                    }
                    skillButton4Img.fillAmount = 1;
                    button4 = true;
                    button4WaitTime = skillScriptsTable.skillTables[currentSkillIndex].coolTime;
                    SkillSelect(spriteName);
                    StartCoroutine(Skillbutton4Cooltime(skillScriptsTable.skillTables[currentSkillIndex].coolTime));
                }
                break;
        }
    }

    void SkillSelect(string imgName)
    {
        switch(imgName)
        {
            case "SparkIconImg":
                SkillSpark();
                break;
            case "HealIconImg":
                SkillHeal();
                break;
            case "ForcewaveIconImg":
                SkillForcewave();
                break;
            case "LightningBallIconImg":
                SkillLightningBall();
                break;
            case "ShiningIconImg":
                SkillShining();
                break;
        }
    }

    void SkillSpark()
    {
        if(playerSc.currentPlayerInfo.mp >= skillScriptsTable.skillTables[1].useMp)
        {
            //playerRotation();
            GameObject a = Instantiate(skillScriptsTable.skillTables[1].prefab, point.position, point.rotation);
            playerSc.currentPlayerInfo.mp -= skillScriptsTable.skillTables[1].useMp;
            Debug.Log("Spark");
        }
    }

    void SkillHeal()
    {
        if(playerSc.currentPlayerInfo.mp >= skillScriptsTable.skillTables[2].useMp)
        {
            //test용
            playerSc.currentPlayerInfo.mp -= skillScriptsTable.skillTables[2].useMp;
            //파티클이 재생 되도록 껐다 켜준다
            playerTr.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
            playerTr.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
            //hp 20%회복 될수있게
            if(playerSc.currentPlayerInfo.hp < playerSc.currentMaxStatus.hp)
            {
                playerSc.currentPlayerInfo.hp += ((playerSc.currentMaxStatus.hp / 100) * 20);

                if (playerSc.currentPlayerInfo.hp > playerSc.currentMaxStatus.hp)
                    playerSc.currentPlayerInfo.hp = playerSc.currentMaxStatus.hp;
            }
        }
    }

    void SkillForcewave()
    {
        if(playerSc.currentPlayerInfo.mp >= skillScriptsTable.skillTables[3].useMp)
        {
            //playerRotation();
            GameObject a = Instantiate(skillScriptsTable.skillTables[3].prefab, point.position, point.rotation);
            playerSc.currentPlayerInfo.mp -= skillScriptsTable.skillTables[3].useMp;
            Debug.Log("ForcewaveIconImg");
        }
    }

    void SkillLightningBall()
    {
        if(playerSc.currentPlayerInfo.mp >= skillScriptsTable.skillTables[4].useMp)
        {
            //playerRotation();
            GameObject a = Instantiate(skillScriptsTable.skillTables[4].prefab, point.position, point.rotation);
            playerSc.currentPlayerInfo.mp -= skillScriptsTable.skillTables[4].useMp;
            Debug.Log("LightningBallIconImg");
        }
    }

    void SkillShining()
    {
        if(playerSc.currentPlayerInfo.mp >= skillScriptsTable.skillTables[5].useMp)
        {
            //playerRotation();
            GameObject a = Instantiate(skillScriptsTable.skillTables[5].prefab, point.position, point.rotation);
            playerSc.currentPlayerInfo.mp -= skillScriptsTable.skillTables[5].useMp;
            Debug.Log("ShiningIconImg");
        }
    }

    //attack의 쿨타임
    IEnumerator attackButtonCooltime(float cool)
    {
        while (boolAttack)
        {
            yield return new WaitForSeconds(cool);
            boolAttack = false;
        }
    }

    IEnumerator Skillbutton1Cooltime(float cool)
    {
        while (button1)
        {
            yield return new WaitForSeconds(cool);
            button1 = false;
        }
    }

    IEnumerator Skillbutton2Cooltime(float cool)
    {
        while (button2)
        {
            yield return new WaitForSeconds(cool);
            button2 = false;
        }
    }

    IEnumerator Skillbutton3Cooltime(float cool)
    {
        while (button3)
        {
            yield return new WaitForSeconds(cool);
            button3 = false;
        }
    }

    IEnumerator Skillbutton4Cooltime(float cool)
    {
        while (button4)
        {
            yield return new WaitForSeconds(cool);
            button4 = false;
        }
    }
   
    //attack시의 캐릭터모션을 일정시간후에 attack에서 run으로 바꾸기위해 쓴다
    IEnumerator attackMotionDelay()
    {
        while(boolRotation)
        {
            yield return new WaitForSeconds(0.7f);
            boolRotation = false;
        }
    }

    IEnumerator skillMotionDelay()
    {
        while(skillRote)
        {
            yield return new WaitForSeconds(0.8f);
            skillRote = false;
        }
    }

    void playerRotation()
    {
        if(skillRote && targetTr.childCount != 0)
        {
            Vector3 vec = targetTr.transform.GetChild(0).transform.position - playerTr.position;
            vec.Normalize();
            vec.y = 0;
            Quaternion q = Quaternion.LookRotation(vec);
            playerTr.rotation = q;
        }
    }
}

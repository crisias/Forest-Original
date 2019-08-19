using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiEffect : MonoBehaviour
{
    //칸 마다 위치해 있는 이미지를 접근할곳
    Image[] slotEffects = new Image[10];
    //이펙트 sprite 배열(드래그앤 드롭)
    public Sprite[] grandCross = new Sprite[15];
    //코르틴에서 쓸 현재 이미지 번호
    int index = 0;
    //코르틴에서 접근 제한을 둘 bool값
    bool waitBool;
    //칸마다 이펙트를 실행하게 조절할 bool값
    public bool[] effectCtrlBool = new bool[10];
    //canvas에 접근할 tag, tr;
    string canvasTag;
    Transform canvasTr;
    //shopNpc에 접근할 tag, tr, sc;
    string shopNpcTag;
    Transform shopNpcTr;
    NpcShop npcShopSc;

    void Start()
    {
        //canvas에 접근
        canvasTag = FindTag.getInstance().canvas;
        canvasTr = Find.getInstance().FindTagTransform(canvasTag);
        //shopNpc에 접근
        shopNpcTag = FindTag.getInstance().shopNpc;
        shopNpcTr = Find.getInstance().FindTagTransform(shopNpcTag);
        npcShopSc = shopNpcTr.GetComponent<NpcShop>();
        //칸마다의 image에 접근
        for (int i = 0; i < slotEffects.Length; i++)
            slotEffects[i] = canvasTr.GetChild(17).transform.GetChild(1).transform.GetChild(i).gameObject.GetComponent<Image>();
    }

    void Update()
    {
        //랜덤 뽑기 창이 활성화 됐다면
        if (canvasTr.GetChild(16).transform.GetChild(2).transform.GetChild(4).transform.GetChild(1).gameObject.activeInHierarchy)
        {
            //1~10칸의 bool값이 false고 waitbool이 false라면 진입가능
            if (!effectCtrlBool[0] && !waitBool)
                StartCoroutine(EffectWait(0));
            else if (!effectCtrlBool[1] && !waitBool)
                StartCoroutine(EffectWait(1));
            else if (!effectCtrlBool[2] && !waitBool)
                StartCoroutine(EffectWait(2));
            else if (!effectCtrlBool[3] && !waitBool)
                StartCoroutine(EffectWait(3));
            else if (!effectCtrlBool[4] && !waitBool)
                StartCoroutine(EffectWait(4));
            else if (!effectCtrlBool[5] && !waitBool)
                StartCoroutine(EffectWait(5));
            else if (!effectCtrlBool[6] && !waitBool)
                StartCoroutine(EffectWait(6));
            else if (!effectCtrlBool[7] && !waitBool)
                StartCoroutine(EffectWait(7));
            else if (!effectCtrlBool[8] && !waitBool)
                StartCoroutine(EffectWait(8));
            else if (!effectCtrlBool[9] && !waitBool)
                StartCoroutine(EffectWait(9));
        }
    }
    //이펙트 스프라이트를 일정시간 간격으로 재생하게할 코루틴
    IEnumerator EffectWait(int num)
    {
        //일정시간 동안 재진입이 안돼도록 true로 한다
        waitBool = true;
        yield return new WaitForSeconds(0.05f);
        //현재 칸의 이미지를 활성화 한다
        canvasTr.GetChild(17).transform.GetChild(1).transform.GetChild(num).gameObject.SetActive(true);
        //이미지를 현재 번호의 이펙트 이미지로 변경한다
        slotEffects[num].sprite = grandCross[index];
        //재진입이 가능하도록 false로 한다
        waitBool = false;
        //이펙트 이미지 번호를 올린다
        index++;
        //이펙트 이미지 번호가 최대 이펙트 이미지 번호보다 높다면
        if (index > 14)
        {
            //이펙트 이미지 번호 초기화
            index = 0;
            //현재칸에 다시 진입하지 못하도록 true로 바꾼다
            effectCtrlBool[num] = true;
            //현재칸의 slotcuber를 비활성화해서 아이템 보여준다
            canvasTr.GetChild(17).transform.GetChild(0).transform.GetChild(num).gameObject.SetActive(false);
            //현재칸의 이미지를 비활성화 한다
            canvasTr.GetChild(17).transform.GetChild(1).transform.GetChild(num).gameObject.SetActive(false);
        }
    }
}

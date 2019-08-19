using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    //player 주위에있는 오브젝트를 trigger를 사용해서 검출된 오브젝트를 담을 리스트
    public List<GameObject> targets = new List<GameObject>();

    //타겟팅된 객체의 outline을 생성 시켜줄 쉐이더(드래그앤드롭) (이전꺼)
    //public Shader bDragonOutLine;

    //player주위에있는 객체중 타겟팅된 객체를 지정하기위한 int
    int index;

    //target태그를 받아서 targetTr을 잡을때 쓴다
    string targetTag;
    Transform targetTr;

    //아이콘 클릭 효과를 주기 위해 사용할 sprits를 보관할곳
    public List<Sprite> targetIconSprites = new List<Sprite>();

    //TargetButton tag를 받아서 Tr과 이미지를 받을곳
    string targetButtonTag;
    Transform targetButtonTr;
    Image targetButtonImg;

    string bDragonHiveTag;
    Transform bDragonHiveTr;

    string frogHiveTag;
    Transform frogHiveTr;

    void Awake()
    {
        //Target tag로 transform에 접근한다
        targetTag = FindTag.getInstance().target;
        targetTr = Find.getInstance().FindTagTransform(targetTag);

        //tag받아와서 Tr과 이미지에 접근한다
        targetButtonTag = FindTag.getInstance().targetButton;
        targetButtonTr = Find.getInstance().FindTagTransform(targetButtonTag);
        targetButtonImg = targetButtonTr.GetComponent<Image>();

        bDragonHiveTag = FindTag.getInstance().bDragonHive;
        bDragonHiveTr = Find.getInstance().FindTagTransform(bDragonHiveTag);

        frogHiveTag = FindTag.getInstance().frogHive;
        frogHiveTr = Find.getInstance().FindTagTransform(frogHiveTag);
    }

    void Start()
    {
        //targetbutton 이미지의 초기 설정
        targetButtonImg.sprite = targetIconSprites[0];
    }

    void Update()
    {
        //targets리스트에 오브젝트가 없고 target오브젝트의 자식이 있을때
        if (targets.Count == 0 && targetTr.childCount != 0)
        {
            //수정전
            //targetTr.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1].shader = null;
            //수정후
            targetTr.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.SetFloat("_OutlineWidth", 1.0f);
            targetTr.GetChild(0).transform.SetParent(bDragonHiveTr);
            targetButtonImg.sprite = targetIconSprites[0];
        }

        for (int i = 0; i < targets.Count; i++)
        {
            if (!targets[i].gameObject.activeInHierarchy)
                targets.RemoveAt(i);
        }

        if (targetTr.childCount == 0)
        {
            targetButtonImg.sprite = targetIconSprites[0];
        }
    }

    public void TargetSet()
    {
        if (UiManager.ride)
        {
            //target오브젝트의 자식이 없을때
            if (targetTr.childCount == 0)
                targetButtonImg.sprite = targetIconSprites[1];
            //target오브젝트의 자식이 있을때
            else
                targetButtonImg.sprite = targetIconSprites[3];

            //targets(내주변의 오브젝트를 탐지해서 리스트에 넣는다)리스트에 오브젝트가 들어있을때
            if (targets.Count != 0)
            {
                float dist1 = 10000f;
                float dist2;

                //targets에 들어있는 오브젝트 수만큼 반복 하면서 dist1과 dist2를 비교하고 dist1에 적은수를 저장하고 그순번의 번호를 index에 저장한다(index로 가장가까운 오브젝트로 접근)
                for (int i = 0; i < targets.Count; i++)
                {
                    dist2 = Vector3.Distance(transform.position, targets[i].transform.position);
                    if (dist2 <= dist1)
                    {
                        dist1 = dist2;
                        index = i;
                    }
                }
                //target의 자식이 없을때 (타겟팅된 객체가 없을때)
                if (targetTr.childCount == 0)
                {
                    //수정전
                    //targets[index].gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1].shader = bDragonOutLine;
                    //수정후
                    targets[index].gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.SetFloat("_OutlineWidth", 1.05f);
                    targets[index].transform.SetParent(targetTr);
                }
                //target의 자식이 있을때 (타겟팅된 객체가 있을때)
                else
                {
                    //수정전
                    //targetTr.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1].shader = null;
                    //수정후
                    targetTr.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.SetFloat("_OutlineWidth", 1.0f);
                    targetTr.GetChild(0).transform.SetParent(null);
                    //수정전
                    //targets[index].gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().materials[1].shader = bDragonOutLine;
                    //수정후
                    targets[index].gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.SetFloat("_OutlineWidth", 1.05f);
                    targets[index].transform.SetParent(targetTr);
                }
            }
            //target의 자식이 없으면 검은 이미지로
            if (targetTr.childCount == 0)
                targetButtonImg.sprite = targetIconSprites[0];
            //target의 자식이 있으면 빨간 이미지로
            else
                targetButtonImg.sprite = targetIconSprites[2];
        }
    }

    //Trigger를 이용해서 targets리스트에 오브젝트를 담는다
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "BDragon")
            targets.Add(coll.gameObject);
        if (coll.gameObject.tag == "Frog")
            targets.Add(coll.gameObject);
    }
    //Trigger를 이용해서 targets리스트에서 오브젝트를 뺀다
    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "BDragon")
            targets.Remove(coll.gameObject);
        if (coll.gameObject.tag == "Frog")
            targets.Remove(coll.gameObject);
    }
}

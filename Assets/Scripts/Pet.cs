using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    //현재 펫 정보를 받을곳
    public PetInfo petInfo;

    //petManager에 접근하기 위해 필요한 Tag Tr PetManager
    string petManagerTag;
    Transform petManagerTr;
    PetManager petManagerInfo;

    void Awake()
    {
        //tag Tr 잡아서 PetManager에 접근
        petManagerTag = FindTag.getInstance().petManager;
        petManagerTr = Find.getInstance().FindTagTransform(petManagerTag);
        petManagerInfo = petManagerTr.GetComponent<PetManager>();
    }

    void Start()
    {
        //현재 펫 정보를 받는다
        petInfo = petManagerInfo.currentPet;
        //현재 펫을 생성
        GameObject p = Instantiate(petInfo.Kind);
        //부모를 지정하고 position을 잡은뒤 오브젝트 꺼둔다
        p.transform.SetParent(transform);
        p.transform.position = petManagerTr.position;
        p.SetActive(false);
    }

    void Update()
    {
        
    }
}

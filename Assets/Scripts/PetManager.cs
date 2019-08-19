using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PetInfo
{
    //펫 종류 프리펩
    public GameObject Kind;
    //펫 속도
    public int Speed;
    //펫 가격
    public int Price;

    public PetInfo(GameObject kind, int speed, int price)
    {
        Kind = kind;
        Speed = speed;
        Price = price;
    }
}

public class PetManager : MonoBehaviour
{
    //펫 프리펩들 드래그앤드롭으로 연결
    public GameObject dog;
    public GameObject fox;
    public GameObject bear;
    public GameObject panda;
    //펫 종류대로 정보 지정해주고 보관할곳
    public List<PetInfo> petKind = new List<PetInfo>();
    //현재 펫의 정보
    public PetInfo currentPet;

    void Awake()
    {
        //펫 종류대로 정보를 지정해준다
        petKind.Add(new PetInfo(dog, 3, 0));
        petKind.Add(new PetInfo(fox, 5, 10));
        petKind.Add(new PetInfo(panda, 7, 20));
        petKind.Add(new PetInfo(bear, 10, 30));
        //현재 펫을 지정한다
        currentPet = petKind[0];
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Find : MonoBehaviour
{
    //자신의 클래스를 담을 instance를 선언
    private static Find instance;
    public static Find getInstance()
    {
        //instance가 비었다면
        if(!instance)
        {
            //FindObjectOfType로 찾아서 넣는다
            instance = FindObjectOfType(typeof(Find)) as Find;
            //그래도 비었다면
            if(!instance)
            {
                //GameObject를 하나 만들고 컴퍼넌트로 추가한다
                GameObject a = new GameObject("Find");
                instance = a.AddComponent<Find>();
            }   
        }
        return instance;
    }

    //string으로 원하는 tag를 달고있는 Object에 접근해줄 함수
    public Transform FindTagTransform(string name)
    {
        return GameObject.FindGameObjectWithTag(name).GetComponent<Transform>();
    }
}

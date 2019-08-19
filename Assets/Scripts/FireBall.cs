using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    //player tag에 접근하기 위한 string과 Tr
    string playerTag;
    Transform playerTr;

    Player player;

    string targetTag;
    Transform targetTr;

    //거리계산을 위한 float
    float dist;

    void Start()
    {
        //player tag에 접근하여 player tr 정보를 가져온다
        playerTag = FindTag.getInstance().player;
        playerTr = Find.getInstance().FindTagTransform(playerTag);

        player = playerTr.GetComponent<Player>();

        targetTag = FindTag.getInstance().target;
        targetTr = Find.getInstance().FindTagTransform(targetTag);
    }

    void Update()
    {
        //생성된 위치에서 앞쪽으로 날아간다
        transform.position += transform.forward * Time.deltaTime * 20f;

        //현재 fireball과 player사이의 거리계산
        dist = Vector3.Distance(transform.position, playerTr.position);

        if(dist >= 30)
        {
            //일단 destroy로 처리한다
            Destroy(gameObject);
        }
    }

    //void OnCollisionEnter()
    //{
    //    Destroy(gameObject);
    //}
}

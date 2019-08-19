using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogLeg : MonoBehaviour
{
    //플레이어 정보
    string playerTag;
    Transform playerTr;
    Player playerSc;
    //몬스터 정보
    string frogTag;
    Transform frogTr;
    Frog frogSc;

    void Start()
    {
        playerTag = FindTag.getInstance().player;
        playerTr = Find.getInstance().FindTagTransform(playerTag);
        playerSc = playerTr.GetComponent<Player>();

        frogTag = FindTag.getInstance().frog;
        frogTr = Find.getInstance().FindTagTransform(frogTag);
        frogSc = frogTr.GetComponent<Frog>();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "damagePoint")
        {
            Debug.Log("atk");
            int atk = (frogSc.currentInfo.Atk/4) - playerSc.currentPlayerInfo.def;
            if(atk > 0)
            {
                playerSc.currentPlayerInfo.hp -= atk;
            }
        }
    }
}

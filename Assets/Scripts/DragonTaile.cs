using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonTaile : MonoBehaviour
{
    string playerTag;
    Transform playerTr;
    Player playerSc;

    public BDragon bDragonSc;
    
    void Start()
    {
        playerTag = FindTag.getInstance().player;
        playerTr = Find.getInstance().FindTagTransform(playerTag);
        playerSc = playerTr.GetComponent<Player>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "damagePoint")
        {
            int damage = bDragonSc.currentInfo.Atk - playerSc.currentPlayerInfo.def;
            if (damage > 0)
            {
                playerSc.currentPlayerInfo.hp -= damage;
            }
            //Debug.Log(damage);
            //Debug.Log(playerSc.currentPlayerInfo.hp);
        }
    }
}

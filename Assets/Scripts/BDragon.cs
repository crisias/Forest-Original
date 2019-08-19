using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BDragon : MonoBehaviour
{
    string playerTag;
    Transform playerTr;
    Player playerSc;

    public GameObject target;

    public MonsterInfo currentInfo;

    string monsterManagerTag;
    Transform monsterManagerTr;
    MonsterManager monsterManagerSc;

    MonsterCtrl monsterCtrl;

    GameObject fenceFL;
    GameObject fenceBR;

    float patrolDist = 20;
    float traceDist = 21;
    float attackDist = 7;
    float speed = 5;
    float stop = 5;

    public bool dragonDead;

    string bDragonDeadTag;
    Transform bDragonDeadTr;

    string bDragonHiveTag;
    Transform bDragonHiveTr;

    public Animation ani;

    public float atk;

    int expCount = 1;

    string databaseTag;
    Transform databaseTr;
    ItemManager itemManagerSc;

    public int rand;

    public int Nope;

    void Awake()
    {
        playerTag = FindTag.getInstance().player;
        playerTr = Find.getInstance().FindTagTransform(playerTag);
        playerSc = playerTr.GetComponent<Player>();
    }

    void Start()
    {
        fenceFL = GameObject.FindGameObjectWithTag("FenceFL");
        fenceBR = GameObject.FindGameObjectWithTag("FenceBR");

        target = playerTr.gameObject;

        monsterManagerTag = FindTag.getInstance().monsterManager;
        monsterManagerTr = Find.getInstance().FindTagTransform(monsterManagerTag);
        monsterManagerSc = monsterManagerTr.GetComponent<MonsterManager>();

        monsterCtrl = transform.GetComponent<MonsterCtrl>();

        currentInfo = new MonsterInfo(monsterManagerSc.monsterInfos[0].Name, monsterManagerSc.monsterInfos[0].Hp, monsterManagerSc.monsterInfos[0].Atk, monsterManagerSc.monsterInfos[0].Def, monsterManagerSc.monsterInfos[0].Dex, monsterManagerSc.monsterInfos[0].Exp, monsterManagerSc.monsterInfos[0].Prefab, monsterManagerSc.monsterInfos[0].MonsterAni);

        bDragonDeadTag = FindTag.getInstance().bDragonDead;
        bDragonDeadTr = Find.getInstance().FindTagTransform(bDragonDeadTag);

        bDragonHiveTag = FindTag.getInstance().bDragonHive;
        bDragonHiveTr = Find.getInstance().FindTagTransform(bDragonHiveTag);

        ani = transform.GetChild(0).GetComponent<Animation>();

        databaseTag = FindTag.getInstance().dataBase;
        databaseTr = Find.getInstance().FindTagTransform(databaseTag);
        itemManagerSc = databaseTr.GetComponent<ItemManager>();
    }

    void Update()
    {
        if (!dragonDead)
        {
            monsterCtrl.Move(target, patrolDist, traceDist, attackDist, stop, fenceBR, fenceFL);
        }
        if (currentInfo.Hp <= 0)
        {
            Dead();
        }
        dragonAni();
    }

    void Dead()
    {
        if (expCount == 1)
        {
            monsterCtrl.expUp(currentInfo.Exp);
            expCount -= 1;
        }
        dragonDead = true;
        monsterCtrl.Dead(bDragonDeadTr.gameObject);

        rand = Random.Range(32, 46);

        Nope++;

        if (Nope == 1)
        {
            itemManagerSc.ItemAdd(rand);
        }
    }

    void dragonAni()
    {
        if(!PlayerCtrl.playerDead)
        {
            if (monsterCtrl.attack && !monsterCtrl.hurt && !dragonDead)
            {
                ani.CrossFade("DragonAttack", 0.1f);
            }
            else if (monsterCtrl.trace && !monsterCtrl.hurt && !dragonDead)
            {
                ani.CrossFade("DragonRun", 0.1f);
            }
            else if (monsterCtrl.patrol && !monsterCtrl.hurt && !dragonDead)
            {
                ani.CrossFade("DragonRun", 0.1f);
            }
            else if (dragonDead)
            {
                ani.CrossFade("DragonDie", 0.1f);
            }
        }
        else
        {
            ani.CrossFade("DragonRun", 0.1f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
        int rand = Random.Range(0, 100);

        int cri = Random.Range(0, 100);

        atk = 0;

        if (collision.gameObject.tag == "FireBall")
        {
            gameObject.transform.GetChild(5).gameObject.SetActive(false);
            gameObject.transform.GetChild(5).gameObject.SetActive(true);
            if (rand > currentInfo.Dex)
            {
                if (cri < playerSc.currentPlayerInfo.cripro)
                {
                    atk = (playerSc.currentPlayerInfo.str + itemManagerSc.str) + ((playerSc.currentPlayerInfo.cridem / 100) * playerSc.currentPlayerInfo.str) - currentInfo.Def;
                    CollisionEffect(collision);
                }
                else
                {
                    atk = playerSc.currentPlayerInfo.str + itemManagerSc.str - currentInfo.Def;
                    CollisionEffect(collision);
                }
            }
            else
                Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Spark")
        {
            gameObject.transform.GetChild(3).gameObject.SetActive(false);
            gameObject.transform.GetChild(3).gameObject.SetActive(true);
            if (rand > currentInfo.Dex)
            {
                if (cri < playerSc.currentPlayerInfo.cripro)
                {
                    atk = (playerSc.currentPlayerInfo.str + itemManagerSc.str) * 2 + ((playerSc.currentPlayerInfo.cridem / 100) * playerSc.currentPlayerInfo.str) - currentInfo.Def;
                    CollisionEffect(collision);
                }
                else
                {
                    atk = (playerSc.currentPlayerInfo.str + itemManagerSc.str) * 2 - currentInfo.Def;
                    CollisionEffect(collision);
                }
            }
            else
                Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Forcewave")
        {
            Debug.Log("forceeffect");
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
            gameObject.transform.GetChild(1).gameObject.SetActive(true);

            if (cri < playerSc.currentPlayerInfo.cripro)
            {
                atk = (playerSc.currentPlayerInfo.str + itemManagerSc.str) / 2 + ((playerSc.currentPlayerInfo.cridem / 100) * playerSc.currentPlayerInfo.str) - currentInfo.Def;
                CollisionEffect(collision);
            }
            else
            {
                atk = (playerSc.currentPlayerInfo.str + itemManagerSc.str) / 2 - currentInfo.Def;
                CollisionEffect(collision);
            }
        }
        else if (collision.gameObject.tag == "Lightning")
        {
            gameObject.transform.GetChild(2).gameObject.SetActive(false);
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
            if (rand > currentInfo.Dex)
            {
                if (cri < playerSc.currentPlayerInfo.cripro)
                {
                    atk = playerSc.currentPlayerInfo.str + itemManagerSc.str + ((playerSc.currentPlayerInfo.cridem / 100) * playerSc.currentPlayerInfo.str);
                    CollisionEffect(collision);
                }
                else
                {
                    atk = playerSc.currentPlayerInfo.str + itemManagerSc.str;
                    CollisionEffect(collision);
                }
            }
            else
                Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Shining")
        {
            gameObject.transform.GetChild(4).gameObject.SetActive(false);
            gameObject.transform.GetChild(4).gameObject.SetActive(true);
            if (rand > currentInfo.Dex)
            {
                if (cri < playerSc.currentPlayerInfo.cripro + 5)
                {
                    atk = (playerSc.currentPlayerInfo.str + itemManagerSc.str) * 1.5f + ((playerSc.currentPlayerInfo.cridem / 100) * playerSc.currentPlayerInfo.str) - currentInfo.Def;
                    CollisionEffect(collision);
                }
                else
                {
                    atk = (playerSc.currentPlayerInfo.str + itemManagerSc.str) * 1.5f - currentInfo.Def;
                    CollisionEffect(collision);
                }
            }
            else
                Destroy(collision.gameObject);
        }
    }

    void CollisionEffect(Collision coll)
    {
        Destroy(coll.gameObject);
        ani.CrossFade("DragonHurt", 0.1f);
        monsterCtrl.Hurt(target);
        currentInfo.Hp -= (int)atk;
    }
}

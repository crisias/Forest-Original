using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Frog : MonoBehaviour
{
    //플레이어 오브젝트에 접근하기 위해
    string playerTag;
    Transform playerTr;
    Player playerSc;
    //타겟에 오브젝트에 접근하기 위해
    public GameObject target;
    //현재 몬스터에 대한 정보
    public MonsterInfo currentInfo;
    //monsterManager 스크립트에 접근하기위해
    string monsterManagerTag;
    Transform monsterManagerTr;
    MonsterManager monsterManagerSc;
    //monsterCtrl 스크립트에 접근하기위해
    MonsterCtrl monsterCtrl;
    //frog의 한정 이동 공간
    GameObject frogFenceFL;
    GameObject frogFenceBR;
    //몬스터의 기본 거리 및 속도 제동거리 설정
    float patrolDist = 20;
    float traceDist = 21;
    float attackDist = 7;
    float speed = 5;
    float stop = 5;
    //몬스터의 죽음을 확인할 bool값
    public bool frogDead;
    //몬스터 모아놓을 곳
    string frogHiveTag;
    Transform frogHiveTr;
    //애니메이션 연결
    public Animation ani;
    //연산된 데미지 결과값을 담을곳
    public float atk;
    //경험치 함수 제어
    int expCount = 1;
    //itemManager 스크립트에 접근하기위해
    string databaseTag;
    Transform databaseTr;
    ItemManager itemManagerSc;
    //아이템 드랍할 랜덤 값
    public int rand;
    //item add 함수 제어
    public int Nope;

    void Awake()
    {
        //플레이어 오브젝트에 접근
        playerTag = FindTag.getInstance().player;
        playerTr = Find.getInstance().FindTagTransform(playerTag);
        playerSc = playerTr.GetComponent<Player>();
    }

    void Start()
    {
        //몬스터 이동 제한줄 펜스 접근
        frogFenceFL = GameObject.FindGameObjectWithTag("FrogFenceFL");
        frogFenceBR = GameObject.FindGameObjectWithTag("FrogFenceBR");
        //플레이어(타겟)에 접근
        target = playerTr.gameObject;
        //monsterManager 스크립트에 접근
        monsterManagerTag = FindTag.getInstance().monsterManager;
        monsterManagerTr = Find.getInstance().FindTagTransform(monsterManagerTag);
        monsterManagerSc = monsterManagerTr.GetComponent<MonsterManager>();
        //monsterCtrl 스크립트에 접근
        monsterCtrl = transform.GetComponent<MonsterCtrl>();
        //현재 몬스터의 정보
        currentInfo = new MonsterInfo(monsterManagerSc.monsterInfos[1].Name, monsterManagerSc.monsterInfos[1].Hp, monsterManagerSc.monsterInfos[1].Atk, monsterManagerSc.monsterInfos[1].Def, monsterManagerSc.monsterInfos[1].Dex, monsterManagerSc.monsterInfos[1].Exp, monsterManagerSc.monsterInfos[1].Prefab, monsterManagerSc.monsterInfos[1].MonsterAni);
        //몬스터 모아놓을 곳 접근
        frogHiveTag = FindTag.getInstance().frogHive;
        frogHiveTr = Find.getInstance().FindTagTransform(frogHiveTag);
        //애니메이션 제어 하기위해 접근
        ani = transform.GetChild(0).GetComponent<Animation>();
        //아이템 정보에 접근하기위해 접근
        databaseTag = FindTag.getInstance().dataBase;
        databaseTr = Find.getInstance().FindTagTransform(databaseTag);
        itemManagerSc = databaseTr.GetComponent<ItemManager>();
    }

    void Update()
    {
        //몬스터가 죽지않았다면
        if (!frogDead)
        {
            //이동 함수 계속 실행
            monsterCtrl.Move(target, patrolDist, traceDist, attackDist, stop, frogFenceBR, frogFenceFL);
        }
        //몬스터의 체력이 0이거나 작다면
        if (currentInfo.Hp <= 0)
        {
            //죽음 상태로
            Dead();
        }
        //애니메이션 제어 함수
        FrogAni();
    }
    //죽으면 실행되는 함수
    void Dead()
    {
        if (expCount == 1)
        {
            //현재 몬스터 정보의 경험치 만큼 오르게 한다
            monsterCtrl.expUp(currentInfo.Exp);
            //다시 실행되지 않게 1빼준다
            expCount -= 1;
        }
        //몬스터 죽음 상태 true로
        frogDead = true;
        //monsterCtrl 죽음 함수 실행
        monsterCtrl.Dead(frogHiveTr.gameObject);
        //랜덤 아이템 생성을 위한 랜덤값 생성
        rand = Random.Range(32, 46);
        //한번만 실행되게 제어한다
        Nope++;
        if (Nope == 1)
        {
            //아이템을 추가한다
            itemManagerSc.ItemAdd(rand);
        }
    }
    //애니메이션 제어 함수
    void FrogAni()
    {
        //플레이어가 죽지않았다면
        if(!PlayerCtrl.playerDead)
        {
            //피격이나 죽지않고 공격중일때
            if (monsterCtrl.attack && !monsterCtrl.hurt && !frogDead)
            {
                ani.CrossFade("Attack2", 0.1f);
            }
            //피격이나 죽지않고 추적중일때
            else if (monsterCtrl.trace && !monsterCtrl.hurt && !frogDead)
            {
                ani.CrossFade("Run", 0.1f);
            }
            //피격이나 죽지않고 순찰중일때
            else if (monsterCtrl.patrol && !monsterCtrl.hurt && !frogDead)
            {
                ani.CrossFade("Run", 0.1f);
            }
            //죽었을때
            else if (frogDead)
            {
                ani.CrossFade("Death", 0.1f);
            }
        }
        //플레이어가 죽었다면
        else
        {
            ani.CrossFade("Run", 0.1f);
        }
    }
    //충돌처리 진입
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
        //희피 랜덤값
        int rand = Random.Range(0, 100);
        //크리티컬 랜덤값
        int cri = Random.Range(0, 100);
        //저장된 데미지 초기화
        atk = 0;
        //충돌체가 파이어볼일때(tag별로 데미지 연산 처리 다르게 돌아간다)
        if (collision.gameObject.tag == "FireBall")
        {
            //스킬 피격 이펙트 처리
            gameObject.transform.GetChild(5).gameObject.SetActive(false);
            gameObject.transform.GetChild(5).gameObject.SetActive(true);
            //희피 실패했을때
            if (rand > currentInfo.Dex)
            {
                //치명타 떴을때
                if (cri < playerSc.currentPlayerInfo.cripro)
                {
                    //데미지연산(캐릭터 공격력 + 장비 공격력)+(캐릭터크리티컬확률/100 * 캐릭터데미지)-(몬스터방어력)
                    atk = (playerSc.currentPlayerInfo.str + itemManagerSc.str) + ((playerSc.currentPlayerInfo.cridem / 100) * playerSc.currentPlayerInfo.str) - currentInfo.Def;
                    CollisionEffect(collision);
                }
                //치명타 안떴을때
                else
                {
                    //데미지연산(기본 공격력 만큼 데미지)
                    atk = playerSc.currentPlayerInfo.str + itemManagerSc.str - currentInfo.Def;
                    CollisionEffect(collision);
                }
            }
            //희피 했을때 스킬 발사체 삭제
            else
                Destroy(collision.gameObject);
        }
        //스킬이 Spark일때
        else if (collision.gameObject.tag == "Spark")
        {
            //스킬 피격 이펙트
            gameObject.transform.GetChild(3).gameObject.SetActive(false);
            gameObject.transform.GetChild(3).gameObject.SetActive(true);
            //회피 실패했을때
            if (rand > currentInfo.Dex)
            {
                //크리티컬 떴을때
                if (cri < playerSc.currentPlayerInfo.cripro)
                {
                    //데미지연산(캐릭터공격력+장비공격력) * 2 + ((플레이어크리확률/100)*플레이어공격력) - 몬스터방어력
                    atk = (playerSc.currentPlayerInfo.str + itemManagerSc.str) * 2 + ((playerSc.currentPlayerInfo.cridem / 100) * playerSc.currentPlayerInfo.str) - currentInfo.Def;
                    CollisionEffect(collision);
                }
                //크리티컬 안떴을때
                else
                {
                    //기본공격력의 2배 데미지
                    atk = (playerSc.currentPlayerInfo.str + itemManagerSc.str) * 2 - currentInfo.Def;
                    CollisionEffect(collision);
                }
            }
            //회피 성공시 발사체 삭제
            else
                Destroy(collision.gameObject);
        }
        //피격 스킬이 forcewave일때
        else if (collision.gameObject.tag == "Forcewave")
        {
            //스킬 피격 이펙트
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
            //크리티컬 떴을때
            if (cri < playerSc.currentPlayerInfo.cripro)
            {
                //공격데미지연산(캐릭터공격력+장비공격력) / 2 + ((플레이어클리티컬확률/100)*플레이어공격력) - 몬스터방어력
                atk = (playerSc.currentPlayerInfo.str + itemManagerSc.str) / 2 + ((playerSc.currentPlayerInfo.cridem / 100) * playerSc.currentPlayerInfo.str) - currentInfo.Def;
                CollisionEffect(collision);
            }
            //크리티컬 안떴을때
            else
            {
                //회피무시 대신 공격력 반절
                atk = (playerSc.currentPlayerInfo.str + itemManagerSc.str) / 2 - currentInfo.Def;
                CollisionEffect(collision);
            }
        }
        //피격 스킬이 lightning일때
        else if (collision.gameObject.tag == "Lightning")
        {
            //피격 스킬 이펙트
            gameObject.transform.GetChild(2).gameObject.SetActive(false);
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
            //회피 실패시
            if (rand > currentInfo.Dex)
            {
                //크리티컬 떴을때
                if (cri < playerSc.currentPlayerInfo.cripro)
                {
                    //공격데미지연산(플레이어공격력+장비공격력)+((플레이어크리티컬확률/100)*플레이어공격력);
                    atk = playerSc.currentPlayerInfo.str + itemManagerSc.str + ((playerSc.currentPlayerInfo.cridem / 100) * playerSc.currentPlayerInfo.str);
                    CollisionEffect(collision);
                }
                //크리티컬 안떴을때
                else
                {
                    //공격방어무시
                    atk = playerSc.currentPlayerInfo.str + itemManagerSc.str;
                    CollisionEffect(collision);
                }
            }
            //회피성공시 스킬 충돌체 삭제
            else
                Destroy(collision.gameObject);
        }
        //사용 스킬이 shining일때 
        else if (collision.gameObject.tag == "Shining")
        {
            //스킬 피격 이펙트
            gameObject.transform.GetChild(4).gameObject.SetActive(false);
            gameObject.transform.GetChild(4).gameObject.SetActive(true);
            //회피 실패시
            if (rand > currentInfo.Dex)
            {
                //크리티컬 떴을때 범위 +5 확률 증가
                if (cri < playerSc.currentPlayerInfo.cripro + 5)
                {
                    //공격데미지연산 (캐릭터공격력+장비공격력) * 1.5f + ((캐릭터치명타확률/100)*캐릭터공격력) - 몬스터방어력
                    atk = (playerSc.currentPlayerInfo.str + itemManagerSc.str) * 1.5f + ((playerSc.currentPlayerInfo.cridem / 100) * playerSc.currentPlayerInfo.str) - currentInfo.Def;
                    CollisionEffect(collision);
                }
                //크리티컬 안떴을때
                else
                {
                    atk = (playerSc.currentPlayerInfo.str + itemManagerSc.str) * 1.5f - currentInfo.Def;
                    CollisionEffect(collision);
                }
            }
            //회피 성공시 삭제
            else
                Destroy(collision.gameObject);
        }
    }
    //피격 함수
    void CollisionEffect(Collision coll)
    {
        //스킬 충돌체 지워준다
        Destroy(coll.gameObject);
        //몬스터 피격 모션
        ani.CrossFade("Take Damage1", 0.1f);
        //피격후 wait
        monsterCtrl.Hurt(target);
        //몬스터 hp 감소
        currentInfo.Hp -= (int)atk;
    }
}

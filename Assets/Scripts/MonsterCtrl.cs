using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    //캐릭터의 navemeshagent에 접근
    NavMeshAgent navAgen;
    //순찰상태 bool
    public bool patrol;
    //공격상태 bool
    public bool attack;
    //공격상태 bool
    public bool hit;
    //추격상태 bool
    public bool trace;
    //피격상태 bool
    public bool hurt;
    //추적에서 순찰로 변경시 돌아올 포인트
    Vector3 returnPoint;
    //이동할 포인트
    Vector3 movePoint;
    //죽었는지 확인
    bool dead;
    //타겟으로 쓸 플레이어 오브젝트 접근
    string playerTag;
    Transform playerTr;
    Player playerSc;
    
    void Start()
    {
        //navmeshagent 접근
        navAgen = transform.GetComponent<NavMeshAgent>();
        //플레이어 오브젝트 접근
        playerTag = FindTag.getInstance().player;
        playerTr = Find.getInstance().FindTagTransform(playerTag);
        playerSc = playerTr.GetComponent<Player>();
    }
    //몬스터 이동 함수
    public void Move(GameObject target, float patrol, float trace, float attack, float stop, GameObject _fenceBR, GameObject _fenceFL)
    {
        //플레이어와 몬스터간의 거리연산
        float dist = Vector3.Distance(transform.position, target.transform.position);
        //플레이어가 죽지않았다면
        if(!PlayerCtrl.playerDead)
        {
            //거리가 어택거리보다 적으면 공격
            if (dist < attack)
            {
                Attack(target, stop);
            }
            //거리가 추격거리보다 작으면 추격
            else if (dist < trace)
            {
                Trace(target);
            }
            //거리가 순찰보다 작으면 순찰
            else if (dist > patrol)
            {
                Patrol(_fenceBR, _fenceFL);
            }
        }
        //플레이어가 죽었다면
        else
        {
            Patrol(_fenceBR, _fenceFL);
        }
    }
    //순찰 함수(매개변수는 몬스터 순찰중에 나가지 못하게 하는 틀)
    void Patrol(GameObject __fenceBR, GameObject __fenceFL)
    {
        //리턴포인트가 있다면
        if(returnPoint != Vector3.zero)
        {
            //순찰상태로
            patrol = true;
            //제동거리는 0
            navAgen.stoppingDistance = 0f;
            //이동장소는 리턴포인트
            navAgen.SetDestination(returnPoint);
        }
        //순찰중이 아니라면
        else if (!patrol)
        {
            //순찰중으로
            patrol = true;
            //이동장소는 랜덤 장소
            movePoint = RandomPoint();
            //랜덤장소생성후에 펜스 벗어났는지 확인후 그축만 수정
            if (movePoint.x > __fenceBR.transform.position.x)
                movePoint.x = __fenceBR.transform.position.x - 10;
            if (movePoint.x < __fenceFL.transform.position.x)
                movePoint.x = __fenceFL.transform.position.x + 10;
            if (movePoint.z > __fenceFL.transform.position.z)
                movePoint.z = __fenceFL.transform.position.z - 10;
            if (movePoint.z < __fenceBR.transform.position.z)
                movePoint.z = __fenceBR.transform.position.z + 10;
            //제동거리 0으로 변경
            navAgen.stoppingDistance = 0f;
            //이동장소는 무브포인트
            navAgen.SetDestination(movePoint);
        }
        //이동장소에 도착했다면
        if (navAgen.remainingDistance == 0)
        {
            //순찰중 false로
            patrol = false;
            //리턴포인트는 초기화
            returnPoint = Vector3.zero;
        }
    }
    //랜덤 장소 생성
    Vector3 RandomPoint()
    {
        //x축 랜덤값 생성
        float x = Random.Range(-20.0f, 20.0f);
        //z축 랜덤값 생성
        float z = Random.Range(-20.0f, 20.0f);
        //현재 좌표에 랜덤값 더한다
        Vector3 setPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        //설정된 좌표를 반환한다
        return setPoint;
    }
    //추격 함수 매개변수는 타겟
    void Trace(GameObject target)
    {
        //공격상태 false
        attack = false;
        //추격상태 true
        trace = true;
        //추격상태라면
        if(patrol)
        {
            //리턴포인트를 지금좌표로 셋팅
            returnPoint = transform.position;
            //순찰중 false
            patrol = false;
        }
        //이동장소를 타겟으로 잡는다
        navAgen.SetDestination(target.transform.position);
    }
    //공격 함수 매개변수는 타겟, 제동거리
    void Attack(GameObject target, float stop)
    {
        //공격중 true
        attack = true;
        //이동을 멈춘다
        navAgen.stoppingDistance = stop;
    }
    //죽었을때 함수
    public void Dead(GameObject DeadPoint)
    {
        //죽음 true
        dead = true;
        //죽고나서 충돌이동하는 문제때문에 velocity초기화함
        //gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //같은 문제로 angularvelocity초기화함
        //gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        //navmeshagent 정지
        navAgen.isStopped = true;
        //죽으면 특정 장소를 부모로
        gameObject.transform.SetParent(DeadPoint.transform);
        //스킨랜더 꺼준다
        gameObject.transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_OutlineWidth", 1.0f);
        //죽은뒤의 wait꺼준다
        StartCoroutine(DeadWait());
    }
    //데미지 입었을때 함수
    public void Hurt(GameObject target)
    {
        //.isStopped = true;
        //피격중 true
        hurt = true;
        //데미지 받으면 wait coroutine시작
        StartCoroutine(HurtWait());
        //피격코르틴 끝나면
        if (!hurt)
        {
            Debug.Log("bool");
            //navAgen.isStopped = false;
            //장소를 타겟 장소로
            navAgen.SetDestination(target.transform.position);
        }
    }
    //충돌 시작
    void OnCollisionEnter(Collision coll)
    {
        //충돌한 객체가 같은 몬스터일때
        if(coll.gameObject.name == "BDragon")
        {
            //navmeshagent 정지
            navAgen.isStopped = true;
            //순찰중 true
            patrol = true;
            //제동 거리를 0으로
            navAgen.stoppingDistance = 0f;
            //부딪히면 서로 계속 붙어있는 상태를 수정하기위해 현재 몬스터 위치를 뒤로
            navAgen.SetDestination(new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 10f));
        }
    }
    //경험치 증가 함수
    public void expUp(int exp)
    {
        //플레이어 경험치에 현재 경험치 증가
        playerSc.currentPlayerInfo.exp += exp;
    }
    //피격시 데미지 wait줄 코루틴
    IEnumerator HurtWait()
    {
        while(hurt)
        {
            yield return new WaitForSeconds(0.3f);
            hurt = false;
        }
    }
    //죽은다음 wait줄 코루틴
    IEnumerator DeadWait()
    {
        while(dead)
        {
            yield return new WaitForSeconds(2.8f);
            gameObject.SetActive(false);
            dead = false;
        }
    }
}

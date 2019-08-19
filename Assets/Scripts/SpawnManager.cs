using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //드래곤 스폰 포인트 오브젝트
    string dragonSpawnTag;
    Transform dragonSpawnTr;
    //드래곤 오브젝트 모아놓을곳
    string bDragonHiveTag;
    Transform bDragonHiveTr;
    //드래곤 죽으면 모아놓을곳
    string bDragonDeadTag;
    Transform bDragonDeadTr;
    //개구리 스폰 포인트 오브젝트
    string frogSpawnTag;
    Transform frogSpawnTr;
    //개구리 오브젝트 모아놓을곳
    string frogHiveTag;
    Transform frogHiveTr;
    //드래곤 프리펩(드래그앤 드롭)
    public GameObject BDragonPrefab;
    //개구리 프리펩(드래그앤 드롭)
    public GameObject frogPrefab;
    //드래곤의 스폰 포인트 리스트
    public List<Transform> dragonSpawnPoints = new List<Transform>();
    //개구리의 스폰 포인트 리스트
    public List<Transform> frogSpawnPoints = new List<Transform>();
    //리스폰 bool값
    bool respawn;

    void Start()
    {
        //각각의 오브젝트에 연결
        dragonSpawnTag = FindTag.getInstance().dragonSpawnPoint;
        dragonSpawnTr = Find.getInstance().FindTagTransform(dragonSpawnTag);

        bDragonHiveTag = FindTag.getInstance().bDragonHive;
        bDragonHiveTr = Find.getInstance().FindTagTransform(bDragonHiveTag);

        bDragonDeadTag = FindTag.getInstance().bDragonDead;
        bDragonDeadTr = Find.getInstance().FindTagTransform(bDragonDeadTag);

        frogSpawnTag = FindTag.getInstance().frogSpawnPoint;
        frogSpawnTr = Find.getInstance().FindTagTransform(frogSpawnTag);

        frogHiveTag = FindTag.getInstance().frogHive;
        frogHiveTr = Find.getInstance().FindTagTransform(frogHiveTag);
        //드래곤 스폰 포인트 셋팅
        for (int i = 0; i < dragonSpawnTr.childCount; i++)
        {
            dragonSpawnPoints.Add(dragonSpawnTr.GetChild(i).transform);
        }
        //드래곤 초기 생성(자식개수 만큼 생성 현재 10)
        for (int i = 0; i < dragonSpawnTr.childCount; i++)
        {
            GameObject a = Instantiate(BDragonPrefab, dragonSpawnPoints[i]);
            a.transform.name = "BDragon" + i;
            a.transform.SetParent(bDragonHiveTr);
        }
        //개구리 스폰 포인트 셋팅
        for(int i = 0; i < frogSpawnTr.childCount; i++)
        {
            frogSpawnPoints.Add(frogSpawnTr.GetChild(i).transform);
        }
        //개구리 초기 생성(자식 개수 만큼 생성)
        for(int i = 0; i < frogSpawnTr.childCount; i++)
        {
            GameObject b = Instantiate(frogPrefab, frogSpawnPoints[i]);
            b.transform.name = "Frog" + i;
            b.transform.SetParent(frogHiveTr);
        }
    }

    void Update()
    {
        if(bDragonDeadTr.childCount > 0 && !respawn)
        {
            respawn = true;
            StartCoroutine(ReSpawnWait());
        }
    }

    IEnumerator ReSpawnWait()
    {
        while(respawn)
        {
            yield return new WaitForSeconds(10f);
            Destroy(bDragonDeadTr.GetChild(0).gameObject);
            int rand = Random.Range(0, 10);
            GameObject a = Instantiate(BDragonPrefab, dragonSpawnPoints[rand].position, dragonSpawnPoints[rand].rotation);
            a.transform.SetParent(bDragonHiveTr);
            respawn = false;
        }
    }
}

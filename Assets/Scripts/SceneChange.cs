using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    bool wait;

    void Start()
    {
        StartCoroutine(Wait());
    }

    void Update()
    {
        
    }

    public void ChangeScene()
    {
        //선택창 씬으로 이동한다
        if (wait)
            SceneManager.LoadScene("SelectScene");
    }

    public void ChangeScene2()
    {
        //현재 가능한 직업이 마법사 밖에 없기 때문에 마법사를 선택했을때만 씬을 바꿀수 있게 설정
        if (SelectSceneUiManager.selectJob == "Magician")
            SceneManager.LoadScene("MainScene");
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
        wait = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Xml;
using System.IO;

public class SelectSceneUiManager : MonoBehaviour
{
    //ui 변경을 위해 캔버스를 잡음(드래그앤 드롭)
    public GameObject canvas;
    //현재 선택된 직업의 텍스트값
    public static string selectJob;
    //상태 표시할때 직업을 한글로 표시하기 위해서 사용
    string korJob;
    //플레이어 정보가 저장된 xml 이름
    string xmlName = "playerData.xml";
    //filecheck 탐색 결과
    public static bool result;

    void Start()
    {
        //if문에서 filecheck 결과를 사용해야하는데 if문마다 들어가서 탐색하고 오면 안좋으니 이곳에서 결과 받아서 전달
        result = FileCheck();
    }
    
    void Update()
    {
        
    }
    //xml파일이 있는지 없는지 확인해서 있으면 true 없으면 false를 반환한다
    bool FileCheck()
    {
        //xml파일 저장 경로
        string filePath = Application.persistentDataPath + "/" + xmlName;
       
        System.IO.FileInfo fi = new System.IO.FileInfo(filePath);
        //Exists 존재 하는지 확인해준다
        if (fi.Exists)
            return true;
        else
            return false;
    }
    //모든직업 버튼에서 사용 한다
    public void SelectJob()
    {
        //현재 사용한 버튼 오브젝트를 파악
        GameObject currentButton = EventSystem.current.currentSelectedGameObject;
        //선택된 버튼의 이름을 직업이름으로 저장
        selectJob = currentButton.name;
        //선택된 직업이 마법사라면
        if(selectJob == "Magician")
        {
            //대기 텍스트를 비활성화
            canvas.transform.GetChild(3).gameObject.SetActive(false);
            //캐릭터 스텟 텍스트를 활성화
            canvas.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
            ShowStatus();
        }
        //다른 직업이 선택됐다면
        else
        {
            //대기 텍스트를 활성화
            canvas.transform.GetChild(3).gameObject.SetActive(true);
            //대기 텍스트가 변경되어있을수 있으므로 텍스트를 원상태로 변경
            canvas.transform.GetChild(3).GetComponent<Text>().text = "직업을 선택해 주세요";
            //캐릭터 스텟 텍스트를 비활성화
            canvas.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
            //선택 실패 창 활성화
            canvas.transform.GetChild(4).gameObject.SetActive(true);
        }
    }
    //ui닫는다
    public void CloseWindow()
    {
        //선택 실패창 비활성화
        canvas.transform.GetChild(4).gameObject.SetActive(false);
    }
    //xml에서 정보를 가져온다
    public void Load()
    {
        string xmlFilePath = Application.persistentDataPath + "/" + xmlName;

        using (XmlReader xmlReader = XmlReader.Create(xmlFilePath))
        {
            while (xmlReader.Read())
            {
                if (xmlReader.IsStartElement())
                {
                    switch (xmlReader.Name)
                    {
                        case "Job":
                            if (xmlReader.Read())
                                canvas.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = korJob;
                            break;
                        case "Level":
                            if (xmlReader.Read())
                                canvas.transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = "레벨 : " + xmlReader.Value.Trim();
                            break;
                        case "Hp":
                            if (xmlReader.Read())
                                canvas.transform.GetChild(2).transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().text = "체력 : " + xmlReader.Value.Trim();
                            break;
                        case "Mp":
                            if (xmlReader.Read())
                                canvas.transform.GetChild(2).transform.GetChild(0).transform.GetChild(3).GetComponent<Text>().text = "마력 : " + xmlReader.Value.Trim();
                            break;
                        case "Str":
                            if (xmlReader.Read())
                                canvas.transform.GetChild(2).transform.GetChild(0).transform.GetChild(4).GetComponent<Text>().text = "방어력 : " + xmlReader.Value.Trim();
                            break;
                        case "Def":
                            if (xmlReader.Read())
                                canvas.transform.GetChild(2).transform.GetChild(0).transform.GetChild(5).GetComponent<Text>().text = "공격력 : " + xmlReader.Value.Trim();
                            break;
                        case "Cripro":
                            if (xmlReader.Read())
                                canvas.transform.GetChild(2).transform.GetChild(0).transform.GetChild(6).GetComponent<Text>().text = "치명타 확률 : " + xmlReader.Value.Trim() + "%";
                            break;
                        case "Cridem":
                            if (xmlReader.Read())
                                canvas.transform.GetChild(2).transform.GetChild(0).transform.GetChild(7).GetComponent<Text>().text = "치명타 데미지 : " + xmlReader.Value.Trim() + "%";
                            break;
                    }
                }
            }
        }
    }

    //스텟 보여줄 함수
    void ShowStatus()
    {
        //현재 선택한 직업을 한국말로 다른 스트링에 저장
        switch (selectJob)
        {
            case "Magician":
                korJob = "마법사";
                break;
        }
        //xml파일이 있다면
        if(result)
        {
            //스텟 텍스트들 변경
            Load();
        }
        //저장된 데이터가 없다면
        else
        {
            //스텟 텍스트 비활성화
            canvas.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
            //대기 텍스트창 활성화
            canvas.transform.GetChild(3).gameObject.SetActive(true);
            //대기 텍스트창 텍스트 변경
            canvas.transform.GetChild(3).GetComponent<Text>().text = "캐릭터 정보가 없습니다";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStickCtrl : MonoBehaviour
{ 
    //joystick으로 움직일 객체(펫이 움직이기 때문에 펫오브젝트를 드래그앤드롭으로 연결
    public Transform player;
    //joystick stick (드래그앤드롭으로 연결)
    public Transform stick;

    //stick 움직였다가 제자리로 돌아갈때 필요한 값
    private Vector3 stickFirstPos;
    private Vector3 joyVec;
    private float radius;
    //joystick 사용중인지 확인하는 값
    public static bool moveFlag;

    public int cameraSwap = 1;
    //드래그로 카메라 회전시킬때 그값 만큼 회전 시키려고 값받아온다(드래그앤드롭)
    public FollowCam followCam;

    //팔로우캠에 넘겨줄 터치 bool값
    public bool joystickTouch;

    void Start()
    {
        //조이스틱 배경의 반지름을 구한다
        radius = GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        //조이스틱의 처음위치를 저장
        stickFirstPos = stick.transform.position;

        float can = transform.parent.GetComponent<RectTransform>().localScale.x;
        radius *= can;

        moveFlag = false;
    }

    void Update()
    {
        if(UiManager.ride)
        {
            //Debug.Log(moveFlag);
            //stick이동중이면 오브젝트 이동
            if (moveFlag)
                player.transform.Translate(Vector3.forward * Time.deltaTime * 10f);
        }
    }

    //드래그 중일때 이벤트 처리
    public void Drag(BaseEventData _Data)
    {
        moveFlag = true;
        PointerEventData Data = _Data as PointerEventData;
        Vector3 Pos = Data.position;

        joyVec = (Pos - stickFirstPos).normalized;

        float Dis = Vector3.Distance(Pos, stickFirstPos);

        if (Dis < radius)
            stick.position = stickFirstPos + joyVec * Dis;
        else
            stick.position = stickFirstPos + joyVec * radius;

        player.transform.eulerAngles = new Vector3(0, (Mathf.Atan2(cameraSwap * joyVec.x, cameraSwap * joyVec.y) * Mathf.Rad2Deg) + followCam.xAngle, 0);
    }

    //드래그 끝났을때 이벤트 처리
    public void DragEnd()
    {
        //스틱 원위치로
        stick.position = stickFirstPos;
        joyVec = Vector3.zero;
        moveFlag = false;
    }

    public void TouchCheck()
    { 
        joystickTouch = true;
        Debug.Log("touch : " + joystickTouch);
    }

    public void TouchUnCheck()
    {
        joystickTouch = false;
        Debug.Log("touch : " + joystickTouch);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FollowCam : MonoBehaviour
{
    //canvas tag tr에 접근 하기 위해 필요
    string canvasTag;
    Transform canvasTr;
    //JoystickCtrl에 접근
    JoyStickCtrl joyStickCtrl;
    //카메라 앞뒤로 위치 줄때 쓸 bool값
    bool cameraChange;

    float screenTestH = Screen.height;
    float screenTest = Screen.width;

    //카메라가 쫒을 대상
    public Transform player;

    Vector3 FirstPoint;
    Vector3 SecondPoint;
    public float xAngle;
    float xAngleTemp;

    public GameObject testText;

    Touch tempTouchs;

    void Start()
    {
        //각각의 정보에 접근
        canvasTag = FindTag.getInstance().canvas;
        canvasTr = Find.getInstance().FindTagTransform(canvasTag);
        joyStickCtrl = canvasTr.GetChild(0).GetComponent<JoyStickCtrl>();

        xAngle = 0;
        this.transform.rotation = Quaternion.Euler(0, xAngle, 0);
    }

    void Update()
    {
        float x;
        float y;

        x = screenTest / 4;
        y = screenTestH / 2;

        if (Input.touchCount > 0 && !UiManager.inventorySwitch && !UiManager.skillUiSwitch && !NpcShop.shopBool)
        {
            if (Input.GetTouch(0).position.x > x || Input.GetTouch(0).position.y > y)
            {
                if (Input.touchCount == 1)
                {
                    if (!JoyStickCtrl.moveFlag)
                    {
                        if (Input.GetTouch(0).phase == TouchPhase.Began)
                        {
                            FirstPoint = Input.GetTouch(0).position;
                            xAngleTemp = xAngle;
                        }
                        if (Input.GetTouch(0).phase == TouchPhase.Moved)
                        {
                            SecondPoint = Input.GetTouch(0).position;
                            xAngle = xAngleTemp + (SecondPoint.x - FirstPoint.x) * 180 / Screen.width;
                            this.transform.rotation = Quaternion.Euler(0.0f, xAngle, 0.0f);
                        }
                    }
                }
                if (Input.touchCount == 2)
                {
                    if (Input.GetTouch(1).phase == TouchPhase.Began)
                    {
                        FirstPoint = Input.GetTouch(1).position;
                        xAngleTemp = xAngle;
                    }
                    if (Input.GetTouch(1).phase == TouchPhase.Moved)
                    {
                        SecondPoint = Input.GetTouch(1).position;
                        xAngle = xAngleTemp + (SecondPoint.x - FirstPoint.x) * 180 / Screen.width;
                        this.transform.rotation = Quaternion.Euler(0.0f, xAngle, 0.0f);
                    }
                }
            }
            else if (Input.GetTouch(1).position.x > x || Input.GetTouch(1).position.y > y)
            {
                if (Input.touchCount == 1)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        FirstPoint = Input.GetTouch(0).position;
                        xAngleTemp = xAngle;
                    }
                    if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        SecondPoint = Input.GetTouch(0).position;
                        xAngle = xAngleTemp + (SecondPoint.x - FirstPoint.x) * 180 / Screen.width;
                        this.transform.rotation = Quaternion.Euler(0.0f, xAngle, 0.0f);
                    }
                }
                if (Input.touchCount == 2)
                {
                    if (Input.GetTouch(1).phase == TouchPhase.Began)
                    {
                        FirstPoint = Input.GetTouch(1).position;
                        xAngleTemp = xAngle;
                    }
                    if (Input.GetTouch(1).phase == TouchPhase.Moved)
                    {
                        SecondPoint = Input.GetTouch(1).position;
                        xAngle = xAngleTemp + (SecondPoint.x - FirstPoint.x) * 180 / Screen.width;
                        this.transform.rotation = Quaternion.Euler(0.0f, xAngle, 0.0f);
                    }
                }
            }
        }


        //if (!joyStickCtrl.joystickTouch)
        //{
        //    //터치 입력 횟수
        //    if (Input.touchCount > 0)
        //    {
        //        if (Input.GetTouch(0).phase == TouchPhase.Began)
        //        {
        //            FirstPoint = Input.GetTouch(0).position;
        //            xAngleTemp = xAngle;
        //        }
        //        if (Input.GetTouch(0).phase == TouchPhase.Moved)
        //        {
        //            SecondPoint = Input.GetTouch(0).position;
        //            xAngle = xAngleTemp + (SecondPoint.x - FirstPoint.x) * 180 / Screen.width;
        //            this.transform.rotation = Quaternion.Euler(0.0f, xAngle, 0.0f);
        //        }

        //        //if (Input.GetTouch(1).phase == TouchPhase.Began && joyStickCtrl.joystickTouch)
        //        //{
        //        //    FirstPoint = Input.GetTouch(1).position;
        //        //    xAngleTemp = xAngle;
        //        //}
        //        //if (Input.GetTouch(1).phase == TouchPhase.Moved && joyStickCtrl.joystickTouch)
        //        //{
        //        //    SecondPoint = Input.GetTouch(1).position;
        //        //    xAngle = xAngleTemp + (SecondPoint.x - FirstPoint.x) * 180 / Screen.width;
        //        //    this.transform.rotation = Quaternion.Euler(0.0f, xAngle, 0.0f);
        //        //}
        //    }
        //}
    }

    //public void SwapCameraPosition()
    //{
    //    cameraChange = !cameraChange;
    //
    //    //bool값에 따라 카메라 위치와 조이스틱 상하좌우 반전을 위해 int 값 변경
    //    if (cameraChange)
    //    {
    //        transform.rotation = new Quaternion(0, 180.0f, 0, 0);
    //        joyStickCtrl.cameraSwap = -1;
    //    }
    //    else
    //    {
    //        transform.rotation = new Quaternion(0, 0, 0, 0);
    //        joyStickCtrl.cameraSwap = 1;
    //    }
    //}

    void LateUpdate()
    {
        //Position만 계속 수정
        transform.position = player.transform.GetChild(0).transform.position;
    }
}

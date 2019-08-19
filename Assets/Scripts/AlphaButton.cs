using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaButton : MonoBehaviour
{
    public float AlphaThreshold = 0.1f;

    void Update()
    {
        //이미지의 알파값에 접근해서 UI 버튼 주변이 클릭 안되게 막는다
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = AlphaThreshold;
    }
}

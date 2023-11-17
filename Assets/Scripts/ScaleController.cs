using System;
using UnityEngine;

public class ScaleController : MonoBehaviour
{
    public CheckHand checkHand;
    public Transform target;
    public float scaleCheckValue;
    public float min;
    public float max;
    void Start()
    {
    }

    private void Update()
    {
        if (checkHand.IsHandAllIn)
        {
            if (checkHand.IsLeftPinchAndRightPinch)
            {
                Scale();
            }
        }
    }

    private bool IsEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
    {
        //函数传入上一次触摸两点的位置与本次触摸两点的位置计算出用户的手势  
        var leng1 = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
        var leng2 = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));
        if (leng1 < leng2)
        {
            //放大手势  
            return true;
        }
        else
        {
            //缩小手势  
            return false;
        }
    }

    public Vector3 oldRightPos;
    public Vector3 oldLeftPos;

    private void Scale()
    {
        var rightPos = checkHand.rightHand.PalmPosition;
        var leftPos = checkHand.leftHand.PalmPosition;
        Debug.Log(Vector3.Distance(rightPos, oldRightPos) * 100);
        if (IsEnlarge(oldRightPos, oldLeftPos, rightPos, leftPos))
        {
            if (Vector3.Distance(rightPos, oldRightPos) * 100 > scaleCheckValue)
            {
                target.localScale += Vector3.one * 0.1f;
                if (target.localScale.x > max)
                {
                    target.localScale = Vector3.one * 2.5f;
                }
            }
        }
        else
        {
            if (Vector3.Distance(rightPos, oldRightPos) * 100 > scaleCheckValue)
            {
                target.localScale -= Vector3.one * 0.1f;
                if (target.localScale.x < min)
                {
                    target.localScale = Vector3.one;
                }
            }
        }

        oldLeftPos = leftPos;
        oldRightPos = rightPos;
    }
}
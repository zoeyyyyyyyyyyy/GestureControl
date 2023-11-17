using System;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    public CheckHand checkHand;
    public Transform target;  
    public bool isCom;
    public UIInteractiveLogic uiInteractiveLogic; 
    public ObjRotationInvoker objRotationInvoker;
    void Start()
    {
        checkHand.leftMoveCompleteAction += () => { objRotationInvoker.UnDo(); };
        checkHand.rightMoveCompleteAction += () => { objRotationInvoker.ReDo(); };
    }


    private void Update()
    {
        if (checkHand.IsHandAllIn)
        {
            if (checkHand.IsLeftCloseFullAndRightHandOpenOne)
            {
                // Debug.Log("左手拳头 右手食指");
                isCom = true;
            } 
            if (isCom && checkHand.IsLeftOpenFullAndRightHandOpenOne)
            {
                // Debug.Log("旋转目标");
                Debug.Log(uiInteractiveLogic.uiObj.name);
                Invoke(uiInteractiveLogic.uiObj.name, 0);
                isCom = false;
            }

            if (checkHand.IsOpenAllTwoHand)
            {
                objRotationInvoker.Clear();
                // Debug.Log("双手伸开");
                Rotation();
                isCom = false;
            }
        }
    }

    private void Rotation()
    {
        var dir = checkHand.leftHand.PalmPosition - checkHand.rightHand.PalmPosition; //位置差，方向   
        //点积的计算方式为: a·b =| a |·| b | cos < a,b > 其中 | a | 和 | b | 表示向量的模 。   
        var angleZ = Mathf.Acos(Vector3.Dot(Vector3.up.normalized, dir.normalized)) * Mathf.Rad2Deg; //通过点乘求出夹角  
        var angleY = Mathf.Acos(Vector3.Dot(Vector3.forward.normalized, dir.normalized)) * Mathf.Rad2Deg; //通过点乘求出夹角   
        // if (isLockYAxis)
        // {
        //     target.eulerAngles = new Vector3(0, 0, (angleZ - 90) * speed);
        // }
        // else
        // {
        //     target.eulerAngles = new Vector3(0, -angleY * speed, 0);
        // } 
        target.eulerAngles = new Vector3((angleZ - 90), (-angleY +90), 0);
        // target.LookAt( checkHand.leftHand.PalmPosition);
    }

    public void RotationActionL90()
    {
        objRotationInvoker.Rotation(new RotationCommand(RotationDirection.Left, target));
    }

    public void RotationActionR90()
    {
        objRotationInvoker.Rotation(new RotationCommand(RotationDirection.Right, target));
    }

    public void RotationActionF90()
    {
        objRotationInvoker.Rotation(new RotationCommand(RotationDirection.Up, target));
    }


    private void Awake()
    {
        objRotationInvoker = new ObjRotationInvoker();
    }

  
}
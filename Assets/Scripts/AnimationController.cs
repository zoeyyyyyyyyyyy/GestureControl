using System;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public CheckHand checkHand;
    public Animator animator;
    public float speed;
    public bool isCom;

    void Start()
    {
    }

    private void Update()
    {
        if (checkHand.IsHandAllIn)
        {
            if (checkHand.IsLeftOpenFullHandAndRightPinch)
            {
                isCom = true;
            }

            if (isCom && checkHand.IsOpenAllTwoHand)
            {
                isCom = false;
                animator.enabled = !animator.enabled;
                if (animator.enabled)
                {
                    animator.speed = 1;
                }
            }

            if (animator.enabled)
            {
                if (checkHand.rightHand != null)
                {
                    // var value = (((checkHand.rightHand.Rotation.eulerAngles.y - 340) / 10f));
                    var value = (((checkHand.rightHand.Arm.Rotation.eulerAngles.y - 300) / 20f));
                    var clamp = Mathf.Clamp(value, 0, 1f);
                    animator.speed = clamp;
                    Debug.Log(clamp);
                }
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Leap;
using Leap.Unity;
using UnityEngine;


public class CheckHand : MonoBehaviour
{
    public LeapProvider leapProvider;

    [Tooltip("Velocity (m/s) move toward ")] //速度（m/s）走向 
    public float smallestVelocity = 0.4f;

    [Tooltip("Velocity (m/s) move toward ")]
    public float deltaVelocity = 0.7f;

    public float deltaCloseFinger = 0.05f;

    public Hand leftHand;
    public Hand rightHand;
    /// <summary>
    /// 双手是否在监视器前
    /// </summary>
    public bool IsHandAllIn => rightHand != null && leftHand != null;
    /// <summary>
    /// 左手 - 掌 右手 - 捏
    /// </summary>
    // public bool IsLeftOpenFullHandAndRightPinch => IsOpenHand(leftHand) && IsPinchHand(rightHand);
    public bool IsLeftOpenFullHandAndRightPinch => IsOpenHand(leftHand) &&  IsFingerClose(rightHand, new[] {0, 1, 2, 3, 4}) ;
    /// <summary>
    /// 左手 - 掌 右手 - 掌
    /// </summary>
    public bool IsOpenAllTwoHand => IsOpenFullHand(leftHand) && IsOpenFullHand(rightHand);
    /// <summary>
    /// 左手 - 捏动作结束 右手 - 捏动作结束
    /// </summary>
    public bool IsNoPinchAllHand => !IsPinchHand(leftHand) && !IsPinchHand(rightHand);
    /// <summary>
    /// 左手 - 捏 右手 - 捏
    /// </summary>
    public bool IsLeftPinchAndRightPinch => IsPinchHand(leftHand) && IsPinchHand(rightHand);
    /// <summary>
    /// 左手 - 握拳 右手 - 双指
    /// </summary>
    public bool IsLeftCloseFullAndRightHandOpenTwo =>
        IsFingerClose(leftHand, new[] {0, 1, 2, 3, 4}) && IsFingerOpen(rightHand, new int[] {1, 2});
    /// <summary>
    /// 左手 - 握拳 右手 - 单指
    /// </summary>
    public bool IsLeftCloseFullAndRightHandOpenOne =>
        IsFingerClose(leftHand, new[] {0, 1, 2, 3, 4}) && IsFingerOpen(rightHand, new int[] {1});
    /// <summary>
    /// 左手 - 掌 右手 - 单指
    /// </summary>
    public bool IsLeftOpenFullAndRightHandOpenOne =>
        IsOpenHandForValue(leftHand, 0.2f) && IsFingerOpen(rightHand, new int[] {1});

    private void Update()
    {
        for (int i = 0; i < leapProvider.CurrentFrame.Hands.Count; i++)
        {
            var hand = leapProvider.CurrentFrame.Hands[i];
            if (hand.IsRight)
            {
                rightHand = hand;
            }

            if (hand.IsLeft)
            {
                leftHand = hand;
            }
        }

        switch (leapProvider.CurrentFrame.Hands.Count)
        {
            case 0:
                rightHand = null;
                leftHand = null;
                break;
            case 1:
            {
                var hand = leapProvider.CurrentFrame.Hands[0];
                if (hand.IsLeft)
                {
                    rightHand = null;
                }
                else
                {
                    leftHand = null;
                }

                break;
            }
        }

        CheckAction();
    }


    private void CheckAction()
    {
        if (IsHandAllIn)
        {
            if (IsLeftCloseFullAndRightHandOpenTwo)
            {
                Debug.Log("左手握拳，右手食指中指伸直");
                if (IsMoveLeft(rightHand))
                {
                    if (waitInit == null)
                    {
                        waitInit = StartCoroutine(WaitInit());
                    }

                    if (waitNext == null)
                    {
                        waitNext = StartCoroutine(WaitNext());
                    }

                    if (isCan)
                    {
                        leftMoveCount++;
                        rightMoveCount = 0;
                        isCan = false;
                        if (leftMoveCount >= leftMoveTargetTimes)
                        {
                            Debug.Log("执行事件");
                            leftMoveCompleteAction?.Invoke();
                            leftMoveCount = 0;
                            StopCoroutine(waitInit);
                            waitInit = null;
                        }
                    }

                    Debug.Log("向左划");
                }

                if (IsMoveRight(rightHand))
                {
                    if (waitInit == null)
                    {
                        waitInit = StartCoroutine(WaitInit());
                    }

                    if (waitNext == null)
                    {
                        waitNext = StartCoroutine(WaitNext());
                    }

                    if (isCan)
                    {
                        rightMoveCount++;
                        leftMoveCount = 0;
                        isCan = false;
                        if (rightMoveCount >= leftMoveTargetTimes)
                        {
                            Debug.Log("执行事件");
                            rightMoveCompleteAction?.Invoke();
                            rightMoveCount = 0;
                            StopCoroutine(waitInit);
                            waitInit = null;
                        }
                    }

                    Debug.Log("向右划");
                }
            }
        }
    }

    /// <summary>
    /// 检查手掌向做移动 
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    private bool IsMoveLeft(Hand hand) // 手划向左边
    {
        //x轴移动的速度   deltaVelocity = 0.7f    isStationary (hand)  判断hand是否禁止  
        return hand.PalmVelocity.x < -deltaVelocity && !IsStationary(hand);
    }

    private bool IsMoveRight(Hand hand) // 手划向右边
    {
        return hand.PalmVelocity.x > deltaVelocity && !IsStationary(hand);
    }

    private bool IsMoveUp(Hand hand) //手向上 
    {
        return hand.PalmVelocity.y > deltaVelocity && !IsStationary(hand);
    }

    private bool IsMoveDown(Hand hand) //手向下  
    {
        return hand.PalmVelocity.y < -deltaVelocity && !IsStationary(hand);
    }

    public float grabStrength = 0.2f;

    private bool IsGrabHand(Hand hand) //是否抓取
    {
        return hand.GrabStrength < grabStrength; //抓取力 
    }

    /// <summary>
    /// 是否捏 拇指食指捏同时其他手指伸展
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    private bool IsPinchHand(Hand hand)
    {
        return hand != null && Math.Abs(hand.PinchStrength - 1f) < 0.1f && IsFingerOpen(hand, new[] {2, 3, 4});
    }

    /// <summary>
    /// 检查多个手指的弯曲度
    /// </summary>
    /// <param name="hand">手部数据</param>
    /// <param name="fingerIndex">检查的手指</param>
    /// <returns>true为伸展</returns>
    private bool IsFingerOpen(Hand hand, int[] fingerIndex)
    {
        if (hand != null)
        {
            var fingers = new List<Finger>();
            for (int i = 0; i < fingerIndex.Length; i++)
            {
                fingers.Add(hand.Fingers[fingerIndex[i]]);
            }

            //IsExtended触发条件大概是GrabStrength= 0.7，因此不适用所有状况 握紧判断需要其他逻辑
            //伸展手指集合
            var extendedFinger = hand.Fingers.Where(x => x.IsExtended).ToList();
            //计算交集
            var result = extendedFinger.Intersect(fingers).ToList();
            // return result.Count == fingers.Count && extendedFinger.Count == fingers.Count && otherFingerResult;
            return result.Count == fingers.Count && extendedFinger.Count == fingers.Count;
        }

        return false;
    }


    /// <summary>
    /// 手掌是否伸直
    /// </summary>
    /// <param name="hand"></param>
    /// <returns>true为张开，false为握拳</returns>
    private bool IsOpenFullHand(Hand hand)
    {
        if (hand != null)
        {
            var result = hand.Fingers.FindAll(x => x.IsExtended);
            // return hand.GrabStrength < 0.1f && !IsPinchHand(hand);
            return result.Count == 5;
        }

        return false;
    }

    private bool IsOpenHand(Hand hand)
    {
        if (hand == null) return false;
        return hand.GrabStrength == 0;
    }

    /// <summary>
    /// 手掌弯曲是否达到设定值
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private bool IsOpenHandForValue(Hand hand, float value)
    {
        if (hand == null) return false;
        return hand.GrabStrength <= value;
    }
    /// <summary>
    /// 手指握紧判断
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="fingerIndex"></param>
    /// <returns></returns>
    private bool IsFingerClose(Hand hand, int[] fingerIndex)
    {
        if (hand == null)
        {
            return false;
        }

        var fingers = new List<Finger>();
        for (int i = 0; i < fingerIndex.Length; i++)
        {
            fingers.Add(hand.Fingers[fingerIndex[i]]);
        }
        
        //握紧手指集合
        var extendedFinger = hand.Fingers.Where(x => (x.TipPosition - hand.PalmPosition).magnitude < deltaCloseFinger)
            .ToList();
        //计算交集
        var result = extendedFinger.Intersect(fingers).ToList();
        return result.Count == fingers.Count && extendedFinger.Count == fingers.Count;
    }


    #region 左手移动事件

    public int leftMoveCount;
    public int rightMoveCount;
    public Action leftMoveCompleteAction;
    public Action rightMoveCompleteAction;
    public int leftMoveTargetTimes;
    public float interval = 0.2f;
    private Coroutine waitNext;
    private Coroutine waitInit;
    public bool isCan;

    IEnumerator WaitInit()
    {
        yield return new WaitForSeconds(interval * 3);
        leftMoveCount = 0;
        rightMoveCount = 0;
        waitInit = null;
        Debug.Log("等待时间到达数值初始化");
    }

    IEnumerator WaitNext()
    {
        isCan = true;
        yield return new WaitForSeconds(interval);
        waitNext = null;
    }

    #endregion

    /// <summary>
    /// 是否正在移动
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    private bool IsStationary(Hand hand) // 固定不动的 
    {
        bool stationary = hand.PalmVelocity.magnitude < smallestVelocity;
        return stationary;
    }
}
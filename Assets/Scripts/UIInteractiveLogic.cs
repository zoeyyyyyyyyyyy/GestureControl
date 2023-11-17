using UnityEngine;

public class UIInteractiveLogic : MonoBehaviour
{
    public CheckHand checkHand;
    public Transform point;
    private Camera mainCamera;
    public GameObject uiObj;
    public GameObject uiGroup;
    public Vector3 offset;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void Update()
    {
        var modeSwitch = checkHand.IsLeftCloseFullAndRightHandOpenOne;
        point.gameObject.SetActive(modeSwitch);
        uiGroup.SetActive(modeSwitch);
        if (modeSwitch)
        {
            if (checkHand.rightHand != null)
            {
                var fingerPos = checkHand.rightHand.Fingers[1].TipPosition;

                var screenPos = mainCamera.WorldToScreenPoint(fingerPos);

                point.position = new Vector3(screenPos.x + offset.x, screenPos.y + offset.y, 0);
            }
        }
    }
}
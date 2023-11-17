using System;
using UnityEngine;

public class UI : MonoBehaviour
{
    public GameObject hands;
    public GameObject bar;
    public CheckHand checkHand;

    void Start()
    {
        hands.SetActive(false);
        bar.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            hands.SetActive(!hands.activeSelf);
        }

        bar.SetActive(checkHand.IsHandAllIn);
    }
}
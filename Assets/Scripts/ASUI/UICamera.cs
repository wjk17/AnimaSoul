using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICamera : MonoSingleton<UICamera>
{
    public Button buttonReset;
    public Toggle toggleRotate;

    public RectTransform rectView;
    // Use this for initialization
    void Start()
    {
        buttonReset.onClick.AddListener(ResetCam);
    }
    private void Update()
    {
        if (ASUI.MouseOver(rectView) && Events.Key(KeyCode.Keypad1)) ResetCam();
    }
    void ResetCam()
    {
        CameraController.I.ResetRotation();
    }
}

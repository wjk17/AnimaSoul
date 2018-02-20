using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICamera : MonoBehaviour {
    public Button buttonReset;
    public Toggle toggleRotate;
    // Use this for initialization
    void Start () {
        buttonReset.onClick.AddListener(ResetCam);
    }
    void ResetCam()
    {
        FindObjectOfType<CameraController>().ResetRotation();
    }
}

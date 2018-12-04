using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Esa.UI
{
    public class UICamera : MonoSingleton<UICamera>
    {
        public Button buttonReset;
        public Toggle toggleRotate;

        public RectTransform rectView;
        void Start()
        {
            this.AddInputCB();
            buttonReset.onClick.AddListener(ResetCam);
        }
        private void Update()
        {
            //if (ASUI.MouseOver(rectView) && Events.Key(KeyCode.Keypad1)) ResetCam();
            if (Events.Key(KeyCode.Keypad1)) ResetCam();
        }
        void ResetCam()
        {
            CameraController.I.ResetRotation();
        }
    }
}
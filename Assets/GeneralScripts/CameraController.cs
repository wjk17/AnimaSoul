﻿//CameraController.cs for UnityChan
//Original Script is here:
//TAK-EMI / CameraController.cs
//https://gist.github.com/TAK-EMI/d67a13b6f73bed32075d
//https://twitter.com/TAK_EMI
//
//Revised by N.Kobayashi 2014/5/15 
//Change : To prevent rotation flips on XY plane, use Quaternion in cameraRotate()
//Change : Add the instrustion window
//Change : Add the operation for Mac
//


using UnityEngine;
using System.Collections;

namespace UnityChan
{
    enum MouseButtonDown
    {
        MBD_LEFT = 0,
        MBD_RIGHT,
        MBD_MIDDLE,
    };

    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private GameObject pivotGO = null;

        public bool showInstWindow = true;

        private Vector3 oldPos;
        public bool trackOnX;
        public bool trackOnY;
        public Space space = Space.World;
        public bool wheelControlOn = true;
        public float orthoCamSize;
        public Vector3 orthoCamSizeMOM;
        public float tX;
        public float wheelSensitivity = 0.25f;
        public float dragSensitivity = 0.5f;
        public float trackSensitivity = 0.7f;
        private void Awake()
        {
            pivotGO.transform.rotation = transform.rotation;//否则轴与摄像机旋转不一致会导致cameraRotate方向异常。
            transform.SetParent(pivotGO.transform);
        }
        private void Start()
        {
            orthoCamSize = Camera.main.orthographicSize;
            SyncCamSize();
        }
        public void SyncCamSize()
        {
            var cams = Camera.main.GetComponentsInChildren<Camera>(true);
            foreach (var cam in cams)
            {
                cam.orthographicSize = orthoCamSize;
            }
        }
        void Update()
        {
            mouseEvent();
        }
        //Show Instrustion Window
        public int x = 5;
        public int y = 5;
        public float timer = 0;
        void OnGUI()
        {
            if (showInstWindow)
            {
                timer += Time.deltaTime;
                if (GUI.Button(new Rect(x, y, 200, 90), "") && timer > 0.5f) { showInstWindow = false; }
                GUI.Label(new Rect(x + 20, y + 10, 200, 30), "摄像机操作（点击隐藏）");
                GUI.Label(new Rect(x + 10, y + 30, 200, 30), "右键 / Alt+左键: 旋转");
                GUI.Label(new Rect(x + 10, y + 50, 200, 30), "中键 / Alt+Cmd/Ctrl+左键: 移动");
                GUI.Label(new Rect(x + 10, y + 70, 200, 30), "滚轮 / 2 指滑动: 推拉");
            }
        }

        void mouseEvent()
        {
            float delta = Input.GetAxis("Mouse ScrollWheel");
            if (delta != 0.0f)
                mouseWheelEvent(delta);

            if (Input.GetMouseButtonDown((int)MouseButtonDown.MBD_LEFT) ||
                Input.GetMouseButtonDown((int)MouseButtonDown.MBD_MIDDLE) ||
                Input.GetMouseButtonDown((int)MouseButtonDown.MBD_RIGHT))
                oldPos = Input.mousePosition;

            mouseDragEvent(Input.mousePosition);
        }

        void mouseDragEvent(Vector3 mousePos)
        {
            Vector3 diff = mousePos - oldPos;

            if (Input.GetMouseButton((int)MouseButtonDown.MBD_LEFT))
            {
                //Operation for Mac : "Left Alt + Left Command + LMB Drag" is Track
                if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftCommand)
                    || Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
                {
                    if (diff.magnitude > Vector3.kEpsilon)
                        cameraTranslate(-diff / 100.0f);
                }
                //Operation for Mac : "Left Alt + LMB Drag" is Tumble
                else if (Input.GetKey(KeyCode.LeftAlt))
                {
                    if (diff.magnitude > Vector3.kEpsilon)
                        cameraRotate(new Vector3(diff.y, diff.x, 0.0f));
                }
                //Only "LMB Drag" is no action.
            }
            //Track
            else if (Input.GetMouseButton((int)MouseButtonDown.MBD_MIDDLE))
            {
                if (diff.magnitude > Vector3.kEpsilon)
                    cameraTranslate(-diff / 100.0f);
            }
            //Tumble
            else if (Input.GetMouseButton((int)MouseButtonDown.MBD_RIGHT))
            {
                if (diff.magnitude > Vector3.kEpsilon)
                    cameraRotate(new Vector3(diff.y, diff.x, 0.0f));
            }
            oldPos = mousePos;
        }
        public Canvas canvas;
        public void mouseWheelEvent(float delta)
        {
            if (canvas.transform.Search("Dropdown List") != null) return;
            if (!wheelControlOn) return;
            if (Mathf.Abs(delta) > 0.01f)
            {
                delta *= wheelSensitivity * dragSensitivity;
                orthoCamSize -= delta;
                if (orthoCamSize > orthoCamSizeMOM.z) orthoCamSize = orthoCamSizeMOM.z;
                else if (orthoCamSize < orthoCamSizeMOM.x) orthoCamSize = orthoCamSizeMOM.x;
                SyncCamSize();
            }
            //if (Mathf.Abs(delta) > 0.01f) transform.localPosition += Vector3.forward * delta;
            //var z = transform.localPosition.z;
            //if (z > -0.5f) z = -0.5f;
            //else if (z < -7.5f) z = -7.5f;
            //transform.localPosition = transform.localPosition.SetZ(z);
        }
        void cameraTranslate(Vector3 vec)
        {
            Transform pivot = pivotGO.transform;

            vec *= trackSensitivity;
            //vec.x *= -1 ;

            if (trackOnX) transform.Translate(Vector3.right * vec.x, space);
            //if (trackOnY) pivot.Translate(Vector3.up * vec.y, space);
            if (trackOnY) transform.Translate(Vector3.up * vec.y, space);
        }

        public void cameraRotate(Vector3 eulerAngle)
        {
            //Use Quaternion to prevent rotation flips on XY plane
            //Quaternion q = Quaternion.identity;

            eulerAngle *= dragSensitivity;

            Transform focusTrans = pivotGO.transform;
            //focusTrans.localEulerAngles = focusTrans.localEulerAngles + eulerAngle;
            //focusTrans.Rotate(eulerAngle);
            //var z = focusTrans.localEulerAngles.z;
            tX -= eulerAngle.x;
            var y = focusTrans.localEulerAngles.y + eulerAngle.y;
            var range = 89;
            if (tX > range) tX = range;
            else if (tX < -range) tX = -range;

            //float limit = 360f;
            //float factor = 1f / limit; // 限制Z轴为360倍数。（不直接==0固定，因为四元数旋转有时会通过将某轴+360°避免欧拉角）
            //var round = Mathf.RoundToInt(z * factor);
            //focusTrans.localEulerAngles = focusTrans.localEulerAngles.SetZ(round * limit);
            focusTrans.localEulerAngles = new Vector3(tX, y, 0);

            //Change this.transform.LookAt(this.focus) to q.SetLookRotation(this.focus)
            //q.SetLookRotation(focus);
        }
    }
}
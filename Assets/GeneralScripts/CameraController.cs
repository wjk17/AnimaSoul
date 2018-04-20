//CameraController.cs for UnityChan
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
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (CameraController)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("ResetRotation"))
        {
            o.ResetRotation();
        }
    }
}
#endif

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
    private Quaternion originRotation;
    public void ResetRotation()
    {
        pivotGO.transform.rotation = originRotation;
        tX = 0;
    }
    private void Awake()
    {
        originRotation = transform.rotation;
        pivotGO.transform.rotation = originRotation;//否则轴与摄像机旋转不一致会导致cameraRotate方向异常。
        transform.SetParent(pivotGO.transform);
    }
    private void Start()
    {
        orthoCamSize = GetComponent<Camera>().orthographicSize;
        SyncCamSize();
        ASUI.I.inputCallBacks.Add(new ASGUI.InputCallBack(GetInput, -1));
    }
    public void SyncCamSize()
    {
        var cams = GetComponentsInChildren<Camera>(true);
        foreach (var cam in cams)
        {
            cam.orthographicSize = orthoCamSize;
        }
        var gizmos = FindObjectOfType<GizmosAxis>();
        if (gizmos != null)
        {
            var factor = orthoCamSize / 2.5f;
            gizmos.transform.localScale = Vector3.one * factor;
        }
    }
    void GetInput()
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
        float delta = Events.Axis("Mouse ScrollWheel");
        if (delta != 0.0f)
            mouseWheelEvent(delta);

        if (Events.MouseDown(MouseButton.Left) ||
            Events.MouseDown(MouseButton.Middle) ||
            Events.MouseDown(MouseButton.Right))
            oldPos = Input.mousePosition;

        if (Events.Mouse(MouseButton.Left) ||
            Events.Mouse(MouseButton.Middle) ||
            Events.Mouse(MouseButton.Right))
            mouseDragEvent(Input.mousePosition);
    }
    public Camera cam { get { return GetComponent<Camera>(); } }
    public Vector2 size
    {
        get { var h = cam.orthographicSize * 2f; var w = h * cam.aspect; return new Vector2(w, h); }
    }
    public Vector3 diff;
    public float mouseMoveTrackFactor = 1f;
    void mouseDragEvent(Vector3 mousePos)
    {
        diff = mousePos - oldPos;
        Vector2 diffN = new Vector2(diff.x / Screen.width, diff.y / Screen.height);
        Vector2 diffWorld = Vector2.Scale(diffN, size);
        if (Events.Mouse(MouseButton.Left) && diff.magnitude > Vector3.kEpsilon)
        {
            //Operation for Mac : "Left Alt + Left Command + LMB Drag" is Track
            if ((Events.Alt && Events.Command) || (Events.Alt && Events.Ctrl))
            {
                var right = Vector3.Dot(diff, Vector2.right);
                var delta = right * mouseMoveTrackFactor;
                delta *= wheelSensitivity * dragSensitivity;
                orthoCamSize -= delta;
                if (orthoCamSize > orthoCamSizeMOM.z) orthoCamSize = orthoCamSizeMOM.z;
                else if (orthoCamSize < orthoCamSizeMOM.x) orthoCamSize = orthoCamSizeMOM.x;
                SyncCamSize();

                var left = Vector3.Dot(diff, Vector2.left);
                Debug.Log("left " + right.ToString());
                Debug.Log("right " + right.ToString());
            }
            else if (Events.Alt && Events.Shift)
            {
                cameraTranslate(-diffWorld);
            }
            //Operation for Mac : "Left Alt + LMB Drag" is Tumble
            else if (Events.Alt)
            {
                cameraRotate(new Vector3(diff.y, diff.x, 0.0f));
            }
            //Only "LMB Drag" is no action.
        }
        //Track
        else if (Events.Mouse(MouseButton.Middle))
        {
            //cameraTranslate(-diff / 100.0f);
            cameraTranslate(-diffWorld);
        }
        //Tumble
        else if (Events.Mouse(MouseButton.Right))
        {
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
        if (!FindObjectOfType<UICamera>().toggleRotate.isOn) return;
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosAxis : MonoBehaviour
{
    public Transform[] handles;
    public Material[] mats;
    public Material selected;
    private Vector3 oldPos;
    public int axisIndex;
    public float sensitivity = 0.002f;
    public Vector3 originPosition;
    public Transform controlObj
    {
        set
        {
            _controlObj = value;
            if (value != null) transform.position = value.position;
        }
        get { return _controlObj; }
    }
    [SerializeField]
    private Transform _controlObj;
    public float wheelSensitivity = 20f;
    public bool prevDown;

    public enum Axis
    {
        x, y, z
    }
    void Update()
    {
        CheckClick();
        mouseDragEvent(Input.mousePosition);
    }
    public Vector2 diff;
    public float diffProj;
    public Vector3 diffWorld3;
    public Vector2 axi;
    public Vector2 axisProj;
    public float screenProj;
    public Vector2 localIP;
    public float dotProj;
    public Vector2 iPos;
    private void mouseDragEvent(Vector3 mousePos)
    {
        //if (!Input.GetMouseButton(0)) { axisIndex = -1; return; }
        iPos = mousePos;
        diff = (mousePos - oldPos);
        var cc = FindObjectOfType<CameraController>();
        Vector2 diffN = new Vector2(diff.x / Screen.width, diff.y / Screen.height);
        Vector2 diffLocal = Vector2.Scale(diffN, cc.size);
        var cam = cc.cam.transform;
        diffWorld3 = diffLocal;

        Axis axis = (Axis)axisIndex;
        Vector3 axisV;
        switch (axis)
        {
            case Axis.x: axisV = transform.right; break;
            case Axis.y: axisV = transform.up; break;
            case Axis.z: axisV = transform.forward; break;
            default: axisV = transform.forward; break;// return;
        }

        var localAxisV = cam.InverseTransformDirection(axisV);
        localAxisV = MathTool.ReverseX(localAxisV);
        localAxisV.y = -localAxisV.y;
        axi = localAxisV;

        //p1 = new Vector2(800, 450);
        //p2 = p1 + forward * length;

        //p3 = p1;
        //localIP = ASUI.mousePositionRef - new Vector2(800, 450);
        //p4 = p1 + localIP;
        //dotProj = Vector2.Dot(localIP, forward * length);

        axisProj = new Vector2(
            Vector3.Dot(Vector3.right, localAxisV),
            Vector3.Dot(Vector3.up, localAxisV));
        axi = axisProj;

        var diff2 = iPos - prevPos;
        axi = axi.normalized;
        //screenProj = Vector2.Dot(diff2, forward);
        screenProj = Vector2.Dot(diff, axi);

        p1 = new Vector2(800, 450);
        p2 = p1 + axi * length;

        //screenProj = Vector2.Dot(diffLocal, forwardProj);
        //screenProj = Vector2.Dot(diffLocal, forward);
        p3 = p1;
        p4 = p1 + axi * screenProj * ASUI.facterToReference;
        var v = axi * screenProj;
        var n = new Vector2(v.x / Screen.width, v.y / Screen.height);
        var scnV = Vector2.Scale(n, cc.size);
        if (!Input.GetMouseButton(0)) { axisIndex = -1; return; }
        screenProj = Mathf.Sign(screenProj) * scnV.magnitude;

        if (Mathf.Abs(screenProj) > ASUI.Epsilon)
        {
            AxisTranslate(screenProj * sensitivity);
        }

        oldPos = mousePos;
    }
    public float length = 100;
    public Vector2 p1, p2;
    public Vector2 p3, p4;
    //private void OnRenderObject()
    //{
    //    GLUI.DrawLineWidthIns(p3, p4, 5, Color.red);
    //    GLUI.DrawLineWidthIns(p1, p2, 5);
    //}
    private void AxisTranslate(float len)
    {
        Axis axis = (Axis)axisIndex;
        var space = Space.World;
        switch (axis)
        {
            case Axis.x:
                transform.Translate(Vector3.right * len, space);
                break;
            case Axis.y:
                transform.Translate(Vector3.down * len, space);
                break;
            case Axis.z:
                transform.Translate(Vector3.forward * len, space);
                break;
            default:
                break;
        }
        if (_controlObj != null) _controlObj.position = transform.position;
    }
    void SetMats(Transform t, Material mat)
    {
        foreach (var render in t.GetComponentsInChildren<Renderer>())
        {
            render.material = mat;
        }
    }
    Vector2 prevPos;
    void CheckClick()
    {
        if (prevDown == true && Input.GetMouseButton(0) == false)
        {
            prevDown = false;
        }
        if (!Input.GetMouseButtonDown(0)) return;
        prevPos = Input.mousePosition;
        oldPos = Input.mousePosition;
        var mask = 1 << LayerMask.NameToLayer("Gizmos");
        Ray ray = FindObjectOfType<CameraController>().cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool bHit = Physics.Raycast(ray, out hit, Mathf.Infinity, mask, QueryTriggerInteraction.Collide);
        if (bHit)
        {
            prevDown = true;
            originPosition = hit.transform.position;
            var n = hit.transform.name;
            switch (n)
            {
                case "x": axisIndex = 0; break;
                case "y": axisIndex = 1; break;
                case "z": axisIndex = 2; break;
                default: throw new Exception();
            }
            for (int i = 0; i < handles.Length; i++)
            {
                if (i == axisIndex) continue;
                SetMats(handles[i], mats[i]);
            }
            SetMats(handles[axisIndex], selected);
        }
    }
}

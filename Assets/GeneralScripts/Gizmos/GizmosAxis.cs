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
    public int index;
    public float sensitivity = 0.002f;
    public Vector3 originPosition;
    public Action GetOriginCallBack;
    public Transform controlObj
    {
        set
        {
            _controlObj = value;
            if (value == null) return;
            transform.position = value.position;
        }
        get { return _controlObj; }
    }
    [SerializeField]
    private Transform _controlObj;
    public float wheelSensitivity = 20f;
    public float dragSensitivity;
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
    private void mouseDragEvent(Vector3 mousePos)
    {
        if (!Input.GetMouseButton(0)) { index = -1; return; }
        float delta = Input.GetAxis("Mouse ScrollWheel");        
        if (delta != 0.0f)
        {
            //FindObjectOfType<SliderMgr>().proportionalEditRangeSlider.value += delta * wheelSensitivity;
        }
        Vector3 diff = (mousePos - oldPos) * dragSensitivity;
        if (diff.magnitude > Vector3.kEpsilon)
        {
            AxisTranslate(-diff * sensitivity);
        }
        oldPos = mousePos;
    }
    private void AxisTranslate(Vector3 vec)
    {
        Axis axis = (Axis)index;
        var space = Space.World;
        switch (axis)
        {
            case Axis.x:
                vec.x *= -1;
                transform.Translate(Vector3.right * vec.x, space);
                break;
            case Axis.y:
                vec.y *= -1;
                transform.Translate(Vector3.up * vec.y, space);
                break;
            case Axis.z:
                vec.x *= -Mathf.Sign(Camera.main.transform.position.x);
                transform.Translate(Vector3.forward * vec.x, space);
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
    void CheckClick()
    {
        if (prevDown == true && Input.GetMouseButton(0) == false)
        {
            prevDown = false;
            if (GetOriginCallBack != null) GetOriginCallBack();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (GetOriginCallBack != null) GetOriginCallBack();
        }
        if (Input.GetMouseButton(0) == false)
        {
            FindObjectOfType<CameraController>().wheelControlOn = true;
        }
        if (!Input.GetMouseButtonDown(0)) return;
        oldPos = Input.mousePosition;
        var mask = 1 << LayerMask.NameToLayer("Gizmos");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist = 99999f;
        RaycastHit hit;
        bool bHit = Physics.Raycast(ray, out hit, dist, mask, QueryTriggerInteraction.Collide);
        if (bHit)
        {
            prevDown = true;
            FindObjectOfType<CameraController>().wheelControlOn = false;
            originPosition = hit.transform.position;
            GetOriginCallBack();
            var n = hit.transform.name;
            switch (n)
            {
                case "x": index = 0; break;
                case "y": index = 1; break;
                case "z": index = 2; break;
                default: throw new Exception();
            }
            for (int i = 0; i < handles.Length; i++)
            {
                if (i == index) continue;
                SetMats(handles[i], mats[i]);
            }
            SetMats(handles[index], selected);
        }
    }
}

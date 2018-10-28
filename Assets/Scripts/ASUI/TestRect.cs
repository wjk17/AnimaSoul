using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TestRect : MonoBehaviour
{
    RectTransform rt;
    public Rect rect;
    public Vector2 anchoredPosition;
    public Vector2 sizeDelta;
    public Vector2 offsetMin;
    public Vector2 offsetMax;
    //public Rect rectInParentSpace;
    public Vector2 anchorMin;
    public Vector2 anchorMax;
    public Vector2 AbsRefPos;
    // Use this for initialization
    void Start()
    {
        rt = transform as RectTransform;
    }
    // Update is called once per frame
    void Update()
    {
        rect = rt.rect;
        anchoredPosition = rt.anchoredPosition;
        sizeDelta = rt.sizeDelta;
        offsetMin = rt.offsetMin;
        offsetMax = rt.offsetMax;
        //rectInParentSpace = GetRectInParentSpace();
        anchorMin = rt.anchorMin;
        anchorMax = rt.anchorMax;

        AbsRefPos = ASUI.AbsRefPos(rt);
    }
    internal Rect GetRectInParentSpace()
    {
        Rect rect = this.rect;
        Vector2 vector = this.offsetMin + Vector2.Scale(rt.pivot, rect.size);
        Transform parent = base.transform.parent;
        if (parent != null)
        {
            RectTransform component = parent.GetComponent<RectTransform>();
            if (component != null)
            {
                vector += Vector2.Scale(rt.anchorMin, component.rect.size);
            }
        }
        rect.x += vector.x;
        rect.y += vector.y;
        return rect;
    }
}

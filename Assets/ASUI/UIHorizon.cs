using System.Collections.Generic;
using UnityEngine;

public class UIHorizon
{
    public List<RectTransform> rts;
    public RectTransform current;
    float y;
    public float bottom
    {
        get { return current.anchoredPosition.y - (current == null ? 0 : current.sizeDelta.y); }
    }
    public float right
    {
        get { return current == null ? 0 : current.anchoredPosition.x + current.sizeDelta.x; }
    }
    public void Add(GameObject go)
    {
        Add(go.GetComponent<RectTransform>());
    }
    public void Add(Component com)
    {
        Add(com.GetComponent<RectTransform>());
    }
    public void Add(RectTransform rt)
    {
        rts.Add(rt);
        rt.anchoredPosition = new Vector2(right, y);
        current = rt;
    }
    public UIHorizon(float y)
    {
        this.y = y;
        rts = new List<RectTransform>();
    }
}

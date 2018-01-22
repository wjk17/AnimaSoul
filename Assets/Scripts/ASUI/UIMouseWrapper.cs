using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIMouseEventWrapper : MonoBehaviour
{
    public Action onMouseOver;
    public Action onMouseDown;
    public Action onMouseDrag;
    public void CreateBox2D()
    {
        var box = gameObject.AddComponent<BoxCollider2D>();
        var rt = (transform as RectTransform);
        box.size = rt.rect.size;
        box.offset = Vector2.Scale(rt.rect.size, Vector2.one * 0.5f - rt.pivot);
    }
    private void OnMouseOver()
    {
        if (onMouseOver!=null) onMouseOver();
    }
    private void OnMouseDown()
    {
        if (onMouseDown != null) onMouseDown();
    }
    private void OnMouseDrag()
    {
        if (onMouseDrag != null) onMouseDrag();
    }
}
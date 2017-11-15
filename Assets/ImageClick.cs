﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageClick : MonoBehaviour, IPointerDownHandler
{
    [Range(0, 1)]
    public float on = 0.3f;
    [Range(0, 1)]
    public float off = 0.7f;
    UnityEngine.UI.Image image;
    UnityChan.CameraController cam;
    public RectTransform rt;
    Vector2 sizeOrigin;
    void Start()
    {
        cam = FindObjectOfType<UnityChan.CameraController>();
        if (cam == null) { enabled = false; return; }
        image =transform.GetComponent<UnityEngine.UI.Image>();
        sizeOrigin = image.rectTransform.sizeDelta;
    }
    void Update()
    {
        image.color = new Color(0, 0, 0, cam.showInstWindow ? on : off);
        image.rectTransform.sizeDelta = cam.showInstWindow ? rt.sizeDelta : sizeOrigin;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        cam.showInstWindow = !cam.showInstWindow;
        cam.timer = 0;
    }
}

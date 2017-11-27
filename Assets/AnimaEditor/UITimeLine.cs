using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI控件
public class UITimeLine : MonoBehaviour
{
    public float realtime;
    public float normalizedTime;
    public int XSpaceInRuler; // 时间线指针UI的移动步长
    public float XSpaceInArea; // 标尺每隔多少帧有一个帧数标签显示
    public Canvas canvas;
    private RectTransform area;
    private RectTransform ruler;
    private RectTransform cursor;
    private CanvasScaler scaler;

    private float xFactor;
    private float leftTimer;
    private float rightTimer;
    public float continuousKeyTime = 0.5f; // 上下左右键连发延迟
    public float continuousKeyInterval = 0.01f; // 间隔（其实0.01通常约等于每帧触发）
    public Text uiFrameIndex;
    public bool hover;
    public Vector3 mousePos;
    public Rect uiRect;
    public Vector2 pos2D;
    public int fontSize;

    private bool _hover;
    private int _frameIndex;

    public int frameIndex
    {
        get
        {
            return _frameIndex;
        }
        set
        {
            _frameIndex = value;
            uiFrameIndex.text = "帧：" + _frameIndex.ToString();
        }
    }
    void Start()
    {
        scaler = canvas.GetComponent<CanvasScaler>();
        xFactor = 1 / XSpaceInArea;
        area = transform.Search("Area") as RectTransform;
        ruler = transform.Search("Ruler") as RectTransform;
        cursor = transform.Search("Cursor") as RectTransform;
        var mouse = area.gameObject.AddComponent<UIMouseEventWrapper>();
        mouse.CreateBox2D();
        mouse.onMouseDown = MouseDown;
        mouse.onMouseDrag = MouseDrag;
        mouse.onMouseOver = MouseOver;

        InitGUI();
    }
    private void InitGUI()
    {
        //GLUI.Init();
        ASUI.parent = area;
        ASUI.BeginHorizon();

        ASUI.EndHorizon();
    }
    private void OnGUI()
    {
        UpdateIMUI();
    }
    private void OnRenderObject()
    {
        
    }
    public int accurate;
    public float radius;
    void UpdateIMUI()
    {
        GLUI.BeginOrtho();
        GLUI.accurate = accurate;
        GLUI.radius = radius;
        IMUI.fontSize = fontSize;
        float x, y;
        for (int i = 0; i < 1000; i++)
        {
            if ((i % XSpaceInRuler) == 0)
            {
                x = i * XSpaceInArea;
                if (x > ruler.sizeDelta.x) break;
                x += ruler.anchoredPosition.x;
                y = -ruler.anchoredPosition.y;
                GLUI.DrawLineOrtho(new Vector2(x, y), new Vector2(x, y + ruler.sizeDelta.y));
                IMUI.DrawText(i.ToString(), new Vector2(x, y));
            }
        }
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            leftTimer += Time.deltaTime;
        }
        else { leftTimer = 0; }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rightTimer += Time.deltaTime;
        }
        else { rightTimer = 0; }
        if (leftTimer > continuousKeyTime || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftTimer -= continuousKeyInterval;
            frameIndex--;
        }
        else if (rightTimer > continuousKeyTime || Input.GetKeyDown(KeyCode.RightArrow))
        {
            rightTimer -= continuousKeyInterval;
            frameIndex++;
        }
        hover = _hover;
    }
    private void MouseDown()
    {
    }
    private void MouseOver()
    {
        _hover = true;
    }
    private void LateUpdate()
    {
        _hover = false;
    }
    private void MouseDrag()
    {
        pos2D = GetLocalMousePos();
        var x = (pos2D - area.anchoredPosition).x;
        frameIndex = Mathf.RoundToInt(x * xFactor);
        cursor.anchoredPosition = new Vector2(frameIndex * XSpaceInArea, cursor.anchoredPosition.y);
    }
    Vector2 GetLocalMousePos()
    {
        return GetLocalPos(Input.mousePosition);
    }
    Vector2 GetLocalPos(Vector3 screenPos)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, screenPos, canvas.worldCamera, out pos);
        pos += scaler.referenceResolution * 0.5f;
        return pos;
    }
}
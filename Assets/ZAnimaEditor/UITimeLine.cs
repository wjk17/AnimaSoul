using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI控件
public class UITimeLine : MonoBehaviour
{
    public float realtime;
    public float normalizedTime;
    public RectTransform rect;
    public RectTransform cursor;
    public float XSpaceInRect;
    public Canvas canvas;
    public CanvasScaler scaler;
    public Transform trans;
    public int frameIndex;
    private float factor;
    void Start()
    {
        scaler = canvas.GetComponent<CanvasScaler>();
        factor = 1 / XSpaceInRect;
    }
    private float leftTimer;
    private float rightTimer;
    public float continuousKeyTime = 0.5f;
    public float continuousKeyInterval = 0.01f;
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
        Vector2 pos2D;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos2D);
        pos2D += scaler.referenceResolution * 0.5f;
        bool hover = RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition);
        if (Input.GetMouseButton(0) && hover)
        {
            var x = (pos2D - rect.anchoredPosition).x;

            frameIndex = Mathf.RoundToInt(x * factor);
        }
        cursor.anchoredPosition = new Vector2(frameIndex * XSpaceInRect, cursor.anchoredPosition.y);
    }
}

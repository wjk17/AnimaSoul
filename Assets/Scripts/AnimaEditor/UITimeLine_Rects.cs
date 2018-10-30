using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UITimeLine
{
    /// <summary>
    /// 
    /// </summary>
    RectTransform rt { get { return (transform as RectTransform); } }
    Vector2 rtSize { get { return rt.sizeDelta; } } // 曲线视图区域大小
    Vector2 rtPos
    {
        get
        {
            _rtPos = rt.anchoredPosition;
            _rtPos.y = -_rtPos.y;
            _rtPos.y = UI.scaler.referenceResolution.y - _rtPos.y;
            return _rtPos;
        }
    }
    Vector2 _rtPos;

    /// <summary>
    /// 
    /// </summary>
    RectTransform rtArea { get { return (transform.Search("Area") as RectTransform); } }
    Vector2 rtAreaSize { get { return rtArea.sizeDelta; } } // ������ͼ������С
    Vector2 rtAreaPos
    {
        get
        {
            _rtAreaPos = rtArea.anchoredPosition;
            _rtAreaPos.y = -_rtAreaPos.y;
            return rtPos + _rtAreaPos;
        }
    }    
    Vector2 _rtAreaPos;


    /// <summary>
    /// ruler
    /// </summary>
    RectTransform ruler { get { return (transform.Search("Ruler X") as RectTransform); } }
    Vector2 rulerSize { get { return ruler.sizeDelta; } }
    Vector2 rulerPos
    {
        get
        {
            _rulerPos = ruler.anchoredPosition.ReverseY();
            return  _rulerPos;
        }
    }
    Vector2 _rulerPos;
}

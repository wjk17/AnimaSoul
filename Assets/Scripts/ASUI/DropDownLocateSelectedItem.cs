using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropDownLocateSelectedItem : MonoBehaviour, IPointerClickHandler
{
    public Dropdown drop;
    public RectTransform scrollbar;
    public Transform list;
    public void OnPointerClick(PointerEventData eventData)
    {
        LocateSelectedItem();
    }
    void Start()
    {
        drop = GetComponent<Dropdown>();
        list = transform.Search("Dropdown List");
    }
    void Update()
    {
        if (list != null)
        {
            scrollbar = list.Search("Scrollbar") as RectTransform;

            if (scrollbar != null)
            {
                scrollbar.anchoredPosition = scrollbar.anchoredPosition.SetX(0);
                scrollbar.sizeDelta = scrollbar.sizeDelta.SetX(28);
                //var slidingArea = scrollbar.Search("Sliding Area") as RectTransform;
                //if (slidingArea != null)
                //{
                //    slidingArea.anchoredPosition
                //}
            }
        }
    }
    void LocateSelectedItem()
    {
        list = transform.Search("Dropdown List");
        if (list != null)
        {
            var content = list.Search("Content");
            if (content != null)
            {
                var listH = (list as RectTransform).rect.height;
                var contentHeight = (content as RectTransform).rect.height; ;
                var n = (float)drop.value / drop.options.Count;
                var dropH = (transform as RectTransform).rect.height;
                var y = Mathf.Clamp(n * contentHeight - listH * 0.5f + dropH * 0.5f, 0, contentHeight);
                (content as RectTransform).anchoredPosition = new Vector2(0, y);
            }
        }
    }
}

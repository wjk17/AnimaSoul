using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropDownLocateSelectedItem : MonoBehaviour, IPointerClickHandler
{
    public Dropdown drop;
    public void OnPointerClick(PointerEventData eventData)
    {
        LocateSelectedItem();
    }
    void Start()
    {
        drop = GetComponent<Dropdown>();
    }
    void LocateSelectedItem()
    {
        var list = transform.Search("Dropdown List");
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

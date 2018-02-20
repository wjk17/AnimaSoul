using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class KeepUIPosition : MonoBehaviour
{
    [System.Serializable]
    public class pair
    {
        public RectTransform key;
        public Vector2 value;
    }
    public List<pair> dict;
    public bool log;
    public bool set;
    [ContextMenu("GetPos")]
    void GetPos()
    {
        dict = new List<pair>();
        foreach (var rt in GetComponentsInChildren<RectTransform>())
        {
            if (rt.parent == transform)
            {
                dict.Add(new pair() { key = rt, value = rt.anchoredPosition });
                if (log) Debug.Log(rt.name);
            }
        }
    }
    [ContextMenu("SetPos")]
    void SetPos()
    {
        foreach (var item in dict)
        {
            item.key.anchoredPosition = item.value;
            if (log) Debug.Log(item.key.name);
        }
    }
    private void Update()
    {
        if (set) SetPos();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIIKTargetList : MonoBehaviour
{
    public GameObject itemPrefab;
    public List<GameObject> buttons;
    public float ySpace = 10;
    public float startY = 100;
    void Start()
    {
        for (int i = 0; i < (int)IKTargetSingle.Count; i++)
        {
            var btn = Instantiate(itemPrefab, transform);
            var gridHeight = (itemPrefab.transform as RectTransform).rect.size.y;
            btn.transform.SetUIPosY(startY - i * (gridHeight + ySpace));
            buttons.Add(btn);
            btn.GetComponentInChildren<Text>().text = UIDOFEditor.ikTargetSingleNames[i];
            int idx = i;
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                UIDOFEditor.I.f.dropIKSingleTarget.value = idx;
                UIDOFEditor.I.f.buttonIKSingleSnap.onClick.Invoke();
                UIDOFEditor.I.f.toggleIKSingle.isOn = true;
                // Bug 第一次点击时变成了关节链IK
            });            
        }
        //for (int i = 0; i < (int)IKTarget.Count; i++)
        //{
        //    var btn = Instantiate(itemPrefab, transform);
        //    btn.transform.SetLocalPosY(startY + i * ((itemPrefab.transform as RectTransform).rect.size.y + ySpace));
        //    buttons.Add(btn);
        //    btn.GetComponentInChildren<Text>().text = UIDOFEditor.ikTargetNames[i];
        //    int idx = i;
        //    btn.GetComponent<Button>().onClick.AddListener(() =>
        //    {
        //        UIDOFEditor.I.f.dropIKTarget.value = idx;
        //        UIDOFEditor.I.f.buttonIKSnap.onClick.Invoke();
        //        UIDOFEditor.I.f.toggleIK.isOn = true;
        //    });
        //}
    }
}

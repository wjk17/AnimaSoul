using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDOFControl : MonoBehaviour
{
    public float open;
    public bool right;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var list = new List<ASBone>();
        var listAST = new List<ASTransDOF>();

        list.Add(ASBone.thumb1_l);
        list.Add(ASBone.thumb2_l);
        list.Add(ASBone.thumb3_l);

        list.Add(ASBone.index1_l);
        list.Add(ASBone.index2_l);
        list.Add(ASBone.index3_l);

        list.Add(ASBone.middle1_l);
        list.Add(ASBone.middle2_l);
        list.Add(ASBone.middle3_l);

        list.Add(ASBone.ring1_l);
        list.Add(ASBone.ring2_l);
        list.Add(ASBone.ring3_l);

        list.Add(ASBone.pinky1_l);
        list.Add(ASBone.pinky2_l);
        list.Add(ASBone.pinky3_l);

        foreach (var i in list)
        {
            listAST.Add(UIDOFEditor.I.avatar[right ? i + 1 : i]);
        }
        foreach (var i in listAST)
        {
            i.euler.x = open;
        }
    }
}

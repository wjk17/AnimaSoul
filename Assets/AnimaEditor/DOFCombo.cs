using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOFCombo : MonoBehaviour
{
    public ASAvatar avatar;
    [Range(0, 1)]
    public float handCloseValue = 0;
    public bool rightHand = true;
    void Update()
    {
        UpdateFingersCloseValue();
    }
    void UpdateFingersCloseValue()
    {
        var dofs = new List<ASTransDOF>();
        var bones = new List<ASBone>();
        bones.Add(ASBone.index1_l);
        bones.Add(ASBone.index2_l);
        bones.Add(ASBone.index3_l);
        bones.Add(ASBone.thumb1_l);
        bones.Add(ASBone.thumb2_l);
        bones.Add(ASBone.thumb3_l);
        bones.Add(ASBone.middle1_l);
        bones.Add(ASBone.middle2_l);
        bones.Add(ASBone.middle3_l);
        bones.Add(ASBone.ring1_l);
        bones.Add(ASBone.ring2_l);
        bones.Add(ASBone.ring3_l);
        bones.Add(ASBone.pinky1_l);
        bones.Add(ASBone.pinky2_l);
        bones.Add(ASBone.pinky3_l);

        foreach (var bone in bones)
        {
            dofs.Add(avatar[rightHand ? bone + 1 : bone]);
        }
        float value;
        foreach (var t in dofs)
        {
            //value = t.dof.swingZMin + handCloseValue * (t.dof.swingZMax - t.dof.swingZMin);
            //t.euler.z = value;
            value = t.dof.swingXMin + handCloseValue * (t.dof.swingXMax - t.dof.swingXMin);
            t.euler.x = value;
            t.Update();
        }
    }
}

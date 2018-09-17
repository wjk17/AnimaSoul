using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOFCombo : MonoBehaviour
{
    public Avator avatar;
    [Range(0, 1)]
    public float handCloseValue = 0;
    public bool rightHand = true;
    void Update()
    {
        UpdateFingersCloseValue();
    }
    void UpdateFingersCloseValue()
    {
        var dofs = new List<TransDOF>();
        var bones = new List<Bone>();
        bones.Add(Bone.index1_l);
        bones.Add(Bone.index2_l);
        bones.Add(Bone.index3_l);
        bones.Add(Bone.thumb1_l);
        bones.Add(Bone.thumb2_l);
        bones.Add(Bone.thumb3_l);
        bones.Add(Bone.middle1_l);
        bones.Add(Bone.middle2_l);
        bones.Add(Bone.middle3_l);
        bones.Add(Bone.ring1_l);
        bones.Add(Bone.ring2_l);
        bones.Add(Bone.ring3_l);
        bones.Add(Bone.pinky1_l);
        bones.Add(Bone.pinky2_l);
        bones.Add(Bone.pinky3_l);

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

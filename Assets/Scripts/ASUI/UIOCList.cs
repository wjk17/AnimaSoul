using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOCList : MonoSingleton<UIOCList>
{
    List<Bone> bones;
    void Start()
    {
        //this.AddInputCB();
        //bones = new List<Bone>(){
        //    Bone.hand_l,Bone.forearm_l,
        //    Bone.hand_r,Bone.forearm_r};
        //for (int i = 0; i < (int)IKTargetSingle.Count - 1; i++)
        //{
        //    var curve = UIClip.I.clip.GetCurve((Bone)i);
        //    if (curve != null) bones.Add(curve.ast);
        //}
    }
    void Update()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDOFControl : MonoBehaviour
{
    //public float open;
    [Range(0, 1)] public float finger1;
    public bool isFinger1;
    [Range(0, 1)] public float finger2;
    public bool isFinger2;
    [Range(0, 1)] public float finger3;
    public bool isFinger3;
    [Range(0, 1)] public float finger4;
    public bool isFinger4;
    [Range(0, 1)] public float finger5;
    public bool isFinger5;

    [Range(0, 1)] public float fingerAll;
    public bool isFingerAll;

    public bool right;
    public bool section1;
    public bool section2;
    public bool section3;

    public List<Bone> list;
    public List<TransDOF> listAST;
    void Start()
    {
        list = new List<Bone>();
        listAST = new List<TransDOF>();

        list.Add(Bone.thumb1_l);
        list.Add(Bone.thumb2_l);
        list.Add(Bone.thumb3_l);

        list.Add(Bone.index1_l);
        list.Add(Bone.index2_l);
        list.Add(Bone.index3_l);

        list.Add(Bone.middle1_l);
        list.Add(Bone.middle2_l);
        list.Add(Bone.middle3_l);

        list.Add(Bone.ring1_l);
        list.Add(Bone.ring2_l);
        list.Add(Bone.ring3_l);

        list.Add(Bone.pinky1_l);
        list.Add(Bone.pinky2_l);
        list.Add(Bone.pinky3_l);

        foreach (var i in list)
        {
            listAST.Add(UIDOFEditor.I.avatar[i]);
        }
        foreach (var i in list)
        {
            listAST.Add(UIDOFEditor.I.avatar[i + 1]);
        }
    }
    void Update()
    {
        if (isFinger1) SetFingerValue(0, finger1);
        if (isFinger2) SetFingerValue(3, finger2);
        if (isFinger3) SetFingerValue(6, finger3);
        if (isFinger4) SetFingerValue(9, finger4);
        if (isFinger5) SetFingerValue(12, finger5);
        if (isFingerAll)
        {
            SetFingerValue(0, fingerAll);
            SetFingerValue(3, fingerAll);
            SetFingerValue(6, fingerAll);
            SetFingerValue(9, fingerAll);
            SetFingerValue(12, fingerAll);
        }

        //foreach (var i in listAST)
        //{
        //    i.euler.x = open;
        //}
    }
    void SetFingerValue(int i, float v)
    {
        if (right) i += 15;
        if (section1) SetASTValue(listAST[i], v);
        if (section2) SetASTValue(listAST[i + 1], v);
        if (section3) SetASTValue(listAST[i + 2], v);
    }
    public bool controlX;
    public bool controlZ;
    void SetASTValue(TransDOF ast, float v)
    {
        if (controlX) ast.euler.x = ast.dof.swingXMin + (ast.dof.swingXMax - ast.dof.swingXMin) * v;
        if (controlZ) ast.euler.z = ast.dof.swingZMin + (ast.dof.swingZMax - ast.dof.swingZMin) * v;
    }
}

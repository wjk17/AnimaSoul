using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MixRotation : MonoBehaviour
{
    [Serializable]
    public struct MixUnit // TODO cache ast, not use bone every frame
    {
        public Bone bone;
        public float weight;
    }
    public List<MixUnit> units;
    public TransDOF ast;
    //[Header("// TODO")]
    //public bool x, y, z;
    private void Start()
    {
        ast.Init(transform);
    }
    void Update()
    {
        foreach (var unit in units)
        {
            var target = UIDOFEditor.I.avatar.GetTransDOF(unit.bone);
            ast.euler.y = Mathf.Lerp(ast.euler.y, target.euler.y, unit.weight);
        }
        ast.Update();
    }
}

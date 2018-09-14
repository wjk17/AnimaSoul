using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DOFData
{
    public List<DOF> dofs;
    public DOF GetDOF(Bone bone)
    {
        foreach (var dof in dofs)
        {
            if (bone == dof.bone)
                return dof;
        }
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(DOFMgr))]
public class DOFMgrEditor : E_ShowButtons<DOFMgr> { }
#endif
public class DOFMgr : MonoXmlSL<DOFData>
{
    public DOF this[Bone bone]
    {
        get
        {
            try
            {
                return GetDOF(bone);
            }
            catch
            {
#if UNITY_EDITOR
                throw;
#endif
                return null;
            }
        }
    }
    public DOF GetDOF(Bone bone)
    {
        return data.GetDOF(bone);
    }
}

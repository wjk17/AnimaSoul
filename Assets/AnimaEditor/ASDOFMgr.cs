using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
//[CustomPropertyDrawer(typeof(ASDOF))]
//public class ASDOFDrawer : PropertyDrawer
//{
//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        return h * 3;
//    }
//    public static float nameWidth = 80;
//    public static float labelWidth = 30;
//    public static float valueWidth = 20;
//    internal static float valueX1;
//    internal static float valueX2;
//    public static TextAnchor align;
//    public Rect pos;
//    public float h;
//    public int y;
//    public float x;
//    public int xSlice = 6;

//    Rect Rect(float w, float xOS = 0f)
//    {
//        x += w;
//        return new Rect(pos.x + x - w + xOS , pos.y + y * h, w, h);
//    }
//    public static bool init = false;
//    public static Rect rect1;
//    public static Rect rect2;
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
        
//        var indent = EditorGUI.indentLevel;
//        EditorGUI.indentLevel = 0;
//        EditorGUI.BeginProperty(pos, label, property);
//        pos = position;
//        pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

//        var minProp = property.FindPropertyRelative("twistMin");
//        var maxProp = property.FindPropertyRelative("twistMax");
//        var swingXMinProp = property.FindPropertyRelative("swingXMin");
//        var swingXMaxProp = property.FindPropertyRelative("swingXMax");
//        var swingZMinProp = property.FindPropertyRelative("swingZMin");
//        var swingZMaxProp = property.FindPropertyRelative("swingZMax");
//        h = EditorGUI.GetPropertyHeight(minProp);


//        //var s = EditorStyles.numberField;
//        //s.normal.textColor = Color.red;

//        var s = EditorStyles.label;
//        var o = new GUIStyle(s);
//        s.normal.textColor = Color.red;
//        s.alignment = align;

//        y = 0;
//        x = 0;
//        EditorGUI.LabelField(Rect(nameWidth), new GUIContent("自转"));
//        if (!init)
//        {
//            rect1 = Rect(valueWidth);
//            rect2 = Rect(valueWidth);
//            init = true;
//        }
//        EditorGUI.PropertyField(rect1, minProp, new GUIContent("向内"));
//        EditorGUI.PropertyField(rect2, maxProp, new GUIContent("向外"));
//        //EditorGUILayout.BeginHorizontal();
//        //EditorGUILayout.LabelField(new GUIContent("自转"), GUILayout.Width(nameWidth));
//        //EditorGUILayout.PropertyField(minProp, new GUIContent("向内"), GUILayout.Width(valueWidth));
//        //EditorGUILayout.PropertyField(maxProp, new GUIContent("向外"), GUILayout.Width(valueWidth));
//        //EditorGUILayout.EndHorizontal();
//        //y++;
//        //x = -1;
//        //EditorGUI.LabelField(Rect(nameWidth), new GUIContent("摆动"));
//        //EditorGUI.LabelField(Rect(labelWidth), new GUIContent("向前"));
//        //EditorGUI.PropertyField(Rect(valueWidth), swingXMinProp, GUIContent.none);
//        //EditorGUI.LabelField(Rect(labelWidth), new GUIContent("向后"));
//        //EditorGUI.PropertyField(Rect(valueWidth), swingXMaxProp, GUIContent.none);
//        //y++;
//        //x = -1;
//        //EditorGUI.LabelField(Rect(nameWidth), new GUIContent("    "));
//        //EditorGUI.LabelField(Rect(labelWidth), new GUIContent("向内"));
//        //EditorGUI.PropertyField(Rect(valueWidth), swingZMinProp, GUIContent.none);
//        //EditorGUI.LabelField(Rect(labelWidth), new GUIContent("向外"));
//        //EditorGUI.PropertyField(Rect(valueWidth), swingZMaxProp, GUIContent.none);

//        EditorGUI.indentLevel = indent;
//        EditorGUI.EndProperty();
//        s.normal = o.normal;
//        s.alignment = o.alignment;
//    }
//}
//[CustomPropertyDrawer(typeof(DOFSetting))]
//public class DOFSettingDrawer : PropertyDrawer
//{
//    public Rect pos;
//    public Color c = Color.HSVToRGB(0, 0, 80f / 255f);
//    public List<bool> Foldout = new List<bool>();
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {

//        var indent = EditorGUI.indentLevel;
//        EditorGUI.BeginProperty(position, label, property);
//        EditorGUI.indentLevel = 0;
//        label.text = "各关节活动范围限制（DOF）设置";
//        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
//        EditorGUILayout.LabelField(label);
//        pos = position;

//        ASDOFDrawer.rect1 = EditorGUILayout.RectField(ASDOFDrawer.rect1);
//        ASDOFDrawer.rect2 = EditorGUILayout.RectField(ASDOFDrawer.rect2);
//        ASDOFDrawer.nameWidth = EditorGUILayout.FloatField(new GUIContent("UI_nameWidth"), ASDOFDrawer.nameWidth);
//        ASDOFDrawer.labelWidth = EditorGUILayout.FloatField(new GUIContent("UI_labelWidth"), ASDOFDrawer.labelWidth);
//        ASDOFDrawer.valueWidth = EditorGUILayout.FloatField(new GUIContent("UI_valueWidth"), ASDOFDrawer.valueWidth);
//        ASDOFDrawer.valueX1 = EditorGUILayout.FloatField(new GUIContent("UI_valueX1"), ASDOFDrawer.valueX1);
//        ASDOFDrawer.valueX2 = EditorGUILayout.FloatField(new GUIContent("UI_valueX2"), ASDOFDrawer.valueX2);
//        ASDOFDrawer.align = (TextAnchor)EditorGUILayout.EnumPopup(new GUIContent("UI_align"), ASDOFDrawer.align); 
//        var s = new GUIStyle();
//        c = EditorGUILayout.ColorField("UI_color", c);
//        s.normal.textColor = c;

//        var props = property.FindPropertyRelative("dofs");
//        //var h = EditorGUI.GetPropertyHeight(props.GetArrayElementAtIndex(0));
//        var y = pos.y;
//        while (props.arraySize > Foldout.Count) Foldout.Add(false);
//        for (int i = 0; i < props.arraySize; i++)
//        {
//            var dof = props.GetArrayElementAtIndex(i);
//            y += EditorGUI.GetPropertyHeight(dof);
//            var bone = dof.FindPropertyRelative("bone");
//            var n = new GUIContent(bone.enumDisplayNames[bone.enumValueIndex]);
//            //var y = pos.y + h * i;
//            Foldout[i] = EditorGUILayout.Foldout(Foldout[i], n, true);
//            if (Foldout[i]) EditorGUILayout.PropertyField(dof, GUIContent.none);
//        }

//        EditorGUI.indentLevel = indent;
//        EditorGUI.EndProperty();
//    }
//}
[CustomEditor(typeof(ASDOFMgr))]
public class ASDOFMgrEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (ASDOFMgr)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Load"))
        {
            o.Load();
        }
        if (GUILayout.Button("Save"))
        {
            o.Save();
        }
    }
}
#endif
[Serializable]
public class DOFSetting
{
    public List<ASDOF> dofs;
    public ASDOF GetDOF(ASBone bone)
    {
        foreach (var dof in dofs)
        {
            if (bone == dof.bone)
                return dof;
        }
        return null;
    }
}
public class ASDOFMgr : MonoBehaviour
{
    public ASDOF this[ASBone bone]
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
    public DOFSetting DOFSetting;
    public string path;
    public string folder = "Settings/";
    public string fileName = "DOFSetting.xml";
    public ASDOF GetDOF(ASBone bone)
    {
        return DOFSetting.GetDOF(bone);
    }
    public void Load()
    {
        //DOFSetting.dofs = DOFLiminator.DefaultHumanDOF();
        var dataPath = Application.dataPath;
        var rootPath = dataPath + "/../";
        path = rootPath + folder + fileName;
        DOFSetting = Serializer.XMLDeSerialize<DOFSetting>(path);
    }
    public void Save()
    {
        var dataPath = Application.dataPath;
        var rootPath = dataPath + "/../";
        path = rootPath + folder + fileName;
        Serializer.XMLSerialize(DOFSetting, path);
    }
}

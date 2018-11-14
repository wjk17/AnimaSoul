using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Esa;
public class UIDOFEditor_Fields : MonoBehaviour
{
    public InputField inputTwistMin;
    public InputField inputTwistMax;
    public Button buttonTwistReset;
    public Slider sliderTwist;
    public InputField inputTwist;

    public InputField inputSwingXMin;
    public InputField inputSwingXMax;
    public Button buttonSwingXReset;
    public Slider sliderSwingX;
    public InputField inputSwingX;

    public InputField inputSwingZMin;
    public InputField inputSwingZMax;
    public Button buttonSwingZReset;
    public Slider sliderSwingZ;
    public InputField inputSwingZ;

    public Button buttonSaveDOF;
    public Button buttonSaveClip;
    public Text labelFileName;
    public InputField inputFileName;
    public Button buttonLoadClip;
    public Button buttonNewClip;
    public Button buttonInsertKeyFrame;

    public Toggle toggleIK;
    public Button buttonIKSnap;
    public Toggle toggleIKSingle;
    public Button buttonIKSingleSnap;
    public Toggle toggleWeaponIK;

    public Toggle toggleLockOneSide;
    public Toggle toggleLockMirror;

    public Dropdown dropBone;
    public Dropdown dropIKTarget;
    public Dropdown dropIKSingleTarget;
    public Dropdown dropLockOneSide;
    public Dropdown dropLockMirror;

    public Button buttonEulerReset;

    public bool ignoreChanged;
    [Button]
    void GetUIToField()
    {
        var area = transform.Search("Area");
        var ts = new System.Type[] {
            typeof(Button), typeof(Slider), typeof(Toggle),
            typeof(Dropdown) ,typeof(InputField),typeof(Text)};
        
        this.SetUIFieldByName(area, ts);
    }
}

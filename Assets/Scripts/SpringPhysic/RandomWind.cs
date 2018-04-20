//
//RandomWind.cs for unity-chan!
//
//Original Script is here:
//ricopin / RandomWind.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
//修正2014/12/20
//風の方向変化/重力影響を追加.
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityChan
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(RandomWind))]
    public class RandomWindEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var o = (RandomWind)target;
            base.OnInspectorGUI();
            if (GUILayout.Button("GO"))
            {
                o.ToggleSwitch();
            }
        }
    }
#endif
    public class RandomWind : MonoBehaviour
    {
        public SpringBone[] springBones;
        public bool isWindActive;

        public float windPower;             //風の強さ.
        public float min;
        public float max;
        public float gravity = 0.98f;       //重力の強さ.

        public LabelSlider slider;
        public AnimationCurve curve;
        public float addKeyInterval;
        private float addKeyTimer;
        public bool evaluateCurve;
        public float timeScale;

        private void Awake()
        {
            if (slider != null) slider.Init(windPower, min, max, ChangePower);
        }
        public void ChangePower(float v)
        {
            windPower = v;
        }
        public void GetSpringBones()
        {
            var mgrs = GetComponentsInChildren<SpringManager>();
            foreach (var mgr in mgrs)
            {
                mgr.dynamicRatio = 1f;
            }

            springBones = GetComponentsInChildren<SpringBone>();
            foreach (var bone in springBones)
            {
                bone.GetOriginPos();
            }
        }
        public void ResetSpringBones()
        {
            var mgrs = GetComponentsInChildren<SpringManager>();
            foreach (var mgr in mgrs)
            {
                mgr.dynamicRatio = 0f;
            }

            springBones = GetComponentsInChildren<SpringBone>();
            foreach (var bone in springBones)
            {
                bone.ResetOriginPos();
            }
        }
        public void ToggleSwitch()
        {
            isWindActive = !isWindActive;

            if (isWindActive) GetSpringBones();
            else ResetSpringBones();
        }

        [ContextMenu("GetBones")]
        void GetBones()
        {
            springBones = GetComponentsInChildren<SpringBone>();
            foreach (var bone in springBones)
            {
                bone.GetOriginPos();
            }
        }
        void Update()
        {
            if (evaluateCurve)
            {
                addKeyTimer += Time.deltaTime;
                if (addKeyTimer > addKeyInterval)
                {
                    addKeyTimer = 0f;
                    curve.AddKey(Time.time, Mathf.PerlinNoise(Time.time, 0.0f));
                }
            }
            var v = curve.Evaluate(Time.time * timeScale);
            Vector3 force = Vector3.zero;
            if (isWindActive)
            {
                force = new Vector3(v * windPower * 0.001f, gravity * -0.001f, 0);
                for (int i = 0; i < springBones.Length; i++)
                {
                    springBones[i].springForce = force;
                }

            }
        }
    }
}
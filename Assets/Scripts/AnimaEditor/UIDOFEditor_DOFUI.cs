using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Esa.UI
{
    public partial class UIDOFEditor
    {
        private void OnTwistReset()
        {
            f.sliderTwist.value = 0;
            if (ast == null) ast.euler.y = f.sliderTwist.value;
        }
        private void OnSwingXReset()
        {
            f.sliderSwingX.value = 0;
            if (ast == null) ast.euler.x = f.sliderSwingX.value;
        }
        private void OnSwingZReset()
        {
            f.sliderSwingZ.value = 0;
            if (ast == null) ast.euler.z = f.sliderSwingZ.value;
        }
        private void OnTwistSliderChanged(float v)
        {
            if (f.ignoreChanged) return;
            if (ast != null) { ast.euler.y = v; UIPlayer.I.Mirror(); }
            f.ignoreChanged = true;
            f.inputTwist.text = v.ToString();
            f.ignoreChanged = false;
        }
        private void OnSwingXSliderChanged(float v)
        {
            if (f.ignoreChanged) return;
            if (ast != null) { ast.euler.x = v; UIPlayer.I.Mirror(); }
            f.ignoreChanged = true;
            f.inputSwingX.text = v.ToString();
            f.ignoreChanged = false;
        }
        private void OnSwingZSliderChanged(float v)
        {
            if (f.ignoreChanged) return;
            if (ast != null) { ast.euler.z = v; UIPlayer.I.Mirror(); }
            f.ignoreChanged = true;
            f.inputSwingZ.text = v.ToString();
            f.ignoreChanged = false;
        }
        private void OnTwistInputChanged(string input)
        {
            if (f.ignoreChanged) return;
            if (ast == null) return;
            float result;
            bool success = float.TryParse(input, out result);
            if (success)
            {
                result = Mathf.Clamp(result, ast.dof.twistMin, ast.dof.twistMax); // 只有解析成功的值会赋值                        
                ast.euler.y = result;
                f.ignoreChanged = true;
                f.sliderTwist.value = result;
                f.inputTwist.text = result.ToString();
                f.ignoreChanged = false;
            }
        }
        private void OnSwingXInputChanged(string input)
        {
            if (f.ignoreChanged) return;
            if (ast == null) return;
            float result;
            bool success = float.TryParse(input, out result);
            if (success)
            {
                result = Mathf.Clamp(result, ast.dof.swingXMin, ast.dof.swingXMax);
                ast.euler.x = result;
                f.ignoreChanged = true;
                f.sliderSwingX.value = result;
                f.inputSwingX.text = result.ToString();
                f.ignoreChanged = false;
            }
        }
        private void OnSwingZInputChanged(string input)
        {
            if (f.ignoreChanged) return;
            if (ast == null) return;
            float result;
            bool success = float.TryParse(input, out result);
            if (success)
            {
                result = Mathf.Clamp(result, ast.dof.swingZMin, ast.dof.swingZMax);
                ast.euler.z = result;
                f.ignoreChanged = true;
                f.sliderSwingZ.value = result;
                f.inputSwingZ.text = result.ToString();
                f.ignoreChanged = false;
            }
        }
        // dof范围 最小值或最大值 文本改变事件
        void OnInputXMinChanged(string s)
        {
            if (ast == null) return;
            float result;
            bool success = float.TryParse(f.inputSwingXMin.text, out result);
            if (success)
            {
                ast.dof.swingXMin = result;
                f.ignoreChanged = true;
                f.sliderSwingX.minValue = result;
                f.ignoreChanged = false;
            }
        }
        void OnInputXMaxChanged(string s)
        {
            if (ast == null) return;
            float result;
            bool success = float.TryParse(f.inputSwingXMax.text, out result);
            if (success)
            {
                ast.dof.swingXMax = result;
                f.ignoreChanged = true;
                f.sliderSwingX.maxValue = result;
                f.ignoreChanged = false;
            }
        }
        void OnInputZMinChanged(string s)
        {
            if (ast == null) return;
            float result;
            bool success = float.TryParse(f.inputSwingZMin.text, out result);
            if (success)
            {
                ast.dof.swingZMin = result;
                f.ignoreChanged = true;
                f.sliderSwingZ.minValue = result;
                f.ignoreChanged = false;
            }
        }
        void OnInputZMaxChanged(string s)
        {
            if (ast == null) return;
            float result;
            bool success = float.TryParse(f.inputSwingZMax.text, out result);
            if (success)
            {
                ast.dof.swingZMax = result;
                f.ignoreChanged = true;
                f.sliderSwingZ.maxValue = result;
                f.ignoreChanged = false;
            }
        }
        void OnInputTMinChanged(string s)
        {
            if (ast == null) return;
            float result;
            bool success = float.TryParse(f.inputTwistMin.text, out result);
            if (success)
            {
                ast.dof.twistMin = result;
                f.ignoreChanged = true;
                f.sliderTwist.minValue = result;
                f.ignoreChanged = false;
            }
        }
        void OnInputTMaxChanged(string s)
        {
            if (ast == null) return;
            float result;
            bool success = float.TryParse(f.inputTwistMax.text, out result);
            if (success)
            {
                ast.dof.twistMax = result;
                f.ignoreChanged = true;
                f.sliderTwist.maxValue = result;
                f.ignoreChanged = false;
            }
        }
        // 将最新的数值显示到面板
        internal void UpdateValueDisplay()
        {
            if (ast == null) return;
            f.ignoreChanged = true;
            f.inputTwist.text = ast.euler.y.ToString();
            f.inputSwingX.text = ast.euler.x.ToString();
            f.inputSwingZ.text = ast.euler.z.ToString();
            f.sliderTwist.value = ast.euler.y;
            f.sliderSwingX.value = ast.euler.x;
            f.sliderSwingZ.value = ast.euler.z;
            f.ignoreChanged = false;
        }
        public System.Action onDropdownChanged;
        void OnDropdownChanged(int index)
        {
            var boneInt = (Bone)index;
            dof = dofSet[boneInt];
            ast = avatar[boneInt];
            UpdateDOF();
            if (onDropdownChanged != null) onDropdownChanged();
        }
        void UpdateDOF()
        {
            if (ast == null || dof == null) return;
            f.ignoreChanged = true;
            f.sliderTwist.value = ast.euler.y;
            f.sliderSwingX.value = ast.euler.x;
            f.sliderSwingZ.value = ast.euler.z;
            f.inputTwist.text = ast.euler.y.ToString();
            f.inputSwingX.text = ast.euler.x.ToString();
            f.inputSwingZ.text = ast.euler.z.ToString();
            f.ignoreChanged = false;
            f.inputTwistMin.text = ast.dof.twistMin.ToString();
            f.inputTwistMax.text = ast.dof.twistMax.ToString();
            f.inputSwingXMin.text = ast.dof.swingXMin.ToString();
            f.inputSwingXMax.text = ast.dof.swingXMax.ToString();
            f.inputSwingZMin.text = ast.dof.swingZMin.ToString();
            f.inputSwingZMax.text = ast.dof.swingZMax.ToString();
        }
    }
}
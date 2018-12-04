using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Esa.UI
{
    public partial class UITimeLine//_Fields
    {
        FrameIdx_Key frameIdx_KeyHandler;
        public Text txtFrameIdx;
        public Text txtFrameIdxN;
        public Vector2Int startPosInt
        {
            get { return new Vector2Int(Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.y)); }
        }
        private Vector2 startPos;

        public int frameIdx
        {
            get { return Mathf.RoundToInt(frameIdx_F); }
            set { frameIdx_F = value; }
        }
        [SerializeField] float _frameIdx_F;
        public System.Action<int> onFrameIdxChanged;
        //[MAD.ShowProperty(MAD.ShowPropertyAttribute.EValueType.Float)]
        public float frameIdx_F
        {
            get { return _frameIdx_F; }
            set
            {
                _frameIdx_F = value;
                txtFrameIdx.text = "帧：" + frameIdx.ToString();
                txtFrameIdxN.text = "n：" + frameIdxN.ToString("0.00");
                if (onFrameIdxChanged != null) onFrameIdxChanged(frameIdx);
            }
        }
        public float frameIdxN
        {
            get
            {
                var end = UIClip.I.clip.frameRange.y;
                return end == 0 ? 0 : MathTool.Round(frameIdx_F / end, indexNAccuracy);
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

// UI控件
public class UITimeLine : MonoBehaviour
{
    #region 属性和成员
    public static UITimeLine I
    {
        get
        {
            if (_i == null) _i = FindObjectOfType<UITimeLine>(); return _i;
        }
    }
    private static UITimeLine _i;
    public static int FrameIndex
    {
        get { return I.frameIndex; }
        set { I.frameIndex = value; }
    }
    public static float Fps
    {
        get { return I.fps; }
        set { I.fps = value; }
    }
    public static float timePerFrame
    {
        get { return 1 / Fps; }
    }
    public float fps = 60;
    public float realtime;
    public float normalizedTime;
    public int xSpaceTextInRuler; // 标尺每隔多少帧有一个帧数数字
    public int xSpaceLineInRuler; // 多少帧画一条线
    public float rulerScalerSensitivity = 20;
    public float rulerLength = 200;
    public Canvas canvas;
    private RectTransform area;
    private RectTransform topBar;
    private RectTransform ruler;

    private float leftTimer;
    private float rightTimer;
    private float upTimer;
    private float downTimer;
    public float continuousKeyTime = 0.5f; // 上下左右键连发延迟
    public float continuousKeyInterval = 0.01f; // 间隔（其实0.01通常约等于每帧触发）
    public Text uiFrameIndex;
    public Text uiFrameIndexN;
    public Vector3 mousePos;
    public Rect uiRect;
    public Vector2 pos2D;
    public int fontSize;

    public float lineWidth = 5;
    public Vector2 range
    {
        get { return endPos - startPos; }
    }
    public Vector2 endPos
    {
        get { return startPos + new Vector2(rulerLength, 0); }
    }
    public Vector2Int startPosInt
    {
        get { return new Vector2Int(Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.y)); }
    }
    private Vector2 startPos;

    public static float FrameValue;
    public int frameIndex
    {
        get
        {
            return Mathf.RoundToInt(frameIndexF);
        }
        set
        {
            frameIndexF = value;
        }
    }
    private float _frameIndexFloat;
    public float frameIndexF
    {
        get
        {
            return _frameIndexFloat;
        }
        set
        {
            _frameIndexFloat = value;
            uiFrameIndex.text = "帧：" + frameIndex.ToString();
            uiFrameIndexN.text = "n：" + frameIndexN.ToString("0.00");
        }
    }
    public float frameIndexN
    {
        get
        {
            var end = UIClip.clip.frameRange.y;
            return end == 0 ? 0 : GTool.Round(frameIndexF / end, indexNAccuracy);
        }
    }
    public int indexNAccuracy = 3;
    public string path;
    public string folder = "Clips/";
    public string fileName = "default.xml";
    string rootPath { get { return Application.dataPath + "/../"; } }
    public InsertKeyType insertType;
    public enum InsertKeyType
    {
        EulPos,
        Eul,
        Pos,
    }
    //public static ASObjectCurve ObjCurve
    //{
    //    get { return UIClip.clip[I.trans]; }
    //}
    public static ASObjectCurve ObjCurve
    {
        get { return UIClip.clip[I.ast]; }
    }
    ASObjectCurve objCurve
    {
        //get { return UIClip.clip[trans]; }
        get { return UIClip.clip[ast]; }
    }
    ASTransDOF ast
    {
        get { return UIDOFEditor.I.ast; }
    }
    //Transform trans
    //{
    //    get { return UIDOFEditor.I.ast.transform; }
    //}
    Vector3 euler
    {
        get { return UIDOFEditor.I.ast.euler; }
    }
    Vector3 pos
    {
        //get { return trans.localPosition; }
        get { return ast.transform.localPosition; }
    }
    #endregion
    void Start()
    {
        frameIndex = 0;
        area = transform.Search("Area") as RectTransform;
        topBar = transform.Search("TopBar") as RectTransform;
        ruler = transform.Search("Ruler X") as RectTransform;
        InitASUI();
        ASUI.I.inputCallBacks.Add(new ASGUI.InputCallBack(GetInput, 1));
    }
    private void InitASUI()
    {
        IMUI.fontSize = fontSize;
        ASUI.parent = area;
        ASUI.BeginHorizon();
        ASUI.EndHorizon();
    }
    public Vector2 areaP;
    public Vector2 topBarP;
    public Vector2 rulerP;
    void UpdateASUI()
    {
        ASUI.owner = area;
        GLUI.BeginOrtho();

        float rulerX;
        int num;
        areaP = ASUI.AbsRefPos(area);
        topBarP = ASUI.AbsRefPos(topBar);
        rulerP = ASUI.AbsRefPos(ruler);
        Vector2 p;
        for (int i = 0; i < rulerLength; i++)
        {
            num = startPosInt.x + i;
            rulerX = i / rulerLength;
            var dX = rulerX * ruler.rect.width;
            if ((num % xSpaceTextInRuler) == 0)
            {
                p = new Vector2(rulerP.x + dX, rulerP.y + ruler.rect.height * 0.5f);
                IMUI.DrawText(num.ToString(), p, Vector2.one * 0.5f); // 画字 帧号标签

                p = new Vector2(areaP.x + dX, areaP.y);
                GLUI.DrawLine(p, p + Vector2.up * area.rect.height, Color.grey - ASColor.V * 0.3f); // 画线
            }
            else if ((num % xSpaceLineInRuler) == 0)
            {
                p = new Vector2(areaP.x + dX, areaP.y);
                GLUI.DrawLine(p, p + Vector2.up * area.rect.height, Color.grey - ASColor.V * 0.2f);//画线
            }
            if (UIClip.clip.HasKey(objCurve, num))
            {
                p = new Vector2(areaP.x + dX, areaP.y);
                GLUI.commandOrder = 2;
                GLUI.DrawLine(p, p + Vector2.up * area.rect.height, lineWidth * 0.45f, Color.yellow - ASColor.V * 0.2f);//画线
            }
        }
        if (MathTool.Between(frameIndex, startPos.x, endPos.x))
        {
            rulerX = (frameIndex - startPos.x) / rulerLength;
            p = new Vector2(areaP.x + rulerX * area.rect.width, areaP.y);
            GLUI.commandOrder = 1;
            GLUI.DrawLine(p, p + Vector2.up * area.rect.height, lineWidth, Color.green);
        }
    }
    private void Update()
    {
        UpdateASUI();
    }
    private void MouseDown(MouseButton button)
    {
        use = true;
        MouseDrag(button);
    }
    public float lx;
    private void MouseDrag(MouseButton button)
    {
        use = true;
        var deltaV = ASUI.mousePositionRef - oldPos;
        deltaV = MathTool.Divide(deltaV, area.rect.size);
        deltaV = Vector2.Scale(deltaV, new Vector2(rulerLength, 0));
        switch (button)
        {
            case MouseButton.Left:

                lx = ASUI.mousePositionRef.x - area.anchoredPosition.x;
                lx = lx / area.rect.width;
                lx = Mathf.Clamp01(lx);
                frameIndex = (int)startPos.x + Mathf.RoundToInt(lx * rulerLength);
                break;

            case MouseButton.Right:

                break;

            case MouseButton.Middle:

                startPos -= deltaV;
                break;
            default: throw null;
        }
        oldPos = ASUI.mousePositionRef;
    }
    Vector2 oldPos;
    bool use, left, right, middle, shift, ctrl;
    public bool over;
    void GetInput()
    {
        var shift = Events.shift;
        var ctrl = Events.ctrl;
        var alt = Events.Alt;
        use = false;
        over = ASUI.MouseOver(area, ruler);
        var simMidDown = Events.MouseDown(MouseButton.Left) && alt;
        if ((Events.MouseDown(MouseButton.Middle) || simMidDown) && over) { oldPos = ASUI.mousePositionRef; MouseDown(MouseButton.Middle); middle = true; }
        if (Events.MouseDown(MouseButton.Left) && over && !simMidDown) { oldPos = ASUI.mousePositionRef; MouseDown(MouseButton.Left); left = true; }
        var simMid = Events.Mouse(MouseButton.Left) && alt;
        if (!Events.Mouse(MouseButton.Middle) && !simMid) middle = false;
        if (!Events.Mouse(MouseButton.Left) || simMid) left = false;
        if (middle) MouseDrag(MouseButton.Middle);
        if (left) MouseDrag(MouseButton.Left);
        float delta = Events.Axis("Mouse ScrollWheel");
        if (delta != 0 && ASUI.MouseOver(area, ruler))
        {
            use = true;
            rulerLength -= delta * rulerScalerSensitivity;
            rulerLength = Mathf.Clamp(rulerLength, 1, Mathf.Infinity);
        }
        if (Events.Key(KeyCode.LeftArrow))
        {
            leftTimer += Time.deltaTime;
        }
        else { leftTimer = 0; }
        if (Events.Key(KeyCode.RightArrow))
        {
            rightTimer += Time.deltaTime;
        }
        else { rightTimer = 0; }
        if (Events.Key(KeyCode.UpArrow))
        {
            upTimer += Time.deltaTime;
        }
        else { upTimer = 0; }
        if (Events.Key(KeyCode.DownArrow))
        {
            downTimer += Time.deltaTime;
        }
        else { downTimer = 0; }
        if (leftTimer > continuousKeyTime || Events.KeyDown(KeyCode.LeftArrow))
        {
            leftTimer -= continuousKeyInterval;
            frameIndex--;
        }
        else if (rightTimer > continuousKeyTime || Events.KeyDown(KeyCode.RightArrow))
        {
            rightTimer -= continuousKeyInterval;
            frameIndex++;
        }
        else if (upTimer > continuousKeyTime * 1.5f || Events.KeyDown(KeyCode.UpArrow))
        {
            upTimer -= continuousKeyInterval * 1.5f;
            if (UIClip.clip.curves.Count > 0)
            {
                var keys = UIClip.clip.curves[0].timeCurve.keys;
                for (int i = keys.Count - 1; i >= 0; i--)
                {
                    if (keys[i].frameIndex < frameIndex)
                    {
                        frameIndex = keys[i].frameIndex;
                        break;
                    }
                }
            }
        }
        else if (downTimer > continuousKeyTime * 1.5f || Events.KeyDown(KeyCode.DownArrow))
        {
            downTimer -= continuousKeyInterval * 1.5f;
            if (UIClip.clip.curves.Count > 0)
            {
                var keys = UIClip.clip.curves[0].timeCurve.keys;
                for (int i = 0; i < keys.Count; i++)
                {
                    if (keys[i].frameIndex > frameIndex)
                    {
                        frameIndex = keys[i].frameIndex;
                        break;
                    }
                }
            }
        }
        if (Events.KeyDown(KeyCode.I))
        {
            if (Events.Key(KeyCode.LeftAlt) || Events.Key(KeyCode.RightAlt))
            {
                RemoveKey();
            }
            else
            {
                InsertKey();
            }
        }
        if (use) Events.Use();
    }
    public void Load()
    {
        path = rootPath + folder + fileName;
        UIClip.clip = Serializer.XMLDeSerialize<ASClip>(path);
    }
    [ContextMenu("Save")]
    public void Save()
    {
        path = rootPath + folder + fileName;
        Serializer.XMLSerialize(UIClip.clip, path);
    }
    private void InsertKey()
    {
        switch (insertType)
        {
            //case InsertKeyType.EulPos: UIClip.clip.AddEulerPos(objCurve, frameIndex, euler, pos); break;
            case InsertKeyType.EulPos: UIClip.clip.AddEulerPosAllCurve(frameIndex); break;
            case InsertKeyType.Eul: break;
            case InsertKeyType.Pos: break;
            default: throw null;
        }
        ASClipTool.GetFrameRange(UIClip.clip);
    }
    private void RemoveKey()
    {
        UIClip.clip.RemoveKey(objCurve, frameIndex);
    }
}
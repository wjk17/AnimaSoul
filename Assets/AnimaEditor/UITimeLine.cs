using UnityEngine;
using UnityEngine.UI;

// UI控件
public class UITimeLine : MonoBehaviour
{
    public static UITimeLine I
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<UITimeLine>(); return instance;
        }
    }
    private static UITimeLine instance;
    public static int FrameIndex
    {
        get { return I.frameIndex; }
        set { I.frameIndex = value; }
    }
    public static ASClip Clip
    {
        get { return I.clip; }
        set { I.clip = value; }
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
    public int xSpaceInRuler; // 标尺每隔多少帧有一个帧数数字
    public int xSpaceLineInRuler; // 多少帧画一条线
    public float rulerScalerSensitivity = 20;
    public float rulerLength = 200;
    public Canvas canvas;
    private RectTransform area;
    private RectTransform ruler;

    private float leftTimer;
    private float rightTimer;
    public float continuousKeyTime = 0.5f; // 上下左右键连发延迟
    public float continuousKeyInterval = 0.01f; // 间隔（其实0.01通常约等于每帧触发）
    public Text uiFrameIndex;
    public Vector3 mousePos;
    public Rect uiRect;
    public Vector2 pos2D;
    public int fontSize;

    public float lineWidth = 5;
    public float moveSensitivity = 0.3f;
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

    private int _frameIndex;
    public int frameIndex
    {
        get
        {
            return _frameIndex;
        }
        set
        {
            _frameIndex = value;
            uiFrameIndex.text = "帧：" + _frameIndex.ToString();
        }
    }
    public string path;
    public string folder = "Clips/";
    public string fileName = "default.xml";
    string rootPath { get { return Application.dataPath + "/../"; } }
    public ASClip clip
    {
        get
        {
            if (_clip == null) I.InitClip(); return _clip;
        }
        set { _clip = value; }
    }
    ASClip _clip;
    public InsertKeyType insertType;
    public enum InsertKeyType
    {
        EulPos,
        Eul,
        Pos,
    }
    public static ASObjectCurve ObjCurve
    {
        get { return I.clip[I.trans]; }
    }
    ASObjectCurve curve
    {
        get { return clip[trans]; }
    }
    Transform trans
    {
        get { return UIDOFEditor.I.ast.transform; }
    }
    Vector3 euler
    {
        get { return UIDOFEditor.I.ast.euler; }
    }
    Vector3 pos
    {
        get { return trans.localPosition; }
    }
    void Start()
    {
        frameIndex = 0;
        area = transform.Search("Area") as RectTransform;
        ruler = transform.Search("Ruler") as RectTransform;
        InitASUI();
        ASUI.I.inputCallBacks.Add(new ASGUI.InputCallBack(GetInput, 1));
    }
    private void InitClip()
    {
        clip = new ASClip();
        foreach (var ast in UIDOFEditor.I.avatar.setting.asts)
        {
            clip.AddCurve(ast.transform);
        }
    }
    private void InitASUI()
    {
        IMUI.fontSize = fontSize;
        ASUI.parent = area;
        ASUI.BeginHorizon();
        ASUI.EndHorizon();
    }
    void UpdateASUI()
    {
        ASUI.owner = area;
        GLUI.BeginOrtho();

        float x, y, rulerX;
        Vector2 p;
        int num;
        for (int i = 0; i < rulerLength; i++)
        {
            num = startPosInt.x + i;
            rulerX = i / rulerLength;
            if ((num % xSpaceInRuler) == 0)
            {
                x = rulerX * ruler.sizeDelta.x;
                x += ruler.anchoredPosition.x;
                y = -ruler.anchoredPosition.y;
                p = new Vector2(x, y);
                IMUI.DrawText(num.ToString(), p, Vector2.one * 0.5f);
                y = -area.anchoredPosition.y;
                p = new Vector2(x, y);
                GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, Color.grey - ASColor.V * 0.3f);
            }
            else if ((num % xSpaceLineInRuler) == 0)
            {
                x = rulerX * area.sizeDelta.x;
                x += area.anchoredPosition.x;
                y = -area.anchoredPosition.y;
                p = new Vector2(x, y);
                GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, Color.grey - ASColor.V * 0.2f);
            }
            if (clip.HasKey(curve, num))
            {
                x = rulerX * area.sizeDelta.x;
                x += area.anchoredPosition.x;
                y = -area.anchoredPosition.y;
                p = new Vector2(x, y);
                GLUI.commandOrder = 2;
                GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, lineWidth * 0.45f, Color.yellow - ASColor.V * 0.2f);
            }
        }
        if (MathTool.Between(frameIndex, startPos.x, endPos.x))
        {
            rulerX = (frameIndex - startPos.x) / rulerLength;
            x = rulerX * area.sizeDelta.x;
            x += area.anchoredPosition.x;
            y = -area.anchoredPosition.y;
            p = new Vector2(x, y);
            GLUI.commandOrder = 1;
            GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, lineWidth, Color.green + ASColor.H * 0.2f);
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
    private void MouseDrag(MouseButton button)
    {
        use = true;
        switch (button)
        {
            case MouseButton.Left:

                var lx = ASUI.mousePositionRef.x - area.anchoredPosition.x;
                lx = lx / area.sizeDelta.x;
                lx = Mathf.Clamp01(lx);
                frameIndex = (int)startPos.x + Mathf.RoundToInt(lx * rulerLength);

                break;
            case MouseButton.Right:

                break;

            case MouseButton.Middle:

                var deltaV = ASUI.mousePositionRef - oldPos;
                startPos -= deltaV * moveSensitivity;
                oldPos = ASUI.mousePositionRef;

                break;
            default: throw null;
        }
    }
    Vector2 oldPos;
    bool use, left, right, middle, shift, ctrl;
    void GetInput()
    {
        var shift = Events.shift;
        var ctrl = Events.ctrl;
        use = false;
        var over = ASUI.MouseOver(area, ruler);
        if (Events.MouseDown(MouseButton.Middle) && over) { oldPos = ASUI.mousePositionRef; MouseDown(MouseButton.Middle); middle = true; }
        if (Events.MouseDown(MouseButton.Left) && over) { oldPos = ASUI.mousePositionRef; MouseDown(MouseButton.Left); left = true; }
        if (!Events.Mouse(MouseButton.Middle)) middle = false;
        if (!Events.Mouse(MouseButton.Left)) left = false;
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
        clip = Serializer.XMLDeSerialize<ASClip>(path);
    }
    [ContextMenu("Save")]
    public void Save()
    {
        path = rootPath + folder + fileName;
        Serializer.XMLSerialize(clip, path);
    }
    private void InsertKey()
    {
        switch (insertType)
        {
            case InsertKeyType.EulPos: clip.AddEulerPos(curve, frameIndex, euler, pos); break;
            case InsertKeyType.Eul: break;
            case InsertKeyType.Pos: break;
            default: throw null;
        }
    }
    private void RemoveKey()
    {
        clip.RemoveKey(curve, frameIndex);
    }
}
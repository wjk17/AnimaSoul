using UnityEngine;
public class LineClip
{
    public enum Result
    {
        origin, // 直接接受R
        processed, // 相交
        discard, // 丢弃
        none
    }
    const int leftBit = 0x1;
    const int rightBit = 0x2;
    const int bottomBit = 0x4;
    const int topBit = 0x8;

    static bool inside(int code) { return code == 0; }
    static bool reject(int code1, int code2) { return (code1 & code2) != 0; }
    static bool accept(int code1, int code2) { return (code1 | code2) == 0; }
    //左下角坐标
    static byte encode(Vector2 p, Vector2 min, Vector2 max)
    {
        byte code = 0;
        if (p.x < min.x) code |= leftBit;
        if (p.x > max.x) code |= rightBit;
        if (p.y < min.y) code |= bottomBit;
        if (p.y > max.y) code |= topBit;
        return code;
    }
    public static Result ClipCohSuth(Vector2 min, Vector2 max, Vector2 p1, Vector2 p2, out Vector2[] p)
    {
        Vector2 tp1 = p1, tp2 = p2;
        var r = ClipCohSuth(min, max, ref tp1, ref tp2);
        p = new Vector2[] { tp1, tp2 };
        return r;
    }
    public static Result ClipCohSuth(Vector2 min, Vector2 max,ref Vector2 p1,ref Vector2 p2)
    {
        byte code1, code2;
        bool plotLine = false;
        Result result = Result.none;
        float m = 0;
        int iter = 0;
        while (result == Result.none && iter < 4)//顶多处理四次 否则出错
        {
            code1 = encode(p1, min, max);
            code2 = encode(p2, min, max);
            if (accept(code1, code2))
            {
                result = plotLine ? Result.processed : Result.origin;//是否经过处理（相交）
            }
            else
            {
                if (reject(code1, code2))
                {
                    result = Result.discard ;//丢弃
                }
                else
                {
                    if (inside(code1))//将窗口外的点标为p1
                    {
                        ASUI.swapPts(ref p1, ref p2);
                        ASUI.swapCodes(ref code1, ref code2);
                    }
                    if (p2.x != p1.x)//使用斜率m来算出线和裁剪边的相交点
                        m = (p2.y - p1.y) / (p2.x - p1.x);
                    if ((code1 & leftBit) != 0)
                    {
                        plotLine = true;
                        p1.y += (min.x - p1.x) * m;
                        p1.x = min.x;
                    }
                    else if ((code1 & rightBit) != 0)
                    {
                        plotLine = true;
                        p1.y += (max.x - p1.x) * m;
                        p1.x = max.x;
                    }
                    else if ((code1 & bottomBit) != 0)
                    {
                        plotLine = true;
                        if (p2.x != p1.x)//只需要为不垂直的线更新p1.x
                            p1.x += (min.y - p1.y) / m;
                        p1.y = min.y;
                    }
                    else if ((code1 & topBit) != 0)
                    {
                        plotLine = true;
                        if (p2.x != p1.x)
                            p1.x += (max.y - p1.y) / m;
                        p1.y = max.y;
                    }
                }
            }
            iter++;
        }
        return result;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public enum EditorCanDragAreaSide
{
    Top =1,
    Bottom=2,
    Left=4,
    Right=8,
}

public static class EditorDrawGUIUtility
{
    /// <summary>
    /// 绘制可拖拽改变区域
    /// </summary>
    /// <param name="dragScreenRect">拖拽区域</param>
    /// <param name="maxScreenRect">最大区域</param>
    /// <param name="drawCallBack">在区域内绘制GUI</param>
    /// <param name="canDragAreaSide">哪条边可拖拽，默认全部可拖拽</param>
    /// <returns></returns>
    public static Rect DrawCanDragArea(Rect dragScreenRect, Rect maxScreenRect, CallBack drawCallBack, EditorCanDragAreaSide canDragAreaSide= EditorCanDragAreaSide.Top| EditorCanDragAreaSide.Bottom| EditorCanDragAreaSide.Left| EditorCanDragAreaSide.Right,string style ="")
    {
        dragScreenRect.xMin = Mathf.Clamp(dragScreenRect.xMin, maxScreenRect.xMin, dragScreenRect.xMax - 5);
        dragScreenRect.xMax = Mathf.Clamp(dragScreenRect.xMax, maxScreenRect.xMin+1, maxScreenRect.xMax );
        dragScreenRect.yMin = Mathf.Clamp(dragScreenRect.yMin, maxScreenRect.yMin, dragScreenRect.yMax-5);
        dragScreenRect.yMax = Mathf.Clamp(dragScreenRect.yMax, maxScreenRect.yMin+1, maxScreenRect.yMax);

        if ((canDragAreaSide & EditorCanDragAreaSide.Left) > 0)
            dragScreenRect.xMin = EditorDrawGUIUtility.DrawHorizontalDragSplitter(dragScreenRect.xMin, maxScreenRect.xMin, dragScreenRect.yMin, 0, 5, maxScreenRect.width, dragScreenRect.height);
        else
            dragScreenRect.xMin = maxScreenRect.xMin;

        if ((canDragAreaSide & EditorCanDragAreaSide.Right) > 0)
            dragScreenRect.xMax = EditorDrawGUIUtility.DrawHorizontalDragSplitter(dragScreenRect.xMax, dragScreenRect.xMin, dragScreenRect.yMin, 5, 0, maxScreenRect.width, dragScreenRect.height);
        else
            dragScreenRect.xMax = maxScreenRect.xMax;

        if ((canDragAreaSide & EditorCanDragAreaSide.Top) > 0)
            dragScreenRect.yMin = EditorDrawGUIUtility.DrawVerticalDragSplitter(dragScreenRect.yMin, maxScreenRect.yMin, dragScreenRect.xMin, 0, 5, maxScreenRect.height, dragScreenRect.width);
        else
            dragScreenRect.yMin = maxScreenRect.yMin;

        if ((canDragAreaSide & EditorCanDragAreaSide.Bottom) > 0)
            dragScreenRect.yMax = EditorDrawGUIUtility.DrawVerticalDragSplitter(dragScreenRect.yMax, dragScreenRect.yMin, dragScreenRect.xMin, 5, 0, maxScreenRect.height, dragScreenRect.width);
        else
            dragScreenRect.yMax = maxScreenRect.yMax;

        if (string.IsNullOrEmpty(style))
            GUILayout.BeginArea(dragScreenRect, "");
        else
            GUILayout.BeginArea(dragScreenRect, "", style);
        if (drawCallBack != null)
        {
            drawCallBack();
        }
        GUILayout.EndArea();

        return dragScreenRect;
    }
    public static float DrawHorizontalDragSplitter(float dragX,float minX, float pos_Y, float minLeftSide, float minRightSide, float width, float splitterHeight)
    {
        Rect dragRect = new Rect(dragX, pos_Y, 5f, splitterHeight);
        dragRect = HandleHorizontalSplitter(dragRect, minX, width, minLeftSide, minRightSide);

        DrawHorizontalSplitter(dragRect);
        return dragRect.x;
    }
    /// <summary>
    /// 绘制可上下拖拽的线
    /// </summary>
    /// <param name="dragY">拖拽的变化Y坐标</param>
    /// <param name="minY">拖拽的最小Y坐标</param>
    /// <param name="pos_X">线从X坐标开始绘制</param>
    /// <param name="minTopSide">总高度范围内可拖拽的最小范围，距离上边界多少</param>
    /// <param name="minBottomSide">总高度范围内可拖拽的最大范围，距离下边界多少</param>
    /// <param name="height">可拖拽总高度</param>
    /// <param name="splitterWidth">绘制线的宽度</param>
    /// <returns></returns>
    public static float DrawVerticalDragSplitter(float dragY,float minY, float pos_X, float minTopSide, float minBottomSide, float height, float splitterWidth)
    {
        Rect dragRect = new Rect(pos_X, dragY, splitterWidth,5f );
        dragRect = HandleVerticalSplitter(dragRect, minY, height, minTopSide, minBottomSide);

        DrawVerticalSplitter(dragRect);
        return dragRect.y;
    }
    public static void DrawHorizontalSplitter(Rect dragRect)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Color color = GUI.color;
            Color b = (!EditorGUIUtility.isProSkin) ? new Color(0.6f, 0.6f, 0.6f, 1.333f) : new Color(0.12f, 0.12f, 0.12f, 1.333f);
            GUI.color *= b;
            Rect position = new Rect(dragRect.x - 1f, dragRect.y, 1f, dragRect.height);
            GUI.DrawTexture(position, EditorGUIUtility.whiteTexture);
            GUI.color = color;
        }
    }
    public static void DrawVerticalSplitter(Rect dragRect)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Color color = GUI.color;
            Color b = (!EditorGUIUtility.isProSkin) ? new Color(0.6f, 0.6f, 0.6f, 1.333f) : new Color(0.12f, 0.12f, 0.12f, 1.333f);
            GUI.color *= b;
            Rect position = new Rect(dragRect.x, dragRect.y-1f, dragRect.width, 1f);
            GUI.DrawTexture(position, EditorGUIUtility.whiteTexture);
            GUI.color = color;
        }
    }
    public static Rect HandleHorizontalSplitter(Rect dragRect,float minX, float width, float minLeftSide, float minRightSide)
    {
        if (Event.current.type == EventType.Repaint)
        {
            EditorGUIUtility.AddCursorRect(dragRect, MouseCursor.SplitResizeLeftRight);
        }
        float num = 0f;

        object[] par = new object[] { dragRect, true };
        Vector2 delta = (Vector2)ReflectionUtils.InvokMethod(typeof(EditorGUI), null, "MouseDeltaReader", ref par);
        float x = delta.x;
        float max = minX + width - minRightSide;
        if (x != 0f)
        {
            dragRect.x += x;
            num = Mathf.Clamp(dragRect.x, minX+ minLeftSide, max);
        }
        if (dragRect.x > max)
        {
            num = max;
        }
        if (num > 0f)
        {
            dragRect.x = num;
        }
        return dragRect;
    }
    public static Rect HandleVerticalSplitter(Rect dragRect,float minY, float height, float minTopSide, float minBottomSide)
    {
        if (Event.current.type == EventType.Repaint)
        {
            EditorGUIUtility.AddCursorRect(dragRect, MouseCursor.SplitResizeUpDown);
        }
        float num = 0f;

        object[] par = new object[] { dragRect, true };
        Vector2 delta = (Vector2)ReflectionUtils.InvokMethod(typeof(EditorGUI), null, "MouseDeltaReader", ref par);
        float y = delta.y;
        float max = minY + height - minBottomSide;
        if (y != 0f)
        {
            dragRect.y += y;
            num = Mathf.Clamp(dragRect.y, minY+ minTopSide, max);
        }
        if (dragRect.y > max)
        {
            num = max;
        }
        if (num > 0f)
        {
            dragRect.y = num;
        }
        return dragRect;
    }

    public static Rect HandleDragObject(Rect dragRect)
    {
        Event e = Event.current;

        //if (e.button == 0 && e.type == EventType.MouseDrag)
        {
            object[] par = new object[] { dragRect, true };
            Vector2 delta = (Vector2)ReflectionUtils.InvokMethod(typeof(EditorGUI), null, "MouseDeltaReader", ref par);
            dragRect.position = dragRect.position + delta;
            //e.Use();
        }
        return dragRect;
    }
}


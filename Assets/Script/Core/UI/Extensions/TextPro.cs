using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Extensions/TextPro")]
/// <summary>
/// 扩展Text
/// 1.增强RichText 支持<align="right">Right <align="center">Center  <align="left"> Left
/// </summary>
public class TextPro : Text
{

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        RichTextAlignDataSupport(toFill);
    }

    private void RichTextAlignDataSupport(VertexHelper toFill)
    {
        if (!supportRichText)
        {
            base.OnPopulateMesh(toFill);
            return;
        }

        string changedText = "";
        var orignText = m_Text;
        m_Text = DealWithTextContent(m_Text);
        changedText = m_Text;
        base.OnPopulateMesh(toFill);
        m_Text = orignText;

        IList<UILineInfo> lines = cachedTextGenerator.lines;
        IList<UICharInfo> characters = cachedTextGenerator.characters;
        Rect rectExtents = cachedTextGenerator.rectExtents;
        //Debug.Log("cachedTextGenerator.characterCountVisible :"+cachedTextGenerator.characterCountVisible);
        List<UIVertex> stream = new List<UIVertex>();
        toFill.GetUIVertexStream(stream);

        List<AlignData> removeAlignDatas = new List<AlignData>();

        for (int i = 0; i < alignDatas.Count; i++)
        {
            AlignData alignData = alignDatas[i];


            for (int j = 0; j < lines.Count; j++)
            {
                UILineInfo lInfo = lines[j];
                if (lInfo.startCharIdx <= alignData.startCharIndex)
                {
                    alignData.lineIndex = j;
                    alignData.lineStartCharIndex = lInfo.startCharIdx;
                    if (j == lines.Count - 1)
                    {
                        if (alignData.startCharIndex > cachedTextGenerator.characterCountVisible)
                        {
                            removeAlignDatas.Add(alignData);
                        }
                        else
                            alignData.lineEndCharIndex = cachedTextGenerator.characterCountVisible;
                    }
                }
                else
                {
                    alignData.lineEndCharIndex = lInfo.startCharIdx - 1;
                    break;
                }
            }

            alignDatas[i] = alignData;

        }
        List<int> lineList = new List<int>();

        for (int i = 0; i < alignDatas.Count; i++)
        {
            AlignData alignData = alignDatas[i];
            //Debug.Log("alignData.lineIndex :" + alignData.lineIndex);
            if (lineList.Contains(alignData.lineIndex))
            {
                removeAlignDatas.Add(alignData);
                continue;
            }
            else
            {
                lineList.Add(alignData.lineIndex);
            }
        }
        for (int i = 0; i < removeAlignDatas.Count; i++)
        {
            alignDatas.Remove(removeAlignDatas[i]);
        }
        //Debug.Log("alignDatas :" + alignDatas.Count);

        for (int i = 0; i < alignDatas.Count; i++)
        {
            AlignData alignData = alignDatas[i];
            //Debug.Log("characters.Count :" + characters.Count + "  stream.Count:" + stream.Count + "  alignData.lineEndCharIndex:" + alignData.lineEndCharIndex);
            if (alignData.lineEndCharIndex >= characters.Count)
                continue;
            if (alignData.lineStartCharIndex >= characters.Count)
                continue;
            if ((alignData.lineEndCharIndex * 6) > stream.Count)
                continue;
            if (alignData.lineStartCharIndex * 6 >= stream.Count)
                continue;
            if (alignData.alignType == AlignType.Right)
            {
                UICharInfo uiChar = characters[alignData.lineEndCharIndex];
                float detaMove = rectExtents.width / 2 - uiChar.cursorPos.x - uiChar.charWidth;

                for (int v = alignData.lineStartCharIndex * 6; v < alignData.lineEndCharIndex * 6; v++)
                {
                    UIVertex ver = stream[v];
                    ver.position += new Vector3(detaMove, 0, 0);
                    //ver.color = Color.red;
                    stream[v] = ver;
                }
            }
            else if (alignData.alignType == AlignType.Left)
            {
                UICharInfo uiChar = characters[alignData.lineStartCharIndex];
                float detaMove = (-rectExtents.width / 2) - uiChar.cursorPos.x;
                //Debug.Log("LLeft alignData.lineStartCharIndex：" + alignData.lineStartCharIndex + "  alignData.lineEndCharIndex:" + alignData.lineEndCharIndex);
                for (int v = alignData.lineStartCharIndex * 6; v < alignData.lineEndCharIndex * 6; v++)
                {
                    UIVertex ver = stream[v];
                    ver.position += new Vector3(detaMove, 0, 0);
                    //ver.color = Color.green;
                    stream[v] = ver;
                }
            }
            else if (alignData.alignType == AlignType.Center)
            {
                float lineCharLenth = 0;
                for (int j = alignData.lineStartCharIndex; j < alignData.lineEndCharIndex; j++)
                {
                    lineCharLenth += characters[j].charWidth;
                }


                float detaMove = -lineCharLenth / 2 - characters[alignData.lineStartCharIndex].cursorPos.x;

                for (int v = alignData.lineStartCharIndex * 6; v < alignData.lineEndCharIndex * 6; v++)
                {
                    UIVertex ver = stream[v];
                    ver.position += new Vector3(detaMove, 0, 0);
                    //ver.color = Color.blue;
                    stream[v] = ver;
                }
            }
        }
        toFill.AddUIVertexTriangleStream(stream);
    }
    List<AlignData> alignDatas = new List<AlignData>();
    private string DealWithTextContent(string content)
    {
        alignDatas.Clear();

        var temp = content;
        while (true)
        {
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == '<')
                {
                    int surplusLenth = temp.Length - i - 1;
                    string testStr = "";
                    bool isResult = false;
                    if (surplusLenth >= alignRightString.Length)
                    {
                        testStr = temp.Substring(i, alignRightString.Length);
                        if (testStr == alignRightString)
                        {
                            isResult = true;
                            AlignData align = new AlignData();
                            align.alignType = AlignType.Right;
                            align.startCharIndex = i;
                            alignDatas.Add(align);
                        }
                    }

                    if (!isResult)
                    {
                        if (surplusLenth >= alignLeftString.Length)
                        {
                            testStr = temp.Substring(i, alignLeftString.Length);
                            if (testStr == alignLeftString)
                            {
                                isResult = true;
                                AlignData align = new AlignData();
                                align.alignType = AlignType.Left;
                                align.startCharIndex = i;
                                alignDatas.Add(align);
                            }
                        }
                    }
                    if (!isResult)
                    {
                        if (surplusLenth >= alignCenterString.Length)
                        {
                            testStr = temp.Substring(i, alignCenterString.Length);
                            if (testStr == alignCenterString)
                            {
                                isResult = true;
                                AlignData align = new AlignData();
                                align.alignType = AlignType.Center;
                                align.startCharIndex = i;
                                alignDatas.Add(align);
                            }
                        }
                    }

                    if (isResult)
                    {
                        temp = temp.Remove(i, testStr.Length);
                        i = 0;
                    }
                }
            }
            break;
        }

        //Debug.Log("alignDatas :" + alignDatas.Count);
        return temp;
    }

    private const string alignRightString = "<align=\"right\">";
    private const string alignLeftString = "<align=\"left\">";
    private const string alignCenterString = "<align=\"center\">";
}
public struct AlignData
{
    public AlignType alignType;
    public int startCharIndex;
    public int lineEndCharIndex;
    public int lineStartCharIndex;
    public int lineIndex;
}
public enum AlignType
{
    Right,
    Left,
    Center,
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Extensions/TextPro")]
/// <summary>
/// 扩展Text 这几个标签不要在同一行使用多个
/// 1.增强RichText 支持<align="right">Right <align="center">Center  <align="left"> Left
/// </summary>
public class TextPro : Text
{
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        RichTextAlignDataSupport(toFill);
    }

    private void RemoveSameAlignData(List<AlignData> alignDatas)
    {
        IList<UILineInfo> lines = cachedTextGenerator.lines;
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
    }
    private void RichTextAlignDataSupport(VertexHelper toFill)
    {
        if (!supportRichText)
        {
            base.OnPopulateMesh(toFill);
            return;
        }
        List<AlignData> alignDatas=new List<AlignData>();
        //string changedText = "";
        var orignText = m_Text;
        m_Text = DealWithTextContent(m_Text,ref alignDatas);
        //changedText = m_Text;
        base.OnPopulateMesh(toFill);
        m_Text = orignText;

        RectTransform rectTransform = GetComponent<RectTransform>();
        float rangeWith =rectTransform.sizeDelta.x;

        
        IList<UICharInfo> characters = cachedTextGenerator.characters;
        Rect rectExtents = cachedTextGenerator.rectExtents;

        List<UIVertex> stream = new List<UIVertex>();
        toFill.GetUIVertexStream(stream);
        toFill.Clear();


        RemoveSameAlignData(alignDatas);

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

            int indexEnd = alignData.lineEndCharIndex * 6 - 3;
            int indexStart = alignData.lineStartCharIndex * 6;
            if (alignData.alignType == AlignType.Right)
            {
                float detaMove = rangeWith / 2 - stream[indexEnd].position.x;
                for (int v = alignData.lineStartCharIndex * 6; v < alignData.lineEndCharIndex * 6; v++)
                {
                    UIVertex ver = stream[v];
                    //if (v >= ((alignData.lineEndCharIndex - 1) * 6) && v <= (alignData.lineEndCharIndex * 6))
                        //Debug.Log("Pos :" + ver.position);
                    ver.position += new Vector3(detaMove, 0, 0);
                   
                      //  ver.color = Color.red;
                        stream[v] = ver;
                }
            }
            else if (alignData.alignType == AlignType.Left)
            {
                float detaMove = (-rangeWith / 2) - stream[indexStart].position.x;
                //Debug.Log("LLeft alignData.lineStartCharIndex：" + alignData.lineStartCharIndex + "  alignData.lineEndCharIndex:" + alignData.lineEndCharIndex);
                for (int v = alignData.lineStartCharIndex * 6; v < alignData.lineEndCharIndex * 6; v++)
                {
                    UIVertex ver = stream[v];
                    ver.position += new Vector3(detaMove, 0, 0);
                    //if(v== (alignData.lineEndCharIndex * 6-1))
                    //ver.color = Color.green;
                    stream[v] = ver;
                }
            }
            else if (alignData.alignType == AlignType.Center)
            {
                
                float lineCharLenth = Mathf.Abs(stream[indexEnd].position.x) + Mathf.Abs(stream[indexStart].position.x);
                float detaMove = (lineCharLenth) / 2 - stream[indexEnd].position.x;

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
    private string DealWithTextContent(string content,ref List<AlignData> alignDatas)
    {
        if (alignDatas == null)
            alignDatas = new List<AlignData>();
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
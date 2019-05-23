using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/TextVirtical", 10)]
public class TextVirtical : Text
{
    public bool m_Virtical = true;
    private float lineSpace = 1;
    private float textSpace = 1;
    private float xOffset = 0;
    private float yOffset = 0;
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        base.OnPopulateMesh(toFill);
        if (m_Virtical)
        {
            VirticalText(toFill);
        }
    }

    private void VirticalText(VertexHelper toFill)
    {
        if (!IsActive())
            return;

        lineSpace = fontSize * lineSpacing;
        textSpace = fontSize * lineSpacing;

        xOffset = rectTransform.sizeDelta.x / 2 - fontSize / 2;
        yOffset = rectTransform.sizeDelta.y / 2 - fontSize / 2;

        for (int i = 0; i < cachedTextGenerator.lines.Count; i++)
        {
            UILineInfo line = cachedTextGenerator.lines[i];

            int step = i;
            if (i + 1 < cachedTextGenerator.lines.Count)
            {
                UILineInfo line2 = cachedTextGenerator.lines[i + 1];

                int current = 0;

                for (int j = line.startCharIdx; j < line2.startCharIdx - 1; j++)
                {
                    modifyText(toFill, j, current++, step);
                }
            }
            else if (i + 1 == cachedTextGenerator.lines.Count)
            {
                int current = 0;
                for (int j = line.startCharIdx; j < cachedTextGenerator.characterCountVisible; j++)
                {
                    modifyText(toFill, j, current++, step);
                }
            }
        }
    }

    void modifyText(VertexHelper helper, int i, int charYPos, int charXPos)
    {
        UIVertex lb = new UIVertex();
        helper.PopulateUIVertex(ref lb, i * 4);

        UIVertex lt = new UIVertex();
        helper.PopulateUIVertex(ref lt, i * 4 + 1);

        UIVertex rt = new UIVertex();
        helper.PopulateUIVertex(ref rt, i * 4 + 2);

        UIVertex rb = new UIVertex();
        helper.PopulateUIVertex(ref rb, i * 4 + 3);

        Vector3 center = Vector3.Lerp(lb.position, rt.position, 0.5f);
        Matrix4x4 move = Matrix4x4.TRS(-center, Quaternion.identity, Vector3.one);

        float x = -charXPos * lineSpace + xOffset;
        float y = -charYPos * textSpace + yOffset;

        Vector3 pos = new Vector3(x, y, 0);
        Matrix4x4 place = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
        Matrix4x4 transform = place * move;

        lb.position = transform.MultiplyPoint(lb.position);
        lt.position = transform.MultiplyPoint(lt.position);
        rt.position = transform.MultiplyPoint(rt.position);
        rb.position = transform.MultiplyPoint(rb.position);

        helper.SetUIVertex(lb, i * 4);
        helper.SetUIVertex(lt, i * 4 + 1);
        helper.SetUIVertex(rt, i * 4 + 2);
        helper.SetUIVertex(rb, i * 4 + 3);
    }
}

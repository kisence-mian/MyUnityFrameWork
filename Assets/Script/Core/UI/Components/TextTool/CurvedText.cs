using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Curved text.让文本按照曲线进行显示 【注意对Image的变形 也是可以的】
    /// 说明： 对Text的操作就和 shadow 和 outline 组件类似。
    /// </summary>
    // [RequireComponent(typeof(Text), typeof(RectTransform))]
    [AddComponentMenu("UI/Effects/Extensions/Curved Text")]
    public class CurvedText : BaseMeshEffect
    {
        // 曲线类型
        public AnimationCurve curveForText = AnimationCurve.Linear(0, 0, 1, 10);
        // 曲线程度
        public float curveMultiplier = 1;
        private RectTransform rectTrans;


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (curveForText[0].time != 0)
            {
                var tmpRect = curveForText[0];
                tmpRect.time = 0;
                curveForText.MoveKey(0, tmpRect);
            }
            if (rectTrans == null)
                rectTrans = GetComponent<RectTransform>();
            if (curveForText[curveForText.length - 1].time != rectTrans.rect.width)
                OnRectTransformDimensionsChange();
        }
#endif
        protected override void Awake()
        {
            base.Awake();
            rectTrans = GetComponent<RectTransform>();
            OnRectTransformDimensionsChange();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            rectTrans = GetComponent<RectTransform>();
            OnRectTransformDimensionsChange();
        }
        /// <summary>
        /// Modifies the mesh. 最重要的重载函数
        /// </summary>
        /// <param name="mesh">Mesh.</param>
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            // 从mesh 得到 顶点集
            List<UIVertex> verts = new List<UIVertex>();

            vh.GetUIVertexStream(verts);


            // 顶点的 y值按曲线变换
            for (int index = 0; index < verts.Count; index++)
            {
                var uiVertex = verts[index];
                //Debug.Log ();
                uiVertex.position.y += curveForText.Evaluate(rectTrans.rect.width * rectTrans.pivot.x + uiVertex.position.x) * curveMultiplier;
                verts[index] = uiVertex;
            }

            // 在合成mesh

            vh.AddUIVertexTriangleStream(verts);


        }
        protected override void OnRectTransformDimensionsChange()
        {
            var tmpRect = curveForText[curveForText.length - 1];
            if (rectTrans != null)
            {
                tmpRect.time = rectTrans.rect.width;
                curveForText.MoveKey(curveForText.length - 1, tmpRect);

            }
        }

    }
}

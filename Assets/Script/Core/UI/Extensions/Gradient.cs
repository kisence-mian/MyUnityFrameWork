/// Credit Breyer
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/#post-1780095

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Effects/Extensions/Gradient")]
    public class Gradient : BaseMeshEffect
    {
        [SerializeField]
        private GradientMode _gradientMode = GradientMode.Global;
        [SerializeField]
        private GradientDir _gradientDir = GradientDir.Vertical;
        [SerializeField]
        private bool _overwriteAllColor = false;
        [SerializeField]
        private Color _vertex1 = Color.white;
        [SerializeField]
        private Color _vertex2 = Color.black;
        private Graphic targetGraphic;

        #region Properties
        public GradientMode GradientMode { get { return _gradientMode; } set { _gradientMode = value; graphic.SetVerticesDirty(); } }
        public GradientDir GradientDir { get { return _gradientDir; } set { _gradientDir = value; graphic.SetVerticesDirty(); } }
        public bool OverwriteAllColor { get { return _overwriteAllColor; } set { _overwriteAllColor = value; graphic.SetVerticesDirty(); } }
        public Color Vertex1 { get { return _vertex1; } set { _vertex1 = value; graphic.SetAllDirty(); } }
        public Color Vertex2 { get { return _vertex2; } set { _vertex2 = value; graphic.SetAllDirty(); } }
        #endregion

        protected override void Awake()
        {
            targetGraphic = GetComponent<Graphic>();
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            int count = vh.currentVertCount;
            if (!IsActive() || count == 0)
            {
                return;
            }
            var vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);
            UIVertex uiVertex = new UIVertex();
            if (_gradientMode == GradientMode.Global)
            {
                if (_gradientDir == GradientDir.DiagonalLeftToRight || _gradientDir == GradientDir.DiagonalRightToLeft)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("Diagonal dir is not supported in Global mode");
#endif
                    _gradientDir = GradientDir.Vertical;
                }
                float bottomY = _gradientDir == GradientDir.Vertical ? vertexList[vertexList.Count - 1].position.y : vertexList[vertexList.Count - 1].position.x;
                float topY = _gradientDir == GradientDir.Vertical ? vertexList[0].position.y : vertexList[0].position.x;

                float uiElementHeight = topY - bottomY;

                for (int i = 0; i < count; i++)
                {
                    vh.PopulateUIVertex(ref uiVertex, i);
                    if (!_overwriteAllColor && uiVertex.color != targetGraphic.color)
                        continue;
                    uiVertex.color *= Color.Lerp(_vertex2, _vertex1, ((_gradientDir == GradientDir.Vertical ? uiVertex.position.y : uiVertex.position.x) - bottomY) / uiElementHeight);
                    vh.SetUIVertex(uiVertex, i);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    vh.PopulateUIVertex(ref uiVertex, i);
                    if (!_overwriteAllColor && !CompareCarefully(uiVertex.color, targetGraphic.color))
                        continue;
                    switch (_gradientDir)
                    {
                        case GradientDir.Vertical:
                            uiVertex.color *= (i % 4 == 0 || (i - 1) % 4 == 0) ? _vertex1 : _vertex2;
                            break;
                        case GradientDir.Horizontal:
                            uiVertex.color *= (i % 4 == 0 || (i - 3) % 4 == 0) ? _vertex1 : _vertex2;
                            break;
                        case GradientDir.DiagonalLeftToRight:
                            uiVertex.color *= (i % 4 == 0) ? _vertex1 : ((i - 2) % 4 == 0 ? _vertex2 : Color.Lerp(_vertex2, _vertex1, 0.5f));
                            break;
                        case GradientDir.DiagonalRightToLeft:
                            uiVertex.color *= ((i - 1) % 4 == 0) ? _vertex1 : ((i - 3) % 4 == 0 ? _vertex2 : Color.Lerp(_vertex2, _vertex1, 0.5f));
                            break;

                    }
                    vh.SetUIVertex(uiVertex, i);
                }
            }
        }
        private bool CompareCarefully(Color col1, Color col2)
        {
            if (Mathf.Abs(col1.r - col2.r) < 0.003f && Mathf.Abs(col1.g - col2.g) < 0.003f && Mathf.Abs(col1.b - col2.b) < 0.003f && Mathf.Abs(col1.a - col2.a) < 0.003f)
                return true;
            return false;
        }
    }

    public enum GradientMode
    {
        Global,
        Local
    }

    public enum GradientDir
    {
        Vertical,
        Horizontal,
        DiagonalLeftToRight,
        DiagonalRightToLeft
        //Free
    }
    //enum color mode Additive, Multiply, Overwrite
}
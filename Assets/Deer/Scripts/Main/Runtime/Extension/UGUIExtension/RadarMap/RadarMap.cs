// ================================================
//描 述:Radar map with image
//作 者:Xiaohei.Wang(Wenhao)
//创建时间:2023-04-24 18-57-52
//修改作者:Xiaohei.Wang(Wenhao)
//修改时间:2023-04-24 18-57-52
//版 本:0.1 
// ===============================================

using UnityEngine;
using UnityEngine.UI;

namespace Main.Runtime.UI
{
    /// <summary>
    /// Radar map with image.
    /// </summary>
    public class RadarMap : Image
    {
        /// <summary>
        /// 雷达图边数, 
        /// 当设置小于3时设置失效
        /// </summary>
        [Header("雷达图边数，当设置小于3时设置失效")]
        public int SideCount = 6;

        /// <summary>
        /// 顶点与中心的最小距离
        /// </summary>
        [Header("顶点与中心的最小距离")]
        public float MinDistance = 5;

        /// <summary>
        /// 顶点与中心的最大距离
        /// </summary>
        [Header("顶点与中心的最大距离")]
        public float MaxDistance = 50;

        /// <summary>
        /// 每一边数的大小（0~1表示的百分比）
        /// </summary>
        [Header("每一边数的大小（0~1表示的百分比）")]
        public float[] EachPercent;

        /// <summary>
        /// 第一个顶点的起始弧度。默认从正右方开始
        /// </summary>
        [Header("第一个顶点的起始弧度。默认从正右方开始")]
        public float InitialRadian = 0;//第一个顶点的起始弧度。默认从正右方开始。

        private bool m_isFirst = true;
        private int m_sideCount;
        private float m_minDistance;
        private float m_maxDistance;
        private Vector3[] m_innerPositions;//雷达图最内圈的点
        private Vector3[] m_exteriorPositions;//雷达图最外圈的点
        private RectTransform m_selfTransform;//自身大小

        protected override void Awake()
        {
            if (null == EachPercent)
            {
                EachPercent = new float[m_sideCount];
            }
            SetMaxDistance();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (m_isFirst)
            {
                m_isFirst = !m_isFirst;
                return;
            }
            vh.Clear();//清除原信息

            InitPositions();
            AddVertex(vh);
            AddTriangles(vh);
        }

        private void Update()
        {
            SetSideCount();
            SetMinDistance();
        }

        /// <summary>
        /// Initialize the points positions in radar map.
        /// 初始化雷达图最内圈和最外圈的点
        /// </summary>
        private void InitPositions()
        {
            SetMaxDistance();
            m_innerPositions = new Vector3[m_sideCount];
            m_exteriorPositions = new Vector3[m_sideCount];
            float _tempRadian = InitialRadian;
            float _radiamDelta = 2 * Mathf.PI / m_sideCount;//每两个相邻顶点相差的弧度

            for (int i = 0; i < m_sideCount; i++)
            {
                m_innerPositions[i] = new Vector3(m_minDistance * Mathf.Cos(_tempRadian), m_minDistance * Mathf.Sin(_tempRadian), 0);
                m_exteriorPositions[i] = new Vector3(m_maxDistance * Mathf.Cos(_tempRadian), m_maxDistance * Mathf.Sin(_tempRadian), 0);
                _tempRadian += _radiamDelta;
            }
        }

        /// <summary>
        /// Added the vertex count.
        /// 添加形成三角面片用的顶点数量
        /// </summary>
        private void AddVertex(VertexHelper vh)
        {
            vh.AddVert(Vector3.zero, color, Vector2.zero);//添加轴心点位置为第一个顶点
            for (int i = 0; i < m_sideCount; i++)
            {
                //通过在最内点和最外点间差值得到雷达图顶点实际位置，并添加到为vh的顶点。由于并没有图案，最后一项的uv坐标就随便填了。
                vh.AddVert(Vector3.Lerp(m_innerPositions[i], m_exteriorPositions[i], EachPercent[i]), color, Vector2.zero);
            }
        }

        /// <summary>
        /// Added the triangles count.
        /// 添加三角面片
        /// </summary>
        private void AddTriangles(VertexHelper vh)
        {
            for (int i = 0; i < m_sideCount - 1; i++)
            {
                vh.AddTriangle(0, i + 1, i + 2);
            }
            vh.AddTriangle(0, m_sideCount, 1);
        }

        #region Set value with changed public property.
        /// <summary>
        /// Set size count.
        /// 设置雷达图边数
        /// </summary>
        private void SetSideCount()
        {
            if (m_sideCount == SideCount || SideCount < 3)
                return;

            m_sideCount = SideCount;
            float[] _temp = new float[m_sideCount];

            int _tempLength = EachPercent.Length >= m_sideCount ? m_sideCount : EachPercent.Length;

            for (int i = 0; i < _tempLength; i++)
            {
                _temp[i] = EachPercent[i];
            }
            for (int i = m_sideCount - 1; i >= _tempLength; i--)
            {
                _temp[i] = 1;
            }
            EachPercent = _temp;
            SetVerticesDirty();
        }

        /// <summary>
        /// Set min distance between the vertex and the center.
        /// 设置顶点到中心点的最小距离
        /// </summary>
        private void SetMinDistance()
        {
            if (m_minDistance == MinDistance)
                return;
            m_minDistance = MinDistance;
            SetVerticesDirty();
        }

        /// <summary>
        /// Set max distance between the vertex and the center.
        /// 设置顶点到中心点的最大距离
        /// </summary>
        private void SetMaxDistance()
        {
            if (null == m_selfTransform)
            {
                m_selfTransform = GetComponent<RectTransform>();
            }
            MaxDistance = (m_selfTransform.sizeDelta.x <= m_selfTransform.sizeDelta.y ? m_selfTransform.sizeDelta.x : m_selfTransform.sizeDelta.y) / 2.0f;
            if (m_maxDistance != MaxDistance)
            {
                m_maxDistance = MaxDistance;
            }
        }
        #endregion
    }
}

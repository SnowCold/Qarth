//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


namespace Qarth
{
    public class TestMeshEffect : BaseMeshEffect
    {
        [SerializeField]
        private Vector3 m_RightBottomPos;
        [SerializeField]
        private Image m_Image;

        public float Y;
        public Vector3 apex;
        public float theta;
        public float rho;

        private List<UIVertex> m_VertexList;

        protected override void Awake()
        {
            base.Awake();
            m_Image = gameObject.GetComponent<Image>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (m_VertexList != null)
            {
                ListPool<UIVertex>.Recycle(m_VertexList);
                m_VertexList = null;
            }
        }

        private void Update()
        {
            m_Image.SetVerticesDirty();
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }

            apex = new Vector3(0, Y, 0);

            var raw = ListPool<UIVertex>.Allocate();
            vh.GetUIVertexStream(raw);

            BuildVertexList(raw);

            ListPool<UIVertex>.Recycle(raw);

            vh.Clear();

            vh.AddUIVertexTriangleStream(m_VertexList);
        }

        [SerializeField]
        private int m_CountX = 10;
        [SerializeField]
        private int m_CountY = 10;

        private void BuildVertexList(List<UIVertex> raw)
        {
            if (m_VertexList == null)
            {
                m_VertexList = ListPool<UIVertex>.Allocate();
            }
            else
            {
                m_VertexList.Clear();
            }

            if (raw == null || raw.Count < 4)
            {
                return;
            }

            UIVertex a0 = raw[0];
            UIVertex a1 = raw[1];
            UIVertex a2 = raw[2];
            //UIVertex a3 = raw[4];

            Vector3 p0 = a0.position;
            Vector3 p1 = a1.position;
            Vector3 p2 = a2.position;

            float width = p2.x - p1.x;
            float height = p1.y - p0.y;

            float stepX = width / m_CountX;
            float stepY = height / m_CountY;

            for (int y = 0; y < m_CountY; ++y)
            {
                float posY0 = p0.y + y * stepY;
                float posY1 = p0.y + (y + 1) * stepY;

                float v0 = y / (float)m_CountY;
                float v1 = (y + 1) / (float)m_CountY;

                //posY0 *= RandomHelpler.Range(1.0f, 2.0f);
                //posY1 *= RandomHelpler.Range(1.0f, 2.0f);

                for (int x = 0; x < m_CountX; ++x)
                {
                    float posX0 = p0.x + x * stepX;
                    float posX1 = p0.x + (x + 1) * stepX;

                    float u0 = x / (float)m_CountX;
                    float u1 = (x + 1) / (float)m_CountX;

                    //posX0 *= RandomHelpler.Range(1.0f, 2.0f);
                    //posX1 *= RandomHelpler.Range(1.0f, 2.0f);

                    Vector3 pp0 = new Vector3(posX0, posY0, 0);
                    Vector3 pp1 = new Vector3(posX0, posY1, 0);
                    Vector3 pp2 = new Vector3(posX1, posY1, 0);

                    Vector3 pp3 = new Vector3(posX1, posY1, 0);
                    Vector3 pp4 = new Vector3(posX1, posY0, 0);
                    Vector3 pp5 = new Vector3(posX0, posY0, 0);

                    AddVector(pp0, u0, v0);
                    AddVector(pp1, u0, v1);
                    AddVector(pp2, u1, v1);
                    AddVector(pp3, u1, v1);
                    AddVector(pp4, u1, v0);
                    AddVector(pp5, u0, v0);
                }
            }
        }

        private void AddVector(Vector3 p, float u, float v)
        {
            UIVertex ver = new UIVertex();

            ver.position = p;// (p);

            ver.color = Color.white;
            ver.uv0 = new Vector2(u, v);
            m_VertexList.Add(ver);
        }

        private Vector3 CurlTurn(Vector3 p)
        {
            float R = Mathf.Sqrt(Mathf.Pow(p.x, 2) + Mathf.Pow(p.y - apex.y, 2));
            float r = R * Mathf.Sin(theta);
            float bta = Mathf.Asin(p.x / R) / Mathf.Sin(theta);

            float x = r * Mathf.Sin(bta);
            float y = R + apex.y - r * (1 - Mathf.Cos(bta)) * Mathf.Sin(theta);
            float z = r * (1 - Mathf.Cos(bta)) * Mathf.Cos(theta);

            return new Vector3(x, y, z);
        }
    }
}

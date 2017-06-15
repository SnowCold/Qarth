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
    public enum FlipMode
    {
        RightToLeft,
        LeftToRight
    }
    
    public class BookEffect : MonoBehaviour
    {
        public Canvas canvas;
        [SerializeField]
        RectTransform BookPanel;

        public Image ClippingPlane;
        public Image Right;

        protected float radius1, radius2;
        //Spine Bottom
        protected Vector3 sb;
        //Spine Top
        protected Vector3 st;
        //corner of the page
        protected Vector3 c;
        //Edge Bottom Right
        protected Vector3 ebr;
        //Edge Bottom Left
        protected Vector3 ebl;
        //follow poi
        protected Vector3 f;
        //current flip mode
        protected FlipMode mode = FlipMode.RightToLeft;

        bool pageDragging = false;
        public bool interactable = true;
        void Start()
        {
            float scaleFactor = 1;
            if (canvas)
            {
                scaleFactor = canvas.scaleFactor;
            }

            float pageWidth = (BookPanel.rect.width * scaleFactor - 1) / 2;
            float pageHeight = BookPanel.rect.height * scaleFactor;
            //Left.gameObject.SetActive(false);
            //Right.gameObject.SetActive(false);
            //UpdateSprites();
            Vector3 globalsb = BookPanel.transform.position + new Vector3(0, -pageHeight / 2);
            sb = transformPoint(globalsb);
            Vector3 globalebr = BookPanel.transform.position + new Vector3(pageWidth, -pageHeight / 2);
            ebr = transformPoint(globalebr);
            Vector3 globalebl = BookPanel.transform.position + new Vector3(-pageWidth, -pageHeight / 2);
            ebl = transformPoint(globalebl);
            Vector3 globalst = BookPanel.transform.position + new Vector3(0, pageHeight / 2);
            st = transformPoint(globalst);
            radius1 = Vector2.Distance(sb, ebr);
            float scaledPageWidth = pageWidth / scaleFactor;
            float scaledPageHeight = pageHeight / scaleFactor;
            radius2 = Mathf.Sqrt(scaledPageWidth * scaledPageWidth + scaledPageHeight * scaledPageHeight);
            ClippingPlane.rectTransform.sizeDelta = new Vector2(scaledPageWidth * 2, scaledPageHeight + scaledPageWidth * 2);
            //Shadow.rectTransform.sizeDelta = new Vector2(scaledPageWidth, scaledPageHeight + scaledPageWidth * 0.6f);
            //ShadowLTR.rectTransform.sizeDelta = new Vector2(scaledPageWidth, scaledPageHeight + scaledPageWidth * 0.6f);
            //NextPageClip.rectTransform.sizeDelta = new Vector2(scaledPageWidth, scaledPageHeight + scaledPageWidth * 0.6f);
        }

        public Vector3 transformPoint(Vector3 global)
        {
            Vector2 localPos = BookPanel.InverseTransformPoint(global);
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(BookPanel, global, null, out localPos);
            return localPos;
        }

        void Update()
        {
            if (pageDragging && interactable)
            {
                UpdateBook();
            }
            //Log.i("mouse local pos:" + transformPoint(Input.mousePosition));
            //Log.i("mouse  pos:" + Input.mousePosition);
        }

        public void UpdateBook()
        {
            f = Vector3.Lerp(f, transformPoint(Input.mousePosition), Time.deltaTime * 10);
            if (mode == FlipMode.RightToLeft)
            {
                UpdateBookRTLToPoint(f);
            }
            else
            {
                //UpdateBookLTRToPoint(f);
            }
        }

        public void UpdateBookRTLToPoint(Vector3 followLocation)
        {
            mode = FlipMode.RightToLeft;
            f = followLocation;
            //Shadow.transform.SetParent(ClippingPlane.transform, true);
            //Shadow.transform.localPosition = new Vector3(0, 0, 0);
            //Shadow.transform.localEulerAngles = new Vector3(0, 0, 0);
            Right.transform.SetParent(ClippingPlane.transform, true);

            //Left.transform.SetParent(BookPanel.transform, true);
            //RightNext.transform.SetParent(BookPanel.transform, true);
            c = Calc_C_Position(followLocation);
            Vector3 t1;
            float T0_T1_Angle = Calc_T0_T1_Angle(c, ebr, out t1);
            if (T0_T1_Angle >= -90) T0_T1_Angle -= 180;

            ClippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);
            ClippingPlane.transform.eulerAngles = new Vector3(0, 0, T0_T1_Angle + 90);
            ClippingPlane.transform.position = BookPanel.TransformPoint(t1);

            //page position and angle
            Right.transform.position = BookPanel.TransformPoint(c);
            float C_T1_dy = t1.y - c.y;
            float C_T1_dx = t1.x - c.x;
            float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
            Right.transform.eulerAngles = new Vector3(0, 0, C_T1_Angle);

            //NextPageClip.transform.eulerAngles = new Vector3(0, 0, T0_T1_Angle + 90);
            //NextPageClip.transform.position = BookPanel.TransformPoint(t1);
            //RightNext.transform.SetParent(NextPageClip.transform, true);
            //Left.transform.SetParent(ClippingPlane.transform, true);
            //Left.transform.SetAsFirstSibling();

            //Shadow.rectTransform.SetParent(Right.rectTransform, true);
        }

        private float Calc_T0_T1_Angle(Vector3 c, Vector3 bookCorner, out Vector3 t1)
        {
            Vector3 t0 = (c + bookCorner) / 2;
            float T0_CORNER_dy = bookCorner.y - t0.y;
            float T0_CORNER_dx = bookCorner.x - t0.x;
            float T0_CORNER_Angle = Mathf.Atan2(T0_CORNER_dy, T0_CORNER_dx);
            float T0_T1_Angle = 90 - T0_CORNER_Angle;

            float T1_X = t0.x - T0_CORNER_dy * Mathf.Tan(T0_CORNER_Angle);
            T1_X = normalizeT1X(T1_X, bookCorner, sb);
            t1 = new Vector3(T1_X, sb.y, 0);
            ////////////////////////////////////////////////
            //clipping plane angle=T0_T1_Angle
            float T0_T1_dy = t1.y - t0.y;
            float T0_T1_dx = t1.x - t0.x;
            T0_T1_Angle = Mathf.Atan2(T0_T1_dy, T0_T1_dx) * Mathf.Rad2Deg;
            return T0_T1_Angle;
        }

        private float normalizeT1X(float t1, Vector3 corner, Vector3 sb)
        {
            if (t1 > sb.x && sb.x > corner.x)
                return sb.x;
            if (t1 < sb.x && sb.x < corner.x)
                return sb.x;
            return t1;
        }

        private Vector3 Calc_C_Position(Vector3 followLocation)
        {
            Vector3 c;
            f = followLocation;
            float F_SB_dy = f.y - sb.y;
            float F_SB_dx = f.x - sb.x;
            float F_SB_Angle = Mathf.Atan2(F_SB_dy, F_SB_dx);
            Vector3 r1 = new Vector3(radius1 * Mathf.Cos(F_SB_Angle), radius1 * Mathf.Sin(F_SB_Angle), 0) + sb;

            float F_SB_distance = Vector2.Distance(f, sb);
            if (F_SB_distance < radius1)
                c = f;
            else
                c = r1;
            float F_ST_dy = c.y - st.y;
            float F_ST_dx = c.x - st.x;
            float F_ST_Angle = Mathf.Atan2(F_ST_dy, F_ST_dx);
            Vector3 r2 = new Vector3(radius2 * Mathf.Cos(F_ST_Angle),
               radius2 * Mathf.Sin(F_ST_Angle), 0) + st;
            float C_ST_distance = Vector2.Distance(c, st);
            if (C_ST_distance > radius2)
                c = r2;
            return c;
        }
    }
    
}

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
    [ExecuteInEditMode]
    public class RaycastArea : UnityEngine.UI.MaskableGraphic
    {
        //private static Color HideColor = new Color(0, 0, 0, 0);

        protected RaycastArea()
        {
            useLegacyMeshGeneration = false;
        }
        /*
        protected override void Awake()
        {
            base.Awake();
            useLegacyMeshGeneration = false;
            color = HideColor;
        }
        */

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

        protected override void UpdateGeometry()
        {
            base.UpdateGeometry();
            gameObject.layer = LayerDefine.LAYER_HIDE_UI;
        }
    }
}

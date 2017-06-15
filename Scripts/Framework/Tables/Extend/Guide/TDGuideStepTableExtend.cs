//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Qarth;

namespace Qarth
{
    public partial class TDGuideStepTable
    {
        static void CompleteRowAdd(TDGuideStep tdData)
        {

        }

		public static List<TDGuideStep> GetDataAsGuideID(int guideID)
		{
			List<TDGuideStep> result = new List<TDGuideStep> ();
			for (int i = 0; i < m_DataList.Count; ++i)
			{
				if (m_DataList[i].guideID == guideID)
				{
					result.Add (m_DataList [i]);
				}
			}

			return result;
		}

		public static TDGuideStep GetGuideFirstStep(int guideID)
		{
			for (int i = 0; i < m_DataList.Count; ++i)
			{
				if (m_DataList[i].guideID == guideID)
				{
					return m_DataList [i];
				}
			}

			return null;
		}

		public static TDGuideStep GetGuideLastStep(int guideID)
		{
			for (int i = m_DataList.Count - 1; i >= 0; --i)
			{
				if (m_DataList[i].guideID == guideID)
				{
					return m_DataList [i];
				}
			}

			return null;
		}
    }
}

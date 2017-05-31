using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using PTGame.Framework;

namespace PTGame.Framework
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
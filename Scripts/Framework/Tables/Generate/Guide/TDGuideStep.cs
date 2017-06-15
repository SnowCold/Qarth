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
    public partial class TDGuideStep
    {
      
        private EInt m_Id = 0;
        private EInt m_GuideID = 0;
		private EInt m_RequireStepId = 0;
        private string m_Trigger;
        private string m_Command;
        private string m_CommonParam;
		private bool m_KeyFrame;
      
        /// <summary>
        /// ID
        /// </summary>
        public int id { get { return m_Id; } }
      
        /// <summary>
        /// GuideID
        /// </summary>
        public int guideID { get { return m_GuideID; } }
      
		public int requireStepId { get { return m_RequireStepId; } }

        /// <summary>
        /// Trigger1
        /// </summary>
        public string trigger {get { return m_Trigger; } }
      
        /// <summary>
        /// Command1
        /// </summary>
        public string command {get { return m_Command; } }
      
        /// <summary>
        /// CommandCommonParam
        /// </summary>
        public string commonParam {get { return m_CommonParam; } }

		public bool keyFrame { get { return m_KeyFrame; } }

        public void ReadRow(DataStreamReader dataR, int[] filedIndex)
        {
          int col = 0;
          while(true)
          {
            col = dataR.MoreFieldOnRow();
            if (col == -1)
            {
              break;
            }
            switch (filedIndex[col])
            { 
                case 0:
                    m_Id = dataR.ReadInt();
                    break;
                case 1:
                    m_GuideID = dataR.ReadInt();
                    break;
                case 2:
                    m_Trigger = dataR.ReadString();
                    break;
                case 3:
                    m_Command = dataR.ReadString();
                    break;
                case 4:
                    m_CommonParam = dataR.ReadString();
                    break;
				case 5:
					m_RequireStepId = dataR.ReadInt();
					break;
				case 6:
					m_KeyFrame = dataR.ReadBool ();
					break;
                default:
                    break;
            }
          }

        }
        
        public static Dictionary<string, int> GetFieldHeadIndex()
        {
          Dictionary<string, int> ret = new Dictionary<string, int>(9);
          
          ret.Add("Id", 0);
          ret.Add("GuideID", 1);
          ret.Add("Trigger", 2);
          ret.Add("Command", 3);
          ret.Add("CommonParam", 4);
		  ret.Add ("RequireStepId", 5);
		  ret.Add ("KeyFrame", 6);
          return ret;
        }
        
        
    }
}//namespace LR

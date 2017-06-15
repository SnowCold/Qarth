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
    public partial class TDGuide
    {
      
        private EInt m_Id = 0;
        private string m_Trigger;
		private string m_CommonParam;
        private string m_JumpTrigger;
		private EInt m_RequireGuideId = 0;
      	
        /// <summary>
        /// ID
        /// </summary>
        public int id {get { return m_Id; } }
      
        /// <summary>
        /// Trigger1
        /// </summary>
        public string trigger {get { return m_Trigger; } }

        public string jumpTrigger { get { return m_JumpTrigger; } }

		/// <summary>
		/// CommandCommonParam
		/// </summary>
		public string commonParam {get { return m_CommonParam; } }

		public int requireGuideId { get { return m_RequireGuideId; } }

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
                    m_Trigger = dataR.ReadString();
                    break;
				case 2:
					m_CommonParam = dataR.ReadString();
					break;
				case 3:
					m_RequireGuideId = dataR.ReadInt();
					break;
                case 4:
                    m_JumpTrigger = dataR.ReadString();
                    break;
                default:
                    break;
            }
          }

        }
        
        public static Dictionary<string, int> GetFieldHeadIndex()
        {
          Dictionary<string, int> ret = new Dictionary<string, int>(4);
          
          ret.Add("Id", 0);
          ret.Add("Trigger", 1);
	      ret.Add ("CommonParam", 2);
		  ret.Add("RequireGuideId", 3);
          ret.Add("JumpTrigger", 4);
          return ret;
        }
        
        
    }
}//namespace LR

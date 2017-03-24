using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using System.Diagnostics;

namespace PTGame.Framework
{
	[TMonoSingletonAttribute("[Tools]/DebugLogger")]
	public class DebugLogger : TMonoSingleton<DebugLogger>
	{
		public static int MAX_DUMP_STACK_LINE = 6;
		
		private static List<string>     m_Lines = new List<string>();
        private static List<string>     m_WriteTxt = new List<string>();
		private string                  m_Outpath;

        public void InitDebugLogger()
        {
            Log.i("Init[DebugLogger]");
        }

        #region DumpStack
        public static void DumpStack()
        {
            StackTrace sT = new StackTrace(true);
            string msg = "";
            for (int i = 1; i < MAX_DUMP_STACK_LINE; ++i)
            {
                StackFrame frame = sT.GetFrame(i);
                if (frame == null)
                {
                    break;
                }
                String flName = frame.GetFileName();
                int lineNo = frame.GetFileLineNumber();
                String methodName = frame.GetMethod().Name;
                msg += String.Format("{0}: {1}() :[{2}]\n", flName, methodName, lineNo);
            }
            if (msg.Length > 0)
            {
                Log.w("********** BEGIN STACK **********");
                Log.w(msg);
                Log.w("**********  END STACK **********");
            }
        }
        #endregion


        private void Start()
		{
			
			if(AppConfig.S.dumpPath.Length > 0)
			{
				m_Outpath = AppConfig.S.dumpPath + "/outLog.txt";
				Log.i("日志记录文件：" + m_Outpath);
			}
			else
			{
				m_Outpath = Application.persistentDataPath + "/outLog.txt";
				Log.i("日志记录文件：" + m_Outpath);
			}

			if(AppConfig.S.AppMode != APP_MODE.ReleaseMode && AppConfig.S.dumpToFile)
			{
				if(System.IO.File.Exists(m_Outpath))
				{
					File.Delete(m_Outpath);
				}
				Application.logMessageReceived += HandleLog;
			}
			
		}
		
		private void Update()
		{
			//因为写入文件的操作必须在主线程中完成，所以在Update中哦给你写入文件。
			if(m_WriteTxt.Count > 0)
			{
				string[] temp = m_WriteTxt.ToArray();
				foreach(string t in temp)
				{
					using(StreamWriter writer = new StreamWriter(m_Outpath, true, Encoding.UTF8))
					{
						writer.WriteLine(t);
					}
					m_WriteTxt.Remove(t);
				}
			}
		}
		
		private void HandleLog(string logString, string stackTrace, LogType type)
		{
			m_WriteTxt.Add(logString);
			if(type == LogType.Error || type == LogType.Exception || type == LogType.Warning)
			{
				SaveLog(logString);
				SaveLog(stackTrace);
			}
		}

		static public void SaveLog(params object[] objs)
		{
			string text = "";
			for(int i = 0; i < objs.Length; ++i)
			{
				if(i == 0)
				{
					text += objs[i].ToString();
				}
				else
				{
					text += ", " + objs[i].ToString();
				}
			}
			if(Application.isPlaying)
			{
				if(m_Lines.Count > 20)
				{
					m_Lines.RemoveAt(0);
				}
				m_Lines.Add(text);
			}
		}
	  
		void OnGUI()
		{
			
			if(AppConfig.S.dumpToScreen && m_Lines.Count > 0)
			{
				GUI.color = Color.red;
				for(int i = 0, imax = m_Lines.Count; i < imax; ++i)
				{
					GUILayout.Label(m_Lines[i]);
				}
			}
			
		}

	}

}


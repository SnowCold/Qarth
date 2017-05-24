using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
	public class MonoFuncCall : IUINodeFinder
	{
		private UINodeFinder m_Finder;
		private string m_TypeName;
		private string m_FuncName;
		private string[] m_FuncParam;
		private Transform m_Result;

		public override string ToString ()
		{
			return string.Format("Panel:{0},UI:{1}", m_TypeName, m_FuncName);
		}

		public void SetParam(string[] param)
		{
			if (param.Length < 4)
			{
				return;
			}

			m_Finder = new UINodeFinder();
			m_Finder.SetParam(param);

			m_TypeName = param[2];
			m_FuncName = param[3];

			if (param.Length > 4)
			{
				m_FuncParam = new string[param.Length - 4];
				for (int i = 0; i < m_FuncParam.Length; ++i)
				{
					m_FuncParam [i] = param[i + 4];
				}
			}
		}

		public Transform FindNode(bool search)
		{
			//if (search)
			{
				m_Result = FindNodeInner ();
			}

			return m_Result;
		}


		private Transform FindNodeInner()
		{
			Transform node = m_Finder.FindNode(true);
			if (node == null)
			{
				return null;
			}

			Type targetType = Type.GetType("PTGame.PaiLogic." + m_TypeName);

			if (targetType == null)
			{
				Log.e ("Not Find Type Class:" + m_TypeName);
				return null;
			}

			Component com = node.GetComponent(targetType);

			if (com == null)
			{
				Log.e ("Not Find Componment:" + m_TypeName);
				return null;
			}

			var methodInfo = targetType.GetMethod(m_FuncName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

			if (methodInfo == null)
			{
				Log.e ("Not Find Method Info:" + m_FuncName);
				return null;
			}

			//timer.Begin("P2");
			object result = null;

			try
			{
				result = methodInfo.Invoke(com, m_FuncParam);
			}
			catch (Exception e)
			{
				result = null;
				Log.e(e);
			}

			return result as Transform;
		} 
	}
}


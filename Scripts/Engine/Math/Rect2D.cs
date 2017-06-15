//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System.Collections;

namespace Qarth
{

	public class Rect2D
	{
		private float m_X, m_Y, m_Width, m_Height;
		private float m_MinX, m_MinY, m_MaxX, m_MaxY;

		public float xMin
		{
			get { return m_MinX; }
		}

		public float xMax
		{
			get { return m_MaxX; }
		}

		public float yMin
		{
			get { return m_MinY; }
		}

		public float yMax
		{
			get { return m_MaxY; }
		}
		
		public float x
		{
			get { return m_X; }
			set { m_X = value; UpdateValue(); }
		}
		
		public float y
		{
			get { return m_Y; }
			set { m_Y = value; UpdateValue(); }
		}
		
		public float width
		{
			get { return m_Width; }
			set { m_Width = value; UpdateValue(); }
		}
		
		public float height
		{
			get { return m_Height; }
			set { m_Height = value; UpdateValue(); }
		}
		
		
		public Rect2D(float x, float y, float width, float height)
		{
			m_X = x;
			m_Y = y;
			m_Width = width;
			m_Height = height;
			UpdateValue();
		}
		
		public bool IsInRect(float x, float y)
		{
			if(x < m_MinX || x > m_MaxX || y < m_MinY || y > m_MaxY)
			{
				return false;
			}
			return true;
		}
		
		protected void UpdateValue()
		{
			m_MinX = m_X - m_Width * 0.5f;
			m_MaxX = m_X + m_Width * 0.5f;
			m_MinY = m_Y - m_Height * 0.5f;
			m_MaxY = m_Y + m_Height * 0.5f;
		}
	}

}

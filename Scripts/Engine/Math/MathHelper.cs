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
	
	public enum eMovingDir
	{
		kNull,
		kUp,
		kDown,
		kLeft,
		kRight,
		kUp_Right,
		kUp_Left,
		kDown_Right,
		kDown_Left
	}

	public class MathHelper
	{
		public static float AngleOfVector(Vector3 v)
		{
			return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
		}

		public static float AngleFrom2Position(Vector3 from, Vector3 to)
		{
			//[0-180][-180-0]
			float angle = Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg;//Math.Atan2((Y2 - Y1), (X2 - X2)) * 180 / Math.PI
			/*
			if (angle < 0)
			{
				angle += 360;
			}
			*/
			return angle;
		}


		public static eMovingDir Angle2AnimDir(float angle)
		{
			if(angle >= -45.0f)
			{
				if(angle <= 30.0f)//45
				{
					return eMovingDir.kRight;
				}
				else if(angle <= 90.0f)
				{
					return eMovingDir.kUp_Right;
				}
				else if(angle <= 150.0f)
				{
					return eMovingDir.kUp_Left;
				}
				/*
				else if(angle < 135.0f)
				{
					return eMovingDir.kUp;
				}
				*/
				return eMovingDir.kLeft;
			}
			else
			{
				if(angle >= -90.0f)
				{
					return eMovingDir.kDown_Right;
				}
				else if(angle >= -135.0f)
				{
					return eMovingDir.kDown_Left;
				}
				/*
				if(angle >= -135.0f)
				{
					return eMovingDir.kDown;
				}
				*/
				return eMovingDir.kLeft;
			}
		}
	}

}


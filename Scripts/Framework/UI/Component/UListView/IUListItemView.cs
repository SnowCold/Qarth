//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using UnityEngine.UI;

namespace Qarth
{
	public abstract class IUListItemView : MonoBehaviour
	{
		RectTransform rectTransform;

		public virtual Vector2 GetItemSize(int index)
		{
			if (null == rectTransform) 
			{
				rectTransform = transform as RectTransform;
			}
			return rectTransform.rect.size;
		}
	}
}

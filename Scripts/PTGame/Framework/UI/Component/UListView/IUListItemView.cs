//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/PTFramework
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using UnityEngine.UI;

namespace SCEngine
{
	public abstract class IUListItemView : MonoBehaviour
	{
		RectTransform rectTransform;

		public abstract void SetData(int index, object data);

		public virtual Vector2 GetItemSize(object data)
		{
			if (null == rectTransform) 
			{
				rectTransform = transform as RectTransform;
			}
			return rectTransform.rect.size;
		}
	}
}

//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Qarth
{
	public class UGridListView : IUListView
	{
		[Tooltip("List Item Object, must set")]
		public GameObject			itemPrefab;
		protected List<GameObject>	lstItems;

		private int 				numPerRow;
		private	int					numPerColumn;
		private	Vector2				padding;
		private Vector2				itemSize;

		public override void Init ()
		{
			base.Init ();

			// record the item size
			IUListItemView itemView = itemPrefab.GetComponent<IUListItemView> ();
			itemSize = itemView.GetItemSize (null);

			// record max numbers per row/column
			numPerRow = (int)(scrollRectSize.x / (itemSize.x + spacing.x));
			numPerColumn = (int)(scrollRectSize.y / (itemSize.y + spacing.y));
			if (numPerRow < 1 || numPerColumn < 1) 
			{
				Debug.LogError("ScrollRect size is too small to contain even one item");
			}

			// to make items center aligned
			padding = Vector2.zero;

			// spawn pool for listitems
			lstItems = new List<GameObject> ();
		}

		public override int	GetMaxShowItemNum()
		{
			int max = 0;
			// calculate the max show nums
			switch (layout) 
			{
			case Layout.Horizontal:
				max = ((int)(scrollRectSize.x / itemSize.x) + 2) * numPerColumn;
				break;
			case Layout.Vertical:
				max = ((int)(scrollRectSize.y / itemSize.y) + 2) * numPerRow;
				break;
			}
			return max;
		}

		public override int GetStartIndex()
		{
			Vector2 anchorPosition = content.anchoredPosition;
			anchorPosition.x *= -1;
			int index = 0;

			switch (layout)
			{
			case Layout.Vertical:
				index = (int)(anchorPosition.y / (itemSize.y + spacing.y)) * numPerRow;
				break;
			case Layout.Horizontal:
				index = (int)(anchorPosition.x / (itemSize.x + spacing.x)) * numPerColumn;
				break;
			}
			if (index < 0)	index = 0;
            if (index >= lstData.Count) index = 0;
			return index;
		}

		public override Vector2 GetItemAnchorPos(int index)
		{
			Vector2 basePos = Vector2.zero;
			Vector2 offset = Vector2.zero;
			RectTransform contentRectTransform = content.transform as RectTransform;
			Vector2 contentRectSize = contentRectTransform.rect.size;

			if (layout == Layout.Horizontal) 
			{
				int offsetIndex = index % numPerColumn;
				basePos.x = -contentRectSize.x / 2 + itemSize.x / 2;
				offset.x = (index / numPerColumn) * (itemSize.x + spacing.x);
				offset.y = contentRectSize.y / 2 - itemSize.y / 2 - offsetIndex * (itemSize.y + spacing.y);
			} 
			else 
			{
                int offsetIndex = index % numPerRow;
				basePos.y = contentRectSize.y / 2 - itemSize.y / 2;
				offset.y = -(index / numPerRow) * (itemSize.y + spacing.y);
				offset.x = -(contentRectSize.x - itemSize.x)/2 + offsetIndex * (itemSize.x + spacing.x);
			}

			return basePos + offset + padding;
		}

		public override Vector2 GetContentSize()
		{
			Vector2 size = scrollRectSize;
			int count = lstData.Count;

			switch (layout) 
			{
			case Layout.Horizontal:
				count = (count + numPerColumn - 1) / numPerColumn;
				size.x = itemSize.x * count + spacing.x *( count > 0 ? count -1 : count );
				break;
			case Layout.Vertical:
				count = (count + numPerRow - 1) / numPerRow;
				size.y = itemSize.y * count + spacing.y * ( count > 0 ? count - 1 : count );
				break;
			}
			return size;
		}

		public override GameObject GetItemGameObject(Transform content, int index)
		{
			if(index < lstItems.Count)
			{
				GameObject go = lstItems[index];
				if(false == go.activeSelf)
				{
					go.SetActive(true);
				}
				return lstItems [index];
			}
			else 
			{
				GameObject go = GameObject.Instantiate(itemPrefab, content, false) as GameObject;
				lstItems.Add (go);
				return go;
			}
		}
		
		public override void HideNonuseableItems ()
		{
			for (int i = lstData.Count; lstItems != null && i < lstItems.Count; ++i) 
			{
				if(lstItems[i].activeSelf)
				{
					lstItems[i].SetActive(false);
				}
			}
		}

		public override Vector2	GetItemSize(int index)
		{
			return itemSize;
		}
	}
}

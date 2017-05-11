using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

namespace PTGame.Framework
{
	public enum Alignment
	{
		Left,
		Mid,
		Right,
		Top,
		Bottom
	}

	public class USimpleListView : IUListView
	{
		[Tooltip("List Item Object, must set")]
		public GameObject			itemPrefab;
		public Alignment 			alignment;
		private List<GameObject>	lstItems;
		private Vector2				itemSize;

		public override void Init ()
		{
			base.Init ();

			// record the item size
			IUListItemView itemView = itemPrefab.GetComponent<IUListItemView> ();
			itemSize = itemView.GetItemSize (null);

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
				max = (int) Mathf.Ceil(scrollRectSize.x / (itemSize.x + spacing.x)) + 1;
				break;
			case Layout.Vertical:
				max = (int) Mathf.Ceil(scrollRectSize.y / (itemSize.y + spacing.y)) + 1;
				break;
			}
			return max;
		}

		public override int GetStartIndex()
		{
			Vector2 anchorPosition = content.anchoredPosition;
			// because the anchor is relative to the left-top corner of the scrollview
			// so the passed x is on the left, the passed y is on the top
			anchorPosition.x *= -1;	
			int index = 0;

			switch (layout)
			{
			case Layout.Vertical:
				index = (int)(anchorPosition.y / (itemSize.y + spacing.y));
				break;
			case Layout.Horizontal:
				index = (int)(anchorPosition.x / (itemSize.x + spacing.x));
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
				basePos.x = -contentRectSize.x / 2 + itemSize.x / 2;
				offset.x = index * (itemSize.x + spacing.x);
				switch(alignment)
				{
				case Alignment.Top:
					offset.y = -(contentRectSize.y - itemSize.y)/2;
					break;
				case Alignment.Bottom:
					offset.y = (contentRectSize.y - itemSize.y)/2;
					break;
				}
			} 
			else 
			{
				basePos.y = contentRectSize.y / 2 - itemSize.y / 2;
				offset.y = -index * (itemSize.y + spacing.y);
				switch(alignment)
				{
				case Alignment.Left:
					offset.x = -(contentRectSize.x - itemSize.x)/2;
					break;
				case Alignment.Right:
					offset.x = (contentRectSize.x - itemSize.x)/2;
					break;
				}
			}

			return basePos + offset;
		}

		public override Vector2 GetContentSize()
		{
			Vector2 size = scrollRectSize;
			int count = lstData.Count;
			switch (layout) 
			{
			case Layout.Horizontal:
				size.x = itemSize.x * count + spacing.x *( count > 0 ? count - 1 : count );
				break;
			case Layout.Vertical:
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
			for (int i = GetCurrentShowItemNum(); lstItems != null && i < lstItems.Count; ++i) 
			{
                if (lstItems[i].activeSelf)
                {
                    lstItems[i].SetActive(false);
                }
			}
		}

		public override Vector2		GetItemSize(int index)
		{
			return itemSize;
		}
	}
}
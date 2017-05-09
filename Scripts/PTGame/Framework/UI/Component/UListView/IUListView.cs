using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace NSUListView
{	
	public enum Layout
	{
		Vertical,
		Horizontal
	}

	[RequireComponent(typeof(ScrollRect))]
	public abstract class IUListView : MonoBehaviour, IPointerClickHandler
	{
		public Layout 				layout;
		public Vector2				spacing;
		public bool					needMask;

		protected ScrollRect 		scrollRect;
		protected bool				initialized = false;
		protected RectTransform		content;
		protected Vector2			scrollRectSize;
		protected int				lastStartInex = 0;
		protected List<object>		lstData;
		protected Vector2			leftTopCorner = Vector2.zero;
		private bool				leftTopCornerInit = false;

		public virtual void Init()
		{
			// set attributes of scrollrect
			scrollRect = GetComponent<ScrollRect> ();
			scrollRect.onValueChanged.AddListener (OnValueChanged);

			// set the scroll direction
			switch (layout) 
			{
			case Layout.Horizontal:
				scrollRect.horizontal = true;
				scrollRect.vertical = false;
				break;
			case Layout.Vertical:
				scrollRect.horizontal = false;
				scrollRect.vertical = true;
				break;
			}

			// add a scrollrect content
			GameObject go = new GameObject ();
			go.name = "content";
			content = go.AddComponent (typeof(RectTransform)) as RectTransform;
			content.SetParent (transform);
			content.pivot = new Vector2 (0, 1);
			content.anchorMin = content.anchorMax = content.pivot;
			content.anchoredPosition = Vector2.zero;
			content.localScale = Vector3.one;
			scrollRect.content = content;

			// record some sizes
			RectTransform scrollRectTransform = scrollRect.transform as RectTransform;
			scrollRectSize = scrollRectTransform.rect.size;

			// add mask
			if (needMask) 
			{
				Image image = gameObject.AddComponent(typeof(Image)) as Image;
				image.color = new Color32(0, 0, 0, 5);
				gameObject.AddComponent(typeof(Mask));
			}
		}

		public void  SetData(List<object> lstData)
		{
			if (false == initialized) 
			{
				Init();
				initialized = true;
			}

			this.lstData = lstData;
			RefreshListView ();
		}

		private void OnValueChanged(Vector2 pos)
		{
			int startIndex = GetStartIndex ();
			if (startIndex != lastStartInex 
			    && startIndex >= 0)
			{
				RefreshListView();
				lastStartInex = startIndex;
			}
		}

		protected void RefreshListView()
		{
			// set the content size
			Vector2 size = GetContentSize ();
			RectTransform contentRectTransform = content.transform as RectTransform;
			contentRectTransform.sizeDelta = size;

			// set the item postion and data
			int startIndex = GetStartIndex ();
			if (startIndex < 0)	startIndex = 0;

			for (int i=0; i<GetCurrentShowItemNum(); ++i)
			{
				GameObject go = GetItemGameObject(i);
				RectTransform trans = go.transform as RectTransform;
				trans.SetParent(content);
				trans.pivot = trans.anchorMin = trans.anchorMax = new Vector2(0.5f, 0.5f);
				trans.anchoredPosition = GetItemAnchorPos(startIndex + i);
				trans.localScale = Vector3.one;
				IUListItemView itemView = go.GetComponent<IUListItemView>();
				itemView.SetData(lstData[startIndex + i]);
			}

			// dont show the extra items shown before
			HideNonuseableItems ();

			// set the progress: 
			int dataCount = GetDataCount();
			dataCount -= (GetMaxShowItemNum ()-2);
			if (dataCount < 1) dataCount = 1;
			float progress = (startIndex + 1)/(float)dataCount;
			progress = Mathf.Clamp01(progress);
			OnProgress(progress);
		}

		/// <summary>
		/// Gets the current show item number.
		/// </summary>
		/// <returns>The current show item number.</returns>
		protected int GetCurrentShowItemNum()
		{
			int startIndex = GetStartIndex ();
			int maxShowNum = GetMaxShowItemNum ();
			int maxItemNum = lstData.Count - startIndex;
			return maxShowNum < maxItemNum ? maxShowNum : maxItemNum;
		}

		/// <summary>
		/// Gets the top left corner screen point.
		/// </summary>
		/// <returns>The top left corner screen point.</returns>
		private Vector2 GetTopLeftCornerScreenPoint()
		{
			if (false == leftTopCornerInit) 
			{
				RectTransform rectTrans = scrollRect.transform as RectTransform;
				Vector3[] corners = new Vector3[4];
				rectTrans.GetWorldCorners (corners);
				Canvas canvas = GetComponentInParent<Canvas> ();
				if (null != canvas && null != canvas.worldCamera && RenderMode.ScreenSpaceOverlay != canvas.renderMode) {
					Camera cam = canvas.worldCamera;
					corners [1] = cam.WorldToScreenPoint (corners [1]);
				}
				leftTopCorner = new Vector2 (corners [1].x, corners [1].y);
			}
			return leftTopCorner;
		}

		/// <summary>
		/// Raises the pointer enter event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerClick(PointerEventData eventData)
		{
			//get the pos relative to left-top corner of the scrollview
			Vector2 clickPos =  eventData.position - GetTopLeftCornerScreenPoint();
			Vector2 anchorPosition = -content.anchoredPosition;

			anchorPosition += clickPos;
			anchorPosition.x -= content.rect.size.x / 2;
			anchorPosition.y += content.rect.size.y / 2;

			// set the item postion and data
			int startIndex = GetStartIndex ();
			if (startIndex < 0)	startIndex = 0;
			
			for (int i=0; i<GetCurrentShowItemNum(); ++i)
			{
				Vector2 itemAnchorPos = GetItemAnchorPos(startIndex + i);
				Vector2 itemSize = GetItemSize(startIndex + i);
				if(Mathf.Abs(anchorPosition.x - itemAnchorPos.x) <= itemSize.x/2 &&
				   Mathf.Abs(anchorPosition.y - itemAnchorPos.y) <= itemSize.y/2)
				{
					OnClick(startIndex + i);
					break;
				}
			}
		}

		/// <summary>
		/// Raises the click event.
		/// </summary>
		/// <param name="pos">Position.</param>
		public virtual void OnClick(int index)
		{
		}

		/// <summary>
		/// Raises the progress event when progress change
		/// </summary>
		/// <param name="progress">Progress.</param>
		public virtual void OnProgress(float progress)
		{
		}

		/// <summary>
		/// Gets the data count.
		/// default return 1
		/// </summary>
		/// <returns>The data count.</returns>
		public virtual int GetDataCount()
		{
			if (null == lstData)return 1;
			else return lstData.Count;
		}

		/// <summary>
		/// Gets the max show item number.
		/// </summary>
		/// <returns>The max show item number.</returns>
		public abstract int			GetMaxShowItemNum();
		/// <summary>
		/// Gets the rect tranform size of the content of the ScrollRect
		/// </summary>
		/// <returns>The content size.</returns>
		public abstract Vector2 	GetContentSize();
		/// <summary>
		/// Gets the anchor position of the item indexed index
		/// Assume that anchorMin = anchorMax = pivot = new Vector(0.5f, 0.5f)
		/// </summary>
		/// <returns>The item anchor position.</returns>
		/// <param name="index">index of item</param>
		public abstract Vector2 	GetItemAnchorPos(int index);
		/// <summary>
		/// Gets the start index of the item to be shown
		/// </summary>
		/// <returns>The start index(start from 0)</returns>
		public abstract int 		GetStartIndex();
		/// <summary>
		/// Gets the item game object.
		/// </summary>
		/// <returns>The item game object.</returns>
		/// <param name="index">Index.</param>
		public abstract GameObject	GetItemGameObject(int index);
		/// <summary>
		/// Hides the nonuseable items.
		/// </summary>
		public abstract	void		HideNonuseableItems();
		/// <summary>
		/// Gets the size of the item of specified index
		/// </summary>
		/// <returns>The item size.</returns>
		/// <param name="index">Index.</param>
		public abstract Vector2		GetItemSize(int index);
	}
}
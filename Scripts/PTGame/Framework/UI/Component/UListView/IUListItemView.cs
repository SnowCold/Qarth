using UnityEngine;
using UnityEngine.UI;

namespace NSUListView
{
	public abstract class IUListItemView : MonoBehaviour
	{
		RectTransform rectTransform;

		public abstract void SetData(object data);

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
using UnityEngine;
using UnityEngine.UI;

namespace PTGame.Framework
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
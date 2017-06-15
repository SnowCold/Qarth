//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;

namespace Qarth
{
    public static class TweenHelper
    {
        public static Tweener DoText(this Text label, string context, float speed)
        {
            if (label == null || string.IsNullOrEmpty(context))
            {
                return null;
            }
            float duration = context.Length * speed;

            label.text = "";

            int index = 0;
            DOGetter<int> getter = () =>
            {
                return index;
            };

            DOSetter<int> setter = (x) =>
            {
                index = x;
                label.text = context.Substring(0, x);
            };

            return DOTween.To(getter, setter, context.Length, duration);
        }

        public static Tweener DoDelay(float duration)
        {
            int index = 0;
            DOGetter<int> getter = () =>
            {
                return index;
            };

            DOSetter<int> setter = (x) =>
            {
                index = x;
            };

            return DOTween.To(getter, setter, 1, duration);
        }

		public static Tweener DoSpriteScale(Image image,Run<bool> callback,bool bWin)
		{
			if (image == null)
			{
				return null;
			}

			return image.transform.DOScale(Vector3.one,0.8f).OnComplete(()=>{
				image.transform.DOScale(Vector3.zero,0.8f).SetDelay(2f).OnComplete(()=>{
					if(callback != null)
						callback(bWin);
				});
			});
		}

        public static Sequence DoScale(Transform target, float dstValue, float duration, int loop = -1)
        {
            Vector3 originScale = target.transform.localScale;

            return DOTween.Sequence()
                .Append(target.transform.DOScale(dstValue, duration * 0.5f).SetEase(Ease.InSine))
                .Append(target.transform.DOScale(originScale, duration * 0.5f).SetEase(Ease.OutSine))
                .SetLoops(loop);
        }
        
    }
}

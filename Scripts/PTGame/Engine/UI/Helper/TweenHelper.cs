using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;

namespace PTGame.Framework
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
    }
}

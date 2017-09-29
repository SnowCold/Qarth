//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG;
using DG.Tweening;

namespace Qarth
{
	
	public class BubbleAnim
	{

		public static Tween PlayPopAction(Transform transform, float duration)
		{
			Vector3 scale = transform.localScale;
			Tween endTween = transform.DOScale(scale, 0.4f * duration);
			scale *= 1.2f;
			Tween beginTween = transform.DOScale(scale, 0.6f * duration);//.SetEase(Ease.OutBounce);
			return DOTween.Sequence().Append(beginTween).Append(endTween);
		}

		class SCValue
		{
			float m_Value;
			public float value
			{
				get { return m_Value; }
				set { m_Value = value; }
			}
			public SCValue(float value)
			{
				m_Value = value;
			}
		}

		class SCScaleWrap
		{
			private Transform 	m_Transfrom;
			private Vector3		m_DefaultScale;
			private SCValue		m_T;
			private SCValue 	m_R;

			public SCScaleWrap(Transform t, SCValue T, SCValue R)
			{
				m_Transfrom = t;
				m_Schedule = 0;
				m_DefaultScale = m_Transfrom.localScale;
				m_R = R;
				m_T = T;
			}

			private float m_Schedule;
			public float schedule
			{
				get { return m_Schedule; }
				set 
				{ 
					m_Schedule = value;
					Vector3 scale = m_DefaultScale;
					float v = Mathf.Sin(m_T.value) * (m_R.value * (1.0f - m_Schedule));
					scale.x += v;
					scale.y -= v;
					m_Transfrom.localScale = scale;
				}
			}
		}

		public static Sequence PlayBubbleAction(Transform transform, float duration, float circle, int loop = -1)
		{
			SCValue T = new SCValue(0);
			SCValue R = new SCValue(0.1f);

			SCScaleWrap wrap = new SCScaleWrap(transform, T, R);

			return DOTween.Sequence().Join(DOTween.To(()=>T.value, x=>T.value = x, Mathf.PI * circle, duration))
					.Join(DOTween.To(()=>wrap.schedule, x=>wrap.schedule = x, 1.0f, duration)).SetLoops(loop);
		}

        public static Sequence PlayBubbleAction2(Transform transform, float duration, float delay, int loop = -1)
        {
            Vector3 scale = transform.localScale;
            var sequence = DOTween.Sequence();
            if (delay > 0)
            {
                sequence.AppendInterval(delay);
            }
            sequence.Append(transform.DOScale(scale * 1.1f, duration).SetEase(Ease.OutElastic, 0.1f, 0.3f))
                .Append(transform.DOScale(scale, duration).SetEase(Ease.OutElastic, 0.1f, 0.3f)).SetLoops(loop).SetEase(Ease.Linear);

            return sequence;
        }

        public static Sequence PlayBreathEffect(Transform transform, float duration, int loop = -1)
        {
            Vector3 scale = transform.localScale;
            Vector3 beginScale = scale * 0.9f;
            transform.localScale = beginScale;
            return DOTween.Sequence()
                .Append(transform.DOScale(scale * 1.1f, duration).SetEase(Ease.InOutSine))
                .AppendInterval(duration * 0.2f)
                .Append(transform.DOScale(beginScale, duration).SetEase(Ease.InOutSine)).SetLoops(loop).SetEase(Ease.Linear);
        }

        //这个模式需要完善 并测试下性能才能用
        private static Dictionary<string, Tween> s_AllTweenMap = new Dictionary<string, Tween>(50);
		public static void playTweenToTarget(Object obj, Tween tween, string name, bool complate)
		{
			string key = string.Format("{0}-{1}", obj.GetInstanceID(), name);

			Tween old;
			if(s_AllTweenMap.TryGetValue(key, out old))
			{
				if(old.IsComplete())
				{
				}
				else
				{
					if(complate)
					{
						old.Complete();
					}
					else
					{
						old.Kill();
					}
				}

				s_AllTweenMap.Remove(key);
			}

			s_AllTweenMap.Add(key, tween);
		}
	}

}

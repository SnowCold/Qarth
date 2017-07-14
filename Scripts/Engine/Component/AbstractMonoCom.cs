//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace Qarth
{
    public class AbstractMonoCom : MonoBehaviour, ICom
    {
        private AbstractActor m_AbstractActor;

        public AbstractActor actor
        {
            get { return m_AbstractActor; }
        }

        public virtual int comOrder
        {
            get { return ComOrderDefine.DEFAULT; }
        }

        public void AwakeCom(AbstractActor actor)
        {
            m_AbstractActor = actor;

            OnActorBind(actor);

            OnComAwake();
        }

        public void OnComDisable()
        {

        }

        public void OnComEnable()
        {

        }

        public void OnComLateUpdate(float dt)
        {
            throw new NotSupportedException();
        }

        public virtual void OnComStart()
        {

        }

        public void OnComUpdate(float dt)
        {
            throw new NotSupportedException();
        }

        public void DestroyCom()
        {
            OnComDestroy();
            m_AbstractActor = null;
            Destroy(this);
        }

#region 子类继承
        protected virtual void OnActorBind(AbstractActor actor)
        {

        }

        protected virtual void OnComAwake()
        {

        }
        protected virtual void OnComDestroy()
        {

        }
#endregion
    }
}

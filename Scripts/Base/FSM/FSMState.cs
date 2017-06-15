//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System.Collections;

namespace Qarth
{
    public class FSMState<T>
    {
        public virtual string stateName
        {
            get { return this.GetType().Name; }
        }

        public virtual void Enter(T entity)
        {

        }

        public virtual void Execute(T entity, float dt)
        {

        }

        public virtual void Exit(T entity)
        {

        }

        public virtual void OnMsg(T entity, int key, params object[] args)
        {

        }
    }

}

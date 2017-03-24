using UnityEngine;
using System.Collections;

namespace PTGame.Framework
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

        public virtual void Execute(T entity)
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

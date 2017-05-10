using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public interface ITransitionAction
    {
        ITransitionHandler transitionHandler
        {
            get;
            set;
        }

        bool transitionSameTime
        {
            get;
        }

        void PrepareTransition();
        void TransitionIn(AbstractPanel panel);
        void TransitionOut(AbstractPanel panel);

        void OnTransitionDestroy();
    }

}

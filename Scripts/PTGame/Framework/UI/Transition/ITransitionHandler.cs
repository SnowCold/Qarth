using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public interface ITransitionHandler
    {
        AbstractPanel transitionPanel
        {
            get;
        }
        void OnTransitionPrepareFiish();
        void OnTransitionInFinish();
        void OnTransitionOutFinish();
    }

}

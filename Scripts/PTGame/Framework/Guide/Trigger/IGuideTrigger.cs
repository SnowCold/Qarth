using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public interface ITrigger
    {
        bool isReady { get; }
        void Start(Action<ITrigger> l);
        void Stop();
    }
}

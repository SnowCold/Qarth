using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Qarth;

namespace Qarth
{
    public class EmptyTrigger : ITrigger
    {
        public bool isReady
        {
            get
            {
                return true;
            }
        }

        public void SetParam(object[] param)
        {

        }

        public void Start(Action<bool, ITrigger> l)
        {
            if (l != null)
            {
                l(true, this);
            }
        }

        public void Stop()
        {

        }
    }
}

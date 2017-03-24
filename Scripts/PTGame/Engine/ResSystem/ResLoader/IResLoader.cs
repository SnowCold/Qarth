using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public interface IResLoader
    {
        void Add2Load(string name, Action<bool, IRes> listener, bool lastOrder = true);
        void ReleaseAllRes();
        void UnloadImage(bool flag);
    }
}

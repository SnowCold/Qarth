using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public interface ISkillReleaser
    {
        void OnSkillRelease(ISkill skill);
        void OnSkillRemove(ISkill skill);
    }
}

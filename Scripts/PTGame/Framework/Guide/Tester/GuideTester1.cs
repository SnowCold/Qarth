using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Framework
{
    public class GuideTester1 : MonoBehaviour
    {
        private Guide m_Guide;
        private void Awake()
        {
            m_Guide = new Guide();

            m_Guide.CreateStep().Append(new HighlightUICommand("HomePanel", "Root/TopUI/MovingAreaRoot/StartButton"));

            m_Guide.Start();
        }
    }
}

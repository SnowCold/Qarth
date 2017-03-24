using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TMonoSingletonAttribute : System.Attribute
    {
        private string m_AbsolutePath;

        public TMonoSingletonAttribute(string relativePath)
        {
            m_AbsolutePath = relativePath;
        }

        public string AbsolutePath
        {
            get { return m_AbsolutePath; }
        }
    }
}

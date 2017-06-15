//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;

namespace Qarth
{
    public interface ITestUnit
    {
        void StartTest();
    }

    public class BaseTestUnit : ITestUnit
    {
        public virtual void StartTest()
        {

        }

        protected void WriteBegin(string title)
        {
            Console.WriteLine(string.Format("*********** {0} Begin **********", title));
        }

        protected void WriteEnd(string title)
        {
            Console.WriteLine(string.Format("*********** {0} End **********", title));
        }

        protected void Write(string msg)
        {
            Console.Write(msg);
        }

        protected void WriteLine(string msg, params object[] args)
        {
            Console.WriteLine(string.Format(msg, args));
        }
    }
}



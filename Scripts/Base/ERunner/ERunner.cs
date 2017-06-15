//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Qarth
{
    public sealed partial class ERunner
    {
        public static bool Run(Run run)
        {
            try
            {
                run();
            }
            catch (Exception ex)
            {
                Log.e(ex.ToString());
                return false;
            }

            return true;
        }

        public static bool Run(out Exception except, Run run)
        {
            try
            {
                run();
            }
            catch (Exception ex)
            {
                except = ex;
                return (false);
            }

            except = null;

            return (true);
        }

        public static bool Run<T>(Run<T> run, T p)
        {
            try
            {
                run(p);
            }
            catch (Exception e)
            {
                Log.e(e);
                return false;
            }

            return true;
        }

        public static bool Run<T1, T2>(Run<T1, T2> run, T1 p1, T2 p2)
        {
            try
            {
                run(p1, p2);
            }
            catch (Exception e)
            {
                Log.e(e);
                return false;
            }

            return true;
        }

        public static bool Run<T1, T2, T3>(Action<T1, T2, T3> run, T1 p1, T2 p2, T3 p3)
        {
            try
            {
                run(p1, p2, p3);
            }
            catch (Exception e)
            {
                Log.e(e);
                return false;
            }

            return true;
        }

        public static Boolean Run<T>(out Exception except, Run<T> run, T p)
        {
            try
            {
                run(p);
            }
            catch (Exception ex)
            {
                except = ex;
                return false;
            }

            except = null;

            return true;
        }

        public static Boolean Run<T1, T2>(out Exception except, Run<T1, T2> run, T1 p1, T2 p2)
        {
            try
            {
                run(p1, p2);
            }
            catch (Exception ex)
            {
                except = ex;
                return false;
            }

            except = null;

            return true;
        }

        public static bool Run<T1, T2, T3>(out Exception except, Action<T1, T2, T3> run, T1 p1, T2 p2, T3 p3)
        {
            try
            {
                run(p1, p2, p3);
            }
            catch (Exception ex)
            {
                except = ex;
                return false;
            }

            except = null;

            return true;
        }

        private ERunner() { }
    }
}

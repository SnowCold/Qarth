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
using System.IO;
using System.Text;

namespace Qarth
{
    public class Helper
    {
        public static Dictionary<string, string> GetUrlParmDict(string parmStr)
        {
            List<string> parmList = Helper.String2ListString(parmStr, "&");
            Dictionary<string, string> kVDict = new Dictionary<string, string>();
            for (int i = 0; i < parmList.Count; ++i)
            {
                List<string> parms = Helper.String2ListString(parmList[i], "=");
                if (parms.Count == 2)
                {
                    kVDict.Add(parms[0], parms[1]);
                }
            }
            return kVDict;
        }

        public static List<int> String2ListInt(string value, string splitter = ";")
        {
            List<int> list = new List<int>();

            if (string.IsNullOrEmpty(value) || splitter == null)
            {
                return list;
            }

            string[] strArray = value.Split(splitter[0]);

            for (int i = 0; i < strArray.Length; i++)
            {
                list.Add(Helper.String2Int(strArray[i]));
            }

            return list;
        }

        public static object[] String2ObjectArray(string value, string splitter = ";")
        {

            if (string.IsNullOrEmpty(value) || splitter == null)
            {
                return new string[0];
            }

            string[] strArray = value.Split(splitter[0]);
            return strArray;
        }

        public static List<float> String2ListFloat(string value, string splitter = ";")
        {
            List<float> list = new List<float>();

            if (string.IsNullOrEmpty(value) || splitter == null)
            {
                return list;
            }

            string[] strArray = value.Split(splitter[0]);

            for (int i = 0; i < strArray.Length; i++)
            {
                list.Add(Helper.String2Float(strArray[i]));
            }

            return list;
        }

        public static List<bool> String2ListBool(string value, string splitter = ";")
        {
            List<bool> list = new List<bool>();
            if (string.IsNullOrEmpty(value) || splitter == null)
            {
                return list;
            }

            string[] strArray = value.Split(splitter[0]);

            for (int i = 0; i < strArray.Length; i++)
            {
                list.Add(Helper.String2Bool(strArray[i]));
            }

            return list;
        }

        public static List<string> String2ListString(string value, string splitter = ";")
        {
            List<string> list = new List<string>();

            if (string.IsNullOrEmpty(value) || splitter == null)
            {
                return list;
            }

            string[] strArray = value.Split(splitter[0]);

            for (int i = 0; i < strArray.Length; i++)
            {
                list.Add(strArray[i]);
            }

            return list;
        }

        public static string StringClone(string value)
        {
            if (null == value || value.Length <= 0)
            {
                return string.Empty;
            }

            return string.Copy(value);
        }

        public static int String2Int(string value)
        {
            int ret = 0;
            int.TryParse(value, out ret);

            return ret;
        }


        public static bool String2Bool(string value)
        {
            bool ret = false;
            if (String2Int(value) > 0)
            {
                return true;
            }
            else
            {
                bool.TryParse(value, out ret);
            }
            return ret;
        }

        public static long String2Int64(string value)
        {
            Int64 ret = 0;
            Int64.TryParse(value, out ret);

            return ret;
        }

        public static float String2Float(string value)
        {
            float ret = 0;
            float.TryParse(value, out ret);

            return ret;
        }

        public static Vector2 String2Vector2(string pos)
        {
            return String2Vector2(pos, ',');
        }

        public static Vector2 String2Vector2(string pos, char split)
        {
            if (string.IsNullOrEmpty(pos))
            {
                return Vector2.zero;
            }

            char[] splits = new char[1] { split };
            string[] str = pos.Split(splits);

            float x = str.Length > 0 ? String2Float(str[0]) : 0f;
            float y = str.Length > 1 ? String2Float(str[1]) : 0f;

            Vector2 ret = new Vector2(x, y);

            return ret;
        }

        public static Vector3 String2Vector3(string pos)
        {
            return String2Vector3(pos, ',');
        }

        public static Vector3 String2Vector3(string pos, char split)
        {
            if (string.IsNullOrEmpty(pos))
            {
                return Vector3.zero;
            }

            char[] splits = new char[1] { split };
            string[] str = pos.Split(splits);

            float x = str.Length > 0 ? String2Float(str[0]) : 0f;
            float y = str.Length > 1 ? String2Float(str[1]) : 0f;
            float z = str.Length > 2 ? String2Float(str[2]) : 0f;

            Vector3 ret = new Vector3(x, y, z);

            return ret;
        }

        public static Color String2Color(string value, char split)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Color.white;
            }

            char[] splits = new char[1] { split };
            string[] str = value.Split(splits);

            float r = str.Length > 0 ? String2Int(str[0]) / 255.0f : 0f;
            float g = str.Length > 1 ? String2Int(str[1]) / 255.0f : 0f;
            float b = str.Length > 2 ? String2Int(str[2]) / 255.0f : 0f;
            float a = str.Length > 3 ? String2Int(str[3]) / 255.0f : 1f;

            Color ret = new Color(r, g, b, a);

            return ret;
        }

        public static Color String2ColorHex(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Color.white;
            }

            int length = value.Length;
            int strCount = length / 2;
            if (length % 2 != 0)
            {
                ++strCount;
            }

            string[] str = new string[strCount];
            for (int i = 0; i < strCount; ++i)
            {
                int leftLength = Mathf.Min(2, length - i * 2);
                str[i] = value.Substring(i * 2, leftLength);
            }

            try
            {
                float r = str.Length > 0 ? Convert.ToInt16(str[0], 16) / 255.0f : 0f;
                float g = str.Length > 1 ? Convert.ToInt16(str[1], 16) / 255.0f : 0f;
                float b = str.Length > 2 ? Convert.ToInt16(str[2], 16) / 255.0f : 0f;
                float a = str.Length > 3 ? Convert.ToInt16(str[3], 16) / 255.0f : 1f;

                Color ret = new Color(r, g, b, a);

                return ret;
            }
            catch (Exception e)
            {
                Log.e(e);
            }

            return Color.white;
        }

        public static string ListInt2String(List<int> list, char split = ';')
        {
            if (null == list || list.Count <= 0)
            {
                return "";
            }

            StringBuilder data = new StringBuilder();

            for (int i = 0; i < list.Count; ++i)
            {
                data.Append(list[i]);

                if (i != list.Count - 1)
                {
                    data.Append(split);
                }
            }

            return data.ToString();
        }

        public static int[] String2IntArray(string data, string split = "-")
        {
            string[] strArray = data.Split(split[0]);

            int[] ret = new int[strArray.Length];

            for (int i = 0; i < strArray.Length; ++i)
            {
                ret[i] = Helper.String2Int(strArray[i]);
            }

            return ret;
        }

        public static string SafeFormat(string format, params object[] args)
        {
            string ret = format;

            try
            {
                ret = string.Format(format, args);
            }
            catch (System.Exception ex)
            {
                Log.e(ex.Message + ex.StackTrace);
            }

            return ret;
        }

        public static string SafeFormat(string format, string paramList)
        {
            List<string> args = String2ListString(paramList);

            return SafeFormat(format, args.ToArray());
        }

        public static bool StringStartWith(string str, string value)
        {
            if (null == str || null == value)
            {
                return false;
            }

            if (str.Length < value.Length)
            {
                return false;
            }

            for (int i = 0; i < value.Length; ++i)
            {
                if (str[i] != value[i])
                {
                    return false;
                }
            }

            return true;
        }

        //大于1万显示k
        public static string FormatAmount(long count)
        {
            if (count == 10000)
            {
                return "10K";
            }
            else if (count > 10000)
            {
                return string.Format("{0}K", count / 1000);
            }
            else
            {
                return count.ToString();
            }
        }

        // 从表格读取的是否有效数据，空或者-1为无效数据
        public static bool IsConfigStringValid(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (value == "-1")
            {
                return false;
            }

            if (value.Equals("null") ||
                value.Equals("Null") ||
                value.Equals("NULL"))
            {
                return false;
            }

            return true;
        }

        public static float Distance(Vector3 pos1, Vector3 pos2)
        {
            return (pos1 - pos2).magnitude;
        }

        public static float Distance2D(Vector3 pos1, Vector3 pos2)
        {
            Vector2 a = new Vector2(pos1.x, pos1.z);
            Vector2 b = new Vector2(pos2.x, pos2.z);

            return (a - b).magnitude;
        }

        // 从pos1到pos2的距离是否大于指定值
        public static bool IsDistanceLonger(Vector3 pos1, Vector3 pos2, float distance)
        {
            Vector3 offset = pos1 - pos2;
            float sqrLen = offset.sqrMagnitude;
            return (sqrLen > distance * distance);
        }

        #region file

        public static byte[] LoadDataFromFile(string Path)
        {
            byte[] bytes;
            try
            {
                FileInfo fileInfo = new FileInfo(Path);
                bytes = new byte[fileInfo.Length];
                FileStream fileStream = fileInfo.OpenRead();
                fileStream.Read(bytes, 0, bytes.Length);
            }
            catch (IOException exception)
            {
                Log.e(exception.Message);
                return null;
            }

            return bytes;
        }

        public static string GetFileNameFromPath(string url)
        {
            string name = url.Replace("/", "\\");
            int lastIndex = name.LastIndexOf("\\");

            if (lastIndex >= 0)
            {
                name = name.Substring(lastIndex + 1);
            }

            lastIndex = name.LastIndexOf(".");

            if (lastIndex >= 0)
            {
                return name.Substring(0, lastIndex);
            }

            return string.Empty;
        }

        public static string GetDirFromPath(string url)
        {
            string name = url.Replace("\\", "/");
            int lastIndex = name.LastIndexOf("/");

            return name.Substring(0, lastIndex);
        }

        #endregion

        public static void SetLocalPosition(Transform tran, float x, float y, float z)
        {
            Vector3 vec3 = Vector3.zero;
            vec3.x = x;
            vec3.y = y;
            vec3.z = z;
            tran.localPosition = vec3;
        }

        public static void SetLocalPosition(Transform tran, Vector3 vec3)
        {
            Vector3 newVec3 = Vector3.zero;
            newVec3.x = vec3.x;
            newVec3.y = vec3.y;
            newVec3.z = vec3.z;
            tran.localPosition = vec3;
        }

        public static void SetParent(GameObject go, GameObject parent)
        {
            if (null == go || null == parent)
            {
                return;
            }

            SetParent(go.transform, parent.transform);
        }

        public static void SetParent(GameObject go, Transform parent)
        {
            if (null == go)
            {
                return;
            }

            SetParent(go.transform, parent);
        }

        public static void SetParent(Transform objTrans, Transform parent, bool resetPosition = true)
        {
            if (null == objTrans)
            {
                return;
            }

            objTrans.SetParent(parent);

            if (resetPosition)
            {
                ResetTransform(objTrans);
            }
        }

        public static void ResetTransform(Transform objTrans)
        {
            objTrans.localPosition = Vector3.zero;
            objTrans.localScale = Vector3.one;
            objTrans.localRotation = Quaternion.identity;
        }

        public static void DestroyChildren(GameObject parent)
        {
            if (null == parent)
            {
                return;
            }
            for (int i = parent.transform.childCount - 1; i >= 0; --i)
            {
                Transform child = parent.transform.GetChild(i);
                GameObject.Destroy(child.gameObject);
            }

        }

        public static bool IsMyChild(Transform parent, Transform child)
        {
            if (parent == null || child == null)
            {
                return false;
            }

            Transform lastParent = child.parent;
            while (lastParent != null)
            {
                if (lastParent.GetInstanceID() == parent.GetInstanceID())
                {
                    return true;
                }
                lastParent = lastParent.parent;
            }
            return false;
        }

        public static Transform FindChildRecursively(Transform parent, string name)
        {
            if (null == parent || null == name)
            {
                return null;
            }

            for (int i = 0; i < parent.childCount; ++i)
            {
                Transform child = parent.GetChild(i);

                if (child.name.Equals(name))
                {
                    return child;
                }

                Transform childChild = FindChildRecursively(child, name);

                if (childChild != null)
                {
                    return childChild;
                }
            }

            return null;
        }

        public static List<Transform> FindChildrenRecursively(Transform parent, string name)
        {
            List<Transform> ret = new List<Transform>();

            if (null == parent || null == name)
            {
                return ret;
            }

            Transform[] childs = parent.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < childs.Length; ++i)
            {
                if (childs[i].name == name)
                {
                    ret.Add(childs[i]);
                }
            }

            return ret;
        }

        public static List<T> FindChildrenRecursively<T>(Transform parent, string name) where T : Component
        {
            List<T> ret = new List<T>();

            if (null == parent || null == name)
            {
                return ret;
            }

            Transform[] childs = parent.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < childs.Length; ++i)
            {
                if (childs[i].name == name)
                {
                    T c = childs[i].GetComponent<T>();

                    if (c != null)
                    {
                        ret.Add(c);
                    }
                }
            }

            return ret;
        }

        public static void PassLayer(GameObject template, GameObject go)
        {
            if (null == template || null == go)
            {
                return;
            }

            SetLayerRecursively(go.transform, template.layer);
        }

        public static void SetLayerRecursively(Transform parent, int layer)
        {
            if (null == parent)
            {
                return;
            }

            parent.gameObject.layer = layer;

            Transform[] childs = parent.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < childs.Length; ++i)
            {
                childs[i].gameObject.layer = layer;
            }
        }

        // 将指定的层级替换成新的层级
        public static void SwitchLayerRecursively(Transform parent, int newLayer, int originalLayer)
        {
            // 无需替换相同层级
            if (newLayer == originalLayer)
            {
                return;
            }

            if (null == parent || parent.gameObject.layer != originalLayer)
            {
                return;
            }

            parent.gameObject.layer = newLayer;

            Transform[] childs = parent.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < childs.Length; ++i)
            {
                if (childs[i].gameObject.layer == originalLayer)
                {
                    childs[i].gameObject.layer = newLayer;
                }
            }
        }

        public static void SetShaderRecursively(Transform parent, Shader s)
        {
            if (null == parent)
            {
                return;
            }

            Renderer renderer = parent.GetComponent<Renderer>();
            ParticleSystem ps = parent.GetComponent<ParticleSystem>();

            if (renderer != null && renderer.material != null && null == ps)
            {
                renderer.material.shader = s;
            }

            Transform[] childs = parent.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < childs.Length; ++i)
            {
                Renderer cr = childs[i].GetComponent<Renderer>();
                ParticleSystem cps = childs[i].GetComponent<ParticleSystem>();


                if (cr != null && cr.material != null && null == cps)
                {
                    cr.material.shader = s;
                }
            }
        }

        public static string BackDir(string dir, int floor = 1)
        {
            string subDir = dir;

            for (int i = 0; i < floor; ++i)
            {
                int last = dir.LastIndexOf('/');
                subDir = dir.Substring(0, last);
            }

            return subDir;
        }

        /*
        public static Vector2 UV2BGPos(Vector2 uv)
        {
            float x = (uv.x - 0.5f) * UIBGAutoFit.defaultClipSize.x;
            float y = (uv.y - 0.5f) * UIBGAutoFit.defaultClipSize.y;

            return new Vector2(x, y);
        }

        public static Vector2 LocalPos2UV(Vector3 pos)
        {
            Vector2 uv;
            uv.x = (pos.x + 0.5f * UIBGAutoFit.defaultClipSize.x) / UIBGAutoFit.defaultClipSize.x;
            uv.y = (pos.y + 0.5f * UIBGAutoFit.defaultClipSize.y) / UIBGAutoFit.defaultClipSize.y;
            return uv;
        }

        public static Vector2 UV2ScreenPos(Vector2 uv)
        {
            float x = (uv.x - 0.5f) * UICameraAutoFit.uiScreenSize.x;
            float y = (uv.y - 0.5f) * UICameraAutoFit.uiScreenSize.y;

            return new Vector2(x, y);
        }

        public static Vector2 Design2BGPos(Vector2 design, Vector3 pos)
        {
            Vector2 uv;
            uv.x = pos.x / design.x;
            uv.y = pos.y / design.y;

            float x = uv.x * UIBGAutoFit.defaultClipSize.x;
            float y = uv.y * UIBGAutoFit.defaultClipSize.y;
            return new Vector2(x, y);
        }
        */

        public static Vector3 IgnoreHeight(Vector3 point)
        {
            return new Vector3(point.x, 0f, point.z);
        }

        // 添加组件，防止重复添加
        public static T AddComponent<T>(GameObject go) where T : Component
        {
            T t = go.GetComponent<T>();

            if (t != null)
            {
                return t;
            }

            return go.AddComponent<T>();
        }

        //查找物体组件
        public static T FindChildComponent<T>(Transform parent, string childName) where T : Component
        {
            if (null == parent || string.IsNullOrEmpty(childName))
            {
                return null;
            }

            Transform child = parent.FindChild(childName);

            if (null == child)
            {
                return null;
            }

            return child.GetComponent<T>();
        }

        public static T[] MergerArray<T>(T[] first, T[] second)
        {
            if (null == first || null == second)
            {
                return new T[0];
            }

            T[] result = new T[first.Length + second.Length];
            first.CopyTo(result, 0);
            second.CopyTo(result, first.Length);

            return result;
        }

        public static Vector3 GetSmoothEulerDest(Vector3 begin, Vector3 dest)
        {
            return begin + EulerFit(dest - begin);
        }

        public static Vector3 EulerFit(Vector3 euler)
        {
            float x = AngleFit(euler.x);
            float y = AngleFit(euler.y);
            float z = AngleFit(euler.z);

            return new Vector3(x, y, z);
        }

        // 让角度在[-180, 180)
        public static float AngleFit(float angle)
        {
            angle %= 360f;

            if (angle < -180f)
            {
                angle += 360;
            }

            if (angle >= 180f)
            {
                angle -= 360f;
            }

            return angle;
        }

        // 让角度在[0, 360)
        public static float AngleFit2(float angle)
        {
            angle %= 360f;

            if (angle < 0)
            {
                angle += 360;
            }

            return angle;
        }

        public static float Angle2Radian(float angle)
        {
            return angle * Mathf.PI / 180f;
        }

        public static float Radian2Angle(float radian)
        {
            return radian * 180f / Mathf.PI;
        }

        public static string[] NewStringList(string[] args, int start)
        {
            if (null == args || start >= args.Length || start < 0)
            {
                return new string[0];
            }

            int count = args.Length - start;

            string[] ret = new string[count];
            Array.Copy(args, start, ret, 0, count);

            return ret;
        }

        public static object[] NewArgList(object[] args, int start)
        {
            if (null == args || start >= args.Length || start < 0)
            {
                return new object[0];
            }

            int count = args.Length - start;

            object[] ret = new object[count];
            Array.Copy(args, start, ret, 0, count);

            return ret;
        }

        public static int QueryIntArgs(object[] args, int index)
        {
            if (null == args || args.Length <= index)
            {
                return 0;
            }

            object obj = args[index];

            if (obj is int)
            {
                return (int)obj;
            }
            else if (obj is string)
            {
                return Helper.String2Int((string)obj);
            }

            return 0;
        }

        public static bool QueryBoolArgs(object[] args, int index)
        {
            if (null == args || args.Length <= index)
            {
                return false;
            }

            object obj = args[index];

            if (obj is bool)
            {
                return (bool)obj;
            }

            return false;
        }

        public static float QueryFloatArgs(object[] args, int index)
        {
            if (null == args || args.Length <= index)
            {
                return 0;
            }

            object obj = args[index];

            if (obj is float)
            {
                return (float)obj;
            }

            return 0;
        }

        // 获取函数的函数名
        public static string GetFuncName(string func)
        {
            if (string.IsNullOrEmpty(func))
            {
                return string.Empty;
            }

            string name = string.Empty;

            for (int i = 0; i < func.Length; ++i)
            {
                char c = func[i];

                if (c == ' ')
                {
                    continue;
                }

                if (c == '(' || c == ')')
                {
                    break;
                }

                name += c;
            }

            return name;
        }

        // 获取函数的参数列表
        public static string[] GetFuncParams(string func)
        {
            if (string.IsNullOrEmpty(func))
            {
                return new string[0];
            }

            string paramsStr = string.Empty;

            bool start = false;

            for (int i = 0; i < func.Length; ++i)
            {
                char c = func[i];

                if (!start)
                {
                    if (c == '(')
                    {
                        start = true;
                    }

                    continue;
                }
                else
                {
                    if (c == '(')
                    {
                        continue;
                    }

                    if (c == ')')
                    {
                        break;
                    }

                    paramsStr += c;
                }
            }

            string[] strList = String2ListString(paramsStr, ",").ToArray();

            char[] trims = new char[] { ' ' };

            for (int i = 0; i < strList.Length; ++i)
            {
                strList[i] = strList[i].Trim(trims);
            }

            return strList;
        }

        public static int GetCharacterCount(string str)
        {
            int count = 0;

            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] >= 0 && str[i] <= 127)
                {
                    ++count;
                }
                else
                {
                    count += 2;
                }
            }

            return count;
        }

        public static void SetCollider(GameObject go, bool hasColider)
        {
            if (null == go)
            {
                return;
            }

            Collider[] colliders = go.GetComponentsInChildren<Collider>();

            for (int i = 0; i < colliders.Length; ++i)
            {
                colliders[i].enabled = hasColider;
            }
        }

        public static string GetClassName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            int index = name.IndexOf("_#");

            if (index < 0)
            {
                return string.Empty;
            }

            int suffixIndex = name.IndexOf('.');

            if (suffixIndex < 0)
            {
                return name.Substring(0, index);
            }
            else
            {
                return name.Substring(0, index) + name.Substring(suffixIndex, name.Length - suffixIndex);
            }
        }

        static string GetStackTraceModelName()
        {
            //当前堆栈信息
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame[] sfs = st.GetFrames();
            //过虑的方法名称,以下方法将不会出现在返回的方法调用列表中
            string _filterdName = "ResponseWrite,ResponseWriteError,";
            string _fullName = string.Empty, _methodName = string.Empty;
            for (int i = 1; i < sfs.Length; ++i)
            {
                //非用户代码,系统方法及后面的都是系统调用，不获取用户代码调用结束
                if (System.Diagnostics.StackFrame.OFFSET_UNKNOWN == sfs[i].GetILOffset()) break;
                _methodName = sfs[i].GetMethod().DeclaringType.Name + "::" + sfs[i].GetMethod().Name;//方法名称
                                                                                                     //sfs[i].GetFileLineNumber();//没有PDB文件的情况下将始终返回0
                if (_filterdName.Contains(_methodName)) continue;
                _fullName = _methodName + "()->" + _fullName;
            }
            st = null;
            sfs = null;
            _filterdName = _methodName = null;
            return _fullName.TrimEnd('-', '>');
        }
    }
}

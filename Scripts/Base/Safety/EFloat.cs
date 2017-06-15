//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
#if UNITY_EDITOR
#define DEBUG_EDATA
#endif
//#define DEBUG_EDATA

public struct EFloat
{
    public static readonly float CRC_FACTOR           = -2.3F;
	public static readonly float CRC_OFFSET           = 1000001.3F;

    private float m_data;
    private int m_crc;

#if DEBUG_EDATA
    //private float m_debugVal;
#endif

    public EFloat(float val)
    {
		m_data = 0f;
		m_crc = (int)(0f * CRC_FACTOR + CRC_OFFSET);
#if DEBUG_EDATA
		//m_debugVal = 0f;
#endif
        SetValue(val);
    }

    public void SetValue(float val)
    {
        m_data = val;
        m_crc = (int)(m_data * CRC_FACTOR + CRC_OFFSET);
#if DEBUG_EDATA
        //m_debugVal = val;
#endif
    }

    public float GetValue()
    {
        // NOTE: 此处过滤了结构没有被调用构造方法而出现的校验计算未执行的情况(此时校验检查不成立)
        // IMPORTANT: 此过滤导致了漏过内存复位为0的情况
        if (m_data == 0 && m_crc == 0)
        {
            return 0f;
        }

        if ((int)(m_data * CRC_FACTOR + CRC_OFFSET) != m_crc)
        {
            //CheckMgr.S.CheaterFound();
            return 0f;
        }

        return (m_data);
    }

	public override string ToString ()
	{
		return this.GetValue().ToString();
	}
	
	public string ToString (string format)
	{
		return this.GetValue().ToString(format);
	}

    public static implicit operator float(EFloat value)
    {
        return value.GetValue();
    }

    public static implicit operator EFloat(int value)
    {
        return (new EFloat((float)value));
    }

    public static implicit operator EFloat(float value)
    {
        return (new EFloat(value));
    }

    public static implicit operator EFloat(EInt value)
    {
        return (new EFloat((float)value.GetValue()));
    }
}

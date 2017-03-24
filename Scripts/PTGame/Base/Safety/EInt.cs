#if UNITY_EDITOR
#define DEBUG_EDATA
#endif
//#define DEBUG_EDATA

public struct EInt
{
    public static readonly int DATA_KEY           = 0x2dF8a235;
    public static readonly int CRC_KEY            = 0x738d40f9;

    private int m_data;
    private int m_crc;

#if DEBUG_EDATA
    //private int m_debugVal;
#endif

    public EInt(int value)
    {
		m_data = 0 ^ DATA_KEY;
		m_crc = m_data ^ CRC_KEY;
#if DEBUG_EDATA
        //m_debugVal = 0;
#endif
        SetValue(value);
    }

    public void SetValue(int val)
    {
        m_data = val ^ DATA_KEY;
        m_crc = m_data ^ CRC_KEY;
#if DEBUG_EDATA
        //m_debugVal = val;
#endif
    }

    public int GetValue()
    {
        // NOTE: 此处过滤了结构没有被调用构造方法而出现的校验计算未执行的情况(此时校验检查不成立)
        // IMPORTANT: 此过滤导致了漏过内存复位为0的情况
        if ((m_data | m_crc) == 0)
        {
            return 0;
        }

        if ((m_data ^ CRC_KEY) != m_crc)
        {
            //CheckMgr.S.CheaterFound();
            return 0;
        }

        return m_data ^ DATA_KEY;
    }

	public override string ToString ()
	{
		return this.GetValue().ToString();
	}

    public static implicit operator int(EInt value)
    {
        return value.GetValue();
    }

    public static implicit operator EInt(int value)
    {
        return (new EInt(value));
    }
}
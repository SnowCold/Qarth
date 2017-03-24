using System.Collections;

namespace PTGame.Framework
{
    public class TSingleton<T> : ISingleton where T : TSingleton<T>, new()
	{
		protected static T		m_Instance;
		
		public static T S
		{
			get
			{
				if(m_Instance == null)
				{
					m_Instance = new T();
                    m_Instance.OnSingletonInit();
				}
				return m_Instance;
			}
		}

        public static T ResetInstance()
        {
            m_Instance = new T();
            m_Instance.OnSingletonInit();
            return m_Instance;
        }

        public virtual void OnSingletonInit()
        {
        }
	}
}
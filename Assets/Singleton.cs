using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
{
    static T m_Instance = null;

    public static T Instance
    {
        get
        {
            if (!m_Instance)
            {
                m_Instance = FindObjectOfType<T>();

                if (!m_Instance)
                {
                    string name = "Generated [" + typeof(T).ToString() + "] Singleton";
                    GameObject go = new GameObject(name);
                    m_Instance = go.AddComponent<T>();
                }
            }

            return m_Instance;
        }
    }
}

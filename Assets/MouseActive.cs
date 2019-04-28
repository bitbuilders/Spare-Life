using UnityEngine;

public class MouseActive : Singleton<MouseActive>
{
    [SerializeField] [Range(0.0f, 5.0f)] float m_MinDistance = 0.1f;
    [SerializeField] [Range(0.0f, 5.0f)] float m_CheckRate = 0.5f;

    public bool Active { get; private set; }
    public bool Moving { get { return (Input.GetButton("Left") || Input.GetButton("Right") || 
                                        Input.GetButton("Up") || Input.GetButton("Down")); } }

    Vector3 m_LastPosition;
    float m_CheckTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            m_CheckTime = 0.0f;
            Active = true;
        }

        m_CheckTime += Time.deltaTime;
        if (m_CheckTime >= m_CheckRate)
        {
            m_CheckTime -= m_CheckRate;

            Vector3 dir = Input.mousePosition - m_LastPosition;
            float sqrDist = dir.sqrMagnitude;
            if (sqrDist >= m_MinDistance * m_MinDistance)
            {
                Active = true;
            }
            else if (!Moving)
            {
                Active = false;
            }

            m_LastPosition = Input.mousePosition;
        }
    }
}

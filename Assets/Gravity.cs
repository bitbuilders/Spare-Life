using UnityEngine;

public class Gravity : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] Rigidbody2D m_Rigidbody = null;
    [Space(10)]
    [Header("Forces")]
    [Tooltip("The amount of downward force applied when traveling up")]
    [SerializeField] [Range(0.0f, 20.0f)] float m_UpForce = 2.0f;
    [Tooltip("The amount of downward force applied when traveling down")]
    [SerializeField] [Range(0.0f, 20.0f)] float m_DownForce = 3.0f;
    [Space(10)]
    [Header("Limits")]
    [Tooltip("The Rigidbody's Y velocity must be greater or less than this value before being affected")]
    [SerializeField] [Range(0.0f, 1.0f)] float m_VelocityThreshold = 0.1f;

    private void FixedUpdate()
    {
        if (m_Rigidbody.velocity.y > m_VelocityThreshold)
            m_Rigidbody.velocity += (Vector2.up * Physics2D.gravity.y) * (m_UpForce - 1.0f) * Time.deltaTime;
        if (m_Rigidbody.velocity.y < -m_VelocityThreshold)
            m_Rigidbody.velocity += (Vector2.up * Physics2D.gravity.y) * (m_DownForce - 1.0f) * Time.deltaTime;
    }
}

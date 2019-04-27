using UnityEngine;

public class X0V : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] Rigidbody2D m_Rigidbody = null;
    [Space(10)]
    [Header("Speed")]
    [SerializeField] [Range(0.0f, 1000.0f)] float m_MaxSpeed = 20.0f;
    [SerializeField] [Range(0.0f, 1000.0f)] float m_Acceleration = 10.0f;
    [Space(10)]
    [Header("Jump")]
    [SerializeField] [Range(0.0f, 100.0f)] float m_JumpForce = 20.0f;
    [SerializeField] [Range(0.0f, 5.0f)] float m_GroundTouchRadius = 0.35f;
    [SerializeField] Transform m_Feet = null;
    [SerializeField] LayerMask m_GroundLayer = 0;

    public bool OnGround { get; private set; }

    Vector2 m_Velocity;
    float m_FlipThreshold = 0.05f;

    void Update()
    {
        float inX = Input.GetAxis("Horizontal");

        if (Mathf.Abs(inX) > m_FlipThreshold)
        {
            Vector3 scale = Vector3.one;
            if (inX < 0.0f) scale.x = -1.0f;
            transform.localScale = scale;
        }
        
        Collider2D collided = Physics2D.OverlapCircle(m_Feet.transform.position, m_GroundTouchRadius, m_GroundLayer);
        OnGround = collided;

        if (OnGround && Input.GetButtonDown("Jump"))
        {
            ResetVelocity(Vector2.right);
            m_Rigidbody.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
        }
    }
    
    void FixedUpdate()
    {
        float inX = Input.GetAxis("Horizontal");

        m_Velocity = Vector2.right * inX * m_MaxSpeed * Time.deltaTime;

        m_Rigidbody.AddForce(m_Velocity, ForceMode2D.Force);
    }

    void ResetVelocity(Vector2 axisToKeep)
    {
        float x = m_Rigidbody.velocity.x * axisToKeep.x;
        float y = m_Rigidbody.velocity.y * axisToKeep.y;
        m_Rigidbody.velocity = new Vector2(x, y);
    }
}

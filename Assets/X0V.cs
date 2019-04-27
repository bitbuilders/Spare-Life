using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FlipTrigger : UnityEvent { }

public class X0V : Gunner
{
    [Header("Variables")]
    [SerializeField] Rigidbody2D m_Rigidbody = null;
    [SerializeField] [Range(0.0f, 1.0f)] float m_DeadZone = 0.1f;
    [SerializeField] [Range(0.0f, 100.0f)] float m_MinVelocity = 5.0f;
    [Space(10)]
    [Header("Speed")]
    [SerializeField] [Range(0.0f, 1000.0f)] float m_MaxSpeed = 8.0f;
    [SerializeField] [Range(0.0f, 1000.0f)] float m_Acceleration = 700.0f;
    [SerializeField] [Range(0.0f, 1000.0f)] float m_MaxAcceleration = 50.0f;
    [SerializeField] [Range(0.0f, 1000.0f)] float m_Deceleration = 870.0f;
    [Space(10)]
    [Header("Jump")]
    [SerializeField] [Range(0.0f, 100.0f)] float m_JumpForce = 20.0f;
    [SerializeField] [Range(0.0f, 5.0f)] float m_GroundTouchRadius = 0.35f;
    [SerializeField] Transform m_Feet = null;
    [SerializeField] LayerMask m_GroundLayer = 0;
    [Space(10)]
    [Header("Dash")]
    [SerializeField] [Range(0.0f, 100.0f)] float m_DashStrength = 20.0f;
    [SerializeField] [Range(0.0f, 5.0f)] float m_DashDuration = 0.7f;
    [Space(10)]
    [Header("Flip")]
    [SerializeField] FlipTrigger m_FlipTrigger;

    public bool OnGround { get; private set; }
    public bool Dashing { get { return m_DashTime < m_DashDuration; } }

    Vector2 m_Velocity;
    float m_FlipThreshold = 0.05f;
    float m_DashTime;

    private void Start()
    {
        m_DashTime = m_DashDuration;
    }

    void Update()
    {
        float inX = Input.GetAxis("Horizontal");

        if (Mathf.Abs(inX) > m_FlipThreshold)
        {
            Vector3 prevScale = transform.localScale;

            Vector3 scale = Vector3.one;
            if (inX < 0.0f) scale.x = -1.0f;
            transform.localScale = scale;

            if (prevScale != transform.localScale) m_FlipTrigger.Invoke();
        }

        Collider2D collided = Physics2D.OverlapCircle(m_Feet.transform.position, m_GroundTouchRadius, m_GroundLayer);
        OnGround = collided;

        if (OnGround && Input.GetButtonDown("Jump"))
        {
            ResetVelocity(Vector2.right);
            m_Rigidbody.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
        }

        // Will override jump if used
        m_DashTime += Time.deltaTime;
        if (Input.GetButtonDown("Dash"))
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            Vector2 dir = new Vector2(x, y).normalized;
            ResetVelocity(Vector2.zero);
            m_Rigidbody.AddForce(dir * m_DashStrength, ForceMode2D.Impulse);
            m_DashTime = 0.0f;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            m_Gun.Fire();
        }
    }
    
    void FixedUpdate()
    {
        float inX = Input.GetAxis("Horizontal");

        float speed = inX * m_Acceleration * Time.deltaTime;
        float xSpeed = Mathf.Abs(m_Velocity.x);

        m_Velocity.x += speed;
        m_Velocity.x = Mathf.Clamp(m_Velocity.x, -m_MaxAcceleration, m_MaxAcceleration);

        if (Mathf.Abs(inX) < m_DeadZone && xSpeed > m_MinVelocity)
        {
            m_Velocity.x += -Mathf.Sign(m_Velocity.x) * m_Deceleration * Time.deltaTime;
        }
        else if (Mathf.Abs(inX) < m_DeadZone && xSpeed < m_MinVelocity)
        {
            m_Velocity.x = 0.0f;
        }

        m_Rigidbody.AddForce(m_Velocity, ForceMode2D.Force);
        if (!Dashing && Mathf.Abs(m_Rigidbody.velocity.x) > m_MaxSpeed)
        {
            m_Rigidbody.velocity = new Vector2(Mathf.Sign(m_Rigidbody.velocity.x) * m_MaxSpeed, m_Rigidbody.velocity.y);
        }
    }

    void ResetVelocity(Vector2 axisToKeep)
    {
        float x = m_Rigidbody.velocity.x * axisToKeep.x;
        float y = m_Rigidbody.velocity.y * axisToKeep.y;
        m_Rigidbody.velocity = new Vector2(x, y);
    }
}

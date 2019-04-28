using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FlipTrigger : UnityEvent { }

public class X0V : Gunner
{
    [Space(10)]
    [Header("Variables")]
    [SerializeField] Rigidbody2D m_Rigidbody = null;
    [SerializeField] PivotAim m_Shoulder = null;
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
    [SerializeField] FlipTrigger m_FlipTrigger = null;
    
    public bool Dashing { get { return m_DashTime < m_DashDuration; } }

    List<Debuff> m_Debuffs;
    Vector2 m_Velocity;
    float m_FlipThreshold = 0.05f;
    float m_DashTime;
    float m_StartingSpeed;
    float m_StartingJump;
    float m_StartingDash;
    float m_FireTime;

    private void Start()
    {
        m_Debuffs = new List<Debuff>();
        m_DashTime = m_DashDuration;
        m_StartingSpeed = m_MaxSpeed;
        m_StartingJump = m_JumpForce;
        m_StartingDash = m_DashStrength;
    }

    new void Update()
    {
        base.Update();

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

        m_FireTime += Time.deltaTime;
        m_FireTime = Mathf.Clamp(m_FireTime, 0.0f, m_Gun.FireRate);
        if (m_FireTime >= m_Gun.FireRate && Input.GetButton("Fire1"))
        {
            m_FireTime -= m_Gun.FireRate;
            m_Gun.Fire();
            m_Shoulder.Kick(m_Gun.Kick, m_Gun.Recoil);
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

    public void AddDebuff(Debuff debuff)
    {
        m_Debuffs.Add(debuff);

        switch (debuff.Type)
        {
            case Modifier.ANGLE:
                m_Shoulder.SetAngles(0, 0);
                break;
            case Modifier.JUMP:
                m_JumpForce *= debuff.Value;
                break;
            case Modifier.SPEED:
                m_MaxSpeed *= debuff.Value;
                break;
            case Modifier.DASH:
                m_DashStrength *= debuff.Value;
                break;
        }

        // TODO: Animation/Sound effect

    }

    public void ClearDebuffs()
    {
        foreach (Debuff debuff in m_Debuffs)
        {
            switch (debuff.Type)
            {
                case Modifier.ANGLE:
                    m_Shoulder.RevertAngles();
                    break;
                case Modifier.JUMP:
                    m_JumpForce = m_StartingJump;
                    break;
                case Modifier.SPEED:
                    m_MaxSpeed = m_StartingSpeed;
                    break;
                case Modifier.DASH:
                    m_DashStrength = m_StartingDash;
                    break;
            }
        }

        m_Debuffs.Clear();
    }

    public void RemoveDebuff(Modifier modifier)
    {
        foreach (Debuff debuff in m_Debuffs)
        {
            if (debuff.Type == modifier)
            {
                m_Debuffs.Remove(debuff);
                break;
            }
        }
    }
}

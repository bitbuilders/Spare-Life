using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Gunner
{
    [Space(10)]
    [Header("Variables")]
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
    [Header("Flip")]
    [SerializeField] FlipTrigger m_FlipTrigger = null;
    [Space(10)]
    [Header("AI")]
    [SerializeField] [Range(0.0f, 100.0f)] float m_MaxDistance = 5.0f;
    [SerializeField] [Range(0.0f, 100.0f)] float m_VisionRadius = 5.0f;
    [SerializeField] [Range(0.0f, 100.0f)] float m_PlayerDistance = 5.0f;
    [SerializeField] [Range(0.0f, 100.0f)] float m_PanicDistance = 5.0f;

    Gunner m_ABSOLUTE_THREAT = null;
    Vector2 m_Velocity = Vector2.zero;
    Vector2 m_Dir = Vector2.zero;
    float m_FlipThreshold = 0.05f;
    float m_ShootTime = 0.0f;
    bool m_Moving = false;
    bool m_CanSee = false;

    new void Update()
    {
        if (Dying) return;

        base.Update();

        if (Mathf.Abs(m_Dir.x) > m_FlipThreshold)
        {
            Vector3 prevScale = transform.localScale;

            Vector3 scale = Vector3.one;
            if (m_Dir.x < 0.0f) scale.x = -1.0f;
            transform.localScale = scale;

            if (prevScale != transform.localScale) m_FlipTrigger.Invoke();
        }

        Collider2D collided = Physics2D.OverlapCircle(m_Feet.transform.position, m_GroundTouchRadius, m_GroundLayer);
        OnGround = collided;

        // Jump

        // Shoot
        if (!m_Comrade) return;

        m_ShootTime += Time.deltaTime;
        m_ShootTime = Mathf.Clamp(m_ShootTime, 0.0f, m_Gun.FireRate);

        m_Moving = false;
        m_Dir = Vector2.zero;

        float sign = Mathf.Sign(transform.lossyScale.x);
        m_Shoulder.Target = (Vector2)transform.position + Vector2.right * 2.0f * sign;

        Vector2 comDir = m_Comrade.transform.position - transform.position;
        float comDist = comDir.sqrMagnitude;
        if (comDist > m_PlayerDistance * m_PlayerDistance)
        {
            m_Moving = true;

            m_Dir = comDir.normalized;
            
            if (WallInDirection(m_Dir, 2.0f) || comDist > m_PanicDistance * m_PanicDistance)
            {
                if (WallInDirection(Vector2.up, 3.5f))
                {
                    m_Dir = Vector2.left;
                }
                else
                {
                    if (OnGround)
                    {
                        ResetVelocity(Vector2.right);
                        Jump();
                    }
                }
            }

            return;
        }

        m_ABSOLUTE_THREAT = GetClosestFoe();
        if (m_ABSOLUTE_THREAT)
        {
            Vector2 dir = m_ABSOLUTE_THREAT.transform.position - transform.position;
            float sqrDist = dir.sqrMagnitude;
            if (sqrDist <= m_VisionRadius * m_VisionRadius)
            {
                m_CanSee = CanSeeThreat();
                if (m_CanSee)
                {
                    m_Dir = dir.normalized;

                    if (sqrDist > m_MaxDistance * m_MaxDistance) m_Moving = true;

                    m_Shoulder.Target = m_ABSOLUTE_THREAT.transform.position;

                    if (m_ShootTime >= m_Gun.FireRate)
                    {
                        m_ShootTime -= m_Gun.FireRate;
                        m_Gun.Fire();
                    }
                }
            }
        }
        else
        {
            m_Shoulder.Target = transform.position + transform.right * 2.0f;
        }
    }

    private void FixedUpdate()
    {
        if (Dying) return;

        float xSpeed = 0.0f;

        if (m_Moving)
        {
            float speed = m_Dir.x * m_Acceleration * Time.deltaTime;
            xSpeed = Mathf.Abs(m_Velocity.x);

            m_Velocity.x += speed;
            m_Velocity.x = Mathf.Clamp(m_Velocity.x, -m_MaxAcceleration, m_MaxAcceleration);
        }

        if (Mathf.Abs(m_Dir.x) > m_MinVelocity)
        {
            m_Velocity.x += -Mathf.Sign(m_Velocity.x) * m_Deceleration * Time.deltaTime;
        }
        else if (Mathf.Abs(m_Dir.x) < 0.1f && xSpeed < m_MinVelocity)
        {
            m_Velocity.x = 0.0f;
        }

        m_Rigidbody.AddForce(m_Velocity, ForceMode2D.Force);
        if (Mathf.Abs(m_Rigidbody.velocity.x) > m_MaxSpeed)
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

    Gunner GetClosestFoe()
    {
        Gunner closest = null;

        float dist = float.MaxValue;
        foreach (Gunner gunner in GunnerLodge.Instance.Gunners)
        {
            if (!gunner.Friendly && !gunner.Dying)
            {
                Vector2 dir = gunner.transform.position - transform.position;
                float sqrDist = dir.sqrMagnitude;
                if (sqrDist < dist)
                {
                    dist = sqrDist;
                    closest = gunner;
                }
            }
        }

        return closest;
    }

    bool CanSeeThreat()
    {
        bool canSee = false;
        Vector2 dir = m_ABSOLUTE_THREAT.transform.position - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, m_VisionRadius, m_GroundLayer);
        if (!hit.collider)
        {
            // Hit nothing!
            canSee = true;
        }
        else
        {
            if (hit.collider.gameObject == m_ABSOLUTE_THREAT.gameObject)
            {
                canSee = true;
            }
        }

        return canSee;
    }

    bool CanSeeComrade()
    {
        bool canSee = false;
        Vector2 dir = m_Comrade.transform.position - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * 0.25f, dir, dir.magnitude, m_GroundLayer);
        if (!hit.collider)
        {
            // Hit nothing!
            canSee = true;
        }
        else
        {
            if (hit.collider.gameObject == m_Comrade.gameObject)
            {
                canSee = true;
            }
        }

        return canSee;
    }

    bool WallInDirection(Vector2 dir, float dist)
    {
        bool wallThere = false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, m_GroundLayer);
        if (hit.collider)
        {
            // Hit nothing!
            wallThere = true;
        }

        return wallThere;
    }

    void Jump()
    {
        m_Rigidbody.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
    }
}

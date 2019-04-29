using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Generic Bullet")]
    [SerializeField] protected Rigidbody2D m_Rigidbody = null;
    [SerializeField] [Range(0.0f, 30.0f)] protected float m_Lifetime = 4.0f;
    [SerializeField] [Range(0.0f, 1000.0f)] protected float m_Damage = 20.0f;
    [SerializeField] [Range(0.0f, 100.0f)] protected float m_Knockback = 5.0f;
    [SerializeField] [Range(0.0f, 10.0f)] protected float m_BuildUpTime = 0.0f;
    [SerializeField] protected AnimationCurve m_BuildUpCurve = null;
    [SerializeField] [Range(0.0f, 5.0f)] protected float m_GravityScale = 0.0f;

    public Rigidbody2D RigidBody { get { return m_Rigidbody; } }

    public float Strength
    {
        get { return m_Strength; }
        set
        {
            m_Strength = value;
            m_Rigidbody.velocity = m_Dir * m_Strength;
        }
    }
    public bool Friendly { get { return m_Friendly; } }

    protected bool m_Friendly;
    
    Vector2 m_Dir;
    float m_Strength;

    virtual public void Init(Vector2 dir, float strength, bool friendly)
    {
        m_Dir = dir;
        Strength = strength;
        m_Rigidbody.gravityScale = m_GravityScale;

        float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        m_Friendly = friendly;

        Invoke("Kill", m_Lifetime);
    }

    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        Gunner g = collision.GetComponent<Gunner>();
        if (g)
        {
            if (g.Friendly != m_Friendly)
            {
                Vector2 dir = g.transform.position - transform.position;
                g.Damage(m_Damage, dir.normalized * m_Knockback);
                Kill();
            }
        }
        else if (!collision.GetComponent<Bullet>())
        {
            Kill();
        }
    }

    virtual protected void Kill()
    {
        CancelInvoke();

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}

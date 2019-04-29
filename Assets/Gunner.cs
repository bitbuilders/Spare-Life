using System.Collections;
using UnityEngine;

public abstract class Gunner : MonoBehaviour
{
    [Header("Gunner")]
    [SerializeField] public Rigidbody2D m_Rigidbody;
    [SerializeField] AudioSource m_AudioSource = null;
    [SerializeField] AudioClip m_TradeClip = null;
    [SerializeField] AudioClip m_SacrificeClip = null;
    [SerializeField] AudioClip m_HitClip = null;
    [SerializeField] protected X0V m_Comrade = null;
    [SerializeField] protected Gun m_Gun = null;
    [SerializeField] GameObject m_SacrificeIcons = null;
    [SerializeField] GameObject m_TradeIcons = null;
    [SerializeField] protected PivotAim m_Shoulder = null;
    [SerializeField] protected SpriteRenderer m_Arm = null;
    [SerializeField] protected Transform m_HoldPoint = null;
    [SerializeField] protected bool m_Trader = true; // 2 traders can't swap
    [SerializeField] protected bool m_Evil = false;
    [SerializeField] [Range(0, 300)] int m_GunOrderInLayer = 0;
    [SerializeField] [Range(0.0f, 10.0f)] float m_TradeDistance = 3.0f;
    [SerializeField] [Range(0.0f, 1000.0f)] protected float m_Health = 100.0f;
    [SerializeField] [Range(0.0f, 10.0f)] protected float m_DeathTime = 1.5f;
    [SerializeField] Debuff m_Debuff = null;
    [Space(10)]
    [Header("Animation")]
    [SerializeField] [Range(0.0f, 10.0f)] float m_BlinkTime = 0.5f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_BlinkSpeed = 1.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_MinBlink = 1.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_MaxBlink = 1.5f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_TradeOverride = 0.7f;

    public bool OnGround { get; protected set; }
    public bool Dead { get { return m_Health <= 0.0f; } }
    public bool Friendly { get { return !m_Evil; } }
    public bool Dying { get; protected set; }

    protected float m_MaxHealth;
    protected Vector2 m_DeadPoint;

    float m_ConsistentScale;
    float m_ScaleOverride;
    float m_TotalScale;
    float m_Time;

    protected void Start()
    {
        if (m_SacrificeIcons) m_SacrificeIcons.SetActive(true);
        if (m_TradeIcons) m_TradeIcons.SetActive(false);
        m_MaxHealth = m_Health;

        m_Shoulder.Target = transform.position + transform.right * 2.0f;

        SetGunLayer();
    }

    protected void Update()
    {
        if (!m_Trader) return;

        float s = Mathf.PingPong(Time.time * m_BlinkSpeed, 1.0f);
        m_ConsistentScale = (s * (m_MaxBlink - m_MinBlink)) + m_MinBlink;

        if (!m_Comrade)
        {
            X0V x0v = GunnerLodge.Instance.X0V;
            Vector2 dir = x0v.transform.position - transform.position;
            float sqrDist = dir.sqrMagnitude;

            if (sqrDist <= m_TradeDistance * m_TradeDistance)
            {
                CalculateTime(true, m_SacrificeIcons);

                if (Input.GetButtonDown("Sacrifice"))
                {
                    m_Comrade = x0v;
                    m_Comrade.AddDebuff(m_Debuff); // Sorry bro
                    HudManager.Instance.CreateDebuffIcon(m_Debuff.Type.ToString());
                    m_TradeIcons.SetActive(true);
                    m_SacrificeIcons.SetActive(false);

                    PlaySound(m_SacrificeClip, 1.5f);
                }
            }
            else
            {
                CalculateTime(false, m_SacrificeIcons);
            }
        }
        else
        {
            Vector2 dir = m_Comrade.transform.position - transform.position;
            float sqrDist = dir.sqrMagnitude;

            if (sqrDist <= m_TradeDistance * m_TradeDistance)
            {
                if (Input.GetButtonDown("Trade"))
                {
                    Trade(m_Comrade);

                    PlaySound(m_TradeClip, 1.5f, .8f);
                }

                CalculateTime(true, m_TradeIcons, m_TradeOverride);
            }
            else
            {
                CalculateTime(false, m_TradeIcons, m_TradeOverride);
            }
        }
    }

    void Trade(Gunner g)
    {
        m_Gun = g.SwapGun(m_Gun);
        SetGunPoint();
    }

    Gun SwapGun(Gun gun)
    {
        Gun oldGun = m_Gun;
        m_Gun = gun;

        SetGunPoint();

        return oldGun;
    }
    
    void CalculateTime(bool increment, GameObject icons, float scaleOverride = 1.0f)
    {
        m_Time += increment ? Time.deltaTime : -Time.deltaTime;

        CalculateOverride();
        icons.transform.localScale = Vector3.one * m_TotalScale * scaleOverride;
    }

    void CalculateOverride()
    {
        m_Time = Mathf.Clamp(m_Time, 0.0f, m_BlinkTime);
        float t = m_Time / m_BlinkTime;

        m_ScaleOverride = t;
        m_TotalScale = m_ConsistentScale * m_ScaleOverride;
    }

    void SetGunPoint()
    {
        m_Gun.transform.parent = m_HoldPoint;
        m_Gun.transform.localPosition = Vector3.zero;
        m_Gun.transform.localRotation = Quaternion.identity;
        m_Gun.transform.localScale = Vector3.one;

        SetGunLayer();
    }

    void SetGunLayer()
    {
        m_Gun.GetComponentInChildren<SpriteRenderer>().sortingOrder = m_GunOrderInLayer;
    }

    public void Damage(float destruction, Vector3 force)
    {
        if (Dead) return;

        m_Health -= destruction;
        m_Rigidbody.velocity = Vector2.zero;
        m_Rigidbody.AddForce(force, ForceMode2D.Impulse);
        PlaySound(m_HitClip, 0.7f);

        if (Dead)
        {
            m_DeadPoint = transform.position;
            GetComponent<CapsuleCollider2D>().enabled = false;
            Invoke("Die", m_DeathTime);
            Dying = true;
            StartCoroutine(DieDieDie(force));
        }
    }

    virtual public void Die()
    {
        gameObject.SetActive(false);
        m_Health = m_MaxHealth;
        GunnerLodge.Instance.Gunners.Remove(this);
    }

    public void Push(Vector3 force)
    {
        m_Rigidbody.AddForce(force, ForceMode2D.Force);
    }

    IEnumerator DieDieDie(Vector2 force)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float sign = Mathf.Sign(force.x);
        Vector2 newForce = sign * new Vector2(20.0f, 0.0f);
        Vector2 rotatedForce = Quaternion.Euler(Vector3.forward * 45.0f) * newForce;
        rotatedForce.y *= sign;
        m_Rigidbody.AddForce(rotatedForce, ForceMode2D.Impulse);

        for (float i = 0.0f; i < m_DeathTime; i += Time.deltaTime)
        {
            float t = i / m_DeathTime;
            Color c = spriteRenderer.color;
            c.a = 1.0f - t;
            spriteRenderer.color = c;

            m_Arm.color = c;

            float y = Mathf.Lerp(0.0f, sign * 90.0f, t);
            transform.rotation = Quaternion.Euler(Vector3.forward * y);

            yield return null;
        }
    }

    void PlaySound(AudioClip sound, float volume, float pitch = 1.0f)
    {
        m_AudioSource.pitch = pitch;
        m_AudioSource.volume = volume;
        m_AudioSource.clip = sound;
        m_AudioSource.Play();
    }
}

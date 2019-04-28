using System.Collections;
using UnityEngine;

public abstract class Gunner : MonoBehaviour
{
    [Header("Gunner")]
    [SerializeField] protected X0V m_Comrade = null;
    [SerializeField] protected Gun m_Gun = null;
    [SerializeField] GameObject m_SacrificeIcons = null;
    [SerializeField] GameObject m_TradeIcons = null;
    [SerializeField] protected Transform m_HoldPoint = null;
    [SerializeField] protected bool m_Trader = true; // 2 traders can't swap
    [SerializeField] [Range(0.0f, 10.0f)] float m_TradeDistance = 3.0f;
    [SerializeField] Debuff m_Debuff;
    [SerializeField] [Range(0.0f, 10.0f)] float m_BlinkTime = 0.5f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_BlinkSpeed = 1.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_MinBlink = 1.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_MaxBlink = 1.5f;

    public bool OnGround { get; protected set; }

    float m_ConsistentScale;
    float m_ScaleOverride;
    float m_TotalScale;
    float m_Time;

    private void Start()
    {
        m_SacrificeIcons.SetActive(true);
        m_TradeIcons.SetActive(false);
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
                }

                CalculateTime(true, m_TradeIcons);
            }
            else
            {
                CalculateTime(false, m_TradeIcons);
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
    
    void CalculateTime(bool increment, GameObject icons)
    {
        m_Time += increment ? Time.deltaTime : -Time.deltaTime;

        CalculateOverride();
        icons.transform.localScale = Vector3.one * m_TotalScale;
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
    }
}

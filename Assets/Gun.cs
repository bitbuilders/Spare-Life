using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Generic Gun")]
    [SerializeField] Sprite m_BulletSprite = null;
    [SerializeField] GameObject m_BulletTemplate = null;
    [SerializeField] Transform m_Barrel = null;
    [SerializeField] ParticleSystem m_FireEffect = null;
    [SerializeField] bool m_Evil = false;
    [SerializeField] [Range(0, 100)] int m_PoolSize = 20;
    [SerializeField] [Range(0.0f, 100.0f)] float m_LaunchSpeed = 20.0f;
    [SerializeField] [Range(0.0f, 5.0f)] float m_FireRate = 0.5f;
    [Tooltip("How far the gun kicks up")]
    [SerializeField] [Range(0.0f, 100.0f)] float m_Kick = 10.0f;
    [Tooltip("The time it takes to go back to rest")]
    [SerializeField] [Range(0.0f, 10.0f)] float m_Recoil = 0.1f;

    public float Kick { get { return m_Kick; } }
    public float Recoil { get { return m_Recoil; } }
    public float FireRate { get { return m_FireRate; } }
    public bool Friendly { get { return !m_Evil; } }

    GameObject[] m_BulletPool;

    private void Start()
    {
        m_BulletPool = new GameObject[m_PoolSize];
        for (int i = 0; i < m_BulletPool.Length; i++)
        {
            m_BulletPool[i] = Instantiate(m_BulletTemplate, Vector3.zero, Quaternion.identity, BulletPool.Instance.transform);
            m_BulletPool[i].GetComponentInChildren<SpriteRenderer>().sprite = m_BulletSprite;
            m_BulletPool[i].SetActive(false);
        }
    }

    virtual public void Fire()
    {
        GameObject bullet = Get();
        if (bullet)
        {
            Bullet b = bullet.GetComponent<Bullet>();
            Vector3 dir = transform.right * Mathf.Sign(transform.lossyScale.x);
            b.transform.position = m_Barrel.position;
            b.Init(dir, m_LaunchSpeed, Friendly);

            if (m_FireEffect) m_FireEffect.Play(true);
        }
    }

    protected GameObject Get()
    {
        GameObject bullet = null;
        for (int i = 0; i < m_BulletPool.Length; i++)
        {
            if (!m_BulletPool[i].activeSelf)
            {
                bullet = m_BulletPool[i];
                m_BulletPool[i].SetActive(true);
                break;
            }
        }

        return bullet;
    }
}

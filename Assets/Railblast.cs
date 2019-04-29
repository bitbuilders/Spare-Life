using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Railblast : Bullet
{
    [Space(10)]
    [Header("Rail Blast")]
    [SerializeField] [Range(0.0f, 100.0f)] float m_MaxRange = 20.0f;
    [SerializeField] AnimationCurve m_AlphaOverLife = null;
    [SerializeField] AnimationCurve m_ScrollOverLife = null;
    [SerializeField] [Range(-10.0f, 10.0f)] float m_MaxScrollSpeed = -0.1f;
    [SerializeField] ContactFilter2D m_ContactFilter = default;
    [SerializeField] LayerMask m_PlatformLayer = 0;
    [SerializeField] LayerMask m_GunnerLayer = 0;

    SpriteRenderer m_SpriteRenderer;
    Material m_SpriteMaterial;

    private void Awake()
    {
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public override void Init(Vector2 dir, float strength, bool friendly)
    {
        CancelInvoke();
        base.Init(dir, strength, friendly);

        float pxWidth = m_SpriteRenderer.sprite.rect.size.x;
        float pxScale = pxWidth / m_SpriteRenderer.sprite.pixelsPerUnit;
        float scale = GetRange(dir) / pxScale;
        transform.localScale = new Vector3(scale, 1.0f, 1.0f);

        m_SpriteMaterial = m_SpriteRenderer.material;

        StartCoroutine(Scroll());
    }

    float GetRange(Vector2 dir)
    {
        float range = m_MaxRange;
        List<RaycastHit2D> results = new List<RaycastHit2D>();
        int numHit = Physics2D.Raycast(transform.position, dir, m_ContactFilter, results);
        if (numHit > 0)
        {
            foreach (RaycastHit2D hit in results)
            {
                int hitLayer = hit.transform.gameObject.layer;
                int hitPower = (int)Mathf.Pow(2, hitLayer);
                if (hitPower == m_PlatformLayer)
                {
                    range = hit.distance;
                    break;
                }
                else if ((hitPower & (int)m_GunnerLayer) == (int)hitPower)
                {
                    Gunner g = hit.transform.GetComponent<Gunner>();
                    if (g && g.Friendly == m_Friendly) continue;

                    g.Damage(m_Damage, dir.normalized * m_Knockback);
                }
            }
        }

        return range;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {

    }

    protected override void Kill()
    {
        Color c = m_SpriteMaterial.color;
        c.a = 1.0f;
        m_SpriteMaterial.color = c;

        base.Kill(); // Do last
    }

    IEnumerator Scroll()
    {
        float xPos = 0.0f;
        for (float i = 0.0f; i < m_Lifetime; i += Time.deltaTime)
        {
            float t = i / m_Lifetime;
            float s = m_ScrollOverLife.Evaluate(t) * m_MaxScrollSpeed;
            xPos += s * Time.deltaTime;
            m_SpriteMaterial.mainTextureOffset = new Vector2(xPos, 0);

            Color c = m_SpriteMaterial.color;
            c.a = m_AlphaOverLife.Evaluate(t);
            m_SpriteMaterial.color = c;
            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Bullet
{
    [Space(10)]
    [Header("Grenade")]
    [SerializeField] [Range(0.0f, 10.0f)] float m_ExplosionRadius = 2.25f;
    [SerializeField] AnimationCurve m_ScaleShift = null;
    [SerializeField] AnimationCurve m_ColorShift = null;
    [SerializeField] Gradient m_ColorOverTime = null;
    [SerializeField] GameObject m_Explosion = null;

    SpriteRenderer m_SpriteRenderer;
    Color m_Start;

    private void Awake()
    {
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public override void Init(Vector2 dir, float strength, bool friendly)
    {
        CancelInvoke();
        base.Init(dir, strength, friendly);

        m_Start = m_SpriteRenderer.color;
        StartCoroutine(ColorShift());
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {

    }

    protected override void Kill()
    {
        m_SpriteRenderer.color = m_Start;

        // EXPLODE!!!
        Instantiate(m_Explosion, transform.position, Quaternion.identity, BulletPool.Instance.transform);

        foreach (Gunner g in GunnerLodge.Instance.Gunners)
        {
            if (g.Friendly == m_Friendly) continue;

            Vector2 dir = g.transform.position - transform.position;
            float sqrDist = dir.sqrMagnitude;
            if (sqrDist <= m_ExplosionRadius * m_ExplosionRadius)
            {
                g.Damage(m_Damage, dir.normalized * m_Knockback);
            }
        }

        base.Kill(); // Do last
    }

    IEnumerator ColorShift()
    {
        for (float i = 0.0f; i < m_Lifetime; i += Time.deltaTime)
        {
            float t = i / m_Lifetime;
            Color c = m_ColorOverTime.Evaluate(m_ColorShift.Evaluate(t));
            m_SpriteRenderer.color = c;

            float s = m_ScaleShift.Evaluate(t);
            m_SpriteRenderer.transform.localScale = Vector3.one * s;
            yield return null;
        }

        m_SpriteRenderer.color = m_ColorOverTime.Evaluate(1.0f);
        m_SpriteRenderer.transform.localScale = Vector3.one * m_ScaleShift.Evaluate(1.0f);
    }
}

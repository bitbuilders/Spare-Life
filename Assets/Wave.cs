using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : Bullet
{
    [Space(10)]
    [Header("Wave")]
    [SerializeField] [Range(0.0f, 20.0f)] float m_MaxWidth = 4.0f;
    [SerializeField] AnimationCurve m_ScaleCurve = null;
    [SerializeField] [Range(0.0f, 5.0f)] float m_DeathDelay = 0.75f;

    SpriteRenderer m_SpriteRenderer;
    float m_PixelScale;

    private void Awake()
    {
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public override void Init(Vector2 dir, float strength, bool friendly)
    {
        CancelInvoke();
        base.Init(dir, strength, friendly);

        float pxWidth = m_SpriteRenderer.sprite.rect.size.x;
        m_PixelScale = pxWidth / m_SpriteRenderer.sprite.pixelsPerUnit;

        transform.localScale = Vector3.one;
        m_SpriteRenderer.enabled = true;
        StartCoroutine(ScaleUpBro());
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 dir = collision.transform.position - transform.position;

        Gunner g = collision.GetComponent<Gunner>();
        if (g)
        {
            if (g.Friendly == m_Friendly) return;

            g.Damage(m_Damage, dir.normalized * m_Knockback);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Vector2 dir = collision.transform.position - transform.position;

        Gunner g = collision.GetComponent<Gunner>();
        if (g)
        {
            if (g.Friendly == m_Friendly) return;

            g.Push(transform.right * m_Knockback * Strength);
        }

        Bullet b = collision.GetComponent<Bullet>();
        if (b)
        {
            if (b.Friendly == m_Friendly) return;

            b.RigidBody.AddForce(dir.normalized * m_Knockback, ForceMode2D.Impulse);
        }
    }

    protected override void Kill()
    {
        m_SpriteRenderer.enabled = false;
        Invoke("Disable", m_DeathDelay);
    }

    private void Disable()
    {
        if (gameObject.activeSelf) gameObject.SetActive(false);
    }

    IEnumerator ScaleUpBro()
    {
        float speed = 1.0f / m_Lifetime;
        float invPixel = 1.0f / m_PixelScale;
        for (float i = 0.0f; i < 1.0f; i += Time.deltaTime * speed)
        {
            float s = m_ScaleCurve.Evaluate(i) * m_MaxWidth;
            transform.localScale = Vector3.one * s * invPixel;
            yield return null;
        }
    }
}

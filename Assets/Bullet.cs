using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Generic Bullet")]
    [SerializeField] protected Rigidbody2D m_Rigidbody = null;
    [SerializeField] [Range(0.0f, 30.0f)] float m_Lifetime = 4.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_BuildUpTime = 0.0f;
    [SerializeField] AnimationCurve m_BuildUpCurve = null;
    [SerializeField] [Range(0.0f, 5.0f)] float m_GravityScale = 0.0f;

    public float Strength
    {
        get { return m_Strength; }
        set
        {
            m_Strength = value;
            m_Rigidbody.velocity = m_Dir * m_Strength;
        }
    }

    Vector2 m_Dir;
    float m_Strength;
    
    virtual public void Init(Vector2 dir, float strength)
    {
        m_Dir = dir;
        Strength = strength;
        m_Rigidbody.gravityScale = m_GravityScale;

        float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        transform.rotation = Quaternion.Euler(Vector3.forward * angle);

        Invoke("Kill", m_Lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    void Kill()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}

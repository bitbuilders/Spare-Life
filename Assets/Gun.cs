﻿using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Generic Gun")]
    [SerializeField] Sprite m_BulletSprite = null;
    [SerializeField] GameObject m_BulletTemplate = null;
    [SerializeField] Transform m_Barrel = null;
    [SerializeField] [Range(0, 100)] int m_PoolSize = 20;
    [SerializeField] [Range(0.0f, 100.0f)] float m_LaunchSpeed = 20.0f;

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
            b.Init(dir, m_LaunchSpeed);
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
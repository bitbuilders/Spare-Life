using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Gunner m_Target = null;
    [SerializeField] [Range(0.0f, 20.0f)] float m_FollowSpeed = 5.0f;
    [SerializeField] [Range(0.0f, 20.0f)] float m_Lead = 2.0f;

    void LateUpdate()
    {
        Vector2 point = m_Target.transform.position;
        Vector2 lead = m_Target.m_Rigidbody.velocity * m_Lead;
        Vector2 currentTarget = point + lead;

        transform.position = Vector2.Lerp(transform.position, currentTarget, Time.deltaTime * m_FollowSpeed);
        transform.position += Vector3.back * 10.0f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotAim : MonoBehaviour
{
    [SerializeField] [Range(0.0f, 100.0f)] float m_MouseAimSpeed = 15.0f;
    [SerializeField] [Range(0.0f, 720.0f)] float m_ControllerAimSpeed = 450.0f;
    [SerializeField] [Range(-180.0f, 180.0f)] float m_AngleMin =  -90.0f;
    [SerializeField] [Range(-180.0f, 180.0f)] float m_AngleMax = 90.0f;
    [SerializeField] [Range(0.0f, 1.0f)] float m_DeadZone = 0.01f;

    Camera m_Camera;
    float m_Angle;

    private void Start()
    {
        m_Camera = Camera.main;
    }

    void Update()
    {
        if (MouseActive.Instance.Active)
        {
            UseMousePosition();
        }
        else if (Input.GetJoystickNames().Length > 0)
        {
            UseControllerInput();
        }
    }

    void UseMousePosition()
    {
        Vector2 mousePos = m_Camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mousePos - (Vector2)transform.position;

        float sign = Mathf.Sign(transform.lossyScale.x);
        float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y * sign, dir.x * sign);
        m_Angle = Mathf.Lerp(m_Angle, angle, Time.deltaTime * m_MouseAimSpeed);

        SetRotationFromAngle();
    }

    void UseControllerInput()
    {
        float sign = Mathf.Sign(transform.lossyScale.x);
        m_Angle += Input.GetAxis("Vertical") * m_ControllerAimSpeed * sign * Time.deltaTime;

        SetRotationFromAngle();
    }

    void SetRotationFromAngle()
    {
        m_Angle = Mathf.Clamp(m_Angle, m_AngleMin, m_AngleMax);
        transform.rotation = Quaternion.Euler(Vector3.forward * m_Angle);
    }

    public void Flip()
    {
        m_Angle *= -1.0f;
    }
}

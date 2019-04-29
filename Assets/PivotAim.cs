using System.Collections;
using UnityEngine;

public class PivotAim : MonoBehaviour
{
    [SerializeField] [Range(0.0f, 100.0f)] float m_MouseAimSpeed = 15.0f;
    [SerializeField] [Range(0.0f, 720.0f)] float m_ControllerAimSpeed = 450.0f;
    [SerializeField] [Range(-180.0f, 180.0f)] float m_AngleMin =  -90.0f;
    [SerializeField] [Range(-180.0f, 180.0f)] float m_AngleMax = 90.0f;
    [SerializeField] AnimationCurve m_KickCurve = null;
    [SerializeField] bool m_PlayerDriven = true;

    public Vector2 Target { get; set; }

    Camera m_Camera;
    Vector2 m_StartingAngles;
    float m_Angle;
    float m_AngleOffset;

    private void Start()
    {
        m_Camera = Camera.main;
        m_StartingAngles.x = m_AngleMin;
        m_StartingAngles.y = m_AngleMax;
    }

    void Update()
    {
        if (m_PlayerDriven)
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
        else
        {
            SetRotationFromPoint(Target);
        }
    }

    void UseMousePosition()
    {
        SetRotationFromPoint(m_Camera.ScreenToWorldPoint(Input.mousePosition));
    }

    void UseControllerInput()
    {
        float sign = Mathf.Sign(transform.lossyScale.x);
        m_Angle += Input.GetAxis("Vertical") * m_ControllerAimSpeed * sign * Time.deltaTime;

        SetRotationFromAngle();
    }

    void SetRotationFromPoint(Vector2 point)
    {
        Vector2 dir = point - (Vector2)transform.position;

        float sign = Mathf.Sign(transform.lossyScale.x);
        float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y * sign, dir.x * sign);
        m_Angle = Mathf.Lerp(m_Angle, angle, Time.deltaTime * m_MouseAimSpeed);

        SetRotationFromAngle();
    }

    void SetRotationFromAngle()
    {
        m_Angle = Mathf.Clamp(m_Angle, m_AngleMin, m_AngleMax);
        float offset = m_AngleOffset * Mathf.Sign(transform.lossyScale.x);
        transform.rotation = Quaternion.Euler(Vector3.forward * (m_Angle + offset));
    }

    public void Kick(float strength, float duration)
    {
        StopAllCoroutines();
        m_Angle += m_AngleOffset * Mathf.Sign(transform.lossyScale.x); // Take this out to remove persistent recoil
        StartCoroutine(Offset(strength, duration));
    }

    IEnumerator Offset(float amount, float time)
    {
        for (float i = 0.0f; i < time; i += Time.deltaTime)
        {
            float t = i / time;
            float strength = m_KickCurve.Evaluate(t) * amount;
            m_AngleOffset = strength;
            yield return null;
        }
        m_AngleOffset = 0.0f;
    }

    public void Flip()
    {
        m_Angle *= -1.0f;
    }

    public void SetAngles(float min, float max)
    {
        m_AngleMin = min;
        m_AngleMax = max;
    }

    public void RevertAngles()
    {
        m_AngleMin = m_StartingAngles.x;
        m_AngleMax = m_StartingAngles.y;
    }
}

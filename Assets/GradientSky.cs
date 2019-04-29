using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientSky : MonoBehaviour
{
    [SerializeField] Gradient m_ColorOverTime = null;
    [SerializeField] AnimationCurve m_ColorShift = null;
    [SerializeField] [Range(0.0f, 60.0f)] float m_FullCycleTime = 20.0f;

    Camera m_Camera;
    float m_Time;

    private void Start()
    {
        m_Camera = GetComponent<Camera>();
    }

    private void Update()
    {
        m_Time += Time.deltaTime;
        if (m_Time > m_FullCycleTime) m_Time = 0.0f;

        m_Camera.backgroundColor = GetCurrentColor();
    }

    Color GetCurrentColor()
    {
        float t = m_ColorShift.Evaluate(m_Time / m_FullCycleTime);
        return m_ColorOverTime.Evaluate(t);
    }
}

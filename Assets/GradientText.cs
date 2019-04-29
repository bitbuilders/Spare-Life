using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientText : MonoBehaviour
{
    [SerializeField] Text m_Text = null;
    [SerializeField] Gradient m_ColorOverTime = null;
    [SerializeField] AnimationCurve m_ColorShift = null;
    [SerializeField] [Range(0.0f, 60.0f)] float m_FullCycleTime = 20.0f;

    float m_Time;

    private void Update()
    {
        m_Time += Time.deltaTime;
        if (m_Time > m_FullCycleTime) m_Time = 0.0f;

        m_Text.color = GetCurrentColor();
    }

    Color GetCurrentColor()
    {
        float t = m_ColorShift.Evaluate(m_Time / m_FullCycleTime);
        return m_ColorOverTime.Evaluate(t);
    }
}

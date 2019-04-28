using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Modifier
{
    ANGLE,
    JUMP,
    SPEED,
    DASH
}

[System.Serializable]
public class Debuff
{
    [SerializeField] Modifier m_Type;
    [SerializeField] float m_Value;

    public Modifier Type { get { return m_Type; } }
    public float Value { get { return m_Value; } }

    public Debuff(Modifier type, float value = 0.0f)
    {
        m_Type = type;
        m_Value = value;
    }
}

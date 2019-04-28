using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Icon : MonoBehaviour
{
    [SerializeField] Image m_MainIcon = null;
    [SerializeField] Image m_X = null;
    [SerializeField] AnimationCurve m_MainCurve = null;
    [SerializeField] AnimationCurve m_XCurve = null;
    [SerializeField] [Range(0.0f, 1000.0f)] float m_StartHeight = 500.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_ScalePop = 2.0f;

    private void Awake()
    {
        m_MainIcon.gameObject.SetActive(false);
        m_X.gameObject.SetActive(false);
    }

    public void SetMain(Sprite sprite)
    {
        m_MainIcon.sprite = sprite;
    }

    public void ShowIcon(float time)
    {
        m_MainIcon.gameObject.SetActive(true);
        StartCoroutine(AnimateMainIcon(time));
    }

    IEnumerator AnimateMainIcon(float time)
    {
        for (float i = 0.0f; i < time; i += Time.deltaTime)
        {
            float t = i / time;
            float p = (m_MainCurve.Evaluate(t) * (m_ScalePop));
            m_MainIcon.transform.localScale = Vector3.one * p;
            yield return null;
        }

        m_MainIcon.transform.localScale = Vector3.one;
    }

    public void ShowX(float time)
    {
        m_X.gameObject.SetActive(true);
        StartCoroutine(AnimateIcon(time, m_X, m_XCurve));
    }

    IEnumerator AnimateIcon(float time, Image image, AnimationCurve curve)
    {
        Vector2 target = Vector2.zero;
        Vector2 start = Vector2.up * m_StartHeight;
        for (float i = 0.0f; i < time; i += Time.deltaTime)
        {
            float t = i / time;
            float p = curve.Evaluate(t);
            image.transform.localPosition = Vector2.LerpUnclamped(start, target, p);
            yield return null;
        }

        image.transform.localPosition = target;
    }
}

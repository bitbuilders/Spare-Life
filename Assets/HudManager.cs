using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : Singleton<HudManager>
{
    [System.Serializable]
    public struct IconName
    {
        public string Name;
        public Sprite Sprite;
    }

    [SerializeField] GameObject m_IconTemplate = null;
    [SerializeField] RectTransform m_Canvas = null;
    [SerializeField] RectTransform m_Debuffs = null;
    [SerializeField] List<IconName> m_Icons = null;
    [SerializeField] [Range(0.0f, 10.0f)] float m_IconTimes = 1.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_XTimes = 0.5f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_MoveTime = 0.5f;
    [SerializeField] AnimationCurve m_MoveCurve = null;

    private void Start()
    {

    }

    public void CreateDebuffIcon(string name)
    {
        Sprite s = GetSprite(name.ToUpper());

        if (!s) return;

        GameObject icon = CreateIcon(s);
        StartCoroutine(AnimateIcon(icon));
    }

    Sprite GetSprite(string name)
    {
        Sprite icon = null;

        foreach (IconName i in m_Icons)
        {
            if (i.Name.ToUpper() == name)
            {
                icon = i.Sprite;
                break;
            }
        }

        return icon;
    }

    GameObject CreateIcon(Sprite sprite)
    {
        GameObject go = Instantiate(m_IconTemplate, m_Canvas);
        go.GetComponent<Icon>().SetMain(sprite);

        return go;
    }

    IEnumerator AnimateIcon(GameObject icon)
    {
        Icon i = icon.GetComponent<Icon>();
        i.ShowIcon(m_IconTimes);
        yield return new WaitForSeconds(m_IconTimes);

        i.ShowX(m_XTimes);
        yield return new WaitForSeconds(m_XTimes + 0.25f);

        // Move it
        Vector3 target = GetTargetLocation(i);
        Vector3 start = i.transform.position;
        float startScale = i.transform.localScale.x;
        for (float t = 0.0f; t < m_MoveTime; t += Time.deltaTime)
        {
            float p = t / m_MoveTime;
            float point = m_MoveCurve.Evaluate(p);
            i.transform.position = Vector3.Lerp(start, target, point);
            i.transform.localScale = Vector3.one * Mathf.Lerp(startScale, 1.0f, p);
            yield return null;
        }

        i.transform.SetParent(m_Debuffs, false);
        i.transform.localPosition = Vector3.zero;
    }

    Vector3 GetTargetLocation(Icon i)
    {
        float width = i.GetComponent<RectTransform>().sizeDelta.x;
        float yOff = m_Debuffs.sizeDelta.y / 2.0f;
        float xOff = (width / 2.0f) + width * (m_Debuffs.childCount);

        return m_Debuffs.position + new Vector3(xOff, -yOff);
    }
}

using UnityEngine;

public abstract class Gunner : MonoBehaviour
{
    [SerializeField] protected Gun m_Gun = null;
    [SerializeField] protected Transform m_HoldPoint = null;

    public Gun SwapGun(Gun gun)
    {
        Gun currentGun = m_Gun;
        m_Gun = gun;

        m_Gun.transform.parent = m_HoldPoint;
        m_Gun.transform.localPosition = Vector3.zero;

        return currentGun;
    }
}

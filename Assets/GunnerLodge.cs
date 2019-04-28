using System.Collections.Generic;
using System.Linq;

public class GunnerLodge : Singleton<GunnerLodge>
{
    public List<Gunner> Gunners { get; private set; }
    public X0V X0V { get; private set; }

    private void Start()
    {
        Gunners = FindObjectsOfType<Gunner>().ToList();
        X0V = FindObjectOfType<X0V>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        X0V x0v = collision.GetComponent<X0V>();
        if (x0v)
        {
            // DO finish
            Game.Instance.LoadWinScreen();
        }
    }
}

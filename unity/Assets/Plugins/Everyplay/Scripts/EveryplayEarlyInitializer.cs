using UnityEngine;
using System.Collections;

public class EveryplayEarlyInitializer : MonoBehaviour
{
    void Awake()
    {
        Everyplay.Initialize();
    }

    void Start()
    {
        Destroy(gameObject);
    }
}

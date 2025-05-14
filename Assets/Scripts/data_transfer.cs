using UnityEngine;

public class data_transfer : GameBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}

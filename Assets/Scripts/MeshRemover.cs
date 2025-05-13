using UnityEngine;

public class MeshRemover : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}

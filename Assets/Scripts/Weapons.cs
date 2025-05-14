using UnityEngine;

public class Weapons : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy") || other.CompareTag("goldenfreddy"))
        {
            Destroy(other);
        }
    }
}

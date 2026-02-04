using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    private BoxCollider _collider;
    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the door trigger.");
        }
    }
}


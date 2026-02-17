using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    private BoxCollider _collider;
    [SerializeField] private string _sceneToLoad;
    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySound(Sounds.Success);
            SceneManager.LoadScene(_sceneToLoad);
        }
    }
}


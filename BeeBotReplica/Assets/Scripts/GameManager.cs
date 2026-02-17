using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public UnityEvent<bool> CodeRan = new UnityEvent<bool>();
    public bool IsSuccess;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CodeRan.AddListener(CodeFinished);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CodeFinished(bool isSuccess)
    {
        if(isSuccess)
        {
            IsSuccess = true;
            AudioManager.Instance.PlaySound(Sounds.Success);
        }
        else
        {
            IsSuccess = false;
            AudioManager.Instance.PlaySound(Sounds.Failed);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

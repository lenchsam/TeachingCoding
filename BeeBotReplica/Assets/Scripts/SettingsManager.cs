using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    private Camera _camera;
    UniversalAdditionalCameraData camData;
    private AntialiasingMode antialiasingMode = AntialiasingMode.None;

    private void Start()
    {
        //needed so camera in new scene can also have the changed settings
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;

        _camera = Camera.main;
        camData = _camera.GetUniversalAdditionalCameraData();
    }
    public void SetAntiAliasingMode(int antialiasing)
    {
        antialiasingMode = (AntialiasingMode)antialiasing;
        camData.antialiasing = antialiasingMode;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _camera = Camera.main;
        camData = _camera.GetUniversalAdditionalCameraData();

        //set all settings again for new camera in new scene
        SetAllSettings();

        if (scene.name == "MainMenu")
        {
            SetOptionsMenuUI(); 
        }
    }

    private void SetAllSettings()
    {
        camData.antialiasing = antialiasingMode;
    }

    private void SetOptionsMenuUI()
    {

    }
}

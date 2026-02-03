using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager Instance { get; private set; }
    private List<string> _logs = new();

    [SerializeField] private TMP_Text consoleText;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void DisplayLogs()
    {

    }

    public static void Log(string message)
    {
        Instance.AddLog(message);
    }

    public static void ClearLogs()
    {
        Instance._logs.Clear();
        Instance.RefreshUI();
    }

    private void AddLog(string message)
    {
        _logs.Add(message);

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (consoleText == null)
            return;

        StringBuilder builder = new StringBuilder();

        foreach (var log in _logs)
        {
            builder.AppendLine(log);
        }

        consoleText.text = builder.ToString();
    }
}

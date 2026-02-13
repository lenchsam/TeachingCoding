using UnityEngine;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public struct ScriptableObjectEntry
{
    public string VariableName; //name of variable in code
    public Transform Object;    //reference to gameobject
}

public class ScriptRunner : MonoBehaviour
{
    [SerializeField] private TMP_InputField _TMP_IF;
    [SerializeField] private List<ScriptableObjectEntry> ObjectsToControl;

     private string _sourceCode = "";

    [ContextMenu("Run Code")]
    public void RunCode()
    {
        ConsoleManager.ClearLogs();
        //tokenise
        Lexer lexer = new Lexer(_sourceCode);
        List<Token> tokens = lexer.Tokenise();

        //parse
        RecursiveDescentParser parser = new RecursiveDescentParser(tokens);
        List<Statement> statements = parser.Parse();

        //interpret
        Interpreter interpreter = new Interpreter();

        //set up global variables
        foreach (var entry in ObjectsToControl)
        {
            if (entry.Object != null)
            {
                interpreter.SetGlobal(entry.VariableName, entry.Object);
            }
        }

        _ = interpreter.Interpret(statements);
    }

    public void Run()
    {
        _sourceCode = _TMP_IF.text;
        RunCode();
    }
    void Start()
    {
        RunCode();
    }
}

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
    public TMP_InputField TMP_IF;
    public List<ScriptableObjectEntry> ObjectsToControl;

    [TextArea(5, 20)]
    public string SourceCode = @"
        move(Player; 5; 0; 0);
        move(Enemy; -5; 0; 0);
    ";

    [ContextMenu("Run Code")]
    public void RunCode()
    {
        //tokenise
        Lexer lexer = new Lexer(SourceCode);
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

        interpreter.Interpret(statements);
    }

    public void Run()
    {
        SourceCode = TMP_IF.text;
        RunCode();
    }
    void Start()
    {
        RunCode();
    }
}

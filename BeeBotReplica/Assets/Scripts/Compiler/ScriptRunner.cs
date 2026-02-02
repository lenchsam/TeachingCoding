using UnityEngine;
using System.Collections.Generic;

public class ScriptRunner : MonoBehaviour
{
    [TextArea(5, 20)]
    public string SourceCode = @"
        x = 0;
        count = 0;
        
        while (x == 0) {
            count = count + 1;
            if (count == 5) {
                x = 1; 
            }
        }
        
        result = count * 10;
    ";

    [ContextMenu("Run Code")]
    public void RunCode()
    {
        Debug.Log("Starting Compilation");

        //tokenise
        Lexer lexer = new Lexer(SourceCode);
        List<Token> tokens = lexer.Tokenise();
        Debug.Log($"Lexer found {tokens.Count} tokens.");

        //parse
        RecursiveDescentParser parser = new RecursiveDescentParser(tokens);
        List<Statement> statements = parser.Parse();
        Debug.Log($"Parser found {statements.Count} statements.");

        //interpret
        Interpreter interpreter = new Interpreter(this.transform);
        interpreter.Interpret(statements);

        Debug.Log("Execution Finished");
    }

    void Start()
    {
        RunCode();
    }
}

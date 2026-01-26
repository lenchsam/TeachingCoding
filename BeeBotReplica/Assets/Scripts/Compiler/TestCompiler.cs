using System.Collections.Generic;
using UnityEngine;

public class TestCompiler : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("TestCompiler started");

        string sourceCode = "if (x == 10) { x = 5; }";

        Lexer lexer = new Lexer(sourceCode);
        List<Token> tokens = lexer.Tokenise();
    }
}

using UnityEngine;

public class TestCompiler : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("TestCompiler started");

        string sourceCode = "if (x == 10) { return true; }";

        Lexer lexer = new Lexer(sourceCode);
        var tokens = lexer.Tokenise();

        foreach (var token in tokens)
        {
            Debug.Log($"Type: {token.Type} | Lexeme: '{token.Lexeme}'");
        }
    }
}

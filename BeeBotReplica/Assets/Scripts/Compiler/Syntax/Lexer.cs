using System.Collections.Generic;

public enum TokenType
{
    Identifier,
    Number,
    If,
    Else,
    While,
    True,
    False,
    Plus,
    Minus,
    Star,
    Slash,
    Equals,
    DoubleEquals,
    LeftParen, 
    RightParen,
    LeftBrace,
    RightBrace,
    Semicolon,
    Comma,
    Move,
    Attack,
    EOF //End of file
}
public struct Token
{
    public TokenType Type;
    public string Lexeme; //raw text from source code

    //where in source code
    public int Line;
    public int Column;

    public Token(TokenType type, string lexeme, int line, int column)
    {
        Type = type;
        Lexeme = lexeme;
        Line = line;
        Column = column;
    }
}
public class Lexer
{
    //walks source code and produces a list of tokens
    //dont do any parsing here
    //should convert something like "if (x == 10) { return true; }" into a list of tokens

    private int _start = 0;     //where the current token started
    private int _current = 0;   //what char we are currently looking at

    int _line = 1;
    int _column = 0;
    List<Token> _tokens = new List<Token>();

    string _source;

    //needed for converting string to token type
    private static readonly Dictionary<string, TokenType> _tokenTypeToString = new Dictionary<string, TokenType>
    {
        { "if",     TokenType.If},
        { "else",   TokenType.Else},
        { "while",  TokenType.While},
        { "true",   TokenType.True},
        { "false",  TokenType.False},
        { "Move",   TokenType.Move },
        { "Attack", TokenType.Attack }
    };

    public Lexer(string source)
    {
        _source = source;
    }

    public List<Token> Tokenise()
    {
        while (_current < _source.Length) 
        {
            _start = _current;
            ScanToken();
        }  

        _tokens.Add(new Token(TokenType.EOF, "", _line, _column));
        return _tokens;
    }
    private void ScanToken()
    {
        char c = Advance();

        switch (c)
        {
            //single characters
            case '(': AddToken(TokenType.LeftParen);  break;
            case ')': AddToken(TokenType.RightParen); break;
            case '{': AddToken(TokenType.LeftBrace);  break;
            case '}': AddToken(TokenType.RightBrace); break;
            case ';': AddToken(TokenType.Semicolon);  break;
            case ',': AddToken(TokenType.Comma);      break;
            case '+': AddToken(TokenType.Plus);       break;
            case '*': AddToken(TokenType.Star);       break;
            case '/': AddToken(TokenType.Slash);      break;
            case '-': AddToken(TokenType.Minus);      break;
            //two characters
            case '=':
                //check if next char is also =
                //needed for ==
                AddToken(Match('=') ? TokenType.DoubleEquals : TokenType.Equals);
                break;

            //ignore whitespace but track line/column
            case ' ':
                break;

            case '\n':
                _line++;
                _column = 0;
                break;

            default:
                if (IsNumber(c))
                {
                    Number();
                }
                else if (IsLetter(c))
                {
                    Identifier();
                }
                else
                {
                    //unrecognized character
                    //TODO: handle error ingame
                }
                break;
        }
    }
    //token handlers



    //handles numbers 
    private void Number()
    {
        //keep going while next char is a number
        while (IsNumber(Peek())) Advance();

        AddToken(TokenType.Number);
    }
    
    //handles identifiers and keywords
    private void Identifier()
    {
        //keep going while next char is letter or number
        while (IsAlphaNumeric(Peek())) Advance();

        string text = _source.Substring(_start, _current - _start);

        //is it a keyword, if not its an identifier
        TokenType type = _tokenTypeToString.ContainsKey(text) ? _tokenTypeToString[text] : TokenType.Identifier;

        AddToken(type);
    }

    
    
    //adds token to list
    private void AddToken(TokenType type)
    {
        string lexeme = _source.Substring(_start, _current - _start);
        _tokens.Add(new Token(type, lexeme, _line, _column));
    }
    //moves to next char and returns it
    private char Advance()
    {
        _column++;
        return _source[_current++];
    }
    //checks if current char matches expected char
    private bool Match(char expected)
    {
        if (_current >= _source.Length) return false;
        if (_source[_current] != expected) return false;

        _current++;
        _column++;
        return true;
    }
    //needed for looking ahead without moving forward (e.g. == vs =)
    private char Peek()
    {
        if(_current >= _source.Length)
            return '\0'; //null char
        return _source[_current];
    }
    private bool IsNumber(char c)
    {
        return c >= '0' && c <= '9';
    }

    private bool IsLetter(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
    }

    private bool IsAlphaNumeric(char c)
    {
        return IsLetter(c) || IsNumber(c);
    }
}

using System;
using System.Collections.Generic;

public class RecursiveDescentParser
{
    private List<Token> _tokens;
    private int _current = 0;

    public RecursiveDescentParser(List<Token> tokens)
    {
        _tokens = tokens;
    }
    public List<Statement> Parse()
    {
        List<Statement> statements = new List<Statement>();
        while (!IsAtEnd())
        {
            statements.Add(ParseStatement());
        }
        return statements;
    }

    #region parse statements
    Statement ParseStatement()
    {
        if (Match(TokenType.If)) return ParseIfStatement();
        else if (Match(TokenType.While)) return ParseWhileStatement();
        else if (Match(TokenType.Move)) return ParseMoveStatement();
        else if (Match(TokenType.MoveTo)) return ParseMoveToStatement();
        else if (Match(TokenType.Attack)) return ParseAttackStatement();

        if (Check(TokenType.Identifier) && CheckNext(TokenType.Equals))
        {
            return ParseAssignment();
        }

        return ParseExpressionStatement();
    }

    private Statement ParseAssignment()
    {
        //guaranteed to be identifier followed by equals because of check in ParseStatement
        Token identifierToken = Advance();
        Consume(TokenType.Equals, "Expect '=' after identifier.");
        Expression value = ParseExpression();
        Consume(TokenType.Semicolon, "Expect ';' after assignment value.");
        return new AssignmentStatement(identifierToken.Lexeme, value);
    }

    private IfStatement ParseIfStatement()
    {
        Consume(TokenType.LeftParen, "Expect '(' after 'if'");
        Expression condition = ParseExpression();
        Consume(TokenType.RightParen, "Expect ')' after condition");

        List<Statement> thenBranch = ParseBlock();
        List<Statement> elseBranch = null;

        if (Match(TokenType.Else))
        {
            elseBranch = ParseBlock();
        }

        return new IfStatement(condition, thenBranch, elseBranch);
    }

    private WhileStatement ParseWhileStatement()
    {
        Consume(TokenType.LeftParen, "Expect '(' after 'while'");
        Expression condition = ParseExpression();
        Consume(TokenType.RightParen, "Expect ')' after condition");

        List<Statement> body = ParseBlock();

        return new WhileStatement(condition, body);
    }

    private MoveStatement ParseMoveStatement()
    {
        //parse syntax - (x, y)
        Consume(TokenType.LeftParen, "Expect '(' after 'move'");

        Direction dir;
        if (Match(TokenType.North)) dir = Direction.North;
        else if (Match(TokenType.South)) dir = Direction.South;
        else if (Match(TokenType.East)) dir = Direction.East;
        else if (Match(TokenType.West)) dir = Direction.West;
        else
        {
            ConsoleManager.Log("Expected direction (North, South, East, or West)");
            throw new Exception("Expected direction");
        }

        Consume(TokenType.RightParen, "Expect ')' after direction");
        Consume(TokenType.Semicolon, "Expect ';' after statement");

        return new MoveStatement(dir);
    }

    private MoveToStatement ParseMoveToStatement()
    {
        //parse syntax - (x, y)
        Consume(TokenType.LeftParen, "Expect '(' after 'move'");

        Expression x = ParseExpression();

        Consume(TokenType.Comma, "Expect separator after x");

        Expression y = ParseExpression();

        Consume(TokenType.RightParen, "Expect ')' after z coordinate");
        Consume(TokenType.Semicolon, "Expect ';' after statement");

        return new MoveToStatement(x, y);
    }

    private AttackStatement ParseAttackStatement()
    {
        Consume(TokenType.LeftParen, "Expect '(' after 'attack'");
        //parse target
        Expression target = ParseExpression();
        Consume(TokenType.RightParen, "Expect ')' after target");
        Consume(TokenType.Semicolon, "Expect ';' after statement");
        return new AttackStatement(target);
    }
    private List<Statement> ParseBlock()
    {
        Consume(TokenType.LeftBrace, "Expect '{' before block.");
        List<Statement> statements = new List<Statement>();

        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            statements.Add(ParseStatement());
        }

        Consume(TokenType.RightBrace, "Expect '}' after block.");
        return statements;
    }

    private Statement ParseExpressionStatement()
    {
        Expression expr = ParseExpression();
        Consume(TokenType.Semicolon, "Expect ';' after expression.");
        return new ExpressionStatement(expr);
    }

    #endregion

    #region parse expressions

    private Expression ParseExpression()
    {
        return ParseEquality();
    }

    // ==
    private Expression ParseEquality()
    {
        Expression expr = ParseTerm();

        while (Match(TokenType.DoubleEquals))
        {
            TokenType op = Previous().Type;
            Expression right = ParseTerm();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    // + and -
    private Expression ParseTerm()
    {
        Expression expr = ParseFactor();

        while (Match(TokenType.Plus, TokenType.Minus))
        {
            TokenType op = Previous().Type;
            Expression right = ParseFactor();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    // * and /
    private Expression ParseFactor()
    {
        Expression expr = ParseUnary();

        while (Match(TokenType.Star, TokenType.Slash))
        {
            TokenType op = Previous().Type;
            Expression right = ParseUnary();
            expr = new BinaryExpression(expr, op, right);
        }

        return expr;
    }

    //literals, variables, parentheses
    private Expression ParsePrimary()
    {
        if (Match(TokenType.False)) return new LiteralExpression(false);
        if (Match(TokenType.True)) return new LiteralExpression(true);

        if (Match(TokenType.Number))
        {
            //assumes integer
            return new LiteralExpression(int.Parse(Previous().Lexeme));
        }

        if (Match(TokenType.Identifier))
        {
            return new VariableExpression(Previous().Lexeme);
        }

        if (Match(TokenType.LeftParen))
        {
            Expression expr = ParseExpression();
            Consume(TokenType.RightParen, "Expect ')' after expression.");
            return expr;
        }
        ConsoleManager.Log($"unrecognised token: {Peek().Lexeme}");
        throw new Exception($"unrecognised token: {Peek().Lexeme}");
    }

    private Expression ParseUnary()
    {
        //can add other types here later such as !
        if (Match(TokenType.Minus))
        {
            Token operatorToken = Previous();
            Expression right = ParseUnary();

            return new UnaryExpression(operatorToken, right);
        }
        return ParsePrimary();
    }

    #endregion

    #region helpers
    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }
    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }
    private bool CheckNext(TokenType type)
    {
        if (_current + 1 >= _tokens.Count) return false;
        return _tokens[_current + 1].Type == type;
    }
    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }
    private Token Peek()
    {
        return _tokens[_current];
    }
    private Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }
    private Token Previous()
    {
        return _tokens[_current - 1];
    }
    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();

        ConsoleManager.Log(message);
        throw new Exception(message);
    }
    #endregion
}

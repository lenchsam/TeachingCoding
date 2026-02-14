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
        else if (Match(TokenType.Repeat)) return ParseRepeatStatement();
        else if (Match(TokenType.Move)) return ParseMoveStatement();

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
    private RepeatStatement ParseRepeatStatement()
    {
        Consume(TokenType.LeftParen, "Expect '(' after 'Repeat'");
        Expression count = ParseExpression();
        Consume(TokenType.RightParen, "Expect ')' after count");

        List<Statement> body = ParseBlock();

        return new RepeatStatement(count, body);
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

        if (Match(TokenType.North)) return new LiteralExpression(Direction.North);
        if (Match(TokenType.South)) return new LiteralExpression(Direction.South);
        if (Match(TokenType.East)) return new LiteralExpression(Direction.East);
        if (Match(TokenType.West)) return new LiteralExpression(Direction.West);

        if (Match(TokenType.Number))
        {
            return new LiteralExpression(int.Parse(Previous().Lexeme));
        }

        if (Match(TokenType.Identifier))
        {
            string name = Previous().Lexeme;

            //check if it's a function call
            if (Match(TokenType.LeftParen))
            {
                List<Expression> arguments = new List<Expression>();

                //does it have parameters
                if (!Check(TokenType.RightParen))
                {
                    do
                    {
                        arguments.Add(ParseExpression());
                    } while (Match(TokenType.Comma));
                }

                Consume(TokenType.RightParen, "Expect ')' after function arguments.");
                return new CallExpression(name, arguments);
            }

            //if not a function call, it's a variable
            return new VariableExpression(name);
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

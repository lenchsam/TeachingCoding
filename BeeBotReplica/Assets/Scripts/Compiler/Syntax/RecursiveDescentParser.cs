using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.ConstrainedExecution;

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
        Consume(TokenType.LeftParen, "Expect '(' after 'move'");

        //parse target
        Expression target = ParseExpression();
        Consume(TokenType.Comma, "Expect separator after target");

        //parse coords
        Expression x = ParseExpression();
        Consume(TokenType.Comma, "Expect separator after x");

        Expression y = ParseExpression();
        Consume(TokenType.Comma, "Expect separator after y");

        Expression z = ParseExpression();

        Consume(TokenType.RightParen, "Expect ')' after z coordinate");
        Consume(TokenType.Semicolon, "Expect ';' after statement");

        return new MoveStatement(target, x, y, z);
    }
    
    private AttackStatement ParseAttackStatement()
    {
        Consume(TokenType.LeftParen, "Expect '(' after 'attack'");
        //parse attacker
        Expression attacker = ParseExpression();
        Consume(TokenType.Comma, "Expect ',' after attacker");

        //parse target
        Expression target = ParseExpression();
        Consume(TokenType.RightParen, "Expect ')' after target");
        Consume(TokenType.Semicolon, "Expect ';' after statement");
        return new AttackStatement(attacker, target);
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

        throw new Exception("unrecognised token");
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
        throw new Exception("parse error");
    }
    #endregion
}

using System.Collections.Generic;

public abstract class Statement { }
public abstract class Expression { }

public class AssignmentStatement : Statement
{
    public string Identifier;
    public Expression Value;
    public AssignmentStatement(string identifier, Expression value)
    {
        Identifier = identifier;
        Value = value;
    }
}

public class IfStatement : Statement
{
    public Expression Condition;
    public List<Statement> ThenBranch;
    public List<Statement> ElseBranch;
    public IfStatement(Expression condition, List<Statement> thenBranch, List<Statement> elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }
}

public class WhileStatement : Statement
{
    public Expression Condition;
    public List<Statement> Body;
    public WhileStatement(Expression condition, List<Statement> body)
    {
        Condition = condition;
        Body = body;
    }
}
public class ExpressionStatement : Statement
{
    public Expression Expression;
    public ExpressionStatement(Expression expression)
    {
        Expression = expression;
    }
}
public class MoveStatement : Statement
{
    public Expression X;
    public Expression Y;
    public MoveStatement(Expression x, Expression y)
    {
        X = x;
        Y = y;
    }
}
public class MoveToStatement : Statement
{
    public Expression X;
    public Expression Y;
    public MoveToStatement(Expression x, Expression y)
    {
        X = x;
        Y = y;
    }
}
public class AttackStatement : Statement
{
    public Expression Target;
    public AttackStatement(Expression target)
    {
        Target = target;
    }
}

public class BinaryExpression : Expression
{
    public Expression Left;
    public TokenType Operator;
    public Expression Right;
    public BinaryExpression(Expression left, TokenType op, Expression right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }
}
public class LiteralExpression : Expression
{
    public object Value;
    public LiteralExpression(object value)
    {
        Value = value;
    }
}
public class VariableExpression : Expression
{
    public string Name;
    public VariableExpression(string name)
    {
        Name = name;
    }
}
public class UnaryExpression : Expression
{
    public Token Operator;
    public Expression Right;
    public UnaryExpression(Token operatorToken, Expression right)
    {
        Operator = operatorToken;
        Right = right;
    }
}
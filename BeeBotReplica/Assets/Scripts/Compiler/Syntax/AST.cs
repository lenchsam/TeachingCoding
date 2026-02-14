using System.Collections.Generic;

public abstract class Statement { }
public abstract class Expression { }

#region statements
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
public class RepeatStatement : Statement
{
    public Expression Count;
    public List<Statement> Body;
    public RepeatStatement(Expression count, List<Statement> body)
    {
        Count = count;
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
public enum Direction
{
    North,
    South,
    East,
    West
}

public class MoveStatement : Statement
{
    public Direction Direction;
    public MoveStatement(Direction direction)
    {
        Direction = direction;
    }
}
#endregion

//expressions also holds premade functions which return a value
#region expressions
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
public class CallExpression : Expression
{
    public string Callee; //name of the function
    public List<Expression> Arguments;

    public CallExpression(string callee, List<Expression> arguments)
    {
        Callee = callee;
        Arguments = arguments;
    }
}
#endregion

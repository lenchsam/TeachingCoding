using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Interpreter : MonoBehaviour
{
    private Environment _environment = new Environment();

    public async Task Interpret(List<Statement> statements)
    {
        await Task.Yield();
        foreach (var statement in statements)
        {
            await Task.Delay(500);

            await Execute(statement);
        }
    }
    public void SetGlobal(string name, object value)
    {
        _environment.Assign(name, value);
    }

    //perform the action described by the statement
    private async Task Execute(Statement stmt)
    {
        await Task.Yield();
        switch (stmt)
        {
            case AssignmentStatement a:
                object value = Evaluate(a.Value);
                _environment.Assign(a.Identifier, value);
                break;

            case IfStatement i:
                if (IsTruthy(Evaluate(i.Condition)))
                {
                    await ExecuteBlock(i.ThenBranch);
                }
                else if (i.ElseBranch != null)
                {
                    await ExecuteBlock(i.ElseBranch);
                }
                break;

            case WhileStatement w:
                int safetyCounter = 0;
                while (IsTruthy(Evaluate(w.Condition)))
                {
                    await Task.Delay(500);
                    await ExecuteBlock(w.Body);

                    safetyCounter++;
                    if (safetyCounter > 1000)
                    {
                        ConsoleManager.Log("Infinite loop detected");
                        throw new Exception("infinite loop detected");
                    }
                }
                break;
            case RepeatStatement r:
                int repeatCount = (int)Evaluate(r.Count);

                if (repeatCount < 0)
                {
                    ConsoleManager.Log("Repeat count must be positive");
                    throw new Exception("Repeat count must be positive");
                }

                for (int i = 0; i < repeatCount; i++)
                {
                    await Task.Delay(500);
                    await ExecuteBlock(r.Body);
                }
                break;

            case ExpressionStatement e:
                Evaluate(e.Expression);
                break;
            case MoveStatement m:
                object moveTargetObj = _environment.Get("Player");

                if (moveTargetObj is Transform mt)
                {
                    switch (m.Direction)
                    {
                        case Direction.North:
                            mt.Translate(0, 0, 1);  // +Z
                            break;
                        case Direction.South:
                            mt.Translate(0, 0, -1); // -Z
                            break;
                        case Direction.East:
                            mt.Translate(1, 0, 0);  // +X
                            break;
                        case Direction.West:
                            mt.Translate(-1, 0, 0); // -X
                            break;
                    }
                }
                break;

        }
    }

    private async Task ExecuteBlock(List<Statement> statements)
    {
        await Task.Yield();
        foreach (var stmt in statements)
        {
            await Task.Delay(500);
            await Execute(stmt);
        }
    }

    private object Evaluate(Expression expr)
    {
        switch (expr)
        {
            case LiteralExpression l: return l.Value;
            case VariableExpression v: return _environment.Get(v.Name);
            case BinaryExpression b: return EvaluateBinary(b);
            case UnaryExpression u:
                object right = Evaluate(u.Right);

                if(u.Operator.Type == TokenType.Minus)
                {
                    if (right is int r) return -r;
                }
                ConsoleManager.Log("Unknown Operator Type");
                throw new Exception("Unknown Operator Type");
            case CallExpression c:
                return EvaluateCall(c);
            default:
                ConsoleManager.Log("Unknown expression");
                throw new Exception("Unknown expression.");


        }
    }
    private object EvaluateCall(CallExpression expr)
    {
        switch (expr.Callee)
        {
            case "GetNumEnemies":
                return 1;
            case "IsPathBlocked":
                //get direction
                if (expr.Arguments.Count < 1)
                {
                    ConsoleManager.Log("IsPathBlocked requires 1 direction parameter.");
                    throw new Exception("IsPathBlocked requires 1 direction parameter.");
                }

                //evaluate parameter
                object arg = Evaluate(expr.Arguments[0]);
                if (arg is Direction dir)
                {
                    return CheckForWall(dir);
                }

                ConsoleManager.Log("Parameter must be a Direction.");
                throw new Exception("Parameter must be a Direction.");
            default:
                ConsoleManager.Log($"Unknown function: {expr.Callee}");
                throw new Exception($"Unknown function: {expr.Callee}");
        }
    }

    private bool CheckForWall(Direction dir)
    {
        object playerObj = _environment.Get("Player");
        if (playerObj is Transform pt)
        {
            Vector3 worldDir = Vector3.forward;
            switch (dir)
            {
                case Direction.North: worldDir = Vector3.forward; break;
                case Direction.South: worldDir = Vector3.back; break;
                case Direction.East: worldDir = Vector3.right; break;
                case Direction.West: worldDir = Vector3.left; break;
            }

            //is a wall 1 unit away
            return Physics.Raycast(pt.position, worldDir, 1f);
        }
        return false;
    }

    private object EvaluateBinary(BinaryExpression b)
    {
        object left = Evaluate(b.Left);
        object right = Evaluate(b.Right);

        //are we evaluating an int
        if (left is int l && right is int r)
        {
            switch (b.Operator)
            {
                case TokenType.Plus: return l + r;
                case TokenType.Minus: return l - r;
                case TokenType.Star: return l * r;
                case TokenType.Slash:
                    if (r == 0) {
                        ConsoleManager.Log("Cannot divide by zero");
                        throw new System.Exception("Division by zero"); 
                    }
                    return l / r;
                case TokenType.DoubleEquals: return l == r;
                //add more operators later
                default:
                    ConsoleManager.Log($"Operator {b.Operator} not supported for Integers");
                    throw new System.Exception($"Operator {b.Operator} not supported for Integers");
            }
        }

        //are we evaluating a bool
        if (left is bool lb && right is bool rb)
        {
            switch (b.Operator)
            {
                case TokenType.DoubleEquals: return lb == rb;
                
                default:
                    ConsoleManager.Log($"Operator {b.Operator} not supported for Booleans");
                    throw new System.Exception($"Operator {b.Operator} not supported for Booleans");
            }
        }

        ConsoleManager.Log($"Cannot perform operation {b.Operator} on {left.GetType()} and {right.GetType()}");
        throw new System.Exception($"Cannot perform operation {b.Operator} on {left.GetType()} and {right.GetType()}");
    }

    private bool IsTruthy(object obj)
    {
        if (obj == null) return false;
        if (obj is bool b) return b;
        if (obj is int i) return i != 0; //0 is false
        return true;
    }

}

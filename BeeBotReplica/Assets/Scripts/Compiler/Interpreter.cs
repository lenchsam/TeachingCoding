using System;
using System.Collections.Generic;
using UnityEngine;

public class Interpreter : MonoBehaviour
{
    private Environment _environment = new Environment();
    private Transform _target;

    public Interpreter(Transform target)
    {
        _target = target;
    }
    public void Interpret(List<Statement> statements)
    {
        foreach (var statement in statements)
        {
            Execute(statement);
        }
    }
    public void SetGlobal(string name, object value)
    {
        _environment.Assign(name, value);
    }

    //perform the action described by the statement
    private void Execute(Statement stmt)
    {
        switch (stmt)
        {
            case AssignmentStatement a:
                object value = Evaluate(a.Value);
                _environment.Assign(a.Identifier, value);
                break;

            case IfStatement i:
                if (IsTruthy(Evaluate(i.Condition)))
                {
                    ExecuteBlock(i.ThenBranch);
                }
                else if (i.ElseBranch != null)
                {
                    ExecuteBlock(i.ElseBranch);
                }
                break;

            case WhileStatement w:
                int safetyCounter = 0;
                while (IsTruthy(Evaluate(w.Condition)))
                {
                    ExecuteBlock(w.Body);

                    safetyCounter++;
                    if (safetyCounter > 1000)
                    {
                        throw new Exception("infinite loop detected");
                    }
                }
                break;

            case ExpressionStatement e:
                Evaluate(e.Expression);
                break;
            case MoveStatement m:
                //evaluate the numbers
                object targetObj = Evaluate(m.Target);

                //move target to new position
                if (targetObj is Transform t)
                {
                    int x = (int)Evaluate(m.X);
                    int y = (int)Evaluate(m.Y);
                    int z = (int)Evaluate(m.Z);

                    t.Translate(x, y, z);
                    UnityEngine.Debug.Log($"Moved {t.name} by ({x},{y},{z})");
                }
                else
                {
                    UnityEngine.Debug.LogError($"[Runtime Error] '{m.Target}' is not a valid Game Object!");
                }
                break;
        }
    }

    private void ExecuteBlock(List<Statement> statements)
    {
        foreach (var stmt in statements)
        {
            Execute(stmt);
        }
    }

    private object Evaluate(Expression expr)
    {
        switch (expr)
        {
            case LiteralExpression l: return l.Value;
            case VariableExpression v: return _environment.Get(v.Name);
            case BinaryExpression b: return EvaluateBinary(b);
            default: throw new Exception("Unknown expression.");
        }
    }

    private object EvaluateBinary(BinaryExpression b)
    {
        object left = Evaluate(b.Left);
        object right = Evaluate(b.Right);

        //is it an int
        if (left is int l && right is int r)
        {
            switch (b.Operator)
            {
                case TokenType.Plus: return l + r;
                case TokenType.Minus: return l - r;
                case TokenType.Star: return l * r;
                case TokenType.Slash:
                    if (r == 0) throw new System.Exception("Division by zero");
                    return l / r;
                case TokenType.DoubleEquals: return l == r;
                //add more operators later
                default: throw new System.Exception($"Operator {b.Operator} not supported for Integers");
            }
        }

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

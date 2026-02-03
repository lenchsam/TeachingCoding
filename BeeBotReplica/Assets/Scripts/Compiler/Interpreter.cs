using System;
using System.Collections.Generic;
using UnityEngine;

public class Interpreter : MonoBehaviour
{
    private Environment _environment = new Environment();

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
                        ConsoleManager.Log("Infinite loop detected");
                        throw new Exception("infinite loop detected");
                    }
                }
                break;

            case ExpressionStatement e:
                Evaluate(e.Expression);
                break;
            case MoveStatement m:
                //evaluate the numbers
                object moveTargetObj = _environment.Get("Player");

                //move target to new position
                if (moveTargetObj is Transform mt)
                {
                    int x = (int)Evaluate(m.X);
                    int y = (int)Evaluate(m.Y);

                    mt.Translate(x, 0, y);
                    UnityEngine.Debug.Log($"Moved {mt.name} by ({x},{y})");
                }
                break;
            case MoveToStatement m:
                //evaluate the numbers
                object moveToTargetObj = _environment.Get("Player");

                //move target to new position
                if (moveToTargetObj is Transform mtt)
                {
                    int x = (int)Evaluate(m.X);
                    int y = (int)Evaluate(m.Y);

                    mtt.position = new Vector3(x, 0, y);
                    UnityEngine.Debug.Log($"Moved {mtt.name} by ({x},{y})");
                }
                break;
            case AttackStatement a:
                object attackTargetObj = Evaluate(a.Target);

                if(attackTargetObj is Transform at)
                {
                    //TODO: code for attacking
                    UnityEngine.Debug.Log($"player is attacking {at.name}" );
                }
                else
                {
                    ConsoleManager.Log("Attack target is not valid!");
                    throw new Exception("Attack target is not valid!");
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
            case UnaryExpression u:
                object right = Evaluate(u.Right);

                if(u.Operator.Type == TokenType.Minus)
                {
                    if (right is int r) return -r;
                }
                ConsoleManager.Log("Unknown Operator Type");
                throw new Exception("Unknown Operator Type");
            default:
                ConsoleManager.Log("Unknown expression");
                throw new Exception("Unknown expression.");


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

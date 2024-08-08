namespace Known.Data;

class ExpressionHelper
{
    internal static string TypeCast(ExpressionType type)
    {
        switch (type)
        {
            case ExpressionType.And:
            case ExpressionType.AndAlso:
                return " and ";
            case ExpressionType.Equal:
                return "=";
            case ExpressionType.GreaterThan:
                return ">";
            case ExpressionType.GreaterThanOrEqual:
                return ">=";
            case ExpressionType.LessThan:
                return "<";
            case ExpressionType.LessThanOrEqual:
                return "<=";
            case ExpressionType.NotEqual:
                return "<>";
            case ExpressionType.Or:
            case ExpressionType.OrElse:
                return " or ";
            case ExpressionType.Add:
            case ExpressionType.AddChecked:
                return "+";
            case ExpressionType.Subtract:
            case ExpressionType.SubtractChecked:
                return "-";
            case ExpressionType.Divide:
                return "/";
            case ExpressionType.Multiply:
            case ExpressionType.MultiplyChecked:
                return "*";
            default:
                return null;
        }
    }
}
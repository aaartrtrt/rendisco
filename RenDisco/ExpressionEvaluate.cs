using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RenDisco {
    public static class ExpressionEvaluate {

        /// <summary>
        /// Evaluates a condition expressed as a string.
        /// </summary>
        /// <param name="condition">The condition string.</param>
        /// <returns>Boolean result of the evaluation.</returns>
        public static bool EvaluateCondition(IRuntimeEngine runtime, string condition)
        {
            condition = condition.Trim();

            // Evaluate literal True/False
            if (condition.Equals("True", StringComparison.OrdinalIgnoreCase)) return true;
            if (condition.Equals("False", StringComparison.OrdinalIgnoreCase)) return false;

            // Handle logical NOT
            if (condition.StartsWith("not ")) return !EvaluateCondition(runtime, condition.Substring(4).Trim());

            // Handle and/or logic
            if (condition.Contains(" and ") || condition.Contains(" or ")) return EvaluateBooleanLogic(runtime, condition);

            // Handle variable comparison
            if (Regex.IsMatch(condition, @"[!<>=]+")) return EvaluateComparison(runtime, condition);

            // Check if condition is a variable set to true
            var variableValue = runtime.GetVariable(condition);
            return variableValue != null && !Equals(variableValue, false);
        }

        /// <summary>
        /// Evaluates a comparison expression.
        /// </summary>
        /// <param name="condition">The comparison condition string.</param>
        /// <returns>Boolean result of the comparison.</returns>
        private static bool EvaluateComparison(IRuntimeEngine runtime, string condition)
        {
            var comparisonOperators = new[] { "==", "!=", ">=", "<=", ">", "<" };
            foreach (string op in comparisonOperators)
            {
                int opIndex = condition.IndexOf(op, StringComparison.Ordinal);
                if (opIndex > -1)
                {
                    var left = condition.Substring(0, opIndex).Trim().Trim('\"');
                    var right = condition.Substring(opIndex + op.Length).Trim().Trim('\"');

                    var leftValue = runtime.GetVariable(left) ?? left;
                    var rightValue = runtime.GetVariable(right) ?? right;

                    if (double.TryParse(leftValue.ToString(), out double leftNumber) &&
                        double.TryParse(rightValue.ToString(), out double rightNumber))
                    {
                        return PerformNumericComparison(leftNumber, rightNumber, op);
                    }

                    return PerformStringComparison(leftValue.ToString(), rightValue.ToString(), op);
                }
            }
            return false;
        }

        /// <summary>
        /// Performs a numeric comparison between two values.
        /// </summary>
        /// <param name="left">The numeric value on the left side of the comparison.</param>
        /// <param name="right">The numeric value on the right side of the comparison.</param>
        /// <param name="op">The comparison operator.</param>
        /// <returns>Boolean result of the comparison.</returns>
        private static bool PerformNumericComparison(double left, double right, string op)
        {
            return op switch
            {
                "==" => left == right,
                "!=" => left != right,
                ">" => left > right,
                "<" => left < right,
                ">=" => left >= right,
                "<=" => left <= right,
                _ => false,
            };
        }

        /// <summary>
        /// Performs a string comparison between two values.
        /// </summary>
        /// <param name="left">The string on the left side of the comparison.</param>
        /// <param name="right">The string on the right side of the comparison.</param>
        /// <param name="op">The comparison operator.</param>
        /// <returns>Boolean result of the comparison.</returns>
        private static bool PerformStringComparison(string left, string right, string op)
        {
            int comparisonResult = string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
            return op switch
            {
                "==" => comparisonResult == 0,
                "!=" => comparisonResult != 0,
                ">" => comparisonResult > 0,
                "<" => comparisonResult < 0,
                ">=" => comparisonResult >= 0,
                "<=" => comparisonResult <= 0,
                _ => false,
            };
        }

        /// <summary>
        /// Evaluates boolean logic within a condition string.
        /// </summary>
        /// <param name="condition">The condition string.</param>
        /// <returns>Boolean result of the logic evaluation.</returns>
        private static bool EvaluateBooleanLogic(IRuntimeEngine runtime, string condition)
        {
            var andParts = condition.Split(new[] { " and " }, StringSplitOptions.None);
            var orParts = new List<string>();

            foreach (var part in andParts)
            {
                orParts.AddRange(part.Split(new[] { " or " }, StringSplitOptions.None));
            }

            foreach (var andCondition in andParts)
            {
                if (!EvaluateCondition(runtime, andCondition.Trim()))
                {
                    return false;
                }
            }

            foreach (var orCondition in orParts)
            {
                if (EvaluateCondition(runtime, orCondition.Trim()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
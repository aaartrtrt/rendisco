using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RenDisco
{
    public abstract class Expression
    {
        public abstract string Type { get; }
    }

    public class IndentationExpression: Expression
    {
        public override string Type => "keyword";

        public string Keyword { get; set; }
    }

    public class KeywordExpression : Expression
    {
        public override string Type => "keyword";

        public string Keyword { get; set; }
    }

    public class StringLiteralExpression : Expression
    {
        public override string Type => "string_literal";

        public string Value { get; set; }
    }

    public class NumberLiteralExpression : Expression
    {
        public override string Type => "number_literal";

        public double Value { get; set; }
    }

    public class NonLiteralExpression : Expression
    {
        public override string Type => "non_literal";

        public string Value { get; set; }
    }

    public class ParamPairExpression : Expression
    {
        public override string Type => "param_pair";

        public string? ParamName { get; set; }
        public Expression ParamValue { get; set; }
    }

    public class ParamListExpression : Expression
    {
        public override string Type => "param_list";

        public IList<ParamPairExpression> Params { get; set; } = new List<ParamPairExpression>();
    }

    public class MethodExpression : Expression
    {
        public override string Type => "method";

        public string MethodName { get; set; }
        public ParamListExpression ParamList { get; set; }
    }
}
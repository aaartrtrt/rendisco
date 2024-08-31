using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RenDisco {
    public class Play
    {
        private readonly Play? _parent;
        private readonly IRuntimeEngine _runtime;
        private readonly List<RenpyCommand> _commands;

        public int ProgramCounter { get; set; }

        public Play(IRuntimeEngine runtime, List<RenpyCommand> commands, Play? parent = null)
        {
            _runtime = runtime;
            _commands = commands;
            _parent = parent;
        }

        public void Next(bool returnToParent = false)
        {
            for (int _ = 0; ProgramCounter < _commands.Count; ProgramCounter++)
            {
                var command = _commands[ProgramCounter];
                if (!ExecuteCommand(command))
                    break;
            }

            if (returnToParent && _parent != null) _parent.Next();
        }

        private bool ExecuteCommand(RenpyCommand command)
        {
            if (command is Label label)
            {
                // Nothing to do when encountering a Label itself while executing (can be handled by the parent context)
                // Unless of course... We've inset something onto that label
                if (0 < label.Commands.Count) {
                    var subPlay = new Play(_runtime, label.Commands, this);
                    subPlay.Next();
                }
            }
            else if (command is Dialogue dialogue)
            {
                _runtime.ShowDialogue(dialogue.Character, dialogue.Text);
            }
            else if (command is Scene scene)
            {
                _runtime.ShowImage(scene.Image, scene.Transition);
            }
            else if (command is Show show)
            {
                _runtime.ShowImage(show.Image, show.Transition);
            }
            else if (command is IfCondition ifCondition)
            {
                if (EvaluateCondition(ifCondition.Condition))
                {
                    var subPlay = new Play(_runtime, ifCondition.Content, this);
                    subPlay.Next();
                }
            }
            else if (command is ElifCondition elifCondition)
            {
                if (EvaluateCondition(elifCondition.Condition))
                {
                    var subPlay = new Play(_runtime, elifCondition.Content, this);
                    subPlay.Next();
                }
            }
            else if (command is Jump jump)
            {
                /// TODO: This treats it like a method... be warned.
                var targetLabel = FindLabel(jump.Label);
                if (targetLabel != null)
                {
                    targetLabel?.Next(true);
                }

                // We'll let the Play go nuts
                return false;
            }
            else if (command is Menu menu)
            {
                var choices = new List<string>();
                var responses = new Dictionary<int, List<RenpyCommand>>();

                for (int i = 0; i < menu.Choices.Count; i++)
                {
                    choices.Add(menu.Choices[i].OptionText);
                    responses[i] = menu.Choices[i].Response;
                }

                int selectedChoice = _runtime.ShowChoices(choices);
                var subPlay = new Play(_runtime, responses[selectedChoice], this);
                subPlay.Next();
            }
            else if (command is Define define)
            {
                // Process character definition
                if (define.Value.Contains("Character"))
                {
                    string[] parts = define.Value.Split(new[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
                    string charName = parts[0].Trim('"');

                    if (parts.Length > 1)
                    {
                        string color = parts.Length > 2 ? parts[2].Trim().Substring("color=".Length).Trim('"') : null;
                        _runtime.DefineCharacter(define.Name, color);
                    }
                    else
                    {
                        _runtime.DefineCharacter(define.Name);
                    }
                }
                else
                {
                    _runtime.SetVariable(define.Name, define.Value);
                }
            }
            else
            {
                Console.WriteLine($"Unknown command type encountered: {command.Type}");
            }

            return true;
        }

        private Play? FindLabel(string labelName)
        {
            for (var i = 0; i < _commands.Count; i++)
            {
                var command = _commands[i];
                if (command is Label label && label.Name == labelName)
                {
                    ProgramCounter = i;
                    return this;
                }
            }

            if (_parent == null)
                return null; // Handle as an error or return default/fallback label if appropriate
            else
                return _parent.FindLabel(labelName);
        }

        private bool EvaluateCondition(string condition)
        {
            condition = condition.Trim();

            if (condition.Equals("True", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (condition.Equals("False", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (condition.StartsWith("not "))
            {
                return !EvaluateCondition(condition.Substring(4).Trim());
            }

            if (condition.Contains(" and ") || condition.Contains(" or "))
            {
                return EvaluateBooleanLogic(condition);
            }

            if (Regex.IsMatch(condition, @"[!<>=]+"))
            {
                return EvaluateComparison(condition);
            }

            var variableValue = _runtime.GetVariable(condition);
            return variableValue != null && !Equals(variableValue, false);
        }

        private bool EvaluateComparison(string condition)
        {
            var comparisonOperators = new[] { "==", "!=", ">=", "<=", ">", "<" };
            foreach (string op in comparisonOperators)
            {
                int opIndex = condition.IndexOf(op, StringComparison.Ordinal);
                if (opIndex > -1)
                {
                    var left = condition.Substring(0, opIndex).Trim();
                    var right = condition.Substring(opIndex + op.Length).Trim();

                    var leftValue = _runtime.GetVariable(left) ?? left;
                    var rightValue = _runtime.GetVariable(right) ?? right;

                    if (double.TryParse(leftValue.ToString(), out double leftNumber) && double.TryParse(rightValue.ToString(), out double rightNumber))
                    {
                        return PerformNumericComparison(leftNumber, rightNumber, op);
                    }

                    return PerformStringComparison(leftValue.ToString(), rightValue.ToString(), op);
                }
            }
            return false;
        }

        private bool PerformNumericComparison(double left, double right, string op)
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

        private bool PerformStringComparison(string left, string right, string op)
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

        private bool EvaluateBooleanLogic(string condition)
        {
            var andParts = condition.Split(new[] { " and " }, StringSplitOptions.None);
            var orParts = new List<string>();

            foreach (var part in andParts)
            {
                orParts.AddRange(part.Split(new[] { " or " }, StringSplitOptions.None));
            }

            foreach (var andCondition in andParts)
            {
                if (!EvaluateCondition(andCondition.Trim()))
                {
                    return false;
                }
            }

            foreach (var orCondition in orParts)
            {
                if (EvaluateCondition(orCondition.Trim()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RenDisco {
    /// <summary>
    /// Parses Ren'Py script code and creates a list of RenpyCommand objects to represent the script.
    /// </summary>
    public class ClassicRenpyParser : IRenpyParser
    {
        /// <summary>
        /// Parses a script from a file by file path.
        /// </summary>
        /// <param name="filePath">Path to the Ren'Py script file.</param>
        /// <returns>A list of RenpyCommand objects representing the script.</returns>
        public List<RenpyCommand> ParseFromFile(string filePath)
        {
            string rpyCode = File.ReadAllText(filePath);
            return Parse(rpyCode);
        }

        /// <summary>
        /// Parses Ren'Py script code from a string.
        /// </summary>
        /// <param name="rpyCode">The Ren'Py script code as a string.</param>
        /// <returns>A list of RenpyCommand objects representing the script.</returns>
        public List<RenpyCommand> Parse(string rpyCode)
        {
            List<RenpyCommand> commands = new List<RenpyCommand>();
            string[] lines = Regex.Split(rpyCode, "\r\n|\r|\n");
            Stack<Scope> scopeStack = new Stack<Scope>();

            // Start with the root scope
            scopeStack.Push(new Scope(commands));

            bool insideMultilineString = false;
            string multiLineStringAccumulator = "";
            string? multilineCharacter = null;
            int? initialIndentation = null;

            // Process each line in the script
            foreach (string line in lines)
            {
                // Calculate indentation level and remove leading whitespace
                string trimmedLine = line.TrimStart();
                int rawIndentationLevel = line.Length - trimmedLine.Length;

                // Ignore empty and comment lines outside of multiline strings
                if (!insideMultilineString && (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#")))
                    continue;

                // Initialize initialIndentation if not already set
                initialIndentation ??= rawIndentationLevel;
                int indentationLevel = rawIndentationLevel - initialIndentation.Value;
                scopeStack.Peek().Indentation ??= indentationLevel;

                // Adjust the active scope based on indentation
                AdjustActiveScope(scopeStack, indentationLevel);

                // Process potential multiline strings and adjust scope if needed
                if (insideMultilineString)
                {
                    if (ProcessMultilineString(line, multilineCharacter, ref insideMultilineString, ref multiLineStringAccumulator, scopeStack.Peek()))
                        continue;
                }

                if (ProcessMultilineStringStart(trimmedLine, ref multilineCharacter, ref insideMultilineString, ref multiLineStringAccumulator))
                {
                    continue;
                }
                
                // Process different Ren'Py command types in the line, and adjust scope if needed
                if (ParseLabel(trimmedLine, ref scopeStack, indentationLevel)) continue;
                if (ParseScene(trimmedLine, scopeStack.Peek())) continue;
                if (ParseDefine(trimmedLine, scopeStack.Peek())) continue;
                if (ParseVariableAssignment(trimmedLine, scopeStack.Peek())) continue;
                if (ParseMenu(trimmedLine, ref scopeStack)) continue;
                if (ParseConditionalBlocks(trimmedLine, ref scopeStack)) continue;
                if (ParseJump(trimmedLine, scopeStack.Peek())) continue;
                if (ParsePause(trimmedLine, scopeStack.Peek())) continue;
                if (ParseMusicCommands(trimmedLine, scopeStack.Peek())) continue;
                if (ParseShowHideCommands(trimmedLine, scopeStack.Peek())) continue;
                if (ParseMenuChoices(trimmedLine, scopeStack)) continue;
                if (ParseDialogue(trimmedLine, scopeStack.Peek())) continue;
                if (ParseNarration(trimmedLine, scopeStack.Peek())) continue;
            }

            return commands;
        }
        
        /// <summary>
        /// Adjust the active scope based on indentation level and markers.
        /// </summary>
        /// <param name="scopeStack">The scope stack.</param>
        /// <param name="indentationLevel">The current line indentation level.</param>
        private static void AdjustActiveScope(Stack<Scope> scopeStack, int indentationLevel)
        {
            while (ShouldPopScope(scopeStack, indentationLevel))
            {
                scopeStack.Pop();
            }
        }
        

        #region Scoping and Utility Methods

        private static bool ShouldPopScope(Stack<Scope> scopeStack, int indentationLevel)
        {
            return (indentationLevel < scopeStack.Peek().Indentation && scopeStack.Count > 1) ||
                   (scopeStack.Peek().ScopeHead is Label label && label.Indentation >= indentationLevel);
        }

        private static bool ProcessMultilineString(string line, string? multilineCharacter, ref bool insideMultilineString, ref string multiLineStringAccumulator, Scope currentScope)
        {
            multiLineStringAccumulator += line + "\n";

            if (multilineCharacter != null && line.Trim().EndsWith(multilineCharacter))
            {
                insideMultilineString = false;
                currentScope.Commands.Add(new Dialogue
                {
                    Character = currentScope.LastSpeaker,
                    Text = multiLineStringAccumulator.Trim()
                });
                multilineCharacter = null;
                multiLineStringAccumulator = "";
                return true;
            }

            return false;
        }

        private static bool ProcessMultilineStringStart(string trimmedLine, ref string? multilineCharacter, ref bool insideMultilineString, ref string multiLineStringAccumulator)
        {
            if (trimmedLine.Contains("\"\"\"") || trimmedLine.Contains("'''"))
            {
                multilineCharacter = trimmedLine.Contains("\"\"\"") ? "\"\"\"" : "'''";
                insideMultilineString = true;
                multiLineStringAccumulator = trimmedLine.Substring(trimmedLine.IndexOf(multilineCharacter) + multilineCharacter.Length).Trim() + "\n";
                return true;
            }

            return false;
        }

        private static string ExtractAfter(string input, string keyword)
        {
            int index = input.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
            return index >= 0 ? input.Substring(index + keyword.Length).Trim() : input;
        }

        #endregion

        #region Parsing Command Methods

        /// <summary>
        /// Parses Ren'Py script expressions and returns the appropriate Expression object.
        /// </summary>
        /// <param name="expressionText">The Ren'Py script expression text.</param>
        /// <returns>An Expression object representing the expression.</returns>
        public static Expression ParseExpression(string expressionText)
        {
            string trimmedExpressionText = expressionText.Trim();

            // Check for keywords
            foreach (string keyword in new[] { "if", "elif", "define" })
            {
                if (trimmedExpressionText.StartsWith(keyword))
                {
                    return new KeywordExpression { Keyword = keyword };
                }
            }

            // Check for a string literal or number literal
            if (trimmedExpressionText.StartsWith("\"") || trimmedExpressionText.StartsWith("'''"))
            {
                return ParseStringLiteralExpression(trimmedExpressionText);
            }

            // Handle param list expressions
            if (trimmedExpressionText.StartsWith("(") && trimmedExpressionText.EndsWith(")"))
            {
                return ParseParamListExpression(trimmedExpressionText);
            }

            // Handle method expressions
            if (Regex.IsMatch(trimmedExpressionText, @"\w+\s*\("))
            {
                return ParseMethodExpression(trimmedExpressionText);
            }

            // Check for non-literal values
            if (char.IsLetter(trimmedExpressionText[0]))
            {
                return ParseNonLiteralExpression(trimmedExpressionText);
            }

            throw new ArgumentException($"Unknown expression type encountered: {expressionText}");
        }

        private static StringLiteralExpression ParseStringLiteralExpression(string expressionText)
        {
            return new StringLiteralExpression { Value = expressionText.Trim('\"', '\'') };
        }

        private static NonLiteralExpression ParseNonLiteralExpression(string expressionText)
        {
            return new NonLiteralExpression { Value = expressionText };
        }

        private static ParamListExpression ParseParamListExpression(string expressionText)
        {
            var paramPairs = new List<ParamPairExpression>();
            var paramListText = expressionText.Trim('(', ')');
            var paramElements = paramListText.Split(',');

            foreach (var element in paramElements)
            {
                string[] nameValue = element.Split('=');
                if (nameValue.Length == 2)
                {
                    paramPairs.Add(new ParamPairExpression
                    {
                        ParamName = nameValue[0].Trim(),
                        ParamValue = ParseExpression(nameValue[1])
                    });
                }
                else
                {
                    paramPairs.Add(new ParamPairExpression { ParamValue = ParseExpression(nameValue[0]) });
                }
            }

            return new ParamListExpression { Params = paramPairs };
        }

        private static MethodExpression ParseMethodExpression(string expressionText)
        {
            var methodMatch = Regex.Match(expressionText, @"(\w+)\s*\((.*)\)");
            if (methodMatch.Success && methodMatch.Groups.Count == 3)
            {
                string methodName = methodMatch.Groups[1].Value;
                string paramListText = methodMatch.Groups[2].Value;
                var paramListExpression = ParseParamListExpression(paramListText);

                return new MethodExpression
                {
                    MethodName = methodName,
                    ParamList = paramListExpression as ParamListExpression
                };
            }
            else
            {
                throw new ArgumentException($"Invalid method expression encountered: {expressionText}");
            }
        }

        private static bool ParseLabel(string trimmedLine, ref Stack<Scope> scopeStack, int indentationLevel)
        {
            if (trimmedLine.StartsWith("label "))
            {
                string labelName = ExtractAfter(trimmedLine, "label ").Split(':')[0];
                var label = new Label
                {
                    Name = labelName,
                    Indentation = indentationLevel
                };
                scopeStack.Peek().Commands.Add(label);
                scopeStack.Push(new Scope(label.Commands, label));
                return true;
            }

            return false;
        }

        private static bool ParseScene(string trimmedLine, Scope currentScope)
        {
            if (trimmedLine.StartsWith("scene "))
            {
                string[] parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var scene = new Scene { Image = parts[1] };
                if (parts.Length > 2 && parts[2] == "with")
                {
                    scene.Transition = parts[3];
                }

                currentScope.Commands.Add(scene);
                return true;
            }

            return false;
        }

        private static bool ParseDefine(string trimmedLine, Scope currentScope)
        {
            if (trimmedLine.StartsWith("define "))
            {
                string namePart = trimmedLine.Split('=')[0].Trim();
                string valuePart = ExtractAfter(trimmedLine, "=").Trim();
                

                var defineCmd = new Define
                {
                    Name = ExtractAfter(namePart, "define ").Trim(),
                    Value = valuePart,
                    Definition = ParseMethodExpression(valuePart)
                };
                currentScope.Commands.Add(defineCmd);
                return true;
            }

            return false;
        }

        private static bool ParseVariableAssignment(string trimmedLine, Scope currentScope)
        {
            if (trimmedLine.StartsWith("$ "))
            {
                string namePart = trimmedLine.Split('=')[0].Trim();
                string valuePart = ExtractAfter(trimmedLine, "=");

                var defineCmd = new Define
                {
                    Name = ExtractAfter(namePart, "$ ").Trim(),
                    Value = valuePart
                };
                currentScope.Commands.Add(defineCmd);
                return true;
            }

            return false;
        }

        private static bool ParseMenu(string trimmedLine, ref Stack<Scope> scopeStack)
        {
            if (trimmedLine == "menu:")
            {
                var newMenu = new Menu();
                scopeStack.Peek().Commands.Add(newMenu);
                scopeStack.Push(new Scope(newMenu.Choices.Cast<RenpyCommand>().ToList(), newMenu));
                return true;
            }

            return false;
        }

        private static bool ParseConditionalBlocks(string trimmedLine, ref Stack<Scope> scopeStack)
        {
            if (trimmedLine.StartsWith("if "))
            {
                var ifCondition = new IfCondition
                {
                    Condition = ExtractAfter(trimmedLine, "if ").TrimEnd(':')
                };
                scopeStack.Peek().Commands.Add(ifCondition);
                scopeStack.Push(new Scope(ifCondition.Content, ifCondition));
                return true;
            }

            if (trimmedLine.StartsWith("elif "))
            {
                var elifCondition = new ElifCondition
                {
                    Condition = ExtractAfter(trimmedLine, "elif ").TrimEnd(':')
                };
                scopeStack.Peek().Commands.Add(elifCondition);
                scopeStack.Push(new Scope(elifCondition.Content, elifCondition));
                return true;
            }

            return false;
        }

        private static bool ParseJump(string trimmedLine, Scope currentScope)
        {
            if (trimmedLine.StartsWith("jump "))
            {
                var targetLabel = ExtractAfter(trimmedLine, "jump ");
                currentScope.Commands.Add(new Jump { Label = targetLabel });
                return true;
            }

            return false;
        }

        private static bool ParsePause(string trimmedLine, Scope currentScope)
        {
            if (trimmedLine.StartsWith("pause"))
            {
                var parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                Pause pause = new Pause();

                if (parts.Length > 0)
                    pause.Duration = double.Parse(parts[1]);

                currentScope.Commands.Add(pause);

                return true;
            }

            return false;
        }

        private static bool ParseMusicCommands(string trimmedLine, Scope currentScope)
        {
            if (trimmedLine.StartsWith("play music "))
            {
                string[] parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var playMusic = new PlayMusic { File = parts[2].Trim('"') };
                if (parts.Length > 3 && parts[3] == "fadein")
                {
                    playMusic.FadeIn = double.Parse(parts[4]);
                }
                currentScope.Commands.Add(playMusic);
                return true;
            }

            if (trimmedLine.StartsWith("stop music"))
            {
                var stopMusic = new StopMusic();
                if (trimmedLine.Contains("fadeout"))
                {
                    stopMusic.FadeOut = double.Parse(ExtractAfter(trimmedLine, "fadeout "));
                }
                currentScope.Commands.Add(stopMusic);
                return true;
            }

            return false;
        }

        private static bool ParseShowHideCommands(string trimmedLine, Scope currentScope)
        {
            if (trimmedLine.StartsWith("show "))
            {
                string[] parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var showCommand = new Show { Image = parts[1] };
                if (parts.Length > 2 && parts[2] == "at")
                {
                    showCommand.Position = parts[3];
                }

                if (parts.Length > 4 && parts[4] == "with")
                {
                    showCommand.Transition = parts[5];
                }

                currentScope.Commands.Add(showCommand);
                return true;
            }

            if (trimmedLine.StartsWith("hide "))
            {
                string[] parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var hideCommand = new Hide { Image = parts[1] };
                if (parts.Length > 4 && parts[2] == "with")
                {
                    hideCommand.Transition = parts[3];
                }

                currentScope.Commands.Add(hideCommand);
                return true;
            }

            return false;
        }

        private static bool ParseMenuChoices(string trimmedLine, Stack<Scope> scopeStack)
        {
            if (scopeStack.Peek().ScopeHead is Menu menu &&
                trimmedLine.StartsWith("\"") && trimmedLine.EndsWith(":"))
            {
                string optionText = trimmedLine.Substring(1, trimmedLine.Length - 3);
                var choice = new MenuChoice { OptionText = optionText };
                menu.Choices.Add(choice);
                scopeStack.Push(new Scope(choice.Response, choice));
                return true;
            }

            return false;
        }

        private static bool ParseDialogue(string trimmedLine, Scope currentScope)
        {
            // Detect lines beginning with dialogue quotes or a name followed by dialogue in quotes
            if ((trimmedLine.Contains(" ") && trimmedLine.IndexOf('"') > 0))
            {
                var parts = trimmedLine.Split(new[] { '\"' }, 2, StringSplitOptions.RemoveEmptyEntries);
                string character = parts[0].Trim();
                string text = parts[1].Trim().TrimEnd('\"');
                currentScope.LastSpeaker = character;

                currentScope.Commands.Add(new Dialogue
                {
                    Character = character,
                    Text = text
                });
                return true;
            }

            return false;
        }
        
        private static bool ParseNarration(string trimmedLine, Scope currentScope)
        {
            // Detect lines beginning with dialogue quotes or a name followed by dialogue in quotes
            if (trimmedLine.StartsWith("\""))
            {
                string text = trimmedLine.Trim('\"');

                currentScope.Commands.Add(new Narration
                {
                    Text = text
                });
                return true;
            }

            return false;
        }

        #endregion

        // Scope class for managing nested blocks and commands
        private class Scope
        {
            public List<RenpyCommand> Commands { get; }
            public int? Indentation { get; set; }
            public RenpyCommand? ScopeHead { get; set; }
            public string? LastSpeaker { get; set; }

            public Scope(List<RenpyCommand> commands, RenpyCommand? renpyCommand = null)
            {
                Commands = commands;
                ScopeHead = renpyCommand;
                Indentation = null;
                LastSpeaker = null;
            }
        }
    }
}
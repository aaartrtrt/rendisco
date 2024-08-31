using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RenDisco {
    public class RenpyParser
    {
        public List<RenpyCommand> ParseFromFile(string filePath)
        {
            string rpyCode = File.ReadAllText(filePath);
            return Parse(rpyCode);
        }

        public List<RenpyCommand> Parse(string rpyCode)
        {
            List<RenpyCommand> commands = new List<RenpyCommand>();
            string[] lines = Regex.Split(rpyCode, "\r\n|\r|\n");

            Stack<Scope> scopeStack = new Stack<Scope>();
            scopeStack.Push(new Scope(commands));  // Root scope

            bool insideMultilineString = false;
            string multiLineStringAccumulator = "";
            string? multilineCharacter = null;
            int multilineIndentation;
            int? initialIndentation = null;

            foreach (string line in lines)
            {
                // Get current indentation
                string trimmedLine = line.TrimStart();
                int rawIndentationLevel = line.Length - trimmedLine.Length;

                // Skip comments and empty lines
                if (!insideMultilineString && (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#")))
                    continue;

                // Initialise our scope if we haven't already!
                initialIndentation = initialIndentation ?? rawIndentationLevel;

                // Set our indentation Level
                int indentationLevel = rawIndentationLevel - initialIndentation ?? 0;

                // Initialise our indentation level
                scopeStack.Peek().Indentation = scopeStack.Peek().Indentation ?? indentationLevel;

                // If indentation level decreased, pop scope or if we're a label but we didn't indent
                while (
                    indentationLevel < scopeStack.Peek().Indentation && scopeStack.Count > 1 ||
                    scopeStack.Peek().ScopeHead is Label label && label.Indentation >= indentationLevel
                )
                {
                    scopeStack.Pop();
                }

                // Handle multiline string endings (""" or ''')
                if (insideMultilineString)
                {
                    multiLineStringAccumulator += line + "\n";

                    if (trimmedLine.EndsWith(multilineCharacter))
                    {
                        insideMultilineString = false;
                        scopeStack.Peek().Commands.Add(new Dialogue
                        {
                            Character = scopeStack.Peek().LastSpeaker,
                            Text = multiLineStringAccumulator.Trim()
                        });
                        multilineCharacter = null;
                        multiLineStringAccumulator = "";
                    }
                    continue;
                }

                // Handle multiline string beginnings (""" or ''')
                if (trimmedLine.Contains("\"\"\"") || trimmedLine.Contains("'''"))
                {
                    insideMultilineString = true;
                    multilineCharacter = trimmedLine.Contains("\"\"\"") ? "\"\"\"" : "'''";
                    multiLineStringAccumulator = trimmedLine.Substring(trimmedLine.IndexOf(multilineCharacter) + multilineCharacter.Length).Trim() + "\n";
                    multilineIndentation = indentationLevel;
                    continue;
                }

                // Parsing a Label
                if (trimmedLine.StartsWith("label "))
                {
                    string labelName = trimmedLine.Substring("label ".Length).Split(':')[0].Trim();
                    var label = new Label
                    {
                        Name = labelName,
                        Indentation = indentationLevel
                    };
                    scopeStack.Peek().Commands.Add(label);
                    scopeStack.Push(new Scope(label.Commands, label));
                    continue;
                }

                // Parsing a Scene
                if (trimmedLine.StartsWith("scene "))
                {
                    string[] parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var scene = new Scene { Image = parts[1] };
                    if (parts.Length > 2 && parts[2] == "with")
                    {
                        scene.Transition = parts[3];
                    }
                    scopeStack.Peek().Commands.Add(scene);
                    continue;
                }

                // Parsing a define command for Characters or Variables
                if (trimmedLine.StartsWith("define "))
                {
                    string namePart = trimmedLine.Split("=")[0].Trim();
                    string valuePart = trimmedLine.Substring(trimmedLine.Split("=")[0].Length + 1).Trim();

                    var defineCmd = new Define
                    {
                        Name = namePart.Substring("define ".Length).Trim(),
                        Value = valuePart
                    };
                    scopeStack.Peek().Commands.Add(defineCmd);
                    continue;
                }

                // Parsing a define command for Characters or Variables
                if (trimmedLine.StartsWith("$ "))
                {
                    string namePart = trimmedLine.Split("=")[0].Trim();
                    string valuePart = trimmedLine.Substring(trimmedLine.Split("=")[0].Length + 1).Trim();

                    var defineCmd = new Define
                    {
                        Name = namePart.Substring("$ ".Length).Trim(),
                        Value = valuePart
                    };
                    scopeStack.Peek().Commands.Add(defineCmd);
                    continue;
                }

                // Parsing a Menu
                if (trimmedLine == "menu:")
                {
                    var newMenu = new Menu();
                    scopeStack.Peek().Commands.Add(newMenu);
                    scopeStack.Push(new Scope(newMenu.Choices.Cast<RenpyCommand>().ToList(), newMenu));
                    continue;
                }

                if (trimmedLine.StartsWith("if "))
                {
                    var ifCondition = new IfCondition
                    {
                        Condition = trimmedLine.Substring(3).TrimEnd(':')
                    };
                    scopeStack.Peek().Commands.Add(ifCondition);
                    scopeStack.Push(new Scope(ifCondition.Content, ifCondition));
                    continue;
                }

                if (trimmedLine.StartsWith("elif "))
                {
                    var elifCondition = new ElifCondition
                    {
                        Condition = trimmedLine.Substring(5).TrimEnd(':')
                    };
                    scopeStack.Peek().Commands.Add(elifCondition);
                    scopeStack.Push(new Scope(elifCondition.Content, elifCondition));
                    continue;
                }

                if (trimmedLine.StartsWith("jump "))
                {
                    var targetLabel = trimmedLine.Substring(5).Trim();
                    scopeStack.Peek().Commands.Add(new Jump { Label = targetLabel });
                    continue;
                }

                if (trimmedLine.StartsWith("pause "))
                {
                    var parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    scopeStack.Peek().Commands.Add(new Pause { Duration = double.Parse(parts[1]) });
                    continue;
                }

                if (trimmedLine.StartsWith("play music "))
                {
                    var parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var playMusic = new PlayMusic { File = parts[2].Trim('"') };
                    if (parts.Length > 3 && parts[3] == "fadein")
                    {
                        playMusic.FadeIn = double.Parse(parts[4]);
                    }
                    scopeStack.Peek().Commands.Add(playMusic);
                    continue;
                }

                if (trimmedLine.StartsWith("stop music"))
                {
                    var stopMusic = new StopMusic();
                    if (trimmedLine.Contains("fadeout"))
                    {
                        stopMusic.FadeOut = double.Parse(trimmedLine.Split(' ')[3]);
                    }
                    scopeStack.Peek().Commands.Add(stopMusic);
                    continue;
                }

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
                    scopeStack.Peek().Commands.Add(showCommand);
                    continue;
                }

                if (trimmedLine.StartsWith("hide "))
                {
                    string[] parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var hideCommand = new Hide { Image = parts[1] };
                    if (parts.Length > 4 && parts[2] == "with")
                    {
                        hideCommand.Transition = parts[3];
                    }
                    scopeStack.Peek().Commands.Add(hideCommand);
                    continue;
                }

                // Parsing Menu Choices
                if (
                    scopeStack.Count > 1 && scopeStack.Peek().ScopeHead is Menu menu &&
                    trimmedLine.StartsWith("\"") && trimmedLine.EndsWith(":"))
                {
                    string optionText = trimmedLine.Substring(1, trimmedLine.Length - 3);
                    var choice = new MenuChoice { OptionText = optionText };
                    menu.Choices.Add(choice);
                    scopeStack.Push(new Scope(choice.Response, choice));
                    continue;
                }

                // Parsing Dialogue (Character speaking)
                if (trimmedLine.StartsWith("\"") || ((trimmedLine.Split(' ').Length > 1) ? trimmedLine.Split(' ')[1] : "").StartsWith("\""))
                {
                    var parts = trimmedLine.Split(new[] { '\"' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    string character = parts[0].Trim();
                    string text = parts[1].Trim().TrimEnd('\"');
                    scopeStack.Peek().LastSpeaker = character;  // Store the last speaker
                    scopeStack.Peek().Commands.Add(new Dialogue
                    {
                        Character = character,
                        Text = text
                    });
                    continue;
                }
            }

            return commands;
        }

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
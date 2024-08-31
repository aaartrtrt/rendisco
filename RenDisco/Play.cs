using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RenDisco {
    public class Play
    {
        private readonly Play? _parent;
        private readonly IRuntimeEngine _runtime;
        private readonly List<RenpyCommand> _commands;

        public int ProgramCounter { get; set; }

        /// <summary>
        /// Constructor for the play execution environment.
        /// </summary>
        /// <param name="runtime">The engine that executes the script actions.</param>
        /// <param name="commands">List of commands to execute.</param>
        /// <param name="parent">The parent Play context, used for handling scopes and returns.</param>
        public Play(IRuntimeEngine runtime, List<RenpyCommand> commands, Play? parent = null)
        {
            _runtime = runtime;
            _commands = commands;
            _parent = parent;
        }

        /// <summary>
        /// Execute commands from the current ProgramCounter position.
        /// </summary>
        /// <param name="returnToParent">Specifies if execution should continue in parent context after completing current commands.</param>
        /// <returns>Boolean indicating if execution should continue.</returns>
        public bool Next(bool returnToParent = false, int programCounter = 0)
        {
            // Start Program counter here
            ProgramCounter = programCounter;

            for (; ProgramCounter < _commands.Count; ProgramCounter++)
            {
                var command = _commands[ProgramCounter];
                if (!ExecuteCommand(command))
                    // This means execution was interrupted by a jump or something else.
                    return false;
            }

            if (returnToParent && _parent != null) 
            {
                _parent.Next(true);
            }

            // This means we're free to continue.
            return true;
        }

        /// <summary>
        /// Executes a single command.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>Boolean indicating if execution should continue.</returns>
        private bool ExecuteCommand(RenpyCommand command)
        {
            switch (command)
            {
                case Label label:
                    ExecuteLabel(label);
                    break;
                case Dialogue dialogue:
                    _runtime.ShowDialogue(dialogue.Character, dialogue.Text);
                    break;
                case Scene scene:
                    _runtime.ShowImage(scene.Image, scene.Transition);
                    break;
                case Show show:
                    _runtime.ShowImage(show.Image, show.Transition);
                    break;
                case IfCondition ifCondition:
                    ExecuteConditionalBlock(ifCondition.Condition, ifCondition.Content);
                    break;
                case ElifCondition elifCondition:
                    ExecuteConditionalBlock(elifCondition.Condition, elifCondition.Content);
                    break;
                case Jump jump:
                    ExecuteJump(jump.Label);
                    return false;
                case Menu menu:
                    ExecuteMenu(menu);
                    break;
                case Define define:
                    ExecuteDefine(define);
                    break;
                default:
                    Console.WriteLine($"Unknown command type encountered: {command.Type}");
                    break;
            }
            return true;
        }

        /// <summary>
        /// Executes defined variables or character settings.
        /// </summary>
        /// <param name="define">The define command to execute.</param>
        private void ExecuteDefine(Define define)
        {
            _runtime.ExecuteDefine(define);
        }

        /// <summary>
        /// Display a menu and handle choice consequences.
        /// </summary>
        /// <param name="menu">The menu command containing choices and responses.</param>
        private void ExecuteMenu(Menu menu)
        {
            int selectedChoice = _runtime.ShowChoices(menu.Choices);
            var subPlay = new Play(_runtime, menu.Choices[selectedChoice].Response, this);
            subPlay.Next();
        }

        /// <summary>
        /// Handle commands conditionally.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="content"></param>
        private void ExecuteLabel(Label label)
        {
            var subPlay = new Play(_runtime, label.Commands, this);
            subPlay.Next();
        }

        /// <summary>
        /// Handle commands conditionally.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="content"></param>
        private void ExecuteConditionalBlock(string condition, List<RenpyCommand> content)
        {
            if (EvaluateCondition(condition))
            {
                var subPlay = new Play(_runtime, content, this);
                subPlay.Next();
            }
        }

        /// <summary>
        /// Execute a jump command which modifies the program counter.
        /// </summary>
        /// <param name="labelName">The label name to jump to.</param>
        private void ExecuteJump(string labelName)
        {
            var labelPlay = FindLabel(labelName);
            labelPlay?.Next(returnToParent: true, programCounter: labelPlay.ProgramCounter);
        }

        /// <summary>
        /// Locate a label within the command set.
        /// </summary>
        /// <param name="labelName">The name of the label to find.</param>
        /// <returns>The Play instance associated with the found label, or null if no label found.</returns>
        private Play? FindLabel(string labelName)
        {
            for (var i = 0; i < _commands.Count; i++)
            {
                if (_commands[i] is Label label && label.Name == labelName)
                {
                    ProgramCounter = i;
                    return this;
                }
            }
            return _parent?.FindLabel(labelName);
        }

        /// <summary>
        /// Evaluate a boolean condition.
        /// </summary>
        /// <param name="condition">The condition as a string.</param>
        /// <returns>The result of the condition evaluation.</returns>
        private bool EvaluateCondition(string condition)
        {
            return Evaluate.EvaluateCondition(_runtime, condition);
        }
    }
}
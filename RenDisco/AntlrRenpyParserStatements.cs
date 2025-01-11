using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace RenDisco {
    /// <summary>
    /// Parses Ren'Py script code and creates a list of RenpyCommand objects to represent the script.
    /// </summary>
    public partial class AntlrRenpyParser : RenpyBaseVisitor<object>, IRenpyParser
    {

        public override object VisitLabel_def([NotNull] RenpyParser.Label_defContext context)
        {
            string labelName = context.IDENT().GetText();
            // Deal with arguments
            var label = new Label { Name = labelName /*, Argument = argument*/ };
            if (context.block() != null)
            {
                label.Commands.AddRange((List<RenpyCommand>)Visit(context.block()));
            }
            return label;
        }

        public override object VisitCharacter_def([NotNull] RenpyParser.Character_defContext context)
        {
            string name = context.IDENT().GetText();
            string characterName = context.STRING(0).GetText();
            string stringValue = context.STRING(0).GetText().Trim('"');
            string? color = context.STRING(1)?.GetText().Trim('"');

            return new Define
            {
                Name = name,
                Value = $"Character(\"{stringValue}\"{(color != null ? $", color=\"{color}\"" : "")})",
                Definition = new MethodExpression
                {
                    MethodName = "Character",
                    ParamList = new ParamListExpression
                    {
                        Params = new List<ParamPairExpression>
                        {
                            new ParamPairExpression { ParamValue = new StringLiteralExpression{Value = stringValue}},
                            color != null ? new ParamPairExpression{ParamName = "color", ParamValue = new StringLiteralExpression{Value = color}} : null
                        }.Where(x => x != null).ToList()
                    }
                }
            };
        }


        public override object VisitAssignment([NotNull] RenpyParser.AssignmentContext context)
        {
            string name = context.IDENT().GetText();
            string value = context.expression().GetText();

            return new Define
            {
                Name = name,
                Value = value
            };
        }

        

        public override object VisitScene_def([NotNull] RenpyParser.Scene_defContext context)
        {
            return new Scene { Image = context.IDENT().GetText() };
        }

        public override object VisitPause_def([NotNull] RenpyParser.Pause_defContext context)
        {
            return new Pause { Duration = double.Parse(context.NUMBER().GetText()) };
        }

        public override object VisitPlay_music_def([NotNull] RenpyParser.Play_music_defContext context)
        {
            var playMusic = new PlayMusic { File = context.STRING().GetText().Trim('"') };
            if (context.NUMBER() != null)
            {
                playMusic.FadeIn = float.Parse(context.NUMBER().GetText());
            }
            return playMusic;
        }

        public override object VisitStop_music_def([NotNull] RenpyParser.Stop_music_defContext context)
        {
            var stopMusic = new StopMusic();
            if (context.NUMBER() != null)
            {
                stopMusic.FadeOut = float.Parse(context.NUMBER().GetText());
            }
            return stopMusic;
        }

        public override object VisitJump_def([NotNull] RenpyParser.Jump_defContext context)
        {
            return new Jump { Label = context.IDENT().GetText()};
        }

        /*
        public override object VisitCall_def([NotNull] RenpyParser.Call_defContext context)
        {
            return new Call { Label = context.IDENT().GetText()};
        }
        */

        public override object VisitMenu_def([NotNull] RenpyParser.Menu_defContext context)
        {
            var menu = new Menu();
            foreach (var optionContext in context.menu_option())
            {
                menu.Choices.Add((MenuChoice)Visit(optionContext));
            }
            return menu;
        }

        public override object VisitMenu_option([NotNull] RenpyParser.Menu_optionContext context)
        {
            var choice = new MenuChoice { OptionText = context.STRING().GetText().Trim('"') };
            if (context.block() != null)
            {
                choice.Response.AddRange((List<RenpyCommand>)Visit(context.block()));
            }
            return choice;
        }

        public override object VisitDefault_def([NotNull] RenpyParser.Default_defContext context)
        {
            return new Define { Name = context.IDENT().GetText(), Value = context.expression().GetText() };
        }

        public override object VisitReturn_def([NotNull] RenpyParser.Return_defContext context)
        {
            return new Return();
        }

        public override object VisitDialogue([NotNull] RenpyParser.DialogueContext context)
        {
            return new Dialogue { Character = context.character_ref().IDENT().GetText(), Text = context.STRING().GetText().Trim('"') };
        }

        public override object VisitNarration([NotNull] RenpyParser.NarrationContext context)
        {
            return new Narration { Text = context.STRING().GetText().Trim('"') };
        }

        public override object VisitConditional_block([NotNull] RenpyParser.Conditional_blockContext context)
        {
            var conditional_block = new IfCondition { Condition = context.expression().GetText() };
            conditional_block.Condition = context.expression().GetText();
            conditional_block.Content = (List<RenpyCommand>)Visit(context.block());

            foreach (var elifBlock in context.elif_block())
            {
                conditional_block.ElifConditions.Add((ElifCondition)Visit(elifBlock));
            }

            if (context.else_block() != null)
            {
                conditional_block.ElseConditions = (ElseCondition)Visit(context.else_block());
            }

            return conditional_block;
        }

        public override object VisitElif_block([NotNull] RenpyParser.Elif_blockContext context)
        {
            var elifCondition = new ElifCondition { Condition = context.expression().GetText() };
            elifCondition.Content = (List<RenpyCommand>)Visit(context.block());
            return elifCondition;
        }

        public override object VisitElse_block([NotNull] RenpyParser.Else_blockContext context)
        {
            var elseCondition = new ElseCondition();
            elseCondition.Content =(List<RenpyCommand>)Visit(context.block());
            return elseCondition;
        }

        // ... (Implement other visitor methods for remaining rules like argument, expression if needed)
        public override object VisitArgument([NotNull] RenpyParser.ArgumentContext context)
        {
            return context.expression().GetText();
        }

        public override object VisitExpression([NotNull] RenpyParser.ExpressionContext context)
        {
            return context.GetText();
        }
    }
}
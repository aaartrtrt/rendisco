//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from ./RenDisco/Renpy.g4 by ANTLR 4.13.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="RenpyParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.2")]
[System.CLSCompliant(false)]
public interface IRenpyVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBlock([NotNull] RenpyParser.BlockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement([NotNull] RenpyParser.StatementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.label_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLabel_def([NotNull] RenpyParser.Label_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.character_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCharacter_def([NotNull] RenpyParser.Character_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.scene_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitScene_def([NotNull] RenpyParser.Scene_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.pause_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPause_def([NotNull] RenpyParser.Pause_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.play_music_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPlay_music_def([NotNull] RenpyParser.Play_music_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.stop_music_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStop_music_def([NotNull] RenpyParser.Stop_music_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.jump_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitJump_def([NotNull] RenpyParser.Jump_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.call_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCall_def([NotNull] RenpyParser.Call_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.menu_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMenu_def([NotNull] RenpyParser.Menu_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.menu_option"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMenu_option([NotNull] RenpyParser.Menu_optionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.default_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDefault_def([NotNull] RenpyParser.Default_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.return_def"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReturn_def([NotNull] RenpyParser.Return_defContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.dialogue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDialogue([NotNull] RenpyParser.DialogueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.narration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNarration([NotNull] RenpyParser.NarrationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.character_ref"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCharacter_ref([NotNull] RenpyParser.Character_refContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.aguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAguments([NotNull] RenpyParser.AgumentsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArgument([NotNull] RenpyParser.ArgumentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.conditional_block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitConditional_block([NotNull] RenpyParser.Conditional_blockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.elif_block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitElif_block([NotNull] RenpyParser.Elif_blockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.else_block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitElse_block([NotNull] RenpyParser.Else_blockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.assignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssignment([NotNull] RenpyParser.AssignmentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="RenpyParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression([NotNull] RenpyParser.ExpressionContext context);
}

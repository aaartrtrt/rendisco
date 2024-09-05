using System.Collections.Generic;

namespace RenDisco {
    /// <summary>
    /// Base abstract class for all types of Renpy commands.
    /// </summary>
    public abstract class RenpyCommand
    {
        /// <summary>
        /// Gets the type of command.
        /// </summary>
        public abstract string Type { get; }
    }

    /// <summary>
    /// Represents a label command which acts as a marker for jump commands.
    /// </summary>
    public class Label : RenpyCommand
    {
        public override string Type => "label";
        
        /// <summary>
        /// Gets or sets the name of the label.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the indentation level of the label.
        /// </summary>
        public int Indentation { get; set; }
        
        /// <summary>
        /// Gets or sets the list of commands under this label.
        /// </summary>
        public List<RenpyCommand> Commands { get; set; } = new List<RenpyCommand>();
    }

    /// <summary>
    /// Represents a scene change command, potentially with a transition.
    /// </summary>
    public class Scene : RenpyCommand
    {
        public override string Type => "scene";
        
        /// <summary>
        /// Gets or sets the image to display for the scene.
        /// </summary>
        public string Image { get; set; }
        
        /// <summary>
        /// Gets or sets the transition effect for the scene.
        /// </summary>
        public string Transition { get; set; }
    }

    /// <summary>
    /// Represents a dialogue command where a character speaks.
    /// </summary>
    public class Dialogue : RenpyCommand
    {
        public override string Type => "dialogue";
        
        /// <summary>
        /// Gets or sets the character speaking the dialogue.
        /// </summary>
        public string Character { get; set; }
        
        /// <summary>
        /// Gets or sets the dialogue text.
        /// </summary>
        public string Text { get; set; }
    }

    /// <summary>
    /// Represents a menu command with multiple choices for the player.
    /// </summary>
    public class Menu : RenpyCommand
    {
        public override string Type => "menu";
        
        /// <summary>
        /// Gets or sets the list of choices in the menu.
        /// </summary>
        public List<MenuChoice> Choices { get; set; } = new List<MenuChoice>();
    }

    /// <summary>
    /// Represents a single choice within a menu.
    /// </summary>
    public class MenuChoice : RenpyCommand
    {
        public override string Type => "menu_choice";
        
        /// <summary>
        /// Gets or sets the text for the menu choice.
        /// </summary>
        public string OptionText { get; set; }
        
        /// <summary>
        /// Gets or sets the list of commands to execute if this choice is selected.
        /// </summary>
        public List<RenpyCommand> Response { get; set; } = new List<RenpyCommand>();
    }

    /// <summary>
    /// Represents an if-condition command to execute commands based on a condition.
    /// </summary>
    public class IfCondition : RenpyCommand
    {
        public override string Type => "if";
        
        /// <summary>
        /// Gets or sets the condition to evaluate.
        /// </summary>
        public string Condition { get; set; }
        
        /// <summary>
        /// Gets or sets the list of commands to execute if the condition is true.
        /// </summary>
        public List<RenpyCommand> Content { get; set; } = new List<RenpyCommand>();
    }

    /// <summary>
    /// Represents an elif-condition command to execute commands based on a condition (part of if-elif-else chain).
    /// </summary>
    public class ElifCondition : RenpyCommand
    {
        public override string Type => "elif";
        
        /// <summary>
        /// Gets or sets the condition to evaluate.
        /// </summary>
        public string Condition { get; set; }
        
        /// <summary>
        /// Gets or sets the list of commands to execute if the condition is true.
        /// </summary>
        public List<RenpyCommand> Content { get; set; } = new List<RenpyCommand>();
    }

    /// <summary>
    /// Represents a define command to declare characters or variables.
    /// </summary>
    public class Define : RenpyCommand
    {
        public override string Type => "define";
        
        /// <summary>
        /// Gets or sets the ID of the definition.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Gets or sets the name or key of the definition.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the value to be defined.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the right hand value to be defined.
        /// </summary>
        public MethodExpression? Definition { get; set; }
    }

    /// <summary>
    /// Represents a command to play music with an optional fade-in effect.
    /// </summary>
    public class PlayMusic : RenpyCommand
    {
        public override string Type => "play_music";
        
        /// <summary>
        /// Gets or sets the file path or identifier for the music file to play.
        /// </summary>
        public string File { get; set; }
        
        /// <summary>
        /// Gets or sets the duration of the fade-in effect.
        /// </summary>
        public double? FadeIn { get; set; }
    }

    /// <summary>
    /// Represents a command to stop playing music with an optional fade-out effect.
    /// </summary>
    public class StopMusic : RenpyCommand
    {
        public override string Type => "stop_music";
        
        /// <summary>
        /// Gets or sets the duration of the fade-out effect.
        /// </summary>
        public double? FadeOut { get; set; }
    }

    /// <summary>
    /// Represents a command to show an image on the screen.
    /// </summary>
    public class Show : RenpyCommand
    {
        public override string Type => "show";
        
        /// <summary>
        /// Gets or sets the image to be shown.
        /// </summary>
        public string Image { get; set; }
        
        /// <summary>
        /// Gets or sets the position of the image on the screen.
        /// </summary>
        public string Position { get; set; }
        
        /// <summary>
        /// Gets or sets the transition effect for showing the image.
        /// </summary>
        public string Transition { get; set; }
    }

    /// <summary>
    /// Represents a command to hide an image from the screen.
    /// </summary>
    public class Hide : RenpyCommand
    {
        public override string Type => "hide";
        
        /// <summary>
        /// Gets or sets the image to be hidden.
        /// </summary>
        public string Image { get; set; }
        
        /// <summary>
        /// Gets or sets the transition effect for hiding the image.
        /// </summary>
        public string Transition { get; set; }
    }

    /// <summary>
    /// Represents a pause command to pause the execution for a specified duration.
    /// </summary>
    public class Pause : RenpyCommand
    {
        public override string Type => "pause";
        
        /// <summary>
        /// Gets or sets the duration of the pause in seconds.
        /// </summary>
        public double Duration { get; set; }
    }

    /// <summary>
    /// Represents a jump command to jump to a specified label.
    /// </summary>
    public class Jump : RenpyCommand
    {
        public override string Type => "jump";
        
        /// <summary>
        /// Gets or sets the label name to jump to.
        /// </summary>
        public string Label { get; set; }
    }
}
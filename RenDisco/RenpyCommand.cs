using System.Collections.Generic;

namespace RenDisco {
    public abstract class RenpyCommand
    {
        public abstract string Type { get; }
    }

    public class Label : RenpyCommand
    {
        public override string Type => "label";
        public string Name { get; set; }
        public int Indentation { get; set; }
        public List<RenpyCommand> Commands { get; set; } = new List<RenpyCommand>();
    }

    public class Scene : RenpyCommand
    {
        public override string Type => "scene";
        public string Image { get; set; }
        public string Transition { get; set; }
    }

    public class Dialogue : RenpyCommand
    {
        public override string Type => "dialogue";
        public string Character { get; set; }
        public string Text { get; set; }
    }

    public class Menu : RenpyCommand
    {
        public override string Type => "menu";
        public List<MenuChoice> Choices { get; set; } = new List<MenuChoice>();
    }

    public class MenuChoice : RenpyCommand
    {
        public override string Type => "menu_choice";
        public string OptionText { get; set; }
        public List<RenpyCommand> Response { get; set; } = new List<RenpyCommand>();
    }

    public class IfCondition : RenpyCommand
    {
        public override string Type => "if";
        public string Condition { get; set; }
        public List<RenpyCommand> Content { get; set; } = new List<RenpyCommand>();
    }

    public class ElifCondition : RenpyCommand
    {
        public override string Type => "elif";
        public string Condition { get; set; }
        public List<RenpyCommand> Content { get; set; } = new List<RenpyCommand>();
    }

    public class Define : RenpyCommand
    {
        public override string Type => "define";
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class PlayMusic : RenpyCommand
    {
        public override string Type => "play_music";
        public string File { get; set; }
        public double? FadeIn { get; set; }
    }

    public class StopMusic : RenpyCommand
    {
        public override string Type => "stop_music";
        public double? FadeOut { get; set; }
    }

    public class Show : RenpyCommand
    {
        public override string Type => "show";
        public string Image { get; set; }
        public string Position { get; set; }
        public string Transition { get; set; }
    }

    public class Hide : RenpyCommand
    {
        public override string Type => "hide";
        public string Image { get; set; }
        public string Transition { get; set; }
    }

    public class Pause : RenpyCommand
    {
        public override string Type => "pause";
        public double Duration { get; set; }
    }

    public class Jump : RenpyCommand
    {
        public override string Type => "jump";
        public string Label { get; set; }
    }
}
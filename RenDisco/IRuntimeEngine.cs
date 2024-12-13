using System.Collections.Generic;

namespace RenDisco {
    public interface IRuntimeEngine
    {
        // Method to display dialogue for a specific character
        void ShowDialogue(string? character, string dialogue);

        // Method to display narration (no character)
        void ShowNarration(string narration);

        // Method to show an image with optional transition
        void ShowImage(string image, string? transition = null);

        //Method to hide image with optional transition
        void HideImage(string image, string? transition = null);

        //Method to play music
        void PlayMusic(string file, double? fadein);

        //Method to stop music
        void StopMusic(double? fadeout);

        //Method to pause
        void Pause(double? duration);

        // Method to show choices to the user, and return the selected index
        void ShowChoices(List<MenuChoice> choices);

        // Method to store, define and get Characters
        void DefineCharacter(string id, Dictionary<string, string?> settings);
        // Method to retrieve character details (could be enhanced to return Character object)
        string? GetCharacterColour(string id);
        // Method to retrieve character name (could be enhanced to return Character object)
        string? GetCharacterName(string id);

        // Method to store, define and get variables
        void SetVariable(string name, object value);

        object? GetVariable(string name);

        // Method to handle the execution of Define commands directly
        void ExecuteDefine(Define define);
    }
}
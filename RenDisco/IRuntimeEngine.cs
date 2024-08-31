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

        // Method to show choices to the user, and return the selected index
        int ShowChoices(List<string> choices);

        // Method to store, define and get Characters
        void DefineCharacter(string name, string? colour = null);
        // Method to retrieve character details (could be enhanced to return Character object)
        string? GetCharacterColour(string name);

        // Method to store, define and get variables
        void SetVariable(string name, object value);
        object? GetVariable(string name);
    }
}
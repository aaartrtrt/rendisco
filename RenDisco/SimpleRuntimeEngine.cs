using System;
using System.Collections.Generic;

namespace RenDisco {
    public class SimpleRuntimeEngine : IRuntimeEngine
    {
        private Dictionary<string, object> _variables = new Dictionary<string, object>();
        private Dictionary<string, Dictionary<string, string>> _characters = 
            new Dictionary<string, Dictionary<string, string>>();

        public void ShowDialogue(string? character, string dialogue)
        {
            if (character == null) Console.WriteLine($"{dialogue}");
            else Console.WriteLine($"{character}: {dialogue}");
        }

        public void ShowNarration(string narration)
        {
            Console.WriteLine($"Narration: {narration}");
        }

        public void ShowImage(string image, string? transition = null)
        {
            if (transition != null)
                Console.WriteLine($"Show Image: {image} with {transition} transition");
            else
                Console.WriteLine($"Show Image: {image}");
        }

        public int ShowChoices(List<string> choices)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {choices[i]}");
            }

            Console.Write("Choose an option: ");
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > choices.Count)
            {
                Console.WriteLine("Invalid choice! Please select a valid option.");
            }
            return choice - 1;
        }

        public void DefineCharacter(string id, string name, string? colour = null)
        {
            if (!_characters.ContainsKey(id))
            {
                Dictionary<string, string> characterDict = new Dictionary<string, string>();
                characterDict["name"] = name;
                characterDict["colour"] = colour ?? "#ffffff";
                _characters[id] = characterDict;
            }
        }
        
        public string? GetCharacterName(string id)
        {
            return _characters.ContainsKey(id) ? _characters[id]["name"] : null;
        }

        public string? GetCharacterColour(string id)
        {
            return _characters.ContainsKey(id) ? _characters[id]["colour"] : null;
        }

        public void SetVariable(string name, object value)
        {
            _variables[name] = value;
        }

        public object? GetVariable(string name)
        {
            return _variables.ContainsKey(name) ? _variables[name] : null;
        }
    }
}
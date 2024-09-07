using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RenDisco {
    public class BaseRuntimeEngine : IRuntimeEngine
    {
        private Dictionary<string, object> _variables = new Dictionary<string, object>();
        private Dictionary<string, Dictionary<string, string?>> _characters = new Dictionary<string, Dictionary<string, string?>>();

        /// <summary>
        /// Displays dialogue for a specific character or as narration if no character is provided.
        /// </summary>
        /// <param name="character">The character speaking the dialogue.</param>
        /// <param name="dialogue">The dialogue text.</param>
        public void ShowDialogue(string? character, string dialogue)
        {
            string id = character ?? "";
            Console.WriteLine(_characters[id] == null ? dialogue : $"{_characters[id]["name"]}: {dialogue}");
        }

        /// <summary>
        /// Displays a narration text.
        /// </summary>
        /// <param name="narration">The narration text.</param>
        public void ShowNarration(string narration)
        {
            Console.WriteLine($"Narration: {narration}");
        }

        /// <summary>
        /// Displays an image with an optional transition.
        /// </summary>
        /// <param name="image">The path or identifier for the image.</param>
        /// <param name="transition">The transition effect to use.</param>
        public void ShowImage(string image, string? transition = null)
        {
            Console.WriteLine(transition == null ? $"Show Image: {image}" : $"Show Image: {image} with {transition} transition");
        }

        /// <summary>
        /// Displays choices to the user and returns the index of the selected choice.
        /// </summary>
        /// <param name="choices">List of choice texts.</param>
        /// <returns>The index of the selected choice.</returns>
        public void ShowChoices(List<MenuChoice> choices)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {choices[i].OptionText}");
            }
        }

        /// <summary>
        /// Defines a character with an optional color parameter.
        /// </summary>
        /// <param name="id">The unique identifier for the character.</param>
        /// <param name="name">The display name of the character.</param>
        /// <param name="colour">The optional color associated with the character.</param>
        public void DefineCharacter(string id, Dictionary<string, string?> settings)
        {
            _characters[id] = settings;
        }

        /// <summary>
        /// Retrieves the settings of a character by its identifier.
        /// </summary>
        /// <param name="id">The character's unique identifier.</param>
        /// <returns>The character's name, or null if not found.</returns>
        public Dictionary<string, string?>? GetCharacter(string id)
        {
            return _characters.ContainsKey(id) ? _characters[id] : null;
        }

        /// <summary>
        /// Retrieves the name of a character by its identifier.
        /// </summary>
        /// <param name="id">The character's unique identifier.</param>
        /// <returns>The character's name, or null if not found.</returns>
        public string? GetCharacterName(string id)
        {
            return _characters.ContainsKey(id) ? _characters[id]["name"] : null;
        }

        /// <summary>
        /// Retrieves the color associated with a character by its identifier.
        /// </summary>
        /// <param name="id">The character's unique identifier.</param>
        /// <returns>The character's color, or null if not found.</returns>
        public string? GetCharacterColour(string id)
        {
            return _characters.ContainsKey(id) ? _characters[id]["colour"] : null;
        }

        /// <summary>
        /// Sets a variable by name to a given value.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <param name="value">The value to set.</param>
        public void SetVariable(string name, object value)
        {
            _variables[name] = value;
        }

        /// <summary>
        /// Retrieves the value of a variable by its name.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <returns>The value of the variable, or null if not found.</returns>
        public object? GetVariable(string name)
        {
            return _variables.ContainsKey(name) ? _variables[name] : null;
        }

        /// <summary>
        /// Handles execution of Define commands directly.
        /// </summary>
        /// <param name="define">The Define command to process.</param>
        public void ExecuteDefine(Define define)
        {
            if (define.Definition?.MethodName == "Character")
            {
                Dictionary<string, string?> characterSettings = new Dictionary<string, string?>();

                foreach (ParamPairExpression paramPair in define.Definition.ParamList.Params)
                {
                    switch (paramPair.ParamName) {
                        case "name":
                        case "color":
                        case "portrait":
                            characterSettings[paramPair.ParamName] = ((StringLiteralExpression)paramPair.ParamValue).Value;
                            break;
                        default:
                            if (paramPair.ParamValue is StringLiteralExpression)
                                characterSettings["name"] = ((StringLiteralExpression)paramPair.ParamValue).Value;
                            break;
                    }
                }

                if (characterSettings.ContainsKey("name")) {
                    DefineCharacter(
                        define.Name,
                        characterSettings
                    );
                }
            }
            else
            {
                SetVariable(define.Name, define.Value.Trim('"'));
            }
        }

        /// <summary>
        /// Extracts the string within quotes for parsing purposes.
        /// </summary>
        /// <param name="text">The text containing the quoted string.</param>
        /// <returns>The extracted string.</returns>
        private string ExtractStringWithinQuotes(string text)
        {
            var match = Regex.Match(text, "\"([^\"]*)\"");
            if (match.Success) return match.Groups[1].Value;
            throw new ArgumentException("Provided string does not contain quotes or valid quoted text.");
        }
    }
}
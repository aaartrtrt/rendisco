using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RenDisco;

namespace RenDisco.Test
{
    public class MockRuntimeEngine : IRuntimeEngine
    {
        public List<string> DialogueLog { get; private set; } = new List<string>();
        public List<string> NarrationLog { get; private set; } = new List<string>();
        public List<string> ImageLog { get; private set; } = new List<string>();
        public List<Dictionary<string, string>> VariableState { get; private set; } = new List<Dictionary<string, string>>();

        private Dictionary<string, object> _variables = new Dictionary<string, object>();
        private Dictionary<string, Dictionary<string, string>> _characters =
            new Dictionary<string, Dictionary<string, string>>();

        private int _choiceIndex = 0;

        public void ShowDialogue(string character, string dialogue)
        {
            DialogueLog.Add($"{GetCharacterName(character)}: {dialogue}");
        }

        public void ShowNarration(string narration)
        {
            NarrationLog.Add(narration);
        }

        public void ShowImage(string image, string transition = null)
        {
            ImageLog.Add($"{image}{(transition != null ? " with " + transition : "")}");
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
            VariableState.Add(new Dictionary<string, string> { { name, value.ToString() } });
        }

        public object GetVariable(string name)
        {
            return _variables.ContainsKey(name) ? _variables[name] : null;
        }

        public void Reset()
        {
            _choiceIndex = 0;
            _variables.Clear();
            VariableState.Clear();
            DialogueLog.Clear();
            NarrationLog.Clear();
            ImageLog.Clear();
        }

        public void ShowChoices(List<MenuChoice> choices)
        {
        }

        public void ExecuteDefine(Define define)
        {
            if (define.Value.Contains("Character"))
            {
                string text = define.Value;
                string characterName = ExtractStringWithinQuotes(text.Substring(text.IndexOf('(') + 1));
                if (text.Contains("color"))
                {
                    string color = ExtractStringWithinQuotes(text.Substring(text.IndexOf("color=") + 6));
                    DefineCharacter(define.Name, characterName, color);
                }
                else
                {
                    DefineCharacter(define.Name, characterName);
                }
            }
            else
            {
                SetVariable(define.Name, define.Value.Trim('"'));
            }
        }

        private string ExtractStringWithinQuotes(string text)
        {
            var match = Regex.Match(text, "\"([^\"]*)\"");
            if (match.Success) return match.Groups[1].Value;
            throw new ArgumentException("Provided string does not contain quotes or valid quoted text.");
        }

        public void DefineCharacter(string id, Dictionary<string, string?> settings)
        {
            throw new NotImplementedException();
        }
    }
}
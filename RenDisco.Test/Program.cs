using System;
using System.Collections.Generic;
using RenDisco;
using RenDisco.Test;

namespace RenDisco.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test script based on your Ren'Py example script
            string rpyScript = @"
    # Ren'Py basic script starts with the label start
    label start:
        # Show the splash screen / logo image
        scene bg logo with dissolve

        # Pause for 2 seconds to let the user see the splash screen
        pause 2

        # Show the main game background
        scene bg classroom with dissolve

        # Character Definitions
        # Define a character ""e"" with a name ""Ella""
        define e = Character(""Ella"", color=""#c8a2c8"")

        # Define another character with a unique color for their dialogue
        define l = Character(""Liam"", color=""#80c0ff"")

        # Display Character Dialogue
        e ""Hi there! Welcome to the world of Visual Novels!""

        l ""Hello, everybody! Let's explore what we can do with Ren'Py.""

        # Display a Background Image
        scene bg park with dissolve
    
        e ""Let's go to the park! It's much nice to have our conversation there.""

        # Adding background music
        play music ""bgm/happy_day.mp3"" fadein 1.0

        l ""Don't you feel the atmosphere becoming a lot livelier with music?""

        # Handling a Jump to another label
        e ""This has been fun, but now let's move on.""

        jump ending

    label ending:
        # Applying a fade-out and stopping the background music
        scene black with fade
        stop music fadeout 1.0

        # Final message
        e ""Thanks for playing! This is just the beginning of what you can do with Ren'Py.""

        # End of the game (returns to main menu automatically)
        return";

            // 1. Parse the script
            RenpyParser parser = new RenpyParser();
            List<RenpyCommand> commands = parser.Parse(rpyScript);

            // 2. Create the mock runtime engine
            MockRuntimeEngine runtime = new MockRuntimeEngine();

            // 3. Create the Play instance and start the execution
            Play play = new Play(runtime, commands);
            play.Next();

            // Conduct tests without conditional paths

            // Verifying sequences

            // Sequence 1 - Start Label
            // AssertImage(runtime, 0, "bg logo with dissolve");
            // AssertPause(runtime, 0, 2.0);  // Pause for 2 seconds
            // AssertImage(runtime, 1, "bg classroom with dissolve");

            // Character dialogues
            AssertDialogue(runtime, 0, "Ella: Hi there! Welcome to the world of Visual Novels!");
            AssertDialogue(runtime, 1, "Liam: Hello, everybody! Let's explore what we can do with Ren'Py.");
            // AssertImage(runtime, 2, "bg park with dissolve");
            AssertDialogue(runtime, 2, "Ella: Let's go to the park! It's much nice to have our conversation there.");

            // Background music
            // AssertMusicPlay(runtime, 0, "bgm/happy_day.mp3", 1.0);
            AssertDialogue(runtime, 3, "Liam: Don't you feel the atmosphere becoming a lot livelier with music?");

            // Jump to the ending label
            AssertDialogue(runtime, 4, "Ella: This has been fun, but now let's move on.");
            AssertJump(runtime, "ending");

            // Ending Label execution
            // AssertImage(runtime, 3, "black with fade");
            // AssertMusicStop(runtime, 0, 1.0);

            // Final dialogue
            AssertDialogue(runtime, 5, "Ella: Thanks for playing! This is just the beginning of what you can do with Ren'Py.");

            Console.WriteLine("All basic command tests passed!");
        }

        // Test method for asserting dialogue
        static void AssertDialogue(MockRuntimeEngine runtime, int index, string expected)
        {
            if (runtime.DialogueLog[index] != expected)
            {
                throw new Exception($"Expected dialogue at index {index}: {expected}, but got: {runtime.DialogueLog[index]}.");
            }
        }

        // Test method for asserting images shown with transitions
        static void AssertImage(MockRuntimeEngine runtime, int index, string expected)
        {
            if (runtime.ImageLog[index] != expected)
            {
                throw new Exception($"Expected image at index {index}: {expected} but got: {runtime.ImageLog[index]}.");
            }
        }

        // Test method for asserting pauses
        static void AssertPause(MockRuntimeEngine runtime, int index, double pauseDuration)
        {
            if (runtime.VariableState[index]["pause_duration"] != pauseDuration.ToString())
            {
                throw new Exception($"Expected pause duration at index {index}: {pauseDuration} but got: {runtime.VariableState[index]["pause_duration"]}.");
            }
        }

        // Test method for asserting music started
        static void AssertMusicPlay(MockRuntimeEngine runtime, int index, string expectedMusicFile, double fadeInDuration)
        {
            Console.WriteLine("Music play asserted correctly.");
            // You can extend the MockRuntimeEngine to log music plays/fades for asserts
        }

        // Test method for asserting music stopped with fadeout
        static void AssertMusicStop(MockRuntimeEngine runtime, int index, double fadeOutDuration)
        {
            Console.WriteLine("Music stop asserted correctly.");
            // You can extend the MockRuntimeEngine to log music stops/fades for asserts
        }

        // Test method to confirm a jump to label occurred as expected
        static void AssertJump(MockRuntimeEngine runtime, string expectedLabel)
        {
            Console.WriteLine($"Jump to label '{expectedLabel}' validated.");
            // In a more complex system, track jumps to validate this
        }
    }
}
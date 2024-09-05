using System;
using System.Collections.Generic;
using RenDisco;

class Program
{
    static void Main(string[] args)
    {
        string code = @"
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

    # Displaying Choices to the Player
    menu:
        ""Yes, I love it!"":
            e ""I'm glad you like it!""
            $ mood = ""happy""
        ""No, I prefer silence."":
            e ""Oh, maybe we'll turn it off, then.""
            $ mood = ""neutral""

    # Conditional Statements
    if mood == ""happy"":
        l ""It's so nice to see everyone enjoying themselves.""

        e ""Let's do something fun in this lively place!""

        # Show an expression change of the character
        show ella happy at left with moveinleft

        e ""Look! I'm so happy now!""

    elif mood == ""neutral"":
        l ""Alright, to each their own.""

        e ""Let's tone things down and keep it quiet then.""

        # Use a transition effect (e.g., cross dissolve into new image)
        scene bg classroom with crossfade

        l ""Hmm, isn't it better to have a quiet time indoors?""

        jump ending

    # Adding a screen displayable
    show text ""One Week Later..."" at truecenter with dissolve
    pause 1

    # Removing the text
    hide text with dissolve

    # Using transitions again
    scene bg bedroom with dissolve

    # # Creating a simple imagebutton
    # screen my_button:
    #     imagebutton:
    #         idle ""images/myButtonIdle.png""
    #         hover ""images/myButtonHover.png""
    #         action Show(""my_screen"")

    # Display the button on the main screen
    show screen my_button

    e ""Click that button to see something special.""

    # # Custom screen to be shown on button click
    # screen my_screen:
    #     zorder 100
    #     modal True
    #
    #     frame:
    #         background ""images/fancy_frame.png""
    #         xalign 0.5
    #         yalign 0.5
    #         vbox:
    #             label ""This is a Special Message!""
    #             text ""Ren'Py is really versatile isn't it?""
    #             textbutton ""Close"" action Hide(""my_screen"")

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
        List<RenpyCommand> commands = parser.Parse(code);

        // 2. Create the runtime engine
        IRuntimeEngine runtime = new SimpleRuntimeEngine();

        // 3. Define variables or characters in the runtime engine
        runtime.SetVariable("playerName", "John");   // Example variable

        // 4. Create the Play instance and start the execution
        Play play = new Play(runtime, commands);

        while (true)
        {
            // Check if we need to read a choice from the user
            if (play.WaitingForInput)
            {
                Console.Write("> ");
                int.TryParse(Console.ReadLine(), out int userChoice);

                // Create a StepContext with the user's choice loaded
                StepContext stepContext = new StepContext(userChoice - 1);
                play.Step(stepContext: stepContext);
            }
            else
            {
                Console.WriteLine("-");
                play.Step();
            }
        }
    }
}
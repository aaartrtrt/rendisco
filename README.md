# RenDisco 🪩🕺📖

[![.NET](https://github.com/aaartrtrt/rendisco/actions/workflows/dotnet.yml/badge.svg)](https://github.com/aaartrtrt/rendisco/actions/workflows/dotnet.yml)

RenDisco is a project for .NET to parse and execute scripts written in a subset of Ren'Py - the popular engine for creating visual novels. It allows the integration of Ren'Py-like scripts within C# applications, allowing access to the .NET ecosystem while utilising the easy-to-learn syntax of Ren'Py to create dialogue.

## Features ✨

- **Parsing Support**: Parse Ren'Py-like scripts directly from C#.
- **Execute Commands**: Seamlessly execute parsed commands using a runtime engine.

## Getting Started ✍️

To get started with RenDisco, clone this repository and build the solution in your preferred .NET environment.

### Prerequisites 📦

- .NET Core 3.1 or higher

### Installation 🔧

1. Clone the repository:
   ```bash
   git clone https://github.com/aaartrtrt/RenDisco.git
   ```
2. Navigate to the cloned directory and build the project:
   ```bash
   cd RenDisco
   dotnet build
   ```

See [Documentation/Renpy Parser via ANTLR4.md](Documentation/Renpy Parser via ANTLR4.md) for notes on updating the language grammar.

### Usage 🏗️

Below is a simple example of how to use RenPySharp in your project:

1. **Import RenDisco**:

```cs
using RenDisco;
```

2. **Prepare your script**: Write your Ren'Py script as a string or load it from a file.

```cs
string script = @"
label start:
    e ""Hello, world!""
    jump finish

label finish:
    e ""Goodbye, world!""
    return
";
```

3. **Parse the script**:
   
```cs
Rendisco.IRenpyParser parser = new Rendisco.AntlrRenpyParser();
Rendisco.List<Rendisco.Command> commands = parser.Parse(code);
```

4. **Set up the runtime engine**:

```cs
Rendisco.IRuntimeEngine runtime = new ConsoleRuntimeEngine();
```

5. **Do a Step loop**:

```cs
while (true)
{
    // Check if we need to read a choice from the user
    if (play.WaitingForInput)
    {
        Console.Write("> ");
        int.TryParse(Console.ReadLine(), out int userChoice);

        // Create a StepContext with the user's choice loaded
        InputContext stepContext = new InputContext(userChoice - 1);
        play.Step(stepContext: stepContext);
    }
    else
    {
        Console.WriteLine("-");
        play.Step();
    }
}
```

## License 📝

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Based on the work by the [Ren'Py Visual Novel Engine](https://www.renpy.org/).

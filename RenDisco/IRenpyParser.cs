using System.Collections.Generic;

namespace RenDisco {
    /// <summary>
    /// Parses Ren'Py script code and creates a list of RenpyCommand objects to represent the script.
    /// </summary>
    public interface IRenpyParser {
        /// <summary>
        /// Parses a script from a file by file path.
        /// </summary>
        /// <param name="filePath">Path to the Ren'Py script file.</param>
        /// <returns>A list of RenpyCommand objects representing the script.</returns>
        public List<Command> ParseFromFile(string filePath);
        
        /// <summary>
        /// Parses Ren'Py script code from a string.
        /// </summary>
        /// <param name="rpyCode">The Ren'Py script code as a string.</param>
        /// <returns>A list of RenpyCommand objects representing the script.</returns>
        public List<Command> Parse(string rpyCode);
    }
}
using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager
{
    internal static class Utilities
    {
        public static void CopyFileAndReplaceValues(string inputDir, string inputFile, string outputDir, string outputFile, Dictionary<string, string> variables)
        {
            try
            {
                // Read the content of the source file
                string fileContent = File.ReadAllText(inputDir + inputFile);

                // Replace <variables> with the new value
                foreach (var variable in variables)
                {
                    fileContent = fileContent.Replace(variable.Key, variable.Value);
                }

                // Create the destination directory if it doesn't exist
                Directory.CreateDirectory(outputDir);

                // Combine the destination directory and the file name
                string destinationFilePath = Path.Combine(outputDir, outputFile);

                // Write the modified content to the destination file
                File.WriteAllText(destinationFilePath, fileContent);

                Console.WriteLine("File copied and modified successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}

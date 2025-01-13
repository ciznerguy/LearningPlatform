using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace LearningClassLibrary.Services
{
    public class CodeExecutionService
    {
        public async Task<string> ExecuteUserCode(string userCode)
        {
            try
            {
                Console.WriteLine("Executing user code...");

                StringWriter sw = new StringWriter();
                TextWriter originalOut = Console.Out;

                try
                {
                    Console.SetOut(sw);
                    Console.SetError(sw);

                    // סימון תחילת הפלט של המשתמש
                    Console.WriteLine("[USER_OUTPUT_START]");

                    var scriptOptions = ScriptOptions.Default
                        .WithImports("System")
                        .WithReferences(typeof(object).Assembly, typeof(Console).Assembly);

                    // אם הקוד מכיל מחלקה עם Main, נוסיף קריאה מפורשת ל- Main()
                    string modifiedCode = userCode;
                    if (userCode.Contains("public static void Main()"))
                    {
                        modifiedCode += "\nProgram.Main();";
                    }

                    await CSharpScript.RunAsync(modifiedCode, scriptOptions);

                    // סימון סוף הפלט של המשתמש
                    Console.WriteLine("[USER_OUTPUT_END]");

                    Console.SetOut(originalOut);

                    string consoleOutput = sw.ToString().Trim();
                    sw.Dispose();

                    // בדיקת תקינות המיקום של הסימונים
                    int startIndex = consoleOutput.IndexOf("[USER_OUTPUT_START]") + "[USER_OUTPUT_START]".Length;
                    int endIndex = consoleOutput.IndexOf("[USER_OUTPUT_END]");

                    if (startIndex >= 0 && endIndex > startIndex)
                    {
                        consoleOutput = consoleOutput.Substring(startIndex, endIndex - startIndex).Trim();
                    }
                    else
                    {
                        Console.WriteLine("Warning: Output markers not found correctly.");
                        return "Error: Output markers not detected.";
                    }

                    Console.WriteLine($"Captured Clean Output: '{consoleOutput}'");

                    return string.IsNullOrEmpty(consoleOutput) ? "No output" : consoleOutput;
                }
                finally
                {
                    Console.SetOut(originalOut);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Execution Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }
    }
}


using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

Console.CursorVisible = false;


// maintain configuration lines across iterations
List<string> configLines = new List<string>();

while (true)
{
    Console.WriteLine("What are you looking for? (type '/q' to quit): ");
    string input = Console.ReadLine();

    // reload config each iteration, ignoring comments
    if (File.Exists("./config.txt"))
    {
        configLines = File.ReadAllLines("./config.txt")
            .Where(l => !l.StartsWith("#"))
            .ToList();
    }

    if (input.ToLower().Trim() == "/q")
    {
        break;
    }
    else
    {
        Console.WriteLine("\nSearching for packages...\n");
        string[] searchResults = pacmansearchrequest(input);
        if (searchResults.Length == 1 || (searchResults.Length == 1 && string.IsNullOrWhiteSpace(searchResults[0])))
        {
            Console.WriteLine("No packages found. :[");
            continue;
        }
        else
        {
            Console.WriteLine(searchResults.Length / 2 + " packages found:\n------------------------------");
            for (int i = 0; i < searchResults.Length - 1; i += 1)
            {
                if (i % 2 == 0)
                {
                    bool matchesConfig = false;
                    foreach (var cfg in configLines)
                    {
                        if (searchResults[i].Contains(cfg))
                        {
                            matchesConfig = true;
                            break;
                        }
                    }
                    if (searchResults[i].Contains("[Installiert") || searchResults[i].Contains("[Installed") || searchResults[i].Contains("[Aktuell") || searchResults[i].Contains("[Current") || matchesConfig)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    Console.WriteLine($"{(i / 2) + 1}. {searchResults[i].Trim()}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"    {searchResults[i]}");
                }
            }
        }
        Console.WriteLine("\nEnter the number of the package you want to install (or type '/q' to quit): ");
        string selection = Console.ReadLine();
        if (selection.ToLower().Trim() == "/q")
        {
            break;
        }
        else
        {
            string[] selections = selection.Split(',', ' ').Select(s => s.Trim()).ToArray();
            foreach (string sel in selections)
            {
                if (int.TryParse(sel, out int selectedIndex) && selectedIndex > 0 && selectedIndex <= searchResults.Length / 2)
                {
                    string packageName = searchResults[(selectedIndex - 1) * 2].Split('/')[1].Trim().Split(' ')[0];
                    Console.WriteLine($"\nInstalling {packageName}...\n");
                    string installResult = pacmaninstallrequest(packageName);
                    Console.WriteLine(installResult);
                }
                else
                {
                    Console.WriteLine("Invalid selection. Please try again.");
                }
            }

        }
    }


}


string[] pacmansearchrequest(string searchterm)
{
    string[] results = new string[0];
    try
    {
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "pacman";
        process.StartInfo.Arguments = $"-Ss {searchterm}";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        results = process.StandardOutput.ReadToEnd().Split('\n');
        process.WaitForExit();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
    return results;
}

string pacmaninstallrequest(string packagename)
{
    string result = "";
    try
    {
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "sudo";
        process.StartInfo.Arguments = $"pacman -S \"{packagename}\"";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
    return result;
}
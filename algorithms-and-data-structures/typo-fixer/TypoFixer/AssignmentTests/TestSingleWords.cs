using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AssignmentTests;

public enum PathTypes
{
    SolutionDir,
    ProjectDir,
    ProjectExecDir,
    TestsExecDir
}

public class Tests
{
    private OSPlatform _os;
    private string? _systemTypeArgument;
    private const string ProjectName = "TypoFixer";
    private readonly Dictionary<PathTypes, string> _paths = new();

    // Test cases
    private static readonly object[] MisspeledWords =
    [
        new object[] { "helo", new List<string> { "halo", "held", "hell", "hello", "helm" } },
        new object[] { "recieve", new List<string> { "receive", "relieve", "believe", "recede", "received" } },
        new object[] { "thier", new List<string> { "shier", "their", "thief", "tier", "trier" } },
        new object[] { "speeech", new List<string> { "speech", "screech", "leech", "peach", "perch" } },
        new object[] { "adn", new List<string> { "ad", "ain", "avn", "awn", "add" } },
        new object[] { "sould", new List<string> { "could", "would", "should", "sold", "soul" } },
        new object[] { "writting", new List<string> { "gritting", "writhing", "writing", "witting", "drifting" } },
        new object[] { "quikly", new List<string> { "quickly", "quaky", "quietly", "quill", "quills" } },
        new object[]
        {
            "pneumonoultramicroscopicsilicovolcanokoniosis",
            new List<string>
            {
                "pneumonoultramicroscopicsilicovolcanoconiosis",
                "ultramicroscopic",
                "ultramicroscopes",
                "anthracosilicosis",
                "pneumoconiosis"
            }
        },
        new object[] { "accomodate", new List<string> { "accommodate", "accommodated", "accommodates", "accommodative", "accumulate" } }
    ];

    
    [SetUp]
    public void Setup()
    {
        try
        {
            SetUpPlatform();
            SetUpPaths();
        }
        catch (SystemException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Cannot detect the system type: {e}");
            Console.ResetColor();
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Something went wrong while building relation paths in HuffmanProject: {e}");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Визначає платформу на якій запускаються тести: MacOS - arm/intel, linux - arm/intel_x64, windows - x64/x86
    /// </summary>
    /// <exception cref="SystemException">Викидає помилку якщо ваша система не входить в список вище</exception>
    private void SetUpPlatform()
    {
        var architecture = RuntimeInformation.OSArchitecture;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            _os = OSPlatform.OSX;
            _systemTypeArgument = architecture == Architecture.Arm64 ? "osx-arm64" : "osx-x64";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            _os = OSPlatform.Linux;
            _systemTypeArgument = architecture == Architecture.X64 ? "linux-x64" : "linux-arm64";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _os = OSPlatform.Windows;
            _systemTypeArgument = architecture == Architecture.X64 ? "win-x64" : "win-x86";
        }
        else
            throw new SystemException();
    }

    /// <summary>
    /// Розбудовує усі необхідні шляхи для тестування програми відповідно до <code>enum PathTypes {}</code>
    /// </summary>
    private void SetUpPaths()
    {
        var currentWorkDir = AppDomain.CurrentDomain.BaseDirectory;

        _paths[PathTypes.SolutionDir] = Directory.GetParent(currentWorkDir)!.Parent!.Parent!.Parent!.Parent!.FullName;
        _paths[PathTypes.ProjectDir] = Path.Combine(_paths[PathTypes.SolutionDir], ProjectName);
        _paths[PathTypes.TestsExecDir] = currentWorkDir;
        _paths[PathTypes.ProjectExecDir] = Path.Combine(_paths[PathTypes.SolutionDir],
            $"{ProjectName}/bin/Release/net9.0/{_systemTypeArgument}/publish/");
    }

    /// <summary>
    /// Залежно від системи запускає dotnet CLI команду яка компілює ваш цілий проєкт в один окремий виконувальний файл. 
    /// </summary>
    /// <remarks>Зі спостережень це працює лише на версії dotnet9.0</remarks>
    private void CompileProject()
    {
        string compileCommand =
            $"dotnet publish -c Release -r {_systemTypeArgument} --self-contained true -p:PublishSingleFile=true";
        
        

        var compileStartInfo = new ProcessStartInfo()
        {
            WorkingDirectory = _paths[PathTypes.ProjectDir],
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        if (_os == OSPlatform.Windows)
        {
            compileStartInfo.FileName = "cmd.exe";
            compileStartInfo.Arguments = $"/c {compileCommand}";
        }
        else
        {
            compileStartInfo.FileName = "/bin/bash";
            compileStartInfo.Arguments = $"-c \"{compileCommand}\"";
        }

        using var compile = new Process();
        compile.StartInfo = compileStartInfo;
        compile.Start();

        compile.WaitForExit();
    }

    /// <summary>
    /// Запускає скомпільований файл
    /// </summary>
    /// <param name="badWord">Неправильно написане слово</param>
    private string RunCompiledUnit_GetConsoleOutput(string badWord)
    {
        CompileProject();
        var compiledExecutable = Path.Combine(_paths[PathTypes.ProjectExecDir], ProjectName);

        var processStartInfo = new ProcessStartInfo
        {
            FileName = compiledExecutable,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false
        };

        using var process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
        
        process.StandardInput.WriteLine(badWord);
        process.StandardInput.Close();
        
        var output = process.StandardOutput.ReadToEnd();
        
        process.WaitForExit();

        return output;
    }

    [Test, TestCaseSource(nameof(MisspeledWords))]
    public void Test(string badWord, List<string> suggestions)
    {
        var output = RunCompiledUnit_GetConsoleOutput(badWord);
        var suggestionFound = suggestions.Any(s => output.Contains(s, StringComparison.CurrentCultureIgnoreCase));
        Assert.That(suggestionFound, Is.True, $"None of the suggestions were found in the output for '{badWord}'.");
    }
}
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework.Constraints;

namespace AssignmentTests;

public enum PathTypes
{
    SolutionDir,
    ProjectDir,
    TestsDir,
    TextFilesDir,
    ProjectExecDir,
    TestsExecDir
}

public class Tests
{
    private OSPlatform _os;
    private string? _systemTypeArgument;
    private const string ProjectName = "HuffmanCodes";
    private readonly Dictionary<PathTypes, string> _paths = new();

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
        _paths[PathTypes.TextFilesDir] = Path.Combine(_paths[PathTypes.SolutionDir], $"{ProjectName}/TextFiles/");
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
    /// Запускає скомпільований файл з аргументами командного рядку, те саме що <code>dotnet run -- arg1 arg2</code>
    /// </summary>
    /// <param name="fileEncode">Назва файлу для кодування</param>
    /// <param name="fileDecode">Назва розкодованного файлу</param>
    private void RunEncoder(string fileEncode, string fileDecode)
    {
        CompileProject();
        var compiledExecutable = Path.Combine(_paths[PathTypes.ProjectExecDir], ProjectName);

        var processStartInfo = new ProcessStartInfo
        {
            FileName = compiledExecutable,
            Arguments = $"{fileEncode} {fileDecode}",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false
        };

        using var process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();

        process.WaitForExit();
    }

    [Test]
    public void Encode()
    {
        const string fileEncode = "test.txt";
        const string fileDecode = "testDec.txt";

        RunEncoder(fileEncode, fileDecode);

        string originPath = Path.Combine(_paths[PathTypes.TextFilesDir], fileEncode);
        string decodedPath = Path.Combine(_paths[PathTypes.TextFilesDir], fileDecode);

        var originFileInfo = new FileInfo(originPath);
        var decodedFileInfo = new FileInfo(decodedPath);

        if (!decodedFileInfo.Exists)
        {
            Assert.Fail("FileNotFound for decoded file");
        }
        
        Assert.That(originFileInfo.Length, Is.EqualTo(decodedFileInfo.Length));

        using var streamOrigin = new FileStream(originPath, FileMode.Open, FileAccess.Read);
        using var streamDecoded = new FileStream(decodedPath, FileMode.Open, FileAccess.Read);

        // you may change how much bytes will be read
        const int chunkSize = 128;

        while (streamOrigin.Position < streamOrigin.Length && streamDecoded.Position < streamDecoded.Length)
        {
            int remainingOrigin = (int)(streamOrigin.Length - streamOrigin.Position);
            int remainingDecoded = (int)(streamDecoded.Length - streamDecoded.Position);

            int bytesToRead = Math.Min(chunkSize, Math.Min(remainingOrigin, remainingDecoded));
            
            var originByteArr = new byte[bytesToRead];
            var decodedByteArr = new byte[bytesToRead];
            
            streamOrigin.ReadExactly(originByteArr, 0, bytesToRead);
            streamDecoded.ReadExactly(decodedByteArr, 0, bytesToRead);
            
            if (!originByteArr.SequenceEqual(decodedByteArr))
            {
                string originByteStr = BitConverter.ToString(originByteArr);
                string decodedByteStr = BitConverter.ToString(decodedByteArr);

                string originStr = Encoding.UTF8.GetString(originByteArr);
                string decodedStr = Encoding.UTF8.GetString(decodedByteArr);
                
                int diffIndex = -1;
                for (int i = 0; i < bytesToRead; i++)
                {
                    if (originByteArr[i] != decodedByteArr[i])
                    {
                        diffIndex = i;
                        break;
                    }
                }
                
                Console.WriteLine($"Mismatch found at index {diffIndex}:");
                Console.WriteLine($"Origin Byte Array:  {originByteStr}");
                Console.WriteLine($"Decoded Byte Array: {decodedByteStr}");
                Console.WriteLine($"Origin string: \n{originStr}");
                Console.WriteLine($"Decoded string: \n{decodedStr}");
                Console.WriteLine($"First difference at index {diffIndex}: Origin={originByteArr[diffIndex]}, Decoded={decodedByteArr[diffIndex]}");

                Assert.Fail("Byte arrays differ at index " + diffIndex);
            }
        }
    }
}
namespace HuffmanCodes;

public static class Program
{
    private static string? _exePath;
    private static string? _projectRoot;

    public static void Main(string[] args)
    {
        GetProjectRoot();

        var textFilesPath = Path.Combine(_projectRoot!, "TextFiles");
        if (!Directory.Exists(textFilesPath))
        {
            Directory.CreateDirectory(textFilesPath);
        }

        var inputFileName = "sherlock.txt";
        var encodedFileName = "encoded.txt";
        var codeTableFileName = "code_table.txt";
        var decodedFileName = "decoded.txt";

        var encodedBinaryFileName = "encoded_binary.bin";
        var codeTableBinaryFileName = "code_table_binary.txt";
        var decodedBinaryFileName = "decoded_binary.txt";

        var inputFilePath = Path.Combine(textFilesPath, inputFileName);
        var encodedFilePath = Path.Combine(textFilesPath, encodedFileName);
        var codeTablePath = Path.Combine(textFilesPath, codeTableFileName);
        var decodedFilePath = Path.Combine(textFilesPath, decodedFileName);

        var encodedBinaryFilePath = Path.Combine(textFilesPath, encodedBinaryFileName);
        var codeTableBinaryPath = Path.Combine(textFilesPath, codeTableBinaryFileName);
        var decodedBinaryFilePath = Path.Combine(textFilesPath, decodedBinaryFileName);

        try
        {
            var frequency = SymbolFrequency.GetSymbolFrequency(inputFilePath);
            var tree = HuffmanTree.BuildTree(frequency);
            var codes = HuffmanCoding.CreateCode(tree);

            Console.WriteLine("Коди символів:");
            foreach (var kvp in codes)
            {
                Console.WriteLine($"Symbol '{kvp.Key}': {kvp.Value}");
            }

            var originalText = File.ReadAllText(inputFilePath);
            
            var encodedText = HuffmanArchiver.EncodeText(originalText, codes);
            HuffmanArchiver.SaveEncodedText(encodedText, encodedFilePath);
            HuffmanArchiver.SaveCodeTable(codes, codeTablePath);
            var decodedText = HuffmanArchiver.DecodeText(encodedText, tree);
            HuffmanArchiver.SaveDecodedText(decodedText, decodedFilePath);
            Console.WriteLine($"Закодований файл (текстовий): {encodedFileName}");
            Console.WriteLine($"Таблиця кодів (текстова): {codeTableFileName}");
            Console.WriteLine($"Розкодований файл (текстовий): {decodedFileName}");
            
            HuffmanArchiver.SaveEncodedBinary(encodedText, encodedBinaryFilePath);
            HuffmanArchiver.SaveCodeTable(codes, codeTableBinaryPath);
            HuffmanStats.PrintEntropyAndAverageLength(frequency, codes);
            var loadedBinaryText = HuffmanArchiver.LoadEncodedBinary(encodedBinaryFilePath);
            var decodedBinaryText = HuffmanArchiver.DecodeText(loadedBinaryText, tree);
            HuffmanArchiver.SaveDecodedText(decodedBinaryText, decodedBinaryFilePath);
            Console.WriteLine($"Цифрове кодування збережено в: {encodedBinaryFileName}");
            Console.WriteLine($"Таблиця кодів (для цифрового): {codeTableBinaryFileName}");
            Console.WriteLine($"Розкодовано з цифрового у: {decodedBinaryFileName}");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка: {ex.Message}");
            Console.ResetColor();
        }
    }

    private static void GetProjectRoot()
    {
        try
        {
            _exePath = AppDomain.CurrentDomain.BaseDirectory;
            _projectRoot = Directory.GetParent(_exePath)!.Parent!.Parent!.Parent!.FullName;
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Щось пішло не так з побудовою шляху до TextFiles: {e}");
            Console.ResetColor();
        }
    }
}

using Calculator;

Console.WriteLine("Hello, World!");
Console.WriteLine("Enter your expression: ");
string input = Console.ReadLine();

Tokenizer tokenizer = new Tokenizer(input);
tokenizer.Tokenize();
Console.Write("Tokens: ");
tokenizer.ShowTokens();
Console.WriteLine();

ShuntingYard shuntingYard = new ShuntingYard();
var rpn = shuntingYard.ConvertToRPN(tokenizer.ShowTokens());
Console.Write("\nReversed Polish Notation: ");
rpn.ShowQueue();

NewCalculator calculator = new NewCalculator(rpn.Count, rpn);
int Answer = calculator.Calculate();
Console.WriteLine("\nResults: " + Answer);
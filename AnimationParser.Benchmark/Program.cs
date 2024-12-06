using AnimationParser.Core;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<BenchmarkAnimationParser>();

// Benchmark result:
// 1. with code below
//     const string code = """
//                         (define cross ( (line (0 0) (50 50)) (line (50 0) (0 50)) (circle (25 25) 25) ) )
//                         (place cross (50 50))
//                         (loop 100 ( (shift cross right) (shift cross up) ) )
//                         (erase cross)
//                         """;
//
//    BenchmarkDotNet v0.14.0, Arch Linux
//    13th Gen Intel Core i5-13500H, 1 CPU, 16 logical and 12 physical cores
//    .NET SDK 9.0.101
//    [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
//    DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
//
//
//    | Method                     | Mean     | Error     | StdDev    | Gen0   | Allocated |
//    |--------------------------- |---------:|----------:|----------:|-------:|----------:|
//    | BenchmarkLexing            | 1.198 us | 0.0011 us | 0.0010 us | 0.0153 |     144 B |
//    | BenchmarkParsing           | 2.907 us | 0.0063 us | 0.0056 us | 0.2289 |    2184 B |
//    | BenchmarkLexingToParsing   | 3.566 us | 0.0046 us | 0.0043 us | 0.2403 |    2264 B |
//    | BenchmarkExecuting         | 2.854 us | 0.0020 us | 0.0016 us | 0.4501 |    4240 B |
//    | BenchmarkLexingToExecuting | 6.468 us | 0.0163 us | 0.0145 us | 0.6866 |    6504 B |
//
//    // * Hints *
//    Outliers
//    BenchmarkAnimationParser.BenchmarkParsing: Default           -> 1 outlier  was  removed (2.94 us)
//    BenchmarkAnimationParser.BenchmarkExecuting: Default         -> 2 outliers were removed (2.86 us, 2.87 us)
//    BenchmarkAnimationParser.BenchmarkLexingToExecuting: Default -> 1 outlier  was  removed (6.79 us)
//
//    // * Legends *
//    Mean      : Arithmetic mean of all measurements
//    Error     : Half of 99.9% confidence interval
//    StdDev    : Standard deviation of all measurements
//    Gen0      : GC Generation 0 collects per 1000 operations
//    Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
//    1 us      : 1 Microsecond (0.000001 sec)
//
//    // * Diagnostic Output - MemoryDiagnoser *
//
//
//    // ***** BenchmarkRunner: End *****
//    Run time: 00:01:46 (106.71 sec), executed benchmarks: 5

// 2. with code below
//     const string code = """
//                         (define cross ( (line (0 0) (50 50)) (line (50 0) (0 50)) (circle (25 25) 25) ) )
//                         (place cross (50 50))
//                         (loop 100 ( (shift cross right) (shift cross up) ) )
//                         (erase cross)
//                         (define cross ( (line (0 0) (50 50)) (line (50 0) (0 50)) (circle (25 25) 25) ) )
//                         (place cross (50 50))
//                         (loop 100 ( (shift cross right) (shift cross up) ) )
//                         (erase cross)
//                         (define cross ( (line (0 0) (50 50)) (line (50 0) (0 50)) (circle (25 25) 25) ) )
//                         (place cross (50 50))
//                         (loop 100 ( (shift cross right) (shift cross up) ) )
//                         (erase cross)
//                         (define cross ( (line (0 0) (50 50)) (line (50 0) (0 50)) (circle (25 25) 25) ) )
//                         (place cross (50 50))
//                         (loop 100 ( (shift cross right) (shift cross up) ) )
//                         (erase cross)
//                         (define cross ( (line (0 0) (50 50)) (line (50 0) (0 50)) (circle (25 25) 25) ) )
//                         (place cross (50 50))
//                         (loop 100 ( (shift cross right) (shift cross up) ) )
//                         (erase cross)
//                         (define cross ( (line (0 0) (50 50)) (line (50 0) (0 50)) (circle (25 25) 25) ) )
//                         (place cross (50 50))
//                         (loop 100 ( (shift cross right) (shift cross up) ) )
//                         (erase cross)
//                         """;

//    BenchmarkDotNet v0.14.0, Arch Linux
//    13th Gen Intel Core i5-13500H, 1 CPU, 16 logical and 12 physical cores
//    .NET SDK 9.0.101
//    [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
//    DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
//
//
//    | Method                     | Mean      | Error     | StdDev    | Gen0   | Allocated |
//    |--------------------------- |----------:|----------:|----------:|-------:|----------:|
//    | BenchmarkLexing            |  6.803 us | 0.0101 us | 0.0084 us | 0.0153 |     144 B |
//    | BenchmarkParsing           | 18.321 us | 0.0230 us | 0.0215 us | 1.3123 |   12473 B |
//    | BenchmarkLexingToParsing   | 21.037 us | 0.0165 us | 0.0147 us | 1.3123 |   12553 B |
//    | BenchmarkExecuting         | 16.372 us | 0.0284 us | 0.0251 us | 2.5635 |   24240 B |
//    | BenchmarkLexingToExecuting | 38.014 us | 0.0595 us | 0.0557 us | 3.9063 |   36794 B |
//
//    // * Hints *
//    Outliers
//    BenchmarkAnimationParser.BenchmarkLexing: Default          -> 2 outliers were removed (6.84 us, 6.84 us)
//    BenchmarkAnimationParser.BenchmarkLexingToParsing: Default -> 1 outlier  was  removed (21.48 us)
//    BenchmarkAnimationParser.BenchmarkExecuting: Default       -> 1 outlier  was  removed, 2 outliers were detected (16.33 us, 16.43 us)
//
//    // * Legends *
//    Mean      : Arithmetic mean of all measurements
//    Error     : Half of 99.9% confidence interval
//    StdDev    : Standard deviation of all measurements
//    Gen0      : GC Generation 0 collects per 1000 operations
//    Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
//    1 us      : 1 Microsecond (0.000001 sec)
//
//    // * Diagnostic Output - MemoryDiagnoser *
//
//
//    // ***** BenchmarkRunner: End *****
//    Run time: 00:01:33 (93.92 sec), executed benchmarks: 5

[MemoryDiagnoser]
public class BenchmarkAnimationParser
{
    private const string code = """
                                (define cross ( (line (0 0) (50 50)) (line (50 0) (0 50)) (circle (25 25) 25) ) )
                                (place cross (50 50))
                                (loop 100 ( (shift cross right) (shift cross up) ) )
                                (erase cross)
                                """;

    private List<Token> tokens;
    private List<IAnimationCommand> commands;

    public BenchmarkAnimationParser()
    {
        var lexer = new Lexer(code);
        tokens = lexer.Tokenize().ToList();
        commands = new Parser(tokens).Parse().ToList();
    }

    [Benchmark]
    public void BenchmarkLexing()
    {
        var lexer = new Lexer(code);
        _ = lexer.Tokenize().All(_ => true); // lexer is lazy evaluated, so we need to consume it
    }

    [Benchmark]
    public void BenchmarkParsing()
    {
        var parser = new Parser(tokens);
        _ = parser.Parse().All(_ => true); // parser is lazy evaluated, so we need to consume it
    }

    [Benchmark]
    public void BenchmarkLexingToParsing()
    {
        var lexer = new Lexer(code);
        var parser = new Parser(lexer.Tokenize());
        _ = parser.Parse().All(_ => true); // parser and lexer are lazy evaluated, so we need to consume them
    }

    [Benchmark]
    public void BenchmarkExecuting()
    {
        var context = new AnimationContext();
        foreach (var command in commands)
        {
            command.Execute(context);
        }
    }

    [Benchmark]
    public void BenchmarkLexingToExecuting()
    {
        var lexer = new Lexer(code);
        var parser = new Parser(lexer.Tokenize());
        var context = new AnimationContext();

        foreach (var command in parser.Parse())
        {
            command.Execute(context);
        }
    }
}

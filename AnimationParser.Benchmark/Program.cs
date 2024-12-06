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
//    BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
//    13th Gen Intel Core i5-13500H, 1 CPU, 16 logical and 12 physical cores
//    .NET SDK 9.0.100-rc.1.24452.12
//    [Host]     : .NET 9.0.0 (9.0.24.43107), X64 RyuJIT AVX2
//    DefaultJob : .NET 9.0.0 (9.0.24.43107), X64 RyuJIT AVX2
//
//
//    | Method                     | Mean       | Error    | StdDev   | Gen0   | Allocated |
//    |--------------------------- |-----------:|---------:|---------:|-------:|----------:|
//    | BenchmarkLexing            |   751.4 ns |  1.79 ns |  1.67 ns | 0.0153 |     144 B |
//    | BenchmarkParsing           | 2,200.9 ns |  7.18 ns |  6.72 ns | 0.2289 |    2184 B |
//    | BenchmarkLexingToParsing   | 2,711.3 ns |  6.80 ns |  6.36 ns | 0.2403 |    2264 B |
//    | BenchmarkExecuting         | 1,929.3 ns |  7.19 ns |  5.62 ns | 0.4501 |    4240 B |
//    | BenchmarkLexingToExecuting | 4,976.2 ns | 86.57 ns | 72.29 ns | 0.6866 |    6504 B |
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
//    Run time: 00:01:32 (92 sec), executed benchmarks: 5

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

//    BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
//    13th Gen Intel Core i5-13500H, 1 CPU, 16 logical and 12 physical cores
//    .NET SDK 9.0.100-rc.1.24452.12
//    [Host]     : .NET 9.0.0 (9.0.24.43107), X64 RyuJIT AVX2
//    DefaultJob : .NET 9.0.0 (9.0.24.43107), X64 RyuJIT AVX2
//
//
//    | Method                     | Mean      | Error     | StdDev    | Gen0   | Allocated |
//    |--------------------------- |----------:|----------:|----------:|-------:|----------:|
//    | BenchmarkLexing            |  4.306 us | 0.0163 us | 0.0153 us | 0.0153 |     144 B |
//    | BenchmarkParsing           | 13.206 us | 0.0441 us | 0.0368 us | 1.3123 |   12473 B |
//    | BenchmarkLexingToParsing   | 16.313 us | 0.0626 us | 0.0555 us | 1.3123 |   12553 B |
//    | BenchmarkExecuting         | 11.698 us | 0.0920 us | 0.0718 us | 2.5635 |   24240 B |
//    | BenchmarkLexingToExecuting | 28.540 us | 0.0714 us | 0.0633 us | 3.9063 |   36794 B |
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
//    Run time: 00:01:38 (98.74 sec), executed benchmarks: 5

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
        var tokenstream = lexer.Tokenize().GetEnumerator();
        while (tokenstream.MoveNext());
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

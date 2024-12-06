using AnimationParser.Core;

namespace AnimationParser.Tests;

public class TestLexer
{
    private MockedTokenFactory tokenFactory = null!;

    [SetUp]
    public void SetUp()
    {
        tokenFactory = new MockedTokenFactory();
    }

    [TearDown]
    public void TearDown()
    {
        tokenFactory = null!;
    }

    private void VerifyTokenStream(IEnumerable<Token> lhs, IEnumerable<Token> rhs)
    {
        var lhsEnumerator = lhs.GetEnumerator();
        var rhsEnumerator = rhs.GetEnumerator();

        while (lhsEnumerator.MoveNext() && rhsEnumerator.MoveNext())
        {
            if (!lhsEnumerator.Current.Equals(rhsEnumerator.Current))
            {
                throw new AssertionException($"Expected {rhsEnumerator.Current}, but got {lhsEnumerator.Current}");
            }
        }

        if (lhsEnumerator.MoveNext() || rhsEnumerator.MoveNext())
        {
            throw new AssertionException("Token streams have different lengths");
        }
    }

    [Test]
    public void TestLexer_SingleLineSupport()
    {
        const string input = @"(define cross ( (line (0 0) (50 50)) ) )";
        Token[] expected = [
            tokenFactory.LeftParen,
                tokenFactory.Keyword("define"),
                tokenFactory.Identifier("cross"),
                tokenFactory.LeftParen,
                    tokenFactory.LeftParen,
                        tokenFactory.Keyword("line"),
                        tokenFactory.LeftParen,
                            tokenFactory.Number("0"),
                            tokenFactory.Number("0"),
                        tokenFactory.RightParen,
                        tokenFactory.LeftParen,
                            tokenFactory.Number("50"),
                            tokenFactory.Number("50"),
                            tokenFactory.RightParen,
                        tokenFactory.RightParen,
                    tokenFactory.RightParen,
                tokenFactory.RightParen,
            tokenFactory.EndOfSource,
        ];

        var lexer = new Lexer(input);

        var tokens = lexer.Tokenize();

        VerifyTokenStream(tokens, expected);
    }

    [Test]
    public void TestLexer_PanicIfContainingInvalidCharacter()
    {
        const string input = @"(define cross ( (line (0 0) (50 50)) ) )*/";

        Lexer lexer = new(input);

        Assert.That(() => 
        {
            // Since the lexer is lazy evaluated, we need to enumerate the token stream to trigger the exception
            _ = lexer.Tokenize().All(_ => true);
        }, Throws.Exception);
    }

    [Test]
    public void TestLexer_MultiLineSupport()
    {
        const string input = @"(define cross 
            ( (line (0 0) (50 50)) ) )";

        Token[] expected = [
            tokenFactory.LeftParen,
                tokenFactory.Keyword("define"),
                tokenFactory.Identifier("cross"),
                tokenFactory.LeftParen,
                    tokenFactory.LeftParen,
                        tokenFactory.Keyword("line"),
                        tokenFactory.LeftParen,
                            tokenFactory.Number("0"),
                            tokenFactory.Number("0"),
                        tokenFactory.RightParen,
                        tokenFactory.LeftParen,
                            tokenFactory.Number("50"),
                            tokenFactory.Number("50"),
                        tokenFactory.RightParen,
                    tokenFactory.RightParen,
                tokenFactory.RightParen,
            tokenFactory.RightParen,
            tokenFactory.EndOfSource,
        ];

        var lexer = new Lexer(input);

        var tokens = lexer.Tokenize();

        VerifyTokenStream(tokens, expected);
    }
}

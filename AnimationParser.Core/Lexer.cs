using System.Runtime.CompilerServices;
using System.Collections.Immutable;
using AnimationParser.Core.Tokens;

namespace AnimationParser.Core;

public class Lexer
{
    private readonly string sourceDocument;

    private int CurrentSourceIndex => tokenFactory.CurrentSourceIndex;

    private char CurrentChar => CurrentSourceIndex < sourceDocument.Length ? sourceDocument[CurrentSourceIndex] : '\0';

    private readonly TokenFactory tokenFactory;

    public Lexer(string input)
    {
        sourceDocument = input;
        tokenFactory = new TokenFactory(input);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void MoveNext()
    {
        tokenFactory.MoveNext();
    }

    public IEnumerable<Token> Tokenize()
    {
        while (CurrentSourceIndex < sourceDocument.Length)
        {
            switch (CurrentChar)
            {
                case '\n':
                    tokenFactory.OnNewLine();
                    goto case ' ';
                case ' ':
                case '\t':
                case '\r':
                    MoveNext();
                    continue;

                case '(':
                    MoveNext();
                    yield return tokenFactory.LeftParen;
                    continue;

                case ')':
                    MoveNext();
                    yield return tokenFactory.RightParen;
                    continue;

                default:
                    if (char.IsLetter(CurrentChar))
                    {
                        yield return ReadKeywordOrIdentifier();
                    }
                    else if (char.IsDigit(CurrentChar))
                    {
                        yield return ReadNumber();
                    }
                    else if (CurrentChar == '-')
                    {
                        yield return ReadTokenStartWithMinus();
                    }
                    else
                    {
                        throw new Exception($"Invalid character '{CurrentChar}' at line {tokenFactory.CurrentPosition.LineNo}:{tokenFactory.CurrentPosition.CharNo}");
                    }
                    continue;
            }
        }

        yield return tokenFactory.EndOfSource;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private Token ReadTokenStartWithMinus()
    {
        MoveNext();

        if (char.IsDigit(CurrentChar))
        {
            var numberToken =  ReadNumber();

            numberToken.SourceIndex--; // Move back to include the minus sign
            numberToken.Length++; // Include the minus sign in the length

            return numberToken;
        }
        else
        {
            throw new Exception($"Invalid character '{CurrentChar}' at line {tokenFactory.CurrentPosition.LineNo}:{tokenFactory.CurrentPosition.CharNo}");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private Token ReadKeywordOrIdentifier()
    {
        int start = CurrentSourceIndex;
        while (char.IsLetterOrDigit(CurrentChar) || CurrentChar == '_')
            MoveNext();

        int length = CurrentSourceIndex - start;

        return IsKeyword(sourceDocument.AsSpan(start, length)) switch
        {
            true => tokenFactory.Keyword(length),
            false => tokenFactory.Identifier(length),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private Token ReadNumber()
    {
        int start = CurrentSourceIndex;
        while (char.IsDigit(CurrentChar) || CurrentChar == '.')
            MoveNext();

        return tokenFactory.Number(CurrentSourceIndex - start);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool IsKeyword(ReadOnlySpan<char> text)
    {
        // Static dispatch to avoid string allocations
        switch (text)
        {
            case "define":
            case "place":
            case "shift":
            case "erase":
            case "loop":
            case "line":
            case "circle":
            case "left":
            case "right":
            case "up":
            case "down":
                return true;
            default:
                return false;
        }
    }
}

using AnimationParser.Core.Tokens;

namespace AnimationParser.Tests;

public class TestToken()
{
    private MockedTokenFactory lhsFactory = null!;
    private MockedTokenFactory rhsFactory = null!;

    [SetUp]
    public void SetUp()
    {
        lhsFactory = new MockedTokenFactory();
        rhsFactory = new MockedTokenFactory();
    }

    [TearDown]
    public void TearDown()
    {
        lhsFactory = null!;
        rhsFactory = null!;
    }

    [Test]
    public void TestToken_DifferentHashCodeWithDifferentText()
    {
        var lhs = lhsFactory.Identifier("foo");
        var rhs = rhsFactory.Identifier("bar");

        Assert.That(lhs.GetHashCode(), Is.Not.EqualTo(rhs.GetHashCode()));
    }

    [Test]
    public void TestToken_DifferentHashCodeWithDifferentType()
    {
        var lhs = lhsFactory.Identifier("foo");
        var rhs = rhsFactory.Keyword("foo");

        Assert.That(lhs.GetHashCode(), Is.Not.EqualTo(rhs.GetHashCode()));
    }

    [Test]
    public void TestToken_SameHashCodeWithSameTextAndType()
    {
        rhsFactory.MoveNext();
        rhsFactory.OnNewLine(); // different position and source index

        var lhs = lhsFactory.Identifier("foo");
        var rhs = rhsFactory.Identifier("foo");

        Assert.That(lhs.GetHashCode(), Is.EqualTo(rhs.GetHashCode()));
    }

    [Test]
    public void TestToken_EqualsIfSameTextAndType()
    {
        rhsFactory.MoveNext();
        rhsFactory.OnNewLine(); // different position and source index

        var lhs = lhsFactory.Identifier("foo");
        var rhs = rhsFactory.Identifier("foo");

        Assert.That(lhs, Is.EqualTo(rhs));
    }

    [Test]
    public void TestToken_NotEqualsIfDifferentText()
    {
        var lhs = lhsFactory.Identifier("foo");
        var rhs = rhsFactory.Identifier("bar");

        Assert.That(lhs, Is.Not.EqualTo(rhs));
    }

    [Test]
    public void TestToken_NotEqualsIfDifferentType()
    {
        var lhs = lhsFactory.Identifier("foo");
        var rhs = rhsFactory.Keyword("foo");

        Assert.That(lhs, Is.Not.EqualTo(rhs));
    }

    [Test]
    public void TestToken_NotEqualsForNull()
    {
        var lhs = lhsFactory.Identifier("foo");

        Assert.That(lhs.Equals(null), Is.False);
    }

    [Test]
    public void TestToken_NotEqualsForOtherType()
    {
        var lhs = lhsFactory.Identifier("foo");

        Assert.That(lhs.Equals(new object()), Is.False);
    }
}

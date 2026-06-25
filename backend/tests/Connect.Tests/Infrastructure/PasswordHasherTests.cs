using Connect.Infrastructure.Identity;
using Xunit;

namespace Connect.Tests.Infrastructure;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Hash_is_not_the_plaintext()
    {
        var hash = _hasher.Hash("Password123!");
        Assert.NotEqual("Password123!", hash);
    }

    [Fact]
    public void Verify_returns_true_for_correct_password()
    {
        var hash = _hasher.Hash("Password123!");
        Assert.True(_hasher.Verify("Password123!", hash));
    }

    [Fact]
    public void Verify_returns_false_for_wrong_password()
    {
        var hash = _hasher.Hash("Password123!");
        Assert.False(_hasher.Verify("wrong-password", hash));
    }
}

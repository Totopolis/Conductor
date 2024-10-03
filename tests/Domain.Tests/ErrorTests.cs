using ErrorOr;

namespace Domain.Tests;

public class ErrorTests
{
    [Fact]
    public void TestOneFailure()
    {
        ErrorOr<int> res1 = 1;
        ErrorOr<int> res2 = Error.Failure();
        List<Error> errors = [res1.FirstError, res2.FirstError];

        var combined = errors.ToErrorOr<int>();

        Assert.True(combined.IsError);
    }
}

using CSharpFunctionalExtensions;

namespace Infrastructure.Tests.Sample;

public sealed class BooValue : ValueObject
{
    private BooValue()
    {
    }

    public double Amount { get; private set; }

    public string Note { get; private set; } = default!;

    public static BooValue Create(double amount, string note)
    {
        var boo = new BooValue
        {
            Amount = amount,
            Note = note
        };

        return boo;
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Amount;
        yield return Note;
    }
}

using CSharpFunctionalExtensions;

namespace Infrastructure.Tests.Sample;

public sealed class FooEntity : Entity<Guid>
{
    public FooEntity(Guid id) : base(id)
    {

    }

    public int One { get; private set; }

    public string Two { get; private set; } = default!;

    public BooValue Boo { get; private set; } = default!;

    public static FooEntity Create(
        int one,
        string two,
        BooValue boo)
    {
        var id = Guid.NewGuid();

        var foo = new FooEntity(id)
        {
            One = one,
            Two = two,
            Boo = boo
        };

        return foo;
    }
}

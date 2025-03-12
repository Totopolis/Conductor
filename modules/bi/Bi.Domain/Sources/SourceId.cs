using Vogen;

namespace Bi.Domain.Sources;

[ValueObject<Guid>]
public partial struct SourceId
{
    private static Validation Validate(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Validation.Invalid("SourceId can not be empty");
        }

        if (value.Version != 7)
        {
            return Validation.Invalid("SourceId must be 7 version");
        }

        return Validation.Ok;
    }
}

using Vogen;

namespace Bi.Domain.DataSources;

[ValueObject<Guid>]
public partial struct DbSourceId
{
    private static Validation Validate(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Validation.Invalid("DbSourceId can not be empty");
        }

        if (value.Version != 7)
        {
            return Validation.Invalid("DbSourceId must be 7 version");
        }

        return Validation.Ok;
    }
}

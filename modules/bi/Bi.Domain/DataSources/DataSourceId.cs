using Vogen;

namespace Bi.Domain.DataSources;

[ValueObject<Guid>]
public partial struct DataSourceId
{
    private static Validation Validate(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Validation.Invalid("DataSourceId can not be empty");
        }

        if (value.Version != 7)
        {
            return Validation.Invalid("DataSourceId must be 7 version");
        }

        return Validation.Ok;
    }
}

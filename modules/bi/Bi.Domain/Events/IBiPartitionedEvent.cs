using Bi.Domain.DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Domain.Events;

public interface IBiPartitionedEvent
{
    Guid PartitionKey { get; }
}

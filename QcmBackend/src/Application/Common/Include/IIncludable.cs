using System.Collections.Generic;

namespace QcmBackend.Application.Common.Include
{
    public interface IIncludable
    {
        IList<string>? Includes { get; init; }
    }
}

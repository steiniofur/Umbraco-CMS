using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;

namespace Umbraco.Cms.Core.Persistence.Repositories;

public interface IElementRepository : IContentRepository<int, IElement>, IReadRepository<Guid, IElement>
{
}

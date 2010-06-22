using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Facebook.Graph
{
    /// <summary>
    /// Marker interface that notes that an entity may be searched-via 
    /// <see cref="GraphSession.SearchFor{TEntity}"/>.
    /// </summary>
    public interface ISearchableEntity
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Facebook.Graph
{
    /// <summary>
    /// Indicates that a class may be retrieved as a connected entity (that is, it may not have complete information), 
    /// and that it can be retrieved with an additional request.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity that would be retrieved by a request.</typeparam>
    public interface IConnectedGraphEntity<TEntity>
        where TEntity : GraphEntity
    {
        /// <summary>
        /// Gets whether the current instance is a connected entity.
        /// </summary>
        bool IsConnection { get; }
        /// <summary>
        /// Retrieves the real entity instance from the server.
        /// </summary>
        /// <returns>An instance of the real entity.</returns>
        TEntity GetRealEntity();

        /// <summary>
        /// Asynchronously retrieves the real entity instance from the server.
        /// </summary>
        /// <param name="callback">A reference to the method to call upon completion of the request.</param>
        void GetRealEntityAsync(Action<TEntity> callback);
    }
}

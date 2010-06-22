using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Facebook.Graph
{
    /// <summary>
    /// Marker interface that notes that an entity may be subscribed-to via 
    /// <see cref="RealTimeUpdateManager.RegisterSubscription(string, string, string)"/>.
    /// </summary>
    public interface ISubscribableEntity
    {
    }
}

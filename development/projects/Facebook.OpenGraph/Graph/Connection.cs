using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Facebook.Graph.Util;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents a connection to a list of entities related to a given entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to which connecting is supported.  This entity must be derived from <see cref="GraphEntity"/>.</typeparam>
    [DebuggerDisplay("Connection from {m_sourceEntityID} to {m_connectionName}")]
    public class Connection<TEntity> : IEnumerable<TEntity>, IConnection
        where TEntity : GraphEntity
    {
        /*
         * Use cases for this class:
         * Delay-access based on &metadata=1 retrieved URL
         * Delay-access based on inferring URL from entity property (needs to happen elsewhere) - should yield same URL
         * Immediately loaded data based on a JToken, should still contain URL
         */
        #region fields
        private string m_sourceEntityID, m_connectionName, m_relatedUrl;
        private JToken m_sourceData;
        #endregion

        /// <summary>
        /// Creates a new Connection to the specified entity.
        /// </summary>
        /// <param name="sourceEntityID">The source (containing) entity ID.</param>
        /// <param name="connectionName">The name of the connection.</param>
        /// <param name="sourceData">Source data, if any, retrieved from the object.</param>
        /// <param name="sourceSession">The session associated with the Connection.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sourceEntityID"/>, <paramref name="connectionName"/>, or <paramref name="sourceSession"/> is
        /// <see langword="null"/>.</exception>
        public Connection(string sourceEntityID, string connectionName, JToken sourceData, GraphSession sourceSession)
        {
            if (string.IsNullOrEmpty(sourceEntityID))
                throw new ArgumentNullException("sourceEntityID");
            if (string.IsNullOrEmpty(connectionName))
                throw new ArgumentNullException("connectionName");
            if (sourceSession == null)
                throw new ArgumentNullException("sourceSession");

            m_sourceEntityID = sourceEntityID;
            m_connectionName = connectionName;
            Session = sourceSession;

            m_relatedUrl = string.Format(CultureInfo.InvariantCulture, "{0}{1}/{2}?access_token={3}", sourceSession.BaseUrl, sourceEntityID, connectionName, sourceSession.AccessToken);

            m_sourceData = sourceData;
        }

        private Connection(string sourceEntityID, string connectionName, string relatedUrl, JToken sourceData, GraphSession sourceSession)
            : this(sourceEntityID, connectionName, sourceData, sourceSession)
        {
            m_relatedUrl = relatedUrl;
        }

        /// <summary>
        /// Gets the <see cref="GraphSession"/> with which this connection makes Graph API requests.
        /// </summary>
        public GraphSession Session
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets whether the first page of data has been loaded.
        /// </summary>
        public bool IsLoaded
        {
            get { return m_sourceData != null; }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Loads the data, if it is not already loaded.
        /// </summary>
        public void Load()
        {
            VerifyLoaded();
        }
#endif

        /// <summary>
        /// Asynchronously loads the data, if it is not already loaded.
        /// </summary>
        /// <param name="callback">A reference to the method to call upon completion of this request.  This parameter may be null.</param>
        public void LoadAsync(Action callback = null)
        {
            VerifyLoadedAsync(callback);
        }

        /// <summary>
        /// Gets whether this Connection supports paging via the <see cref="NextPage"/> and <see cref="PreviousPage"/> properties.
        /// </summary>
        /// <remarks>
        /// <para>This property requires that the destination data is loaded.  If the data is not loaded (the <see cref="IsLoaded"/> property returns <see langword="false"/>, then 
        /// this property will return <see langword="null"/>.</para>
        /// </remarks>
        public bool? SupportsPaging
        {
            get
            {
                if (!IsLoaded)
                    return null;

                return m_sourceData["paging"] != null;
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets a <see cref="Connection{TEntity}"/> representing the next page in the connection.
        /// </summary>
        public Connection<TEntity> NextPage
        {
            get
            {
                VerifyLoaded();
                JToken paging = m_sourceData["paging"];
                if (paging == null) 
                    return null;
                return new Connection<TEntity>(m_sourceEntityID, m_connectionName, (string)m_sourceData["paging"]["next"], null, Session);
            }
        }

        /// <summary>
        /// Gets a <see cref="Connection{TEntity}"/> representing the previous page in the connection.
        /// </summary>
        public Connection<TEntity> PreviousPage
        {
            get
            {
                VerifyLoaded();
                JToken paging = m_sourceData["paging"];
                if (paging == null)
                    return null;
                return new Connection<TEntity>(m_sourceEntityID, m_connectionName, (string)m_sourceData["paging"]["previous"], null, Session);
            }
        }
#endif

        #region IEnumerable<TEntity> Members


        /// <inheritdoc />
        public virtual IEnumerator<TEntity> GetEnumerator()
        {
#if !SILVERLIGHT
            if (m_relatedUrl == null)
                throw new NotSupportedException("The related URL of this Connection is null.");

            VerifyLoaded();

            JArray dataArray = (JArray)m_sourceData["data"];
            if (dataArray.Count == 0)
                yield break;

            foreach (var item in dataArray)
            {
                var entity = EntityFromTokenFactory<TEntity>.Create(item, Session, true);
                yield return entity;
            }
#else
            throw new NotSupportedException();
#endif
        }

        /// <summary>
        /// Asynchronously requests an enumerator for this connection.
        /// </summary>
        /// <param name="callback">A reference to the method to call upon completion of the request.</param>
        public virtual void GetEnumeratorAsync(Action<IEnumerable<TEntity>> callback)
        {
            if (m_relatedUrl == null)
                throw new NotSupportedException("The related URL of this Connection is null.");

            VerifyLoadedAsync(() =>
            {
                JArray dataArray = (JArray)m_sourceData["data"];
                List<TEntity> results = new List<TEntity>(dataArray.Count);

                foreach (var item in dataArray)
                {
                    var entity = EntityFromTokenFactory<TEntity>.Create(item, Session, true);
                    results.Add(entity);
                }

                ReadOnlyCollection<TEntity> result = new ReadOnlyCollection<TEntity>(results);
                callback(result);
            });
        }

#if !SILVERLIGHT
        [DebuggerStepThrough]
        private void VerifyLoaded()
        {
            if (!IsLoaded)
            {
                m_sourceData = Fetcher.FetchToken(m_relatedUrl);

                if (m_sourceData.HasValues && m_sourceData["error"] != null)
                    throw ExceptionParser.Parse(m_sourceData["error"]);
            }
        }
#endif

        [DebuggerStepThrough]
        private void VerifyLoadedAsync(Action callback)
        {
            if (!IsLoaded)
            {
                Fetcher.FetchTokenAsync(m_relatedUrl,
                    token =>
                    {
                        m_sourceData = token;
                        if (m_sourceData.HasValues && m_sourceData["error"] != null)
                        {
                            Session.OnAsynchronousException(new AsynchronousGraphExceptionEventArgs(ExceptionParser.Parse(m_sourceData["error"])));
                        }

                        callback();
                    },
                    token =>
                    {
                        Session.OnAsynchronousException(new AsynchronousGraphExceptionEventArgs(ExceptionParser.Parse(token)));
                    });
            }
            else
            {
                callback();
            }
        }

        #endregion

        #region IEnumerable Members
        /// <inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Facebook.Graph
{
    /// <summary>
    /// Represents an enumerable search result for a specific entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of Graph entity being searched.</typeparam>
    public class SearchResult<TEntity> : IEnumerable<TEntity>
    {
        /// <summary>
        /// Creates a new <see cref="SearchResult{TEntity}"/>.
        /// </summary>
        /// <exception cref="NotImplementedException">Thrown in all cases because this class is not yet implemented.</exception>
        public SearchResult()
        {
            throw new NotImplementedException();
        }

        #region IEnumerable<TEntity> Members

        /// <inheritdoc/>
        public IEnumerator<TEntity> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        /// <inheritdoc/>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region paging
        /*
        /// <summary>
        /// Gets whether the next page of data should be loaded automatically.
        /// </summary>
        /// <remarks>
        /// <para>During an enumeration, if this property is set to <see langword="false" />, the <see cref="PageCompleted"/> event will be raised.
        /// To retrieve the next page of data, handle that event and set the <see cref="PageCompletedEventArgs.ShouldGetNextPage"/> property to 
        /// <see langword="true" />.</para>
        /// <note type="caution">It is recommended that you do not use automatic paging when enumerating over very large collections such as feeds, or else you will quickly run into the rate limiter.
        /// Instead, enumerate using smaller pages using the <see cref="PageCompleted"/> event:</note>
        /// <example>
        /// <para>This example demonstrates how to use the <see cref="PageCompleted"/> event instead of automatic paging:</para>
        /// <code lang="csharp">
        /// <![CDATA[
        /// static void PrintPosts(OpenGraphSession session, int limit)
        /// {
        ///     var me = session.GetMe();
        ///     me.Posts.AutoPage = false;
        ///     int count = 0;
        ///     me.Posts.PageCompleted += delegate(object sender, PageCompletedEventArgs e)
        ///     {
        ///         if (count < limit)
        ///             e.ShouldGetNextPage = true;
        ///     };
        ///     
        ///     foreach (var post in me.Posts)
        ///     {
        ///         count++;
        ///         Console.WriteLine(post.Message);
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// <para>Alternatively, you can and should use automatic paging if you only intend to access a set number of elements from the enumeration.  For example, if you are using 
        /// the LINQ-to-Objects extension method <see cref="System.Linq.Enumerable.Take{TSource}(System.Collections.Generic.IEnumerable{TSource}, System.Int32)"/> with a set number of entities to 
        /// return, then it could be usable without hitting the rate limiter:</para>
        /// <example>
        /// <code lang="csharp">
        /// <![CDATA[
        /// static void PrintPosts(OpenGraphSession session, int limit)
        /// {
        ///     var me = session.GetMe();
        ///     me.Posts.AutoPage = true;
        ///     foreach (var post in me.Posts.Take(limit))
        ///     {
        ///         Console.WriteLine(post.Message);
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// </remarks>
        public bool AutoPage
        {
            get;
            set;
        }

        /// <summary>
        /// Fired when <see cref="AutoPage"/> is set to <see langword="false"/> but the connection supports paging, during an enumeration.
        /// </summary>
        public event PageCompletedEventHandler PageCompleted;
        /// <summary>
        /// Invokes the <see cref="PageCompleted"/> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnPageCompleted(PageCompletedEventArgs e)
        {
            if (PageCompleted != null)
                PageCompleted(this, e);
        }

        private bool ShouldContinuePaging()
        {
            if (AutoPage) return true;

            PageCompletedEventArgs e = new PageCompletedEventArgs();
            OnPageCompleted(e);
            return e.ShouldGetNextPage;
        }
         * */
        #endregion
    }
}

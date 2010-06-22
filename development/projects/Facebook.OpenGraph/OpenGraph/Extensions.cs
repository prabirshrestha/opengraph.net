using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook.OpenGraph;
using System.ComponentModel;

namespace Facebook
{
    partial class Extensions
    {
        private static Dictionary<OpenGraphObjectType, string> OgotMap = new Dictionary<OpenGraphObjectType, string> 
        {
            /* activities */
            { OpenGraphObjectType.Activity, "activity" },
            { OpenGraphObjectType.Sport, "sport" },
            /* businesses */
            { OpenGraphObjectType.Bar, "bar" },
            { OpenGraphObjectType.Company, "company" },
            { OpenGraphObjectType.Cafe, "cafe" },
            { OpenGraphObjectType.Hotel, "hotel" },
            { OpenGraphObjectType.Restaurant, "restaurant" },
            /* groups */
            { OpenGraphObjectType.Cause, "cause" },
            { OpenGraphObjectType.SportsLeague, "sports_league" },
            { OpenGraphObjectType.SportsTeam, "sports_team" },
            /* organizations */
            { OpenGraphObjectType.Band, "band" },
            { OpenGraphObjectType.Government, "government" },
            { OpenGraphObjectType.NonProfit, "non_profit" },
            { OpenGraphObjectType.School, "school" },
            { OpenGraphObjectType.University, "university" },
            /* people */
            { OpenGraphObjectType.Actor, "actor" },
            { OpenGraphObjectType.Athlete, "athlete"},
            { OpenGraphObjectType.Author, "author"},
            { OpenGraphObjectType.Director, "director"},
            { OpenGraphObjectType.Musician, "musician"},
            { OpenGraphObjectType.Politician, "politician"},
            { OpenGraphObjectType.PublicFigure, "public_figure"},
            /* places */
            { OpenGraphObjectType.City, "city"},
            { OpenGraphObjectType.Country, "country"},
            { OpenGraphObjectType.Landmark, "landmark"},
            { OpenGraphObjectType.StateOrProvince, "state_province"},
            /* products and entertainment */
            { OpenGraphObjectType.Album, "album"},
            { OpenGraphObjectType.Book, "book"},
            { OpenGraphObjectType.Drink, "drink"},
            { OpenGraphObjectType.Game, "game"},
            { OpenGraphObjectType.Food, "food"},
            { OpenGraphObjectType.Product, "product"},
            { OpenGraphObjectType.Song, "song"},
            { OpenGraphObjectType.Movie, "movie"},
            { OpenGraphObjectType.TelevisionShow, "tv_show"},
            /* websites */
            { OpenGraphObjectType.Blog, "blog"},
            { OpenGraphObjectType.Article, "article"},
            { OpenGraphObjectType.Website, "website"}
        };

        // TODO: Document
        /// <summary>
        /// Formats the value of an <see cref="OpenGraphObjectType"/> value into a string.
        /// </summary>
        /// <param name="srcType">The <see cref="OpenGraphObjectType"/> value to render.</param>
        /// <returns>A string representing the value.</returns>
        /// <exception cref="InvalidEnumArgumentException">[Non-Silverlight] Thrown when <paramref name="srcType"/> is not a valid
        /// value of <see cref="OpenGraphObjectType"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">[Silverlight] Thrown when <paramref name="srcType"/> is not a valid value
        /// of <see cref="OpenGraphObjectType"/>.</exception>
        public static string Format(this OpenGraphObjectType srcType)
        {
            if (!Enum.IsDefined(typeof(OpenGraphObjectType), srcType))
            {
#if !SILVERLIGHT
                throw new InvalidEnumArgumentException("srcType", (int)srcType, typeof(OpenGraphObjectType));
#else
                throw new ArgumentOutOfRangeException("srcType", "Specified value is not a valid OpenGraphObjectType.");
#endif
            }

            string result;
            if (OgotMap.TryGetValue(srcType, out result))
                return result;

#if !SILVERLIGHT
            return srcType.ToString().ToLowerInvariant();
#else
            return srcType.ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture);
#endif
        }
    }
}

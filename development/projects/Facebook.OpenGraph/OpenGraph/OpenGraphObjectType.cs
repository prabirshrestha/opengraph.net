using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Facebook.OpenGraph
{
    /// <summary>
    /// Specifies the list of object types supported by the OpenGraph protocol.
    /// </summary>
    public enum OpenGraphObjectType
    {
        /// <summary>
        /// Specifies a general activity.
        /// </summary>
        Activity,
        /// <summary>
        /// Specifies a sport.
        /// </summary>
        Sport,

        /// <summary>
        /// Specifies a bar.
        /// </summary>
        Bar,
        /// <summary>
        /// Specifies a company.
        /// </summary>
        Company,
        /// <summary>
        /// Specifies a cafe.
        /// </summary>
        Cafe,
        /// <summary>
        /// Specifies a hotel.
        /// </summary>
        Hotel,
        /// <summary>
        /// Specifies a restaurant.
        /// </summary>
        Restaurant,

        /// <summary>
        /// Specifies a cause or a general reason for people to aggregate.
        /// </summary>
        Cause,
        /// <summary>
        /// Specifies a sports league.
        /// </summary>
        SportsLeague,
        /// <summary>
        /// Specifies a sports team.
        /// </summary>
        SportsTeam,

        /// <summary>
        /// Specifies a band.
        /// </summary>
        Band,
        /// <summary>
        /// Specifies a government entity.
        /// </summary>
        Government,
        /// <summary>
        /// Specifies a non-profit organization.
        /// </summary>
        NonProfit,
        /// <summary>
        /// Specifies a school.
        /// </summary>
        School,
        /// <summary>
        /// Specifies a university.
        /// </summary>
        University,

        /// <summary>
        /// Specifies an actor.
        /// </summary>
        Actor,
        /// <summary>
        /// Specifies an athlete.
        /// </summary>
        Athlete,
        /// <summary>
        /// Specifies an author.
        /// </summary>
        Author,
        /// <summary>
        /// Specifies a director.
        /// </summary>
        Director,
        /// <summary>
        /// Specifies a musician.
        /// </summary>
        Musician,
        /// <summary>
        /// Specifies a politician.
        /// </summary>
        Politician,
        /// <summary>
        /// Specifies a public figure.
        /// </summary>
        PublicFigure,

        /// <summary>
        /// Specifies a city.
        /// </summary>
        City,
        /// <summary>
        /// Specifies a country.
        /// </summary>
        Country,
        /// <summary>
        /// Specifies a landmark.
        /// </summary>
        Landmark,
        /// <summary>
        /// Specifies a state or province.
        /// </summary>
        StateOrProvince,

        /// <summary>
        /// Specifies an album.
        /// </summary>
        Album,
        /// <summary>
        /// Specifies a book.
        /// </summary>
        Book,
        /// <summary>
        /// Specifies a drink.
        /// </summary>
        Drink,
        /// <summary>
        /// Specifies a game.
        /// </summary>
        Game,
        /// <summary>
        /// Specifies a food product.
        /// </summary>
        Food,
        /// <summary>
        /// Specifies a generic product.
        /// </summary>
        Product,
        /// <summary>
        /// Specifies a song.
        /// </summary>
        Song,
        /// <summary>
        /// Specifies a movie.
        /// </summary>
        Movie,
        /// <summary>
        /// Specifies a television show.
        /// </summary>
        TelevisionShow,

        /// <summary>
        /// Specifies a blog.  This should be used for the root of a blog and not for blog articles.
        /// </summary>
        Blog,
        /// <summary>
        /// Specifies an article on a website.  This should be used to specify transient content.
        /// </summary>
        Article,
        /// <summary>
        /// Specifies a website.  This should be used for the root of a website.
        /// </summary>
        Website,
    }
}

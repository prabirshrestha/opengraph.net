using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Facebook.Graph
{
    /// <summary>
    /// Specifies the types of permissions allowed by OpenGraph.
    /// </summary>
    [Flags]
    public enum ExtendedPermissions : long
    {
        /// <summary>
        /// Specifies that no extended permissions are required.
        /// </summary>
        None = 0,
        /// <summary>
        /// When combined with other flags, indicates that the Friend version of the permission
        /// should be requested, instead of the user version.  Not all flags contain Friend versions,
        /// and will be noted where they do not.
        /// </summary>
        Friend = 1,

        /// <summary>
        /// Requests that the OAuth access token generated will persist for longer than the 
        /// default session length.  <note type="implementnotes">This extended permission does not
        /// have a related Friend combination.</note>
        /// </summary>
        OfflineAccess = 2,

        /// <summary>
        /// Requests access to the posts in the user's News Feed, and enables the application to 
        /// perform searches against it. <note type="implementnotes">This extended permission does not
        /// have a related Friend combination.</note>
        /// </summary>
        ReadStream = 4,
        /// <summary>
        /// Enables the application to post content, comments, and likes to a user's stream and 
        /// to the streams of the user's friends, without prompting the user each time.
        /// <note type="implementnotes">This extended permission does not
        /// have a related Friend combination.</note>
        /// </summary>
        PublishStream = 8,

        /// <summary>
        /// Provides access to view the user's friend lists. <note type="implementnotes">This extended permission does not
        /// have a related Friend combination.</note>
        /// </summary>
        ReadFriendLists = 0x10,
        /// <summary>
        /// Provides access to read a user's friend requests.  <note type="implementnotes">This extended permission does not
        /// have a related Friend combination.</note>
        /// </summary>
        ReadRequests = 0x20,

        /// <summary>
        /// Provides access to a user's private message inbox.
        /// <note type="implementnotes">This extended permission does not
        /// have a related Friend combination.</note>
        /// </summary>
        ReadMailbox = 0x40,

        /// <summary>
        /// Provides access to utilize a user's Facebook SMS service.
        /// <note type="implementnotes">This extended permission does not
        /// have a related Friend combination.</note>
        /// </summary>
        Sms = 0x80,

        /// <summary>
        /// Provides access to create and modify events on the user's behalf.
        /// <note type="implementnotes">This extended permission does not
        /// have a related Friend combination.</note>
        /// </summary>
        CreateEvent = 0x100,
        /// <summary>
        /// Provides access to RSVP to events on the user's behalf.  <note type="implementnotes">This extended permission does not
        /// have a related Friend combination.</note>
        /// </summary>
        RsvpEvent = 0x200,
        /// <summary>
        /// Allows the user access to an email address for you.  <note type="implementnotes">This extended permission does not
        /// have a related Friend combination.</note>
        /// </summary>
        Email = 0x400,


        /// <summary>
        /// Does not have a specific meaning; reserved for future versions.
        /// </summary>
        /// <remarks>
        /// <para>This is actually a lie; this separates the permissions that have friend versions
        /// from those that do not, to determine how to aggregate appropriate friend permissions.</para>
        /// </remarks>
        Reserved = 0x8000,

        /// <summary>
        /// Provides access to the "About Me" section of the profile in the 
        /// <see cref="User.About"/> property.
        /// </summary>
        AboutMe = 0x20000,

        /// <summary>
        /// Provides access to the user's list of activities in the 
        /// <see cref="User.Activities"/> connection.
        /// </summary>
        Activities = 0x40000,

        /// <summary>
        /// Provides access to the user's full birthday, including the year, as the 
        /// <see cref="User.Birthday"/> property.
        /// </summary>
        Birthday = 0x80000,

        /// <summary>
        /// Provides access to the user's education history via the 
        /// <see cref="User.Education"/> property.
        /// </summary>
        EducationHistory = 0x100000,

        /// <summary>
        /// Provides access to the list of events the user is attending as the 
        /// <see cref="User.Events"/> connection.
        /// </summary>
        Events = 0x200000,  // can be combined with Friend

        /// <summary>
        /// Provides access to the list of groups in which the user is a member via the
        /// <see cref="User.Groups"/> connection.
        /// </summary>
        Groups = 0x400000,

        /// <summary>
        /// Provides access to the user's hometown as the <see cref="User.Hometown"/> property.
        /// </summary>
        Hometown = 0x800000,

        /// <summary>
        /// Provides access to the user's interests as the <see cref="User.Interests"/> connection.
        /// </summary>
        Interests = 0x1000000,

        /// <summary>
        /// Provides access to the user's likes as the <see cref="User.Likes"/> connection.
        /// </summary>
        Likes = 0x2000000,

        /// <summary>
        /// Provides access to the user's current location as the <see cref="User.Location"/> property.
        /// </summary>
        Location = 0x4000000,

        /// <summary>
        /// Provides access to the user's notes as the <see cref="User.Notes"/> connection.
        /// </summary>
        Notes = 0x8000000,

        /// <summary>
        /// Provides access to the user's online/offline presence via the <see cref="User.IsOnline"/> property.
        /// </summary>
        OnlinePresence = 0x10000000,

        /// <summary>
        /// Provides access to the photos and videos in which the user has been tagged.
        /// </summary>
        PhotoVideoTags = 0x20000000,
        
        /// <summary>
        /// Provides access to the photos the user has uploaded via the <see cref="User.Photos"/>
        /// connection.
        /// </summary>
        Photos = 0x40000000,

        /// <summary>
        /// Provides access to the user's family and personal relationships and relationship status.
        /// </summary>
        Relationships = 0x80000000,

        /// <summary>
        /// Provides access to the user's religion and polical affiliations.
        /// </summary>
        ReligionPolitics = 0x100000000,

        /// <summary>
        /// Provides access to the user's most recent status message.
        /// </summary>
        Status = 0x200000000,

        /// <summary>
        /// Provides access to videos the user has uploaded.
        /// </summary>
        Videos = 0x400000000,

        /// <summary>
        /// Provides access to the user's web site URL.
        /// </summary>
        Website = 0x800000000,

        /// <summary>
        /// Provides access to the user's work history via the <see cref="User.Work"/> property.
        /// </summary>
        WorkHistory = 0x1000000000,
    }
}

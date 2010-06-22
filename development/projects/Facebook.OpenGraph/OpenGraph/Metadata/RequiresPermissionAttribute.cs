using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Facebook.Graph;
using EP = Facebook.Graph.ExtendedPermissions;

namespace Facebook.OpenGraph.Metadata
{
    /// <summary>
    /// Indicates that a property requires extended permissions to access.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class RequiresPermissionAttribute : Attribute
    {
        internal static Dictionary<ExtendedPermissions, string> DefinedPermissions = new Dictionary<ExtendedPermissions, string> 
        {
            { EP.OfflineAccess, "offline_access" },
            { EP.ReadStream, "read_stream" },
            { EP.PublishStream, "publish_stream" },
            { EP.ReadFriendLists, "read_friendlists" },
            { EP.ReadRequests, "read_requests" },
            { EP.ReadMailbox, "read_mailbox" },
            { EP.Sms, "sms" },
            { EP.Email, "email" },

            { EP.AboutMe, "user_about_me" },
            { EP.AboutMe | EP.Friend, "friends_about_me" },

            { EP.Activities, "user_activities" },
            { EP.Activities | EP.Friend, "friends_activities" },

            { EP.Birthday, "user_birthday" },
            { EP.Birthday | EP.Friend, "friends_birthday" },

            { EP.EducationHistory, "user_education_history" },
            { EP.EducationHistory | EP.Friend, "friends_education_history" },

            { EP.CreateEvent, "create_event" },
            { EP.RsvpEvent, "rsvp_event" },
            { EP.Events, "user_events" },
            { EP.Events | EP.Friend, "friends_events" },

            { EP.Groups, "user_groups" },
            { EP.Groups | EP.Friend, "friends_groups" },

            { EP.Hometown, "user_hometown" },
            { EP.Hometown | EP.Friend, "friends_hometown" },

            { EP.Interests, "user_interests" },
            { EP.Interests | EP.Friend, "friends_interests" },

            { EP.Likes, "user_likes" },
            { EP.Likes | EP.Friend, "friend_likes" },

            { EP.Location, "user_location" },
            { EP.Location | EP.Friend, "friends_location" },

            { EP.Notes, "user_notes" },
            { EP.Notes | EP.Friend, "friends_notes" },

            { EP.OnlinePresence, "user_online_presence" },
            { EP.OnlinePresence | EP.Friend, "friends_online_presence" },

            { EP.PhotoVideoTags, "user_photo_video_tags" },
            { EP.PhotoVideoTags | EP.Friend, "friends_photo_video_tags" },

            { EP.Photos, "user_photos" },
            { EP.Photos | EP.Friend, "friends_photos" },

            { EP.Relationships, "user_relationships" },
            { EP.Relationships | EP.Friend, "friends_relationships" },

            { EP.ReligionPolitics, "user_religion_politics" },
            { EP.ReligionPolitics | EP.Friend, "friends_religion_politics" },

            { EP.Status, "user_status" },
            { EP.Status | EP.Friend, "friends_status" },

            { EP.Videos, "user_videos" },
            { EP.Videos | EP.Friend, "friends_videos" },

            { EP.Website, "user_website" },
            { EP.Website | EP.Friend, "friends_website" },

            { EP.WorkHistory, "user_work_history" },
            { EP.WorkHistory | EP.Friend, "friends_work_history" }
        };

        internal static Dictionary<string, ExtendedPermissions> PermissionMap =
            DefinedPermissions.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);


        private string m_name;
        /// <summary>
        /// Gets or sets the name of the extended permission required by the OpenGraph API.
        /// </summary>
        /// <remarks>
        /// <para>This property is provided for forward-compatibility by allowing the extended-permissions API to require
        /// specific strings without requiring an update to the <see>ExtendedPermission</see> enumeration.  It does not
        /// perform any parameter validation.</para>
        /// </remarks>
        public string PermissionName
        {
            get;
            set;
        }

        private ExtendedPermissions m_permission;
        /// <summary>
        /// Gets or sets the <see>ExtendedPermission</see> that is represented by this attribute.
        /// </summary>
        /// <remarks>
        /// <para>If this property is successfully set, it has the side effect of overwriting the <see>PermissionName</see>
        /// property with the related OpenGraph permission name.</para>
        /// </remarks>
        /// <exception cref="InvalidEnumArgumentException">Thrown if <paramref name="value"/> is not defined by the 
        /// <see>ExtendedPermission</see> enumeration.</exception>
        public ExtendedPermissions Permission
        {
            get
            {
                return m_permission;
            }
            set
            {
                if (!DefinedPermissions.TryGetValue(value, out m_name))
#if !SILVERLIGHT
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ExtendedPermissions));
#else
                    throw new ArgumentOutOfRangeException("value", "Specified value does not exist for ExtendedPermissions.");
#endif

                m_permission = value;
            }
        }

        /// <summary>
        /// Gets or sets whether this permission is optional for retrieving the applied property.
        /// </summary>
        public bool IsOptional
        {
            get;
            set;
        }
    }
}

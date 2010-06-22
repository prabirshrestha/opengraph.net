using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;

namespace Facebook.OpenGraph.Web
{
    /// <summary>
    /// Presents Open Graph protocol data.
    /// </summary>
    public class OpenGraphData : Control
    {
        private const string REQUIRED = "Open Graph-Required Properties";
        private const string RECOMMENDED = "Open Graph-Recommended Metadata";
        private const string CONTACT = "Open Graph Contact Metadata";
        private const string FACEBOOK = "Facebook Administration Metadata";

        private const string TITLE = "og:title", TYPE = "og:type", URL = "og:url", IMAGE = "og:image", SITE_NAME = "og:site_name";
        private const string ADMINS = "fb:admins", APP_ID = "fb:app_id";
        private const string DESC = "og:description", LAT = "og:latitude", LONG = "og:longitude", ADDR = "og:street-address", LOC = "og:locality", REG = "og:region", ZIP = "og:postal-code",
            COUNTRY = "og:country-name";
        private const string EMAIL = "og:email", PHONE = "og:phone_number", FAX = "og:fax_number";

        private string m_title, m_url;
        private OpenGraphObjectType m_type = OpenGraphObjectType.Article;

        /// <summary>
        /// Creates a new <see cref="OpenGraphData"/> control.
        /// </summary>
        public OpenGraphData()
        {
            
        }

        #region required properties
        /// <summary>
        /// Gets or sets the title to use as the Open Graph page title.
        /// </summary>
        /// <remarks>
        /// <para>This property is required by the Open Graph protocol.  However, if you do not set it, the control will attempt to read the page title from the 
        /// <see cref="Page.Title">Page.Title</see> property.</para>
        /// <para>Failing to set this property may result in an <see cref="InvalidOperationException"/> being thrown during rendering, if the <see cref="RequireValues"/> property is set to 
        /// <see langword="true"/>, if this property is not set or set to an empty string.</para>
        /// </remarks>
        [Category(REQUIRED)]
        [Description("Specifies the title of the page.")]
        [DefaultValue(null)]
        public string Title
        {
            get
            {
                if (!string.IsNullOrEmpty(m_title))
                    return m_title;
                if (Page != null)
                {
                    return Page.Title;
                }

                return null;
            }
            set
            {
                m_title = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to throw an exception during rendering if a required property is not set.
        /// </summary>
        [Category("Behavior")]
        [Description("Specifies whether the control should throw an exception upon rendering if the Open Graph required properties are not set.")]
        [DefaultValue(false)]
        public bool RequireValues { get; set; }


        /// <summary>
        /// Gets or sets the Open Graph object type to use for the page.
        /// </summary>
        /// <remarks>
        /// <para>This property is required by the Open Graph protocol.  Its default value is <see cref="OpenGraphObjectType">OpenGraphObjectType.Article</see>, which specifies that it is considered
        /// to be transient content.  For more information, see the <see href="http://developers.facebook.com/docs/opengraph#types" target="_blank">Object Types</see> documentation within the 
        /// <see href="http://developers.facebook.com/docs/opengraph" target="_blank">Open Graph protocol</see> documentation on Facebook.</para>
        /// </remarks>
        /// <exception cref="InvalidEnumArgumentException">Thrown if set to a value that is not a valid value for <see cref="OpenGraphObjectType"/>.</exception>
        /// <seealso href="http://developers.facebook.com/docs/opengraph#types" target="_blank">Object types - Open Graph protocol documentation</seealso>
        [Category(REQUIRED)]
        [Description("Specifies the type of content being served by this page.")]
        [DefaultValue(OpenGraphObjectType.Article)]
        public OpenGraphObjectType Type 
        {
            get
            {
                return m_type;
            }
            set
            {
                if (!Enum.IsDefined(typeof(OpenGraphObjectType), value))
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(OpenGraphObjectType));

                m_type = value;
            }
        }

        /// <summary>
        /// Gets or sets the URL of the page.
        /// </summary>
        /// <remarks>
        /// <para>This property is required by the Open Graph protocol.  However, if you do not set it, the control will attempt to read the page URL from the 
        /// <see cref="System.Web.HttpRequest.Url">Page.Request.Url</see> property and return a string representing that value.</para>
        /// <para>Failing to set this property may result in an <see cref="InvalidOperationException"/> being thrown during rendering, if the <see cref="RequireValues"/> property is set to 
        /// <see langword="true"/>, if this property is not set or set to an empty string.</para>
        /// </remarks>
        [Category(REQUIRED)]
        [Description("Specifies the URL of the page.")]
        [DefaultValue(null)]
        public string Url 
        {
            get
            {
                if (!string.IsNullOrEmpty(m_url))
                    return m_url;
                if (Page != null)
                {
                    return Page.Request.Url.ToString();
                }

                return null;
            }
            set
            {
                m_url = value;
            }
        }

        /// <summary>
        /// Gets or sets an image that represents the content on this page.
        /// </summary>
        /// <remarks>
        /// <para>This property is required by the Open Graph protocol.  Failing to set this property may result in an <see cref="InvalidOperationException"/> being thrown during rendering, 
        /// if the <see cref="RequireValues"/> property is set to <see langword="true"/>, if this property is not set or set to an empty string.</para>
        /// </remarks>
        [Category(REQUIRED)]
        [Description("Specifies an image that corresponds to the page.")]
        [DefaultValue(null)]
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets the name of your site.
        /// </summary>
        /// <remarks>
        /// <para>This property is required by the Open Graph protocol.  Failing to set this property may result in an <see cref="InvalidOperationException"/> being thrown during rendering, 
        /// if the <see cref="RequireValues"/> property is set to <see langword="true"/>, if this property is not set or set to an empty string.</para>
        /// </remarks>
        [Category(REQUIRED)]
        [Description("Specifies an image that corresponds to the page.")]
        [DefaultValue(null)]
        public string SiteName { get; set; }
        #endregion

        #region Facebook properties

        /// <summary>
        /// Gets or sets an array of Facebook user IDs that correspond to users who have administrative access to this page.
        /// </summary>
        /// <remarks>
        /// <para>This property should not be set via markup; instead, use the <see cref="AdminList"/> property, which accepts a comma-delimited list of strings.  This property is provided as a convenience 
        /// for users who want to bind to an array object, and is dynamically updated whenever a change is made to the <see cref="AdminList"/> property..</para>
        /// </remarks>
        [Category(FACEBOOK)]
        [Description("Specifies a list of Facebook user IDs of users who have admin access.")]
        [DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string[] AdminFacebookIDs
        {
            get
            {
                if (AdminList == null)
                {
                    AdminList = string.Empty;
                }
                return AdminList.Split(',');
            }
            set
            {
                AdminList = string.Join(",", value);
            }
        }

        /// <summary>
        /// Gets or sets a comma-delimited list of Facebook user IDs that correspond to users who have administrative access to this page.
        /// </summary>
        [Category(FACEBOOK)]
        [Description("Specifies a comma-delimited list of Facebook user IDs of users who have admin access.")]
        [DefaultValue("")]
        public string AdminList { get; set; }

        /// <summary>
        /// Gets or sets a Facebook application ID, the developers of which have administrative access to this page.
        /// </summary>
        [Category(FACEBOOK)]
        [Description("Specifies a Facebook application ID, the developers of which have admin access.")]
        [DefaultValue(null)]
        public string AppID { get; set; }
        #endregion

        #region Recommended properties
        /// <summary>
        /// Gets or sets a description for the content on this page.
        /// </summary>
        [Category(RECOMMENDED)]
        [Description("Specifies a description for the content on this page.")]
        [DefaultValue(null)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the latitude for a location.
        /// </summary>
        [Category(RECOMMENDED)]
        [Description("Specifies the latitude for a location.")]
        [DefaultValue(0)]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude for a location.
        /// </summary>
        [Category(RECOMMENDED)]
        [Description("Specifies the longitude for a location.")]
        [DefaultValue(0)]
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the street address for a location.
        /// </summary>
        [Category(RECOMMENDED)]
        [Description("Specifies the street address for a location.")]
        [DefaultValue(null)]
        public string StreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the locality for a location, such as Palo Alto.
        /// </summary>
        [Category(RECOMMENDED)]
        [Description("Specifies the locality (for instance, Palo Alto) for a location.")]
        [DefaultValue(null)]
        public string Locality { get; set; }
        
        /// <summary>
        /// Gets or sets the region for a location, such as California.
        /// </summary>
        [Category(RECOMMENDED)]
        [Description("Specifies the region (for instance, CA) for a location.")]
        [DefaultValue(null)]
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the postal code for a location.
        /// </summary>
        [Category(RECOMMENDED)]
        [Description("Specifies the postal code for a location.")]
        [DefaultValue(null)]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country for a location.
        /// </summary>
        [Category(RECOMMENDED)]
        [Description("Specifies the country for a location.")]
        [DefaultValue(null)]
        public string Country { get; set; }
        #endregion

        #region Contact properties
        /// <summary>
        /// Gets or sets a contact email address for an entity or location.
        /// </summary>
        [Category(CONTACT)]
        [Description("Specifies a contact email address for an entity or location.")]
        [DefaultValue(null)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a phone number of a given entity or location.
        /// </summary>
        [Category(CONTACT)]
        [Description("Specifies a phone number for an entity or location.")]
        [DefaultValue(null)]
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the fax number of a given entity or location.
        /// </summary>
        [Category(CONTACT)]
        [Description("Specifies the fax number of an entity or location.")]
        [DefaultValue(null)]
        public string Fax { get; set; }
        #endregion

        /// <inheritdoc />
        protected override void Render(HtmlTextWriter writer)
        {
            if (RequireValues)
            {
                if (string.IsNullOrEmpty(Title)) throw new InvalidOperationException("Open Graph protocol violation: Title must be specified.");
                if (string.IsNullOrEmpty(Image)) throw new InvalidOperationException("Open Graph protocol violation: Image must be specified.");
                if (string.IsNullOrEmpty(Url)) throw new InvalidOperationException("Open Graph protocol violation: Url must be specified.");
                if (string.IsNullOrEmpty(SiteName)) throw new InvalidOperationException("Open Graph protocol violation: Site name must be specified.");
            }

            Dictionary<string, string> propertiesToRender = new Dictionary<string, string>()
            {
                { TITLE, Title },
                { TYPE, Type.Format() },
                { URL, Url },
                { IMAGE, Image },
                { SITE_NAME, SiteName },
                { ADMINS, AdminList },
                { APP_ID, AppID },
                { DESC, Description },
                { ADDR, StreetAddress },
                { LOC, Locality },
                { REG, Region },
                { ZIP, PostalCode },
                { COUNTRY, Country },
                { EMAIL, Email },
                { PHONE, Phone },
                { FAX, Fax }
            };
            if (Latitude != 0f)
                propertiesToRender.Add(LAT, Latitude.ToString("##0.0#####"));
            if (Longitude != 0f)
                propertiesToRender.Add(LONG, Longitude.ToString("##0.0#####"));

            AddMetaRenderings(propertiesToRender);

            RenderMetaTags(writer, propertiesToRender);
        }

        private void RenderMetaTags(HtmlTextWriter writer, Dictionary<string, string> propertiesToRender)
        {
            foreach (string key in propertiesToRender.Keys)
            {
                string value = propertiesToRender[key];
                if (!string.IsNullOrEmpty(value))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Meta);
                    writer.AddAttribute("property", key, false);
                    writer.AddAttribute("content", value, true);
                    writer.RenderEndTag();
                }
            }
        }

        /// <summary>
        /// Allows derived classes to add additional meta tags.
        /// </summary>
        /// <param name="metaValues">The list of key-value pairs to be encoded.</param>
        protected virtual void AddMetaRenderings(Dictionary<string, string> metaValues)
        {

        }
    }
}

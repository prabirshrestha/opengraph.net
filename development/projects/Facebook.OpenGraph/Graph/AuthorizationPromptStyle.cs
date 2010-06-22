using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Facebook.Graph
{
    /// <summary>
    /// Specifies the kind of authentication prompt to display to the user when requesting authorization.
    /// </summary>
    public enum AuthorizationPromptStyle
    {
        /// <summary>
        /// Displays a full-page authorization screen (the default).
        /// </summary>
        Page ,
        /// <summary>
        /// Displays a compact dialog optimized for web popup windows.
        /// </summary>
        Popup,
        /// <summary>
        /// Displays a WAP/mobile-optimized version of the dialog.
        /// </summary>
        Wireless,
        /// <summary>
        /// Displays an iPhone/Android/smartphone-optimized version of the dialog.
        /// </summary>
        Touch,
    }
}

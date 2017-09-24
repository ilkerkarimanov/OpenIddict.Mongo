﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.Collections.Generic;

namespace OpenIddict.Models
{
    /// <summary>
    /// Represents an OpenIddict application.
    /// </summary>
    public class OpenIddictApplication : OpenIddictApplication<string, OpenIddictAuthorization, OpenIddictToken>
    {
        public OpenIddictApplication()
        {
            // Generate a new string identifier.
            Id = Guid.NewGuid().ToString();
        }
    }

    /// <summary>
    /// Represents an OpenIddict application.
    /// </summary>
    public class OpenIddictApplication<TKey> : OpenIddictApplication<TKey, OpenIddictAuthorization<TKey>, OpenIddictToken<TKey>>
        where TKey : IEquatable<TKey>
    { }

    /// <summary>
    /// Represents an OpenIddict application.
    /// </summary>
    public class OpenIddictApplication<TKey, TAuthorization, TToken> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets or sets the client identifier
        /// associated with the current application.
        /// </summary>
        public virtual string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the hashed client secret
        /// associated with the current application.
        /// </summary>
        public virtual string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the display name
        /// associated with the current application.
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier
        /// associated with the current application.
        /// </summary>
        public virtual TKey Id { get; set; }

        /// <summary>
        /// Gets or sets the logout callback URL
        /// associated with the current application.
        /// </summary>
        public virtual string LogoutRedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the callback URL
        /// associated with the current application.
        /// </summary>
        public virtual string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the application type
        /// associated with the current application.
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// Gets or sets the application status
        /// associated with the current application.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the application origin
        /// associated with the current application.
        /// </summary>
        public string AllowedOrigin { get; set; }
    }
}
/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenIddict.Models;
using OpenIddict.Mongo.Core;
using MongoDB.Driver;
using OpenIddict.Core;

namespace OpenIddict.Mongo.Stores
{
    /// <summary>
    /// Provides methods allowing to manage the applications stored in a database.
    /// </summary>
    /// <typeparam name="TContext">The type of the Entity Framework database context.</typeparam>
    public class OpenIddictApplicationStore<TContext> : OpenIddictApplicationStore<OpenIddictApplication,
                                                                                   OpenIddictAuthorization,
                                                                                   OpenIddictToken,
                                                                                   TContext,
                                                                                   string>
        where TContext : IMongoOpenIddictContext
    {
        public OpenIddictApplicationStore([NotNull]TContext context) : base(context) { }
    }
    /// <summary>
    /// Provides methods allowing to manage the applications stored in a database.
    /// </summary>
    /// <typeparam name="TContext">The type of the Entity Framework database context.</typeparam>
    /// <typeparam name="TKey">The type of the entity primary keys.</typeparam>
    public class OpenIddictApplicationStore<TContext, TKey> : OpenIddictApplicationStore<OpenIddictApplication<TKey>,
                                                                                         OpenIddictAuthorization<TKey>,
                                                                                         OpenIddictToken<TKey>, TContext, TKey>
        where TContext : IMongoOpenIddictContext
        where TKey : IEquatable<TKey>
    {
        public OpenIddictApplicationStore([NotNull]TContext context) : base(context) { }
    }
    /// <summary>
    /// Provides methods allowing to manage the applications stored in a database.
    /// </summary>
    /// <typeparam name="TApplication">The type of the Application entity.</typeparam>
    /// <typeparam name="TAuthorization">The type of the Authorization entity.</typeparam>
    /// <typeparam name="TToken">The type of the Token entity.</typeparam>
    /// <typeparam name="TContext">The type of the Entity Framework database context.</typeparam>
    /// <typeparam name="TKey">The type of the entity primary keys.</typeparam>
    public class OpenIddictApplicationStore<TApplication, TAuthorization, TToken, TContext, TKey> :
        IOpenIddictApplicationStore<TApplication>
        where TApplication : OpenIddictApplication<TKey, TAuthorization, TToken>, new()
        where TAuthorization : OpenIddictAuthorization<TKey, TApplication, TToken>, new()
        where TToken : OpenIddictToken<TKey, TApplication, TAuthorization>, new()
        where TContext : IMongoOpenIddictContext
        where TKey : IEquatable<TKey>
    {
        public OpenIddictApplicationStore([NotNull] TContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Context = context;
        }

        /// <summary>
        /// Gets the database context associated with the current store.
        /// </summary>
        protected virtual TContext Context { get; }

        /// <summary>
        /// Gets the database set corresponding to the <typeparamref name="TApplication"/> entity.
        /// </summary>
        protected IMongoCollection<TApplication> Applications => Context.Database.GetCollection<TApplication>("applications");
        
        /// <summary>
        /// Gets the database set corresponding to the <typeparamref name="TToken"/> entity.
        /// </summary>
        protected IMongoCollection<TToken> Tokens => Context.Database.GetCollection<TToken>("tokens");

        /// <summary>
        /// Creates a new application.
        /// </summary>
        /// <param name="application">The application to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation, whose result returns the application.
        /// </returns>
        public virtual async Task<TApplication> CreateAsync([NotNull] TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            await Applications.InsertOneAsync(application);

            return application;
        }

        /// <summary>
        /// Removes an existing application.
        /// </summary>
        /// <param name="application">The application to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation.
        /// </returns>
        public virtual async Task DeleteAsync(TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }
            var filter = Builders<TApplication>.Filter.Eq(s => s.Id, application.Id);

            await Applications.DeleteOneAsync(filter);
        }

        /// <summary>
        /// Retrieves an application using its unique identifier.
        /// </summary>
        /// <param name="identifier">The unique identifier associated with the application.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns the client application corresponding to the identifier.
        /// </returns>
        public virtual Task<TApplication> FindByIdAsync(string identifier, CancellationToken cancellationToken)
        {
            var key = ConvertIdentifierFromString(identifier);

            var filter = Builders<TApplication>.Filter.Eq(s => s.Id, key);

            var result = Applications.Find(filter);

            return Task.FromResult(result.SingleOrDefault());
        }

        /// <summary>
        /// Retrieves an application using its client identifier.
        /// </summary>
        /// <param name="identifier">The client identifier associated with the application.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns the client application corresponding to the identifier.
        /// </returns>
        public virtual Task<TApplication> FindByClientIdAsync(string identifier, CancellationToken cancellationToken)
        {
            var filter = Builders<TApplication>.Filter.Eq(s => s.ClientId, identifier);

            var result = Applications.Find(filter);

            return Task.FromResult(result.SingleOrDefault());
        }

        /// <summary>
        /// Retrieves an application using its post_logout_redirect_uri.
        /// </summary>
        /// <param name="url">The post_logout_redirect_uri associated with the application.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation, whose result
        /// returns the client application corresponding to the post_logout_redirect_uri.
        /// </returns>
        public virtual Task<TApplication> FindByLogoutRedirectUri(string url, CancellationToken cancellationToken)
        {
            var filter = Builders<TApplication>.Filter.Eq(s => s.LogoutRedirectUri, url);

            var result = Applications.Find(filter);

            return Task.FromResult(result.SingleOrDefault());
        }

        /// <summary>
        /// Retrieves the client identifier associated with an application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns the client identifier associated with the application.
        /// </returns>
        public virtual Task<string> GetClientIdAsync([NotNull]TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return Task.FromResult(application.ClientId);
        }

        /// <summary>
        /// Retrieves the client type associated with an application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns the client type of the application (by default, "public").
        /// </returns>
        public virtual Task<string> GetClientTypeAsync( TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return Task.FromResult(application.Type);
        }

        /// <summary>
        /// Retrieves the display name associated with an application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns the display name associated with the application.
        /// </returns>
        public virtual Task<string> GetDisplayNameAsync(TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return Task.FromResult(application.DisplayName);
        }

        /// <summary>
        /// Retrieves the hashed secret associated with an application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns the hashed secret associated with the application.
        /// </returns>
        public virtual Task<string> GetHashedSecretAsync(TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return Task.FromResult(application.ClientSecret);
        }

        /// <summary>
        /// Retrieves the unique identifier associated with an application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns the unique identifier associated with the application.
        /// </returns>
        public virtual Task<string> GetIdAsync(TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return Task.FromResult(ConvertIdentifierToString(application.Id));
        }

        /// <summary>
        /// Retrieves the logout callback address associated with an application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns the post_logout_redirect_uri associated with the application.
        /// </returns>
        public virtual Task<string> GetLogoutRedirectUriAsync(TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return Task.FromResult(application.LogoutRedirectUri);
        }

        /// <summary>
        /// Retrieves the callback address associated with an application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns the redirect_uri associated with the application.
        /// </returns>
        public virtual Task<string> GetRedirectUriAsync(TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return Task.FromResult(application.RedirectUri);
        }

        /// <summary>
        /// Retrieves the token identifiers associated with an application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns the tokens associated with the application.
        /// </returns>
        public virtual async Task<IEnumerable<string>> GetTokensAsync(TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }
            var filter = Builders<TToken>.Filter.Eq(s => s.ApplicationId, application.Id);

            var result = Tokens.Find(filter).ToList().Select(x => x.Id);
            
            var tokens = new List<string>();

            foreach (var identifier in result)
            {
                tokens.Add(ConvertIdentifierToString(identifier));
            }

            return await Task.FromResult(tokens);
        }

        /// <summary>
        /// Sets the client type associated with an application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="type">The client type associated with the application.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation.
        /// </returns>
        public virtual Task SetClientTypeAsync(TApplication application, string type, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("The client type cannot be null or empty.", nameof(type));
            }

            application.Type = type;

            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets the hashed secret associated with an application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="hash">The hashed client secret associated with the application.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation.
        /// </returns>
        public virtual Task SetHashedSecretAsync(TApplication application,
            string hash, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            application.ClientSecret = hash;

            return Task.FromResult(0);
        }

        /// <summary>
        /// Updates an existing application.
        /// </summary>
        /// <param name="application">The application to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation.
        /// </returns>
        public virtual async Task UpdateAsync(TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            var filter = Builders<TApplication>.Filter.Eq(s => s.Id, application.Id);

            await Applications.ReplaceOneAsync(filter, application);
        }

        /// <summary>
        /// Converts the provided identifier to a strongly typed key object.
        /// </summary>
        /// <param name="identifier">The identifier to convert.</param>
        /// <returns>An instance of <typeparamref name="TKey"/> representing the provided identifier.</returns>
        public virtual TKey ConvertIdentifierFromString(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                return default(TKey);
            }

            return (TKey) TypeDescriptor.GetConverter(typeof(TKey))
                                        .ConvertFromInvariantString(identifier);
        }

        /// <summary>
        /// Converts the provided identifier to its string representation.
        /// </summary>
        /// <param name="identifier">The identifier to convert.</param>
        /// <returns>A <see cref="string"/> representation of the provided identifier.</returns>
        public virtual string ConvertIdentifierToString(TKey identifier)
        {
            if (Equals(identifier, default(TKey)))
            {
                return null;
            }

            return TypeDescriptor.GetConverter(typeof(TKey))
                                 .ConvertToInvariantString(identifier);
        }
    }
}
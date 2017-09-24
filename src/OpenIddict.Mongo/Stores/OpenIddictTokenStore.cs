/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenIddict.Core;
using OpenIddict.Models;
using OpenIddict.Mongo.Core;
using MongoDB.Driver;
using JetBrains.Annotations;

namespace OpenIddict.Mongo.Stores
{
    /// <summary>
    /// Provides methods allowing to manage the tokens stored in a database.
    /// </summary>
    /// <typeparam name="TContext">The type of the Entity Framework database context.</typeparam>
    public class OpenIddictTokenStore<TContext> : OpenIddictTokenStore<OpenIddictToken,
                                                                       OpenIddictApplication,
                                                                       OpenIddictAuthorization, TContext, string>
        where TContext : IMongoOpenIddictContext
    {
        public OpenIddictTokenStore([NotNull] TContext context) : base(context) { }
    }

    /// <summary>
    /// Provides methods allowing to manage the tokens stored in a database.
    /// </summary>
    /// <typeparam name="TContext">The type of the Entity Framework database context.</typeparam>
    /// <typeparam name="TKey">The type of the entity primary keys.</typeparam>
    public class OpenIddictTokenStore<TContext, TKey> : OpenIddictTokenStore<OpenIddictToken<TKey>,
                                                                             OpenIddictApplication<TKey>,
                                                                             OpenIddictAuthorization<TKey>, TContext, TKey>
        where TContext : IMongoOpenIddictContext
        where TKey : IEquatable<TKey>
    {
        public OpenIddictTokenStore([NotNull] TContext context) : base(context) { }
    }

    /// <summary>
    /// Provides methods allowing to manage the tokens stored in a database.
    /// </summary>
    /// <typeparam name="TToken">The type of the Token entity.</typeparam>
    /// <typeparam name="TApplication">The type of the Application entity.</typeparam>
    /// <typeparam name="TAuthorization">The type of the Authorization entity.</typeparam>
    /// <typeparam name="TContext">The type of the Entity Framework database context.</typeparam>
    /// <typeparam name="TKey">The type of the entity primary keys.</typeparam>
    public class OpenIddictTokenStore<TToken, TApplication, TAuthorization, TContext, TKey> : IOpenIddictTokenStore<TToken>
        where TToken : OpenIddictToken<TKey, TApplication, TAuthorization>, new()
        where TApplication : OpenIddictApplication<TKey, TAuthorization, TToken>, new()
        where TAuthorization : OpenIddictAuthorization<TKey, TApplication, TToken>, new()
        where TContext : IMongoOpenIddictContext
        where TKey : IEquatable<TKey>
    {
        public OpenIddictTokenStore([NotNull] TContext context)
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
        /// Gets the database set corresponding to the <typeparamref name="TAuthorization"/> entity.
        /// </summary>
        protected IMongoCollection<TAuthorization> Authorizations => Context.Database.GetCollection<TAuthorization>("authorizations");

        /// <summary>
        /// Gets the database set corresponding to the <typeparamref name="TToken"/> entity.
        /// </summary>
        protected IMongoCollection<TToken> Tokens => Context.Database.GetCollection<TToken>("tokens");

        /// <summary>
        /// Creates a new token.
        /// </summary>
        /// <param name="token">The token to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation, whose result returns the token.
        /// </returns>
        public virtual async Task<TToken> CreateAsync([NotNull] TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            await Tokens.InsertOneAsync(token);

            return token;
        }

        /// <summary>
        /// Creates a new token, which is associated with a particular subject.
        /// </summary>
        /// <param name="type">The token type.</param>
        /// <param name="subject">The subject associated with the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation, whose result returns the token.
        /// </returns>
        public virtual Task<TToken> CreateAsync([NotNull] string type, [NotNull] string subject, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("The token type cannot be null or empty.");
            }

            return CreateAsync(new TToken { Subject = subject, Type = type }, cancellationToken);
        }

        /// <summary>
        /// Retrieves an token using its unique identifier.
        /// </summary>
        /// <param name="identifier">The unique identifier associated with the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns the token corresponding to the unique identifier.
        /// </returns>
        public virtual Task<TToken> FindByIdAsync(string identifier, CancellationToken cancellationToken)
        {
            var key = ConvertIdentifierFromString(identifier);

            var filter = Builders<TToken>.Filter.Eq(s => s.Id, key);

            return Tokens.Find(filter).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves the list of tokens corresponding to the specified subject.
        /// </summary>
        /// <param name="subject">The subject associated with the tokens.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns the tokens corresponding to the specified subject.
        /// </returns>
        public virtual Task<TToken[]> FindBySubjectAsync(string subject, CancellationToken cancellationToken)
        {
            var filter = Builders<TToken>.Filter.Eq(s => s.Subject, subject);

            var result = Tokens.Find(filter).ToList();

            return Task.FromResult(result.ToArray());
        }

        /// <summary>
        /// Retrieves the unique identifier associated with a token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns the unique identifier associated with the token.
        /// </returns>
        public virtual Task<string> GetIdAsync([NotNull] TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return Task.FromResult(ConvertIdentifierToString(token.Id));
        }

        /// <summary>
        /// Retrieves the token type associated with a token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns the token type associated with the specified token.
        /// </returns>
        public virtual Task<string> GetTokenTypeAsync([NotNull] TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return Task.FromResult(token.Type);
        }

        /// <summary>
        /// Retrieves the subject associated with a token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns the subject associated with the specified token.
        /// </returns>
        public virtual Task<string> GetSubjectAsync([NotNull] TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return Task.FromResult(token.Subject);
        }

        /// <summary>
        /// Revokes a token.
        /// </summary>
        /// <param name="token">The token to revoke.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>A <see cref="Task"/> that can be used to monitor the asynchronous operation.</returns>
        public virtual async Task RevokeAsync([NotNull] TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            var filter = Builders<TToken>.Filter.Eq(s => s.Id, token.Id);

            await Tokens.DeleteOneAsync(filter);
        }

        /// <summary>
        /// Sets the authorization associated with a token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="identifier">The unique identifier associated with the authorization.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation.
        /// </returns>
        public virtual async Task SetAuthorizationAsync([NotNull] TToken token,[CanBeNull] string identifier, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (!string.IsNullOrEmpty(identifier))
            {
                var key = ConvertIdentifierFromString(identifier);

                var filter = Builders<TAuthorization>.Filter.Eq(s => s.Id, key);

                var authorization = await Authorizations.Find(filter).SingleOrDefaultAsync();
                if (authorization == null)
                {
                    throw new InvalidOperationException("The authorization associated with the token cannot be found.");
                }

                token.AuthorizationId = authorization.Id;
            }

            else
            {
                var filter = Builders<TToken>.Filter.Eq(s => s.Id, token.Id);

                var tokens = Tokens.Find(filter).ToList();
                if (tokens.Any())
                {
                    var filter2 = Builders<TAuthorization>.Filter.Eq(s => s.Id, tokens.FirstOrDefault().AuthorizationId);

                    // Try to retrieve the authorization associated with the token.
                    // If none can be found, assume that no authorization is attached.
                    var authorization = await Authorizations.Find(filter2).SingleOrDefaultAsync();
                    if (authorization != null)
                    {
                        var filter3 = Builders<TToken>.Filter.Eq(s => s.Id, token.Id);

                        await Tokens.DeleteOneAsync(filter3);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the client application associated with a token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="identifier">The unique identifier associated with the client application.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation.
        /// </returns>
        public virtual async Task SetClientAsync([NotNull]TToken token,[CanBeNull] string identifier, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (!string.IsNullOrEmpty(identifier))
            {
                var key = ConvertIdentifierFromString(identifier);

                var filter = Builders<TApplication>.Filter.Eq(s => s.Id, key);

                var application = await Applications.Find(filter).SingleOrDefaultAsync();
                if (application == null)
                {
                    throw new InvalidOperationException("The application associated with the token cannot be found.");
                }
                token.ApplicationId = application.Id;
            }

            else
            {
                var filter = Builders<TToken>.Filter.Eq(s => s.Id, token.Id);

                var tokens = Tokens.Find(filter).ToList();
                if (tokens.Any())
                {
                    var filter2 = Builders<TApplication>.Filter.Eq(s => s.Id, tokens.FirstOrDefault().ApplicationId);

                    // Try to retrieve the application associated with the token.
                    // If none can be found, assume that no application is attached.
                    var application = await Applications.Find(filter2).SingleOrDefaultAsync();
                    if (application != null)
                    {
                        var filter3 = Builders<TToken>.Filter.Eq(s => s.Id, token.Id);

                        await Tokens.DeleteOneAsync(filter3);
                    }
                }
            }
        }

        /// <summary>
        /// Updates an existing token.
        /// </summary>
        /// <param name="token">The token to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation.
        /// </returns>
        public virtual async Task UpdateAsync([NotNull] TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            var filter = Builders<TToken>.Filter.Eq(s => s.Id, token.Id);

            await Tokens.ReplaceOneAsync(filter, token);
        }

        /// <summary>
        /// Converts the provided identifier to a strongly typed key object.
        /// </summary>
        /// <param name="identifier">The identifier to convert.</param>
        /// <returns>An instance of <typeparamref name="TKey"/> representing the provided identifier.</returns>
        public virtual TKey ConvertIdentifierFromString([CanBeNull]string identifier)
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
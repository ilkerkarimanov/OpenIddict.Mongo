/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using OpenIddict.Core;
using OpenIddict.Models;
using OpenIddict.Mongo.Core;
using OpenIddict.Mongo.Stores;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.Tasks;
using System.Threading;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OpenIddictExtensions
    {
        /// <summary>
        /// Registers the Mongo Core stores. Note: when using the Mongo Core stores,
        /// the entities MUST be derived from the models contained in the OpenIddict.Models package.
        /// </summary>
        /// <param name="builder">The services builder used by OpenIddict to register new services.</param>
        /// <returns>The <see cref="OpenIddictBuilder"/>.</returns>
        public static OpenIddictBuilder AddMongoCoreStores<TContext>(this OpenIddictBuilder builder)
            where TContext : IMongoOpenIddictContext
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            Debug.Assert(builder.ApplicationType != null &&
                         builder.AuthorizationType != null &&
                         builder.ScopeType != null &&
                         builder.TokenType != null, "The entity types exposed by OpenIddictBuilder shouldn't be null.");

            var application = FindGenericBaseType(builder.ApplicationType, typeof(OpenIddictApplication<,,>));
            if (application == null)
            {
                throw new InvalidOperationException("The Mongo Core stores can only be used " +
                                                    "with the built-in OpenIddictApplication entity.");
            }

            var authorization = FindGenericBaseType(builder.AuthorizationType, typeof(OpenIddictAuthorization<,,>));
            if (authorization == null)
            {
                throw new InvalidOperationException("The Mongo Core stores can only be used " +
                                                    "with the built-in OpenIddictAuthorization entity.");
            }

            var scope = FindGenericBaseType(builder.ScopeType, typeof(OpenIddictScope<>));
            if (scope == null)
            {
                throw new InvalidOperationException("The Mongo Core stores can only be used " +
                                                    "with the built-in OpenIddictScope entity.");
            }

            var token = FindGenericBaseType(builder.TokenType, typeof(OpenIddictToken<,,>));
            if (token == null)
            {
                throw new InvalidOperationException("The Mongo Core stores can only be used " +
                                                    "with the built-in OpenIddictToken entity.");
            }

            var converter = TypeDescriptor.GetConverter(application.GenericTypeArguments[0]);
            if (converter == null || !converter.CanConvertFrom(typeof(string)) ||
                                     !converter.CanConvertTo(typeof(string)))
            {
                throw new InvalidOperationException("The specified entity key type is not supported.");
            }

            // Register the application store in the DI container.
            builder.Services.TryAddScoped(
                typeof(IOpenIddictApplicationStore<>).MakeGenericType(builder.ApplicationType),
                typeof(OpenIddictApplicationStore<,,,,>).MakeGenericType(
                    /* TApplication: */ builder.ApplicationType,
                    /* TAuthorization: */ builder.AuthorizationType,
                    /* TToken: */ builder.TokenType,
                    /* TContext: */ typeof(TContext),
                    /* TKey: */ application.GenericTypeArguments[0]));

            // Register the authorization store in the DI container.
            builder.Services.TryAddScoped(
                typeof(IOpenIddictAuthorizationStore<>).MakeGenericType(builder.AuthorizationType),
                typeof(OpenIddictAuthorizationStore<,,,,>).MakeGenericType(
                    /* TAuthorization: */ builder.AuthorizationType,
                    /* TApplication: */ builder.ApplicationType,
                    /* TToken: */ builder.TokenType,
                    /* TContext: */ typeof(TContext),
                    /* TKey: */ authorization.GenericTypeArguments[0]));

            // Register the scope store in the DI container.
            builder.Services.TryAddScoped(
                typeof(IOpenIddictScopeStore<>).MakeGenericType(builder.ScopeType),
                typeof(OpenIddictScopeStore<,,>).MakeGenericType(
                    /* TScope: */ builder.ScopeType,
                    /* TContext: */ typeof(TContext),
                    /* TKey: */ scope.GenericTypeArguments[0]));

            // Register the token store in the DI container.
            builder.Services.TryAddScoped(
                typeof(IOpenIddictTokenStore<>).MakeGenericType(builder.TokenType),
                typeof(OpenIddictTokenStore<,,,,>).MakeGenericType(
                    /* TToken: */ builder.TokenType,
                    /* TApplication: */ builder.ApplicationType,
                    /* TAuthorization: */ builder.AuthorizationType,
                    /* TContext: */ typeof(TContext),
                    /* TKey: */ token.GenericTypeArguments[0]));

            return builder;
        }

        public static OpenIddictBuilder AddMongoCoreStores(this OpenIddictBuilder builder)
        {
            return builder.AddMongoCoreStores<IMongoOpenIddictContext>();
        }

        public static IServiceCollection AddMongoDBContext<TContext, TConn>(this IServiceCollection services)
            where TContext: class,IMongoOpenIddictContext
            where TConn: class, IMongoOpenIddictConnection
        {
            services.AddScoped<IMongoOpenIddictConnection, TConn>();
            services.AddScoped<IMongoOpenIddictContext, TContext>();
            return services;
        }

        public static IServiceCollection AddMongoDBContext(this IServiceCollection services)
        {
            return services.AddMongoDBContext<MongoOpenIddictContext, MongoOpenIddictConnection>();
        }

        private static TypeInfo FindGenericBaseType(Type type, Type definition)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            for (var candidate = type.GetTypeInfo(); candidate != null; candidate = candidate.BaseType?.GetTypeInfo())
            {
                if (candidate.IsGenericType && candidate.GetGenericTypeDefinition() == definition)
                {
                    return candidate;
                }
            }

            return null;
        }
    }
}
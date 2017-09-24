using IDDD.Domain.Membership;
using IDDD.Infrastructure.Mongo.Membership;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System;
using Microsoft.Extensions.DependencyInjection;
using IDDD.Infrastructure.Mongo;

namespace IDDD.Infrastructure.Mongo
{
    public static class MongoIdentityBuilderExtensions
    {
        /// <summary>
        ///     This method only registers mongo stores, you also need to call AddIdentity.
        ///     Consider using AddIdentityWithMongoStores.
        /// </summary>
        /// <param name="builder"></param>
        public static IdentityBuilder RegisterMongoStores<TUser, TRole>(this IdentityBuilder builder, IServiceCollection services)
            where TRole : IdentityRole
            where TUser : IdentityUser
        {
            var mongoDbContext = services.BuildServiceProvider().GetService<IMongoContext>();
            return builder.RegisterMongoStores(
                p => mongoDbContext.Users,
                p => mongoDbContext.Roles);
        }

        /// <summary>
        ///     If you want control over creating the users and roles collections, use this overload.
        ///     This method only registers mongo stores, you also need to call AddIdentity.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TRole"></typeparam>
        /// <param name="builder"></param>
        /// <param name="usersCollectionFactory"></param>
        /// <param name="rolesCollectionFactory"></param>
        public static IdentityBuilder RegisterMongoStores<TUser, TRole>(this IdentityBuilder builder,
            Func<IServiceProvider, IMongoCollection<TUser>> usersCollectionFactory,
            Func<IServiceProvider, IMongoCollection<TRole>> rolesCollectionFactory)
            where TRole : IdentityRole
            where TUser : IdentityUser
        {
            if (typeof(TUser) != builder.UserType)
            {
                var message = "User type passed to RegisterMongoStores must match user type passed to AddIdentity. "
                              + $"You passed {builder.UserType} to AddIdentity and {typeof(TUser)} to RegisterMongoStores, "
                              + "these do not match.";
                throw new ArgumentException(message);
            }
            if (typeof(TRole) != builder.RoleType)
            {
                var message = "Role type passed to RegisterMongoStores must match role type passed to AddIdentity. "
                              + $"You passed {builder.RoleType} to AddIdentity and {typeof(TRole)} to RegisterMongoStores, "
                              + "these do not match.";
                throw new ArgumentException(message);
            }
            builder.Services.AddSingleton<IUserStore<TUser>>(p => new UserStore<TUser>(usersCollectionFactory(p)));
            builder.Services.AddSingleton<IRoleStore<TRole>>(p => new RoleStore<TRole>(rolesCollectionFactory(p)));
            return builder;
        }


        public static IdentityBuilder AddIdentityWithMongoStores(this IServiceCollection services)
        {
            return services.AddIdentityWithMongoStoresUsingCustomTypes<IdentityUser, IdentityRole>();
        }

        public static IdentityBuilder AddIdentityWithMongoStoresUsingCustomTypes<TUser, TRole>(this IServiceCollection services)
            where TUser : IdentityUser
            where TRole : IdentityRole
        {
            return services.AddIdentity<TUser, TRole>()
                .RegisterMongoStores<TUser, TRole>(services)
                .AddDefaultTokenProviders();
        }
    }
}

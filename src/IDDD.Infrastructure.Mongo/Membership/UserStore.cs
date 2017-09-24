using IDDD.Domain.Membership;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace IDDD.Infrastructure.Mongo.Membership
{

    /// <summary>
    ///     When passing a cancellation token, it will only be used if the operation requires a database interaction.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class UserStore<TUser> :
        IUserStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserRoleStore<TUser>,
        IUserLoginStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserLockoutStore<TUser>,
        IQueryableUserStore<TUser>
        where TUser : IdentityUser
    {
        protected readonly IMongoCollection<TUser> _Users;

        protected UserStore() { }

        public UserStore(IMongoCollection<TUser> users)
        {
            _Users = users;
        }

        public virtual void Dispose()
        {
            // no need to dispose of anything, mongodb handles connection pooling automatically
        }

        public virtual async Task<IdentityResult> CreateAsync(TUser user, CancellationToken token)
        {
            await _Users.InsertOneAsync(user, cancellationToken: token);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken token)
        {
            // todo should add an optimistic concurrency check
            await _Users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: token);
            // todo success based on replace result
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken token)
        {
            await _Users.DeleteOneAsync(u => u.Id == user.Id, token);
            // todo success based on delete result
            return IdentityResult.Success;
        }

        public virtual async Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
            => await Task.FromResult(user.Id.Id);

        public virtual async Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
            => await Task.FromResult(user.UserName);

        public virtual async Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
            =>  await Task.FromResult(user.UserName = userName);

        // note: again this isn't used by Identity framework so no way to integration test it
        public virtual async Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
            => await Task.FromResult(user.NormalizedUserName);

        public virtual async Task SetNormalizedUserNameAsync(TUser user, string normalizedUserName, CancellationToken cancellationToken)
            => await Task.FromResult(user.NormalizedUserName = normalizedUserName);

        public virtual Task<TUser> FindByIdAsync(string userId, CancellationToken token)
            => _Users.Find(u => u.Id == new IdentityUserId(userId)).FirstOrDefaultAsync(token);

        public virtual Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken token)
            // todo low priority exception on duplicates? or better to enforce unique index to ensure this
            => _Users.Find(u => u.NormalizedUserName == normalizedUserName).FirstOrDefaultAsync(token);

        public virtual async Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken token)
            => await Task.FromResult(user.PasswordHash = passwordHash);

        public virtual async Task<string> GetPasswordHashAsync(TUser user, CancellationToken token)
            => await Task.FromResult(user.PasswordHash);

        public virtual async Task<bool> HasPasswordAsync(TUser user, CancellationToken token)
            => await Task.FromResult(user.HasPassword());

        public virtual async Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken token)
            => 
        await Task.Factory.StartNew(() =>
            {
                user.AddRole(normalizedRoleName);
            });

        public virtual async Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken token)
            =>
            await Task.Factory.StartNew(() =>
            {
                user.RemoveRole(normalizedRoleName);
            });


        // todo might have issue, I'm just storing Normalized only now, so I'm returning normalized here instead of not normalized.
        // EF provider returns not noramlized here
        // however, the rest of the API uses normalized (add/remove/isinrole) so maybe this approach is better anyways
        // note: could always map normalized to not if people complain
        public virtual async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken token)
            => await Task.FromResult(user.Roles);

        public virtual async Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken token)
            => await Task.FromResult(user.Roles.Contains(normalizedRoleName));

        public virtual async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken token)
            => await _Users.Find(u => u.Roles.Contains(normalizedRoleName))
                .ToListAsync(token);

        public virtual async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken token)
            =>
                                                await Task.Factory.StartNew(() =>
                                                {
                                                    user.AddLogin(login);
                                                });


        public virtual async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
            =>
                                    await Task.Factory.StartNew(() =>
                                    {
                                        user.RemoveLogin(loginProvider, providerKey);
                                    });


        public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken token)
            => await Task.FromResult(user.Logins
                .Select(l => l.ToUserLoginInfo())
                .ToList());

        public virtual Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
            => _Users
                .Find(u => u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey))
                .FirstOrDefaultAsync(cancellationToken);

        public virtual async Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken token)
            => await Task.FromResult(user.SecurityStamp = stamp);

        public virtual async Task<string> GetSecurityStampAsync(TUser user, CancellationToken token)
            => await Task.FromResult(user.SecurityStamp);

        public virtual async Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken token)
            => await Task.FromResult(user.EmailConfirmed);

        public virtual async Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken token)
            => await Task.FromResult(user.EmailConfirmed = confirmed);

        public virtual async Task SetEmailAsync(TUser user, string email, CancellationToken token)
            => await Task.FromResult(user.Email = email);

        public virtual async Task<string> GetEmailAsync(TUser user, CancellationToken token)
            => await Task.FromResult(user.Email);

        // note: no way to intergation test as this isn't used by Identity framework	
        public virtual async Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
            => await Task.FromResult(user.NormalizedEmail);

        public virtual async Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
            => await Task.FromResult(user.NormalizedEmail = normalizedEmail);

        public virtual Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken token)
        {
            // note: I don't like that this now searches on normalized email :(... why not FindByNormalizedEmailAsync then?
            // todo low - what if a user can have multiple accounts with the same email?
            return _Users.Find(u => u.NormalizedEmail == normalizedEmail).FirstOrDefaultAsync(token);
        }

        public virtual async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken token)
            => await Task.FromResult(user.Claims.Select(c => c.ToSecurityClaim()).ToList());

        public virtual Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken token)
        {
            foreach (var claim in claims)
            {
                user.AddClaim(claim);
            }
            return Task.FromResult(0);
        }

        public virtual Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken token)
        {
            foreach (var claim in claims)
            {
                user.RemoveClaim(claim);
            }
            return Task.FromResult(0);
        }

        public virtual async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Factory.StartNew(() =>
               {
                   user.ReplaceClaim(claim, newClaim);
               });


        }

        public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken token)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public virtual Task<string> GetPhoneNumberAsync(TUser user, CancellationToken token)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken token)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken token)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken token)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken token)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public virtual async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _Users
                .Find(u => u.Claims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                .ToListAsync(cancellationToken);
        }

        public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken token)
        {
            return Task.FromResult(new DateTimeOffset?(user.LockoutEndDateUtc.Value));
        }

        public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken token)
        {
            user.LockoutEndDateUtc = lockoutEnd.Value.UtcDateTime;
            return Task.FromResult(0);
        }

        public virtual Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken token)
        {
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public virtual Task ResetAccessFailedCountAsync(TUser user, CancellationToken token)
        {
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public virtual async Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken token)
            => await Task.FromResult(user.AccessFailedCount);

        public virtual async Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken token)
            => await Task.FromResult(user.LockoutEnabled);

        public virtual async Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken token)
            => await Task.FromResult(user.LockoutEnabled = enabled);

        public virtual IQueryable<TUser> Users => _Users.AsQueryable();

        public virtual async Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
            => await Task.Factory.StartNew(() =>
             {
                 user.SetToken(loginProvider, name, value);
             });

        public virtual async Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
            => await Task.Factory.StartNew(() =>
             {
                 user.RemoveToken(loginProvider, name);
             });


        public virtual async Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
            => await Task.FromResult(user.GetTokenValue(loginProvider, name));
    }



}
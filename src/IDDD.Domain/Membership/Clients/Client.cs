using IDDD.Core.Domain;
using System;
using System.Collections.Generic;

namespace IDDD.Domain.Membership.Clients
{
    public class Client: Entity
    {
        
        public ClientId Id { get; set; }
        public string Secret { get; set; }

        public string Name { get; set; }
        public string ApplicationType { get; set; }

        public bool Active { get; set; }
        public string AllowedOrigin { get; set; }

        public string Key { get; set; }
        public string RedirectUri { get; set; }
        public string LogoutRedirectUri { get; set; }

        public string ConfirmationUri { get; set; }

        protected override IEnumerable<object> GetIdentityComponents()
        {
            yield return this.Active;
            yield return this.AllowedOrigin;
            yield return this.ApplicationType;
            yield return this.ConfirmationUri;
            yield return this.Id;
            yield return this.Key;
            yield return this.LogoutRedirectUri;
            yield return this.Name;
            yield return this.RedirectUri;
            yield return this.Secret;

        }
    }
}
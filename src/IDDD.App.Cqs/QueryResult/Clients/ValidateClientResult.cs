using IDDD.Core.Cqs;
using IDDD.Core;
using System;

namespace IDDD.App.Cqs.QueryResult.Clients
{
    public class ValidateClientResult : Result
    {
        public Guid Id { get; }
        public string Name { get; }
        public string AllowedOrigin { get; }
        public string RedirectUri { get;  }

        private ValidateClientResult(string [] errors)
            : base(errors)
        {
        }

        public ValidateClientResult(bool succeeded, string id = null, string name = null, string allowedOrigin = null, string redirectUri = null)
    : base(succeeded)
        {
            Id = Guid.Parse(id);
            Name = name;
            AllowedOrigin = allowedOrigin;
            RedirectUri = redirectUri;
        }

        public ValidateClientResult(bool succeeded, Guid id = default(Guid), string name = null, string allowedOrigin = null, string redirectUri = null)
            : base(succeeded)
        {
            Id = id;
            Name = name;
            AllowedOrigin = allowedOrigin;
            RedirectUri = redirectUri;
        }

        public static ValidateClientResult Failed(params string[] errors)
        {
            return new ValidateClientResult(errors);
        }
    }
}
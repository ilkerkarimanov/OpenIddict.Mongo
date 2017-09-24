namespace IDDD.Domain.Membership
{

    public class IdentityRole
    {
        private IdentityRoleId id;
        
        public string Id
        {
            get
            {
                return id.Id;
            }

            set
            {
                id = new IdentityRoleId(value);
            }
        }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public override string ToString() => Name;
    }
}
namespace EcoMonitor.Core.ValueObjects
{
    public sealed class Permission : IEquatable<Permission>
    {
        public string Code { get; }

        private Permission() { }
        public Permission(string Code)
        {
            if (string.IsNullOrWhiteSpace(Code)) 
                throw new ArgumentException("Permission code cannot be empty", nameof(Code));
            this.Code = Code;
        }
        public override bool Equals(object? obj) => Equals(obj as Permission);

        public bool Equals(Permission? other) =>
            other is not null && string.Equals(Code, other.Code, StringComparison.Ordinal);

        public override int GetHashCode() => Code.GetHashCode(StringComparison.Ordinal);

        public override string ToString() => Code;

        public static implicit operator string(Permission p) => p.Code;

        public static readonly Permission UsersView = new("Users.View");
        public static readonly Permission UsersAdd = new("Users.Add");
        public static readonly Permission UsersEdit = new("Users.Edit");
        public static readonly Permission UsersDelete = new("Users.Delete");

        public static readonly Permission RolesManage = new("Roles.Manage");

        public static readonly Permission PhotosView = new("Photos.View");
        public static readonly Permission PhotosAdd = new("Photos.Add");
        public static readonly Permission PhotosEdit = new("Photos.Edit");
        public static readonly Permission PhotosDelete = new("Photos.Delete");
    }
}

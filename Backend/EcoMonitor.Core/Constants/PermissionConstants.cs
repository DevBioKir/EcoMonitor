namespace EcoMonitor.Core.Models.Users;

public class PermissionConstants
{
    public static readonly Guid UsersViewId = Guid.Parse("2d877c22-0cfa-438e-a272-ebc3e04b368a");
    public static readonly Guid UsersAddId = Guid.Parse("0722fe30-70b7-4861-9b85-98d6b43fc9f2");
    public static readonly Guid UsersEditId = Guid.Parse("faa220fa-16f4-450b-b4cc-6562a0682401");
    public static readonly Guid UsersBlockId = Guid.Parse("b9423e9b-b567-40ab-a698-9f0f0d56551b");
    public static readonly Guid UsersUnblockId = Guid.Parse("9a2dc103-94ab-4ee0-865e-990d4845392b");
    
    public static readonly Guid RolesManageId = Guid.Parse("082a5b05-d471-4378-a609-fa0739979fbb");
    
    public static readonly Guid PhotosViewId = Guid.Parse("4042d6a7-15ff-4691-927a-735914ca0d58");
    public static readonly Guid PhotosAddId = Guid.Parse("a70144ee-cc41-46c5-bcab-d2683050ec71");
    public static readonly Guid PhotosEditId = Guid.Parse("686fabaa-4931-4918-a59e-bae4ab23fa74");
    public static readonly Guid PhotosDeleteId = Guid.Parse("305e92f4-aa23-44bd-8b96-a336015baf84");
}
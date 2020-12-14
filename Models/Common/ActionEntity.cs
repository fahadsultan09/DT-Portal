namespace Models.Common
{
    public class ActionEntity : CreatedEntity
    {
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}

namespace AspKnP231.Data.Entities
{
    public class UserAccess
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid UserRoleId { get; set; }

        public String Login { get; set; } = null!;

        public String Salt { get; set; } = null!;

        public String Dk { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }


        // Навігаційні властивості - посилання на інші сутності - спрощена схема зв'язування даних
        public UserData UserData { get; set; } = null!;   // EF автоматично заповнює сутність за UserId
        public UserRole UserRole { get; set; } = null!;
    }
}

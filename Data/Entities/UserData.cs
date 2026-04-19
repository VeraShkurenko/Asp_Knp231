namespace AspKnP231.Data.Entities
{
    public class UserData
    {
        public Guid Id { get; set; }

        public String Name { get; set; } = null!;

        public String Email { get; set; } = null!;

        public DateTime Birthdate { get; set; }

        public DateTime? DeletedAt { get; set; }


        // інверсна навігаційна властивість - через UserAccess.UserId 
        public ICollection<UserAccess> UserAccesses { get; set; } = [];
    }
}

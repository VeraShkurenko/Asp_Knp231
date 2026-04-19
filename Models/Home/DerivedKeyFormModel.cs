namespace AspKnP231.Models.Home
{
    public class DerivedKeyFormModel
    {
        public string? Password { get; set; }
        public string? Salt { get; set; }
        public bool AutoSalt { get; set; }
        public string? DerivedKey { get; set; }
    }
}

namespace AspKnP231.Services.DateTime
{
    public interface IDateTimeService
    {
        string GetDate();
        string GetTime();
    }

    public class SqlDateTimeService : IDateTimeService
    {
        public string GetDate() => System.DateTime.Now.ToString("yyyy-MM-dd");
        public string GetTime() => System.DateTime.Now.ToString("HH:mm:ss.fff");
    }

    public class NationalDateTimeService : IDateTimeService
    {
        public string GetDate() => System.DateTime.Now.ToString("dd.MM.yyyy");
        public string GetTime() => System.DateTime.Now.ToString("HH:mm:ss");
    }
}

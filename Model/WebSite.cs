namespace SatraWebApplication.Model
{
    public class WebSite
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string URL { get; set; }
        public string? Description { get; set; }

        public  virtual ICollection<WebSiteUpTime> WebSiteUpTimes { get; set; }
    }
}

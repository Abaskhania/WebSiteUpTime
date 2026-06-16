namespace SatraWebApplication.Model
{
    public class WebSiteUpTime
    {
        public string URL { get; set; }
        public string Result { get; set; }
        public string ResultGroup { get; set; }
        public int ID { get; set; }
        public DateTime ResultDate { get; set; }

        public int WebSiteID { get; set; }
        public WebSite WebSites { get; set; }
    }
}

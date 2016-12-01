namespace BtsPortal.Web.ViewModels
{
    public class JsonResultWrapper
    {
        public JsonResultWrapper()
        {
            Success = true;
        }
        public bool Success { get; set; }
        public object Data { get; set; }
        public string Error { get; set; }
    }
}

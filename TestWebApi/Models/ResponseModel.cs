namespace TestWebApi.Models
{
    public class ResponseModel: BaseResponse
    {        
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public int Rating { get; set; }
    }
}

namespace PrjFinanzas360.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Mensaje { get; set; }
        public DateTime ExpiraEn { get; set; }
    }
}

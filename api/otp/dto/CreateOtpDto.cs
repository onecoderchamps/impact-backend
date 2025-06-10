public class CreateOtpDto
{
    public string Phonenumber { get; set; }
}

public class CreateOtpWaDto
{
    public string Phonenumber { get; set; }
}

public class UpdateUserAuthDto
{
    public string CodeOtp { get; set; }
    public string Password { get; set; }
}
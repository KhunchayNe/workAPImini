
public class RespUserInfo
{
    public int Userid { get; set; }
    public int StudentId { get; set; }
    public string FName { get; set; } = string.Empty;
    public string LName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class RespLogin{
    public string Token { get; set; } = string.Empty;
}
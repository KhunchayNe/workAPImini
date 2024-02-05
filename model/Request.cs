using System.ComponentModel.DataAnnotations;

public class login{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AddUser :RespUserInfo {
    public string Password { get; set; } = string.Empty;
}
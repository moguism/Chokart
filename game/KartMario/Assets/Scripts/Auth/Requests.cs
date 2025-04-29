using System;

public class Requests {}

[Serializable]
public class RegisterRequest
{
    public string Email;
    public string Nickname;
    public string Password;
}

public class LoginRequest
{
    public string EmailOrNickname;
    public string Password;
}

[Serializable]
public class LoginResponse
{
    public int id;
    public string accessToken;
}

[Serializable]
public class UserResponse
{
    public string nickname;
}

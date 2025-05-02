using System;
using System.Collections.Generic;

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
public class UserDto
{
    public int id;
    public string nickname;
    public string email;
    public string role;
    public string avatarPath;
    public bool isInQueue;
    public bool banned;
    public int stateId;
    public List<Friendship> friendships;
    public long totalPoints;
    public string steamId;
}


[Serializable]
public class Friendship
{
    public int id;
    public bool accepted;
    public int senderUserId;
    public int receiverUserId;
    public UserDto senderUser;
    public UserDto receiverUser;
}

[Serializable]
public class SteamProfile
{
    public string steamId;
    public string personaName;
    public string avatar;
    public string avatarFull;
}

﻿using server.Models.Entities;

namespace server.Models.DTOs;

// sin contraseña
public class UserDto
{
    public int Id { get; set; }
    public string Nickname { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string AvatarPath { get; set; }
    public bool IsInQueue { get; set; }
    public bool Banned { get; set; }
    public int StateId { get; set; }
    public State State { get; set; }
    public ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();
    public List<BattleDto> Battles{ get; set; } = new List<BattleDto>();
    public long TotalPoints { get; set; } = 0;
    public string VerificationCode { get; set; } = "";

    // Opciones para steam
    public string SteamId { get; set; }
}

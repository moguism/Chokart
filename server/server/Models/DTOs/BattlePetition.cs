﻿namespace server.Models.DTOs;

public class BattlePetition
{
    public int GameMode { get; set; }
    public int TrackId { get; set; }
    public List<FinishKart> FinishKarts { get; set; }
}

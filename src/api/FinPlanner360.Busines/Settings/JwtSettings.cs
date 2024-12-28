﻿namespace FinPlanner360.Busines.Settings;

public sealed class JwtSettings
{
    public string Secret { get; set; }
    public int ExpirationInHours { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}
﻿using TelegramBot.AuditableModel;

namespace TelegramBot.Modifies
{
    public class Modify:BaseEntity
    {
        public string? Name_uz { get; set; }
        public string? Name_en { get; set; }
        public double? Price { get; set; }
    }
}
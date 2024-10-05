using System.ComponentModel.DataAnnotations;
using TelegramBot.AuditableModel;
using TelegramBot.Bot.Entity;
using TelegramBot.Comments.Entities;

namespace TelegramBot.Users.Entity
{
    public class User : BaseEntity
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$")]
        public string FullName { get; set; }
        [Required]
        [RegularExpression(@"^\+998\d{9}$")]
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? password { get; set; }
        public string LanguageCode { get; set; } = "uz";
        public BotState CurrentState { get; set; } = BotState.Start;
        public int Code { get; set; }
        public List<Role> Roles { get; set; } = new List<Role>() { Role.user};
        public List<Comment> Comments { get; set; }

    }
}

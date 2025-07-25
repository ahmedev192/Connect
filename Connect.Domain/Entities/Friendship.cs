using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Connect.Domain.Entities
{
    [Index(nameof(UniqueUser1Id), nameof(UniqueUser2Id), IsUnique = true)]
    public class Friendship
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public int User1Id { get; set; }
        public User User1 { get; set; }
        public int User2Id { get; set; }
        public User User2 { get; set; }

        // Ensure User1Id is always the smaller ID to avoid duplicates
        public int UniqueUser1Id { get => Math.Min(User1Id, User2Id); set => User1Id = value; }
        public int UniqueUser2Id { get => Math.Max(User1Id, User2Id); set => User2Id = value; }
    }
}

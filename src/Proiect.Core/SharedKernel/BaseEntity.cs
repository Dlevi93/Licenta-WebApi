using System;

namespace Proiect.Core.SharedKernel
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public string Author { get; set; } = "Admin";
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
    }
}
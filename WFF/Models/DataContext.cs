using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Web;

namespace WFF.Models
{
    public class DataContext : DbContext, IDataContext
    {
        public DbSet<UserProfile> PerfilesUsuarios { get; set; }
        public DbSet<FormRequest> FormRequests { get; set; }
        public DbSet<History> History { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
        : base(options)
        {
        }
    }

    public interface IDataContext
    {
        DbSet<UserProfile> PerfilesUsuarios { get; set; }
        DbSet<FormRequest> FormRequests { get; set; }
        DbSet<History> History { get; set; }
        DbSet<Attachment> Attachments { get; set; }
    }
}
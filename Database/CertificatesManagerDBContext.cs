using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CertificatesManager.Models;
using Microsoft.EntityFrameworkCore;

namespace CertificatesManager.Database
{
	public class CertificatesManagerDBContext : DbContext
	{
		public CertificatesManagerDBContext(DbContextOptions<CertificatesManagerDBContext> options) : base(options)
		{
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CertificateServer>()
				.HasKey(cd => new { cd.CertificateId, cd.ServerId });
			modelBuilder.Entity<CertificateApplication>()
				.HasKey(cd => new { cd.CertificateId, cd.ApplicationId });
			modelBuilder.Entity<CertificateGroup>()
				.HasKey(cd => new { cd.CertificateId, cd.GroupId });
			modelBuilder.Entity<GroupUser>()
				.HasKey(cd => new { cd.GroupId, cd.UserId });
		}
		public DbSet<Certificate> Certificate { get; set; }
		public DbSet<Application> Application { get; set; }
		public DbSet<Server> Server { get; set; }
		public DbSet<CertificateApplication> CertificateApplication { get; set; }
		public DbSet<CertificateServer> CertificateServer { get; set; }
		public DbSet<CertificateGroup> CertificateGroup { get; set; }
		public DbSet<GroupUser> GroupUser { get; set; }
		public DbSet<Settings> Settings { get; set; }
		public DbSet<Group> Group { get; set; }
		public DbSet<User> User { get; set; }
	}

}

using System;
using AuthenticationServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationServer
{
	public class AuthenticationContext : DbContext
	{

		public DbSet<User> Users { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite("Filename=./database.sqlite");
		}
	}
}

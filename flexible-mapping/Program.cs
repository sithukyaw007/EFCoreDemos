﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demos
{
    public class Program
    {
        private static void Main()
        {
            SetupDatabase();

            using (var db = new BloggingContext())
            {
                var blog = new Blog { Name = ".NET Blog" };

                blog.SetUrl("https://blogs.msdn.microsoft.com/dotnet");

                db.Blogs.Add(blog);
                db.SaveChanges();
            }

            using (var db = new BloggingContext())
            {
                var blog = db.Blogs.Single();

                Console.WriteLine($"{blog.Name}: {blog.Url}");
            }
        }

        private static void SetupDatabase()
        {
            using (var db = new BloggingContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }
    }

    public class BloggingContext : DbContext
    {
        private static readonly ILoggerFactory _loggerFactory = new LoggerFactory()
            .AddConsole((s, l) => l == LogLevel.Information && s.EndsWith("Command"));

        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=Demo.FlexibleMapping;Trusted_Connection=True;ConnectRetryCount=0;")
                .UseLoggerFactory(_loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set up a field mapping
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }

        public string Url { get; private set; }

        public void SetUrl(string url)
        {
            // Perform some domain logic...

            Url = url;
        }
    }
}

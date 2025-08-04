using Ekartes.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ekartes.DAL
{
    public class ekartesDbContext : DbContext
    {
        public ekartesDbContext()
            : base("ekartesDbContext")
        { }

        public DbSet<Melos> Melos { get; set; }
        public DbSet<Dikaiouxos> Dikaiouxos { get; set; }
        public DbSet<FileDikaiouxos> FilesDikaiouxos { get; set; }
        public DbSet<FileMelos> FilesMelos { get; set; }
        public DbSet<Aitima> Aitimata { get; set; }


    }
}
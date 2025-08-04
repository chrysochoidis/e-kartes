namespace ekartes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Aitimas",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Eidos = c.Int(nullable: false),
                        Katastasi = c.Int(nullable: false),
                        Dikaiouxos_ID = c.Int(),
                        Melos_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Dikaiouxos", t => t.Dikaiouxos_ID)
                .ForeignKey("dbo.Melos", t => t.Melos_ID)
                .Index(t => t.Dikaiouxos_ID)
                .Index(t => t.Melos_ID);
            
            CreateTable(
                "dbo.Dikaiouxos",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AM = c.Int(nullable: false),
                        Onoma = c.String(nullable: false),
                        Epitheto = c.String(nullable: false),
                        AT = c.String(nullable: false),
                        Monada = c.String(nullable: false),
                        Vathmos = c.Int(nullable: false),
                        O_S = c.String(nullable: false),
                        KatastasiD = c.Int(nullable: false),
                        Rolos = c.Int(nullable: false),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.FileDikaiouxos",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 255),
                        ContentType = c.String(maxLength: 100),
                        Content = c.Binary(),
                        FileType = c.Int(nullable: false),
                        Dikaiouxos_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Dikaiouxos", t => t.Dikaiouxos_ID)
                .Index(t => t.Dikaiouxos_ID);
            
            CreateTable(
                "dbo.Melos",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AT = c.String(nullable: false),
                        Onoma = c.String(nullable: false),
                        Epitheto = c.String(nullable: false),
                        Sygeneia = c.Int(nullable: false),
                        HmniaEkdosis = c.DateTime(nullable: false),
                        HmniaLiksis = c.DateTime(nullable: false),
                        KwdikosKartas = c.String(),
                        Aitiologia = c.Int(nullable: false),
                        Dikaiouxos_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Dikaiouxos", t => t.Dikaiouxos_ID)
                .Index(t => t.Dikaiouxos_ID);
            
            CreateTable(
                "dbo.FileMelos",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 255),
                        ContentType = c.String(maxLength: 100),
                        Content = c.Binary(),
                        FileType = c.Int(nullable: false),
                        Melos_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Melos", t => t.Melos_ID)
                .Index(t => t.Melos_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileMelos", "Melos_ID", "dbo.Melos");
            DropForeignKey("dbo.Melos", "Dikaiouxos_ID", "dbo.Dikaiouxos");
            DropForeignKey("dbo.Aitimas", "Melos_ID", "dbo.Melos");
            DropForeignKey("dbo.FileDikaiouxos", "Dikaiouxos_ID", "dbo.Dikaiouxos");
            DropForeignKey("dbo.Aitimas", "Dikaiouxos_ID", "dbo.Dikaiouxos");
            DropIndex("dbo.FileMelos", new[] { "Melos_ID" });
            DropIndex("dbo.Melos", new[] { "Dikaiouxos_ID" });
            DropIndex("dbo.FileDikaiouxos", new[] { "Dikaiouxos_ID" });
            DropIndex("dbo.Aitimas", new[] { "Melos_ID" });
            DropIndex("dbo.Aitimas", new[] { "Dikaiouxos_ID" });
            DropTable("dbo.FileMelos");
            DropTable("dbo.Melos");
            DropTable("dbo.FileDikaiouxos");
            DropTable("dbo.Dikaiouxos");
            DropTable("dbo.Aitimas");
        }
    }
}

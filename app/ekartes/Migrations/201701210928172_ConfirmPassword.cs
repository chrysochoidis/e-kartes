namespace ekartes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConfirmPassword : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dikaiouxos", "ConfirmPassword", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dikaiouxos", "ConfirmPassword");
        }
    }
}

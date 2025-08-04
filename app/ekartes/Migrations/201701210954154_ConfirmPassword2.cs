namespace ekartes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConfirmPassword2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Dikaiouxos", "ConfirmPassword");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Dikaiouxos", "ConfirmPassword", c => c.String());
        }
    }
}

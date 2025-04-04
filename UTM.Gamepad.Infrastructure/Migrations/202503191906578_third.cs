namespace UTM.Gamepad.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class third : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Roles", "Id", "dbo.Users");
            DropIndex("dbo.Roles", new[] { "Id" });
            AddColumn("dbo.Users", "RoleId", c => c.Guid());
            CreateIndex("dbo.Users", "RoleId");
            AddForeignKey("dbo.Users", "RoleId", "dbo.Roles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "RoleId", "dbo.Roles");
            DropIndex("dbo.Users", new[] { "RoleId" });
            DropColumn("dbo.Users", "RoleId");
            CreateIndex("dbo.Roles", "Id");
            AddForeignKey("dbo.Roles", "Id", "dbo.Users", "Id");
        }
    }
}

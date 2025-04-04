namespace UTM.Gamepad.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class db : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "ProductName", c => c.String(nullable: false));
            AddColumn("dbo.OrderItems", "ProductImageUrl", c => c.String());
            AddColumn("dbo.OrderItems", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "UserName", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "UserEmail", c => c.String());
            AddColumn("dbo.Orders", "Status", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "ShippingAddress", c => c.String());
            AddColumn("dbo.Orders", "City", c => c.String());
            AddColumn("dbo.Orders", "State", c => c.String());
            AddColumn("dbo.Orders", "ZipCode", c => c.String());
            AddColumn("dbo.Orders", "Country", c => c.String());
            AddColumn("dbo.Orders", "PhoneNumber", c => c.String());
            AddColumn("dbo.Orders", "PaymentMethod", c => c.String());
            AddColumn("dbo.Orders", "Notes", c => c.String());
            AddColumn("dbo.Products", "StockQuantity", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "Category", c => c.String(nullable: false));
            AddColumn("dbo.Products", "ImageUrl", c => c.String());
            AddColumn("dbo.Products", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Products", "UpdatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Orders", "OrderDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.OrderItems", "UnitPrice");
            DropColumn("dbo.Products", "Stock");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Stock", c => c.Int(nullable: false));
            AddColumn("dbo.OrderItems", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Orders", "OrderDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            DropColumn("dbo.Products", "UpdatedDate");
            DropColumn("dbo.Products", "CreatedDate");
            DropColumn("dbo.Products", "ImageUrl");
            DropColumn("dbo.Products", "Category");
            DropColumn("dbo.Products", "StockQuantity");
            DropColumn("dbo.Orders", "Notes");
            DropColumn("dbo.Orders", "PaymentMethod");
            DropColumn("dbo.Orders", "PhoneNumber");
            DropColumn("dbo.Orders", "Country");
            DropColumn("dbo.Orders", "ZipCode");
            DropColumn("dbo.Orders", "State");
            DropColumn("dbo.Orders", "City");
            DropColumn("dbo.Orders", "ShippingAddress");
            DropColumn("dbo.Orders", "Status");
            DropColumn("dbo.Orders", "UserEmail");
            DropColumn("dbo.Orders", "UserName");
            DropColumn("dbo.OrderItems", "Price");
            DropColumn("dbo.OrderItems", "ProductImageUrl");
            DropColumn("dbo.OrderItems", "ProductName");
        }
    }
}

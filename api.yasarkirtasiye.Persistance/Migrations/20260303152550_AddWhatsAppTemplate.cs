using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.yasarkirtasiye.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddWhatsAppTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WhatsAppTemplate",
                table: "SiteSettings",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "WhatsAppTemplate",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WhatsAppTemplate",
                table: "SiteSettings");
        }
    }
}

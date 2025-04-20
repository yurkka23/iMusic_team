using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iMusic.DAL.Migrations
{
    public partial class addBoolWhoWantsToBeSinger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AddColumn<bool>(
                name: "WantToBeSinger",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.DropColumn(
                name: "WantToBeSinger",
                table: "AspNetUsers");

          
        }
    }
}

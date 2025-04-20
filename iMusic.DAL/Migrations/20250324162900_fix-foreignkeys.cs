using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iMusic.DAL.Migrations
{
    public partial class fixforeignkeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_Categories_Id",
                table: "Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_Albums_FavoriteLists_Id",
                table: "Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_FavoriteLists_Id",
                table: "Playlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Categories_Id",
                table: "Songs");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_FavoriteLists_Id",
                table: "Songs");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3d881963-cb0d-4c46-bfe7-edc2a48d9e55"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9a0ce5a5-5b48-44c6-a6d1-5cd14608c4d0"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c013b5b1-b2fb-49f8-8c53-55ab993a8847"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("1cfba6f6-50fa-4304-9140-4bd86f5a5885"), "4a9c639c-4da0-4585-96b1-30535daa18b8", "iMusic.admin", "IMUSIC.ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("70f964d3-7677-4ac5-952b-f4713352ca5e"), "661238ff-ee42-42b7-b606-1a19f76cd087", "iMusic.singer", "IMUSIC.SINGER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("d013ca6b-ea46-4947-9a7f-a249406ff873"), "0e873962-591b-418c-a93e-aeec62f666b6", "iMusic.user", "IMUSIC.USER" });

            migrationBuilder.CreateIndex(
                name: "IX_Songs_CategoryId",
                table: "Songs",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_FavoriteListId",
                table: "Songs",
                column: "FavoriteListId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_FavoriteListId",
                table: "Playlists",
                column: "FavoriteListId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_CategoryId",
                table: "Albums",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_FavoriteListId",
                table: "Albums",
                column: "FavoriteListId");

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_Categories_CategoryId",
                table: "Albums",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_FavoriteLists_FavoriteListId",
                table: "Albums",
                column: "FavoriteListId",
                principalTable: "FavoriteLists",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_FavoriteLists_FavoriteListId",
                table: "Playlists",
                column: "FavoriteListId",
                principalTable: "FavoriteLists",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Categories_CategoryId",
                table: "Songs",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_FavoriteLists_FavoriteListId",
                table: "Songs",
                column: "FavoriteListId",
                principalTable: "FavoriteLists",
                principalColumn: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_Categories_CategoryId",
                table: "Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_Albums_FavoriteLists_FavoriteListId",
                table: "Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_FavoriteLists_FavoriteListId",
                table: "Playlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Categories_CategoryId",
                table: "Songs");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_FavoriteLists_FavoriteListId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Songs_CategoryId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Songs_FavoriteListId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Playlists_FavoriteListId",
                table: "Playlists");

            migrationBuilder.DropIndex(
                name: "IX_Albums_CategoryId",
                table: "Albums");

            migrationBuilder.DropIndex(
                name: "IX_Albums_FavoriteListId",
                table: "Albums");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1cfba6f6-50fa-4304-9140-4bd86f5a5885"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("70f964d3-7677-4ac5-952b-f4713352ca5e"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("d013ca6b-ea46-4947-9a7f-a249406ff873"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("3d881963-cb0d-4c46-bfe7-edc2a48d9e55"), "6c247c81-532a-476b-ac01-eab5930fe8e6", "iMusic.admin", "IMUSIC.ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("9a0ce5a5-5b48-44c6-a6d1-5cd14608c4d0"), "96144362-599c-4400-8060-25c2d1d6b0d1", "iMusic.singer", "IMUSIC.SINGER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("c013b5b1-b2fb-49f8-8c53-55ab993a8847"), "2d70985e-0394-4519-bfdb-2a435b272ca4", "iMusic.user", "IMUSIC.USER" });

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_Categories_Id",
                table: "Albums",
                column: "Id",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_FavoriteLists_Id",
                table: "Albums",
                column: "Id",
                principalTable: "FavoriteLists",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_FavoriteLists_Id",
                table: "Playlists",
                column: "Id",
                principalTable: "FavoriteLists",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Categories_Id",
                table: "Songs",
                column: "Id",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_FavoriteLists_Id",
                table: "Songs",
                column: "Id",
                principalTable: "FavoriteLists",
                principalColumn: "UserId");
        }
    }
}

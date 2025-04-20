using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iMusic.DAL.Migrations
{
    public partial class fixFavoriteList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_FavoriteLists_FavoriteListId",
                table: "Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_FavoriteLists_FavoriteListId",
                table: "Playlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_FavoriteLists_FavoriteListId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Songs_FavoriteListId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Playlists_FavoriteListId",
                table: "Playlists");

            migrationBuilder.DropIndex(
                name: "IX_Albums_FavoriteListId",
                table: "Albums");

           
            migrationBuilder.DropColumn(
                name: "FavoriteListId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "FavoriteListId",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "FavoriteListId",
                table: "Albums");

            migrationBuilder.AddColumn<int>(
                name: "CountRate",
                table: "Playlists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountRate",
                table: "Albums",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FavoriteAlbums",
                columns: table => new
                {
                    AlbumId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FavoritelistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteAlbums", x => new { x.FavoritelistId, x.AlbumId });
                    table.ForeignKey(
                        name: "FK_FavoriteAlbums_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FavoriteAlbums_FavoriteLists_FavoritelistId",
                        column: x => x.FavoritelistId,
                        principalTable: "FavoriteLists",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "FavoritePlaylists",
                columns: table => new
                {
                    PlaylistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FavoritelistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoritePlaylists", x => new { x.FavoritelistId, x.PlaylistId });
                    table.ForeignKey(
                        name: "FK_FavoritePlaylists_FavoriteLists_FavoritelistId",
                        column: x => x.FavoritelistId,
                        principalTable: "FavoriteLists",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_FavoritePlaylists_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FavoriteSongs",
                columns: table => new
                {
                    SongId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FavoritelistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteSongs", x => new { x.FavoritelistId, x.SongId });
                    table.ForeignKey(
                        name: "FK_FavoriteSongs_FavoriteLists_FavoritelistId",
                        column: x => x.FavoritelistId,
                        principalTable: "FavoriteLists",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_FavoriteSongs_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id");
                });


            migrationBuilder.CreateIndex(
                name: "IX_FavoriteAlbums_AlbumId",
                table: "FavoriteAlbums",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoritePlaylists_PlaylistId",
                table: "FavoritePlaylists",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteSongs_SongId",
                table: "FavoriteSongs",
                column: "SongId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteAlbums");

            migrationBuilder.DropTable(
                name: "FavoritePlaylists");

            migrationBuilder.DropTable(
                name: "FavoriteSongs");

          

            migrationBuilder.DropColumn(
                name: "CountRate",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "CountRate",
                table: "Albums");

            migrationBuilder.AddColumn<Guid>(
                name: "FavoriteListId",
                table: "Songs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FavoriteListId",
                table: "Playlists",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FavoriteListId",
                table: "Albums",
                type: "uniqueidentifier",
                nullable: true);

           

            migrationBuilder.CreateIndex(
                name: "IX_Songs_FavoriteListId",
                table: "Songs",
                column: "FavoriteListId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_FavoriteListId",
                table: "Playlists",
                column: "FavoriteListId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_FavoriteListId",
                table: "Albums",
                column: "FavoriteListId");

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
                name: "FK_Songs_FavoriteLists_FavoriteListId",
                table: "Songs",
                column: "FavoriteListId",
                principalTable: "FavoriteLists",
                principalColumn: "UserId");
        }
    }
}

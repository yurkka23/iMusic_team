using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iMusic.DAL.Migrations
{
    public partial class changedeletebehavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_AspNetUsers_SingerId",
                table: "Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteAlbums_FavoriteLists_FavoritelistId",
                table: "FavoriteAlbums");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoritePlaylists_FavoriteLists_FavoritelistId",
                table: "FavoritePlaylists");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteSongs_FavoriteLists_FavoritelistId",
                table: "FavoriteSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_SongPlaylists_Playlists_PlaylistId",
                table: "SongPlaylists");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAlbums_Albums_AlbumId",
                table: "UserAlbums");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSongs_Songs_SongId",
                table: "UserSongs");

           

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_AspNetUsers_SingerId",
                table: "Albums",
                column: "SingerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteAlbums_FavoriteLists_FavoritelistId",
                table: "FavoriteAlbums",
                column: "FavoritelistId",
                principalTable: "FavoriteLists",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoritePlaylists_FavoriteLists_FavoritelistId",
                table: "FavoritePlaylists",
                column: "FavoritelistId",
                principalTable: "FavoriteLists",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteSongs_FavoriteLists_FavoritelistId",
                table: "FavoriteSongs",
                column: "FavoritelistId",
                principalTable: "FavoriteLists",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SongPlaylists_Playlists_PlaylistId",
                table: "SongPlaylists",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAlbums_Albums_AlbumId",
                table: "UserAlbums",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSongs_Songs_SongId",
                table: "UserSongs",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_AspNetUsers_SingerId",
                table: "Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteAlbums_FavoriteLists_FavoritelistId",
                table: "FavoriteAlbums");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoritePlaylists_FavoriteLists_FavoritelistId",
                table: "FavoritePlaylists");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteSongs_FavoriteLists_FavoritelistId",
                table: "FavoriteSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_SongPlaylists_Playlists_PlaylistId",
                table: "SongPlaylists");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAlbums_Albums_AlbumId",
                table: "UserAlbums");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSongs_Songs_SongId",
                table: "UserSongs");

         
            migrationBuilder.AddForeignKey(
                name: "FK_Albums_AspNetUsers_SingerId",
                table: "Albums",
                column: "SingerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteAlbums_FavoriteLists_FavoritelistId",
                table: "FavoriteAlbums",
                column: "FavoritelistId",
                principalTable: "FavoriteLists",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoritePlaylists_FavoriteLists_FavoritelistId",
                table: "FavoritePlaylists",
                column: "FavoritelistId",
                principalTable: "FavoriteLists",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteSongs_FavoriteLists_FavoritelistId",
                table: "FavoriteSongs",
                column: "FavoritelistId",
                principalTable: "FavoriteLists",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SongPlaylists_Playlists_PlaylistId",
                table: "SongPlaylists",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAlbums_Albums_AlbumId",
                table: "UserAlbums",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSongs_Songs_SongId",
                table: "UserSongs",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

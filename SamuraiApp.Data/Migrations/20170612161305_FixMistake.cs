using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SamuraiApp.Data.Migrations
{
    public partial class FixMistake : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDirty",
                table: "SecretIdentity");

            migrationBuilder.DropColumn(
                name: "IsDirty",
                table: "SamuraiBattles");

            migrationBuilder.DropColumn(
                name: "IsDirty",
                table: "Samurais");

            migrationBuilder.DropColumn(
                name: "IsDirty",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "IsDirty",
                table: "Battles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDirty",
                table: "SecretIdentity",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDirty",
                table: "SamuraiBattles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDirty",
                table: "Samurais",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDirty",
                table: "Quotes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDirty",
                table: "Battles",
                nullable: false,
                defaultValue: false);
        }
    }
}

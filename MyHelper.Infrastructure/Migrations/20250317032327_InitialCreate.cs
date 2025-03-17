using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AutoCompleteTextRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Word = table.Column<string>(type: "TEXT", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoCompleteTextRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LauncherItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LauncherType = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LauncherItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EnableLauncherByMouseButton = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableLauncherByCtrlKeys = table.Column<bool>(type: "INTEGER", nullable: false),
                    OpenValidPathByCtrlCC = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastWrittenTimeForTables = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TableInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TableName = table.Column<string>(type: "TEXT", nullable: false),
                    TableComment = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TableColumnInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Table_Id = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPrimaryKey = table.Column<int>(type: "INTEGER", nullable: false),
                    ColumnName = table.Column<string>(type: "TEXT", nullable: false),
                    ColumnComment = table.Column<string>(type: "TEXT", nullable: false),
                    DataType = table.Column<string>(type: "TEXT", nullable: false),
                    IsNullable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableColumnInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableColumnInfos_TableInfos_Table_Id",
                        column: x => x.Table_Id,
                        principalTable: "TableInfos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TableColumnInfos_Table_Id",
                table: "TableColumnInfos",
                column: "Table_Id");

            migrationBuilder.CreateIndex(
                name: "IX_TableInfo_TableName",
                table: "TableInfos",
                column: "TableName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoCompleteTextRecords");

            migrationBuilder.DropTable(
                name: "LauncherItems");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "TableColumnInfos");

            migrationBuilder.DropTable(
                name: "TableInfos");
        }
    }
}

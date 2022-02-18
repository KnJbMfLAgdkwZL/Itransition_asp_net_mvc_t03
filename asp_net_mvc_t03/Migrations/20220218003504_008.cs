using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asp_net_mvc_t03.Migrations
{
    public partial class _008 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "task3");

            migrationBuilder.CreateTable(
                name: "users",
                schema: "task3",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    registrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    lastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                schema: "task3",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    authorId = table.Column<int>(type: "int", nullable: false),
                    toUserId = table.Column<int>(type: "int", nullable: false),
                    head = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false, collation: "Cyrillic_General_CI_AS"),
                    body = table.Column<string>(type: "varchar(2550)", unicode: false, maxLength: 2550, nullable: false, collation: "Cyrillic_General_CI_AS"),
                    createDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    replyId = table.Column<int>(type: "int", nullable: true),
                    @new = table.Column<bool>(name: "new", type: "bit", nullable: false),
                    uid = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.id);
                    table.ForeignKey(
                        name: "messages_users_id_fk",
                        column: x => x.authorId,
                        principalSchema: "task3",
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "messages_users_id_fk_2",
                        column: x => x.toUserId,
                        principalSchema: "task3",
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_messages_authorId",
                schema: "task3",
                table: "messages",
                column: "authorId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_toUserId",
                schema: "task3",
                table: "messages",
                column: "toUserId");

            migrationBuilder.CreateIndex(
                name: "users_mail_uindex",
                schema: "task3",
                table: "users",
                column: "email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "messages",
                schema: "task3");

            migrationBuilder.DropTable(
                name: "users",
                schema: "task3");
        }
    }
}

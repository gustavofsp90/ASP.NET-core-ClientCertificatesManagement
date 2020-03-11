using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CertificatesManager.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificateDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    SubjectName = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    Issuer = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Purpose = table.Column<string>(nullable: true),
                    Environment = table.Column<int>(nullable: false),
                    From = table.Column<DateTime>(nullable: false),
                    To = table.Column<DateTime>(nullable: false),
                    InstallationLink = table.Column<string>(nullable: true),
                    OtherInfos = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Server",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CertificateDetailId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Server", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Server_CertificateDetails_CertificateDetailId",
                        column: x => x.CertificateDetailId,
                        principalTable: "CertificateDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "System",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CertificateDetailId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System", x => x.Id);
                    table.ForeignKey(
                        name: "FK_System_CertificateDetails_CertificateDetailId",
                        column: x => x.CertificateDetailId,
                        principalTable: "CertificateDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Server_CertificateDetailId",
                table: "Server",
                column: "CertificateDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_System_CertificateDetailId",
                table: "System",
                column: "CertificateDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Server");

            migrationBuilder.DropTable(
                name: "System");

            migrationBuilder.DropTable(
                name: "CertificateDetails");
        }
    }
}

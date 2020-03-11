using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CertificatesManager.Migrations
{
    public partial class UpdateName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificateDetailServer");

            migrationBuilder.DropTable(
                name: "CertificateDetailSystem");

            migrationBuilder.DropTable(
                name: "CertificateDetails");

            migrationBuilder.CreateTable(
                name: "Certificate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
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
                    OtherInfo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CertificateServer",
                columns: table => new
                {
                    CertificateId = table.Column<int>(nullable: false),
                    ServerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateServer", x => new { x.CertificateId, x.ServerId });
                    table.ForeignKey(
                        name: "FK_CertificateServer_Certificate_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateServer_Server_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Server",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateSystem",
                columns: table => new
                {
                    CertificateId = table.Column<int>(nullable: false),
                    SystemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateSystem", x => new { x.CertificateId, x.SystemId });
                    table.ForeignKey(
                        name: "FK_CertificateSystem_Certificate_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateSystem_System_SystemId",
                        column: x => x.SystemId,
                        principalTable: "System",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificateServer_ServerId",
                table: "CertificateServer",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateSystem_SystemId",
                table: "CertificateSystem",
                column: "SystemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificateServer");

            migrationBuilder.DropTable(
                name: "CertificateSystem");

            migrationBuilder.DropTable(
                name: "Certificate");

            migrationBuilder.CreateTable(
                name: "CertificateDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Environment = table.Column<int>(nullable: false),
                    Extension = table.Column<string>(nullable: true),
                    From = table.Column<DateTime>(nullable: false),
                    Guid = table.Column<string>(nullable: true),
                    InstallationLink = table.Column<string>(nullable: true),
                    Issuer = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Purpose = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    SubjectName = table.Column<string>(nullable: true),
                    To = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CertificateDetailServer",
                columns: table => new
                {
                    CertificateDetailId = table.Column<int>(nullable: false),
                    ServerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateDetailServer", x => new { x.CertificateDetailId, x.ServerId });
                    table.ForeignKey(
                        name: "FK_CertificateDetailServer_CertificateDetails_CertificateDetailId",
                        column: x => x.CertificateDetailId,
                        principalTable: "CertificateDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateDetailServer_Server_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Server",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateDetailSystem",
                columns: table => new
                {
                    CertificateDetailId = table.Column<int>(nullable: false),
                    SystemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateDetailSystem", x => new { x.CertificateDetailId, x.SystemId });
                    table.ForeignKey(
                        name: "FK_CertificateDetailSystem_CertificateDetails_CertificateDetailId",
                        column: x => x.CertificateDetailId,
                        principalTable: "CertificateDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateDetailSystem_System_SystemId",
                        column: x => x.SystemId,
                        principalTable: "System",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificateDetailServer_ServerId",
                table: "CertificateDetailServer",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateDetailSystem_SystemId",
                table: "CertificateDetailSystem",
                column: "SystemId");
        }
    }
}

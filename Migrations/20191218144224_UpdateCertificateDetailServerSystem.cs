using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CertificatesManager.Migrations
{
    public partial class UpdateCertificateDetailServerSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Server_CertificateDetails_CertificateDetailId",
                table: "Server");

            migrationBuilder.DropTable(
                name: "System");

            migrationBuilder.DropIndex(
                name: "IX_Server_CertificateDetailId",
                table: "Server");

            migrationBuilder.DropColumn(
                name: "CertificateDetailId",
                table: "Server");

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
                        name: "FK_CertificateDetailSystem_Server_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Server",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificateDetailServer");

            migrationBuilder.DropTable(
                name: "CertificateDetailSystem");

            migrationBuilder.AddColumn<int>(
                name: "CertificateDetailId",
                table: "Server",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "System",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CertificateDetailId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true)
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

            migrationBuilder.AddForeignKey(
                name: "FK_Server_CertificateDetails_CertificateDetailId",
                table: "Server",
                column: "CertificateDetailId",
                principalTable: "CertificateDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

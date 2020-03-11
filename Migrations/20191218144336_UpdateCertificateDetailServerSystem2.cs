using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CertificatesManager.Migrations
{
    public partial class UpdateCertificateDetailServerSystem2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CertificateDetailSystem_Server_SystemId",
                table: "CertificateDetailSystem");

            migrationBuilder.CreateTable(
                name: "System",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_System", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CertificateDetailSystem_System_SystemId",
                table: "CertificateDetailSystem",
                column: "SystemId",
                principalTable: "System",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CertificateDetailSystem_System_SystemId",
                table: "CertificateDetailSystem");

            migrationBuilder.DropTable(
                name: "System");

            migrationBuilder.AddForeignKey(
                name: "FK_CertificateDetailSystem_Server_SystemId",
                table: "CertificateDetailSystem",
                column: "SystemId",
                principalTable: "Server",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

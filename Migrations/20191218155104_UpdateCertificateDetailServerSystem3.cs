using Microsoft.EntityFrameworkCore.Migrations;

namespace CertificatesManager.Migrations
{
    public partial class UpdateCertificateDetailServerSystem3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OtherInfos",
                table: "CertificateDetails",
                newName: "OtherInfo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OtherInfo",
                table: "CertificateDetails",
                newName: "OtherInfos");
        }
    }
}

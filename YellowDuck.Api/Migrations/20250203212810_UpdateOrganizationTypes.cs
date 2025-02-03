using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowDuck.Api.Migrations
{
    public partial class UpdateOrganizationTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE UserProfiles SET OrganizationTypeOtherSpecification = 'Entreprises d’économie sociale, organismes communautaires, organismes bénévoles et autres organismes à but non lucratif' WHERE OrganizationType = 0");
            migrationBuilder.Sql("UPDATE UserProfiles SET OrganizationType = 3 WHERE OrganizationType = 0");
            migrationBuilder.Sql("UPDATE UserProfiles SET OrganizationTypeOtherSpecification = 'Entreprises privées à but lucratif' WHERE OrganizationType = 1");
            migrationBuilder.Sql("UPDATE UserProfiles SET OrganizationType = 3 WHERE OrganizationType = 1");
            migrationBuilder.Sql("UPDATE UserProfiles SET OrganizationTypeOtherSpecification = 'Organisations du secteur public' WHERE OrganizationType = 2");
            migrationBuilder.Sql("UPDATE UserProfiles SET OrganizationType = 3 WHERE OrganizationType = 2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE UserProfiles SET OrganizationType = 0 WHERE OrganizationTypeOtherSpecification = 'Entreprises d’économie sociale, organismes communautaires, organismes bénévoles et autres organismes à but non lucratif'");
            migrationBuilder.Sql("UPDATE UserProfiles SET OrganizationTypeOtherSpecification = NULL WHERE OrganizationTypeOtherSpecification = 'Entreprises d’économie sociale, organismes communautaires, organismes bénévoles et autres organismes à but non lucratif'");
            migrationBuilder.Sql("UPDATE UserProfiles SET OrganizationType = 1 WHERE OrganizationTypeOtherSpecification = 'Entreprises privées à but lucratif'");
            migrationBuilder.Sql("UPDATE UserProfiles SET OrganizationTypeOtherSpecification = NULL WHERE OrganizationTypeOtherSpecification = 'Entreprises privées à but lucratif'");
            migrationBuilder.Sql("UPDATE UserProfiles SET OrganizationType = 2 WHERE OrganizationTypeOtherSpecification = 'Organisations du secteur public'");
            migrationBuilder.Sql("UPDATE UserProfiles SET OrganizationTypeOtherSpecification = NULL WHERE OrganizationTypeOtherSpecification = 'Organisations du secteur public'");
        }
    }
}

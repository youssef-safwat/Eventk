using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Entites.Migrations
{
    /// <inheritdoc />
    public partial class AddSpatialPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Location");

            migrationBuilder.AddColumn<Point>(
                name: "LocationPoint",
                table: "Location",
                type: "geography",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationPoint",
                table: "Location");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Location",
                type: "decimal(13,10)",
                precision: 13,
                scale: 10,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Location",
                type: "decimal(13,10)",
                precision: 13,
                scale: 10,
                nullable: true);
        }
    }
}

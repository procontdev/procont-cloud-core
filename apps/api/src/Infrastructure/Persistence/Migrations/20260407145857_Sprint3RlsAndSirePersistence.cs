using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint3RlsAndSirePersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "tenant_id",
                table: "asiento_detalles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "sire_propuestas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    periodo = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    importe_total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    estado = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    generated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sire_propuestas", x => x.id);
                    table.ForeignKey(
                        name: "FK_sire_propuestas_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sire_contabilizacion_resultados",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    propuesta_id = table.Column<Guid>(type: "uuid", nullable: false),
                    asiento_id = table.Column<Guid>(type: "uuid", nullable: true),
                    estado = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    mensaje = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    observacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sire_contabilizacion_resultados", x => x.id);
                    table.ForeignKey(
                        name: "FK_sire_contabilizacion_resultados_asientos_asiento_id",
                        column: x => x.asiento_id,
                        principalTable: "asientos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_sire_contabilizacion_resultados_sire_propuestas_propuesta_id",
                        column: x => x.propuesta_id,
                        principalTable: "sire_propuestas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sire_contabilizacion_resultados_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_asiento_detalles_tenant_id",
                table: "asiento_detalles",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_sire_contabilizacion_resultados_asiento_id",
                table: "sire_contabilizacion_resultados",
                column: "asiento_id");

            migrationBuilder.CreateIndex(
                name: "IX_sire_contabilizacion_resultados_propuesta_id",
                table: "sire_contabilizacion_resultados",
                column: "propuesta_id");

            migrationBuilder.CreateIndex(
                name: "IX_sire_contabilizacion_resultados_tenant_id",
                table: "sire_contabilizacion_resultados",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_sire_propuestas_tenant_id",
                table: "sire_propuestas",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_sire_propuestas_tenant_id_periodo_source",
                table: "sire_propuestas",
                columns: new[] { "tenant_id", "periodo", "source" });

            migrationBuilder.AddForeignKey(
                name: "FK_asiento_detalles_tenants_tenant_id",
                table: "asiento_detalles",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_asiento_detalles_tenants_tenant_id",
                table: "asiento_detalles");

            migrationBuilder.DropTable(
                name: "sire_contabilizacion_resultados");

            migrationBuilder.DropTable(
                name: "sire_propuestas");

            migrationBuilder.DropIndex(
                name: "IX_asiento_detalles_tenant_id",
                table: "asiento_detalles");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "asiento_detalles");
        }
    }
}

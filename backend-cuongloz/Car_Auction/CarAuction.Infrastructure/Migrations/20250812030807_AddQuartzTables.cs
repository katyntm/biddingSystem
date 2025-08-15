using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
#nullable disable

namespace CarAuction.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuartzTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var enableQuartzScript = config.GetValue<bool>("Quartz:EnableSqlScript");

            if (enableQuartzScript)
            {
                var sqlPath = "C:\\Users\\CuongPC10\\Desktop\\OJT_Training\\backend\\Car_Auction\\CarAuction.Infrastructure\\LoadData\\tables_sqlServer.sql";
                if (File.Exists(sqlPath))
                {
                    var sql = File.ReadAllText(sqlPath);
                    migrationBuilder.Sql(sql);
                }
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TABLE IF EXISTS QRTZ_FIRED_TRIGGERS;
                DROP TABLE IF EXISTS QRTZ_PAUSED_TRIGGER_GRPS;
                DROP TABLE IF EXISTS QRTZ_SCHEDULER_STATE;
                DROP TABLE IF EXISTS QRTZ_LOCKS;
                DROP TABLE IF EXISTS QRTZ_SIMPLE_TRIGGERS;
                DROP TABLE IF EXISTS QRTZ_CRON_TRIGGERS;
                DROP TABLE IF EXISTS QRTZ_SIMPROP_TRIGGERS;
                DROP TABLE IF EXISTS QRTZ_BLOB_TRIGGERS;
                DROP TABLE IF EXISTS QRTZ_CALENDARS;
                DROP TABLE IF EXISTS QRTZ_TRIGGERS;
                DROP TABLE IF EXISTS QRTZ_JOB_DETAILS;
            ");
        }

    }
}

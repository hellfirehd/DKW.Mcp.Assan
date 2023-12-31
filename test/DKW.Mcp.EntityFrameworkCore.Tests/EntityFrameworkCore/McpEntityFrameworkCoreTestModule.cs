// MCP Application Suite
// Copyright (C) 2023 Doug Wilson
//
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this
// program. If not, see <https://www.gnu.org/licenses/>.

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace DKW.Mcp.EntityFrameworkCore;

[DependsOn(typeof(McpTestBaseModule))]
[DependsOn(typeof(McpEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpEntityFrameworkCoreSqliteModule))]
public class McpEntityFrameworkCoreTestModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddAlwaysDisableUnitOfWorkTransaction();

		var sqliteConnection = CreateDatabaseAndGetConnection();

		Configure<AbpDbContextOptions>(options =>
		{
			options.Configure(abpDbContextConfigurationContext =>
			{
				abpDbContextConfigurationContext.DbContextOptions.UseSqlite(sqliteConnection);
			});
		});
	}

	private static SqliteConnection CreateDatabaseAndGetConnection()
	{
		var connection = new SqliteConnection("Data Source=:memory:");
		connection.Open();

		new McpDbContext(
			new DbContextOptionsBuilder<McpDbContext>().UseSqlite(connection).Options
		).GetService<IRelationalDatabaseCreator>().CreateTables();

		return connection;
	}
}

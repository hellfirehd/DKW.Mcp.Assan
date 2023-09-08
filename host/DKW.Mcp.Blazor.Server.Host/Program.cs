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

using Serilog;
using Serilog.Events;

namespace DKW.Mcp.Blazor.Server.Host;

public class Program
{
	public async static Task<Int32> Main(String[] args)
	{
		Log.Logger = new LoggerConfiguration()
#if DEBUG
			.MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
			.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
			.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
			.Enrich.FromLogContext()
			.WriteTo.Async(c => c.File("Logs/logs.txt"))
			.WriteTo.Async(c => c.Console())
			.CreateLogger();

		try
		{
			Log.Information("Starting web host.");
			var builder = WebApplication.CreateBuilder(args);
			builder.Host.AddAppSettingsSecretsJson()
				.UseAutofac()
				.UseSerilog();
			await builder.AddApplicationAsync<McpBlazorHostModule>();
			var app = builder.Build();
			await app.InitializeApplicationAsync();
			await app.RunAsync();
			return 0;
		}
		catch (Exception ex)
		{
			if (ex is HostAbortedException)
			{
				throw;
			}

			Log.Fatal(ex, "Host terminated unexpectedly!");
			return 1;
		}
		finally
		{
			Log.CloseAndFlush();
		}
	}
}

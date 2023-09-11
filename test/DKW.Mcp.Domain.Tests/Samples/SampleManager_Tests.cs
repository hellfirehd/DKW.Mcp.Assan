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

using Xunit;

namespace DKW.Mcp.Samples;

public class SampleManager_Tests : McpDomainTestBase
{
	//private readonly SampleManager _sampleManager;

	public SampleManager_Tests()
	{
		//_sampleManager = GetRequiredService<SampleManager>();
	}

	[Fact]
	public Task Method1Async()
	{
		return Task.CompletedTask;
	}
}

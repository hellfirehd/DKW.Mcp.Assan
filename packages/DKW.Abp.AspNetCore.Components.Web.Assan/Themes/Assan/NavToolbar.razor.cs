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

using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Web.Security;
using Volo.Abp.AspNetCore.Components.Web.Theming.Toolbars;

namespace DKW.Abp.AspNetCore.Components.Web.Assan.Themes.Assan;

public partial class NavToolbar : IDisposable
{
	[Inject]
	private IToolbarManager ToolbarManager { get; set; } = default!;

	[Inject]
	protected ApplicationConfigurationChangedService ApplicationConfigurationChangedService { get; set; } = default!;

	private List<RenderFragment> ToolbarItemRenders { get; set; } = new List<RenderFragment>();

	protected async override Task OnInitializedAsync()
	{
		await GetToolbarItemRendersAsync();
		ApplicationConfigurationChangedService.Changed += ApplicationConfigurationChanged;
	}

	private async Task GetToolbarItemRendersAsync()
	{
		var toolbar = await ToolbarManager.GetAsync(StandardToolbars.Main);

		ToolbarItemRenders.Clear();

		var sequence = 0;
		foreach (var item in toolbar.Items)
		{
			ToolbarItemRenders.Add(builder =>
			{
				builder.OpenComponent(sequence++, item.ComponentType);
				builder.CloseComponent();
			});
		}
	}

	private async void ApplicationConfigurationChanged()
	{
		await GetToolbarItemRendersAsync();
		await InvokeAsync(StateHasChanged);
	}

	public void Dispose()
	{
		ApplicationConfigurationChangedService.Changed -= ApplicationConfigurationChanged;
	}
}

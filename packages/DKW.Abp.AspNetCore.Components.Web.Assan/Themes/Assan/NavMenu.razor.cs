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
using Volo.Abp.UI.Navigation;

namespace DKW.Abp.AspNetCore.Components.Web.Assan.Themes.Assan;

public partial class NavMenu : IDisposable
{
	[Inject]
	protected IMenuManager MenuManager { get; set; } = default!;

	[Inject]
	protected ApplicationConfigurationChangedService ApplicationConfigurationChangedService { get; set; } = default!;

	protected ApplicationMenu Menu { get; set; } = default!;

	protected async override Task OnInitializedAsync()
	{
		Menu = await MenuManager.GetMainMenuAsync();
		ApplicationConfigurationChangedService.Changed += ApplicationConfigurationChanged;
	}

	private async void ApplicationConfigurationChanged()
	{
		Menu = await MenuManager.GetMainMenuAsync();
		await InvokeAsync(StateHasChanged);
	}

	public void Dispose()
	{
		ApplicationConfigurationChangedService.Changed -= ApplicationConfigurationChanged;
	}
}

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;


namespace MyFinances.Data.Auth
{
	public class StateProvider : AuthenticationStateProvider
	{
		private readonly ProtectedSessionStorage _sessionStorage;
		private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

		public StateProvider(ProtectedSessionStorage sessionStorage)
		{
			_sessionStorage = sessionStorage;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			try
			{
				var userSessionStorageRessult = await _sessionStorage.GetAsync<UserSession>("UserSession");
				var userSession = userSessionStorageRessult.Success ? userSessionStorageRessult.Value : null;
				if (userSession == null)
					return await Task.FromResult(new AuthenticationState(_anonymous));

				var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Name, userSession.UserName), new Claim(ClaimTypes.Role, userSession.Role) }, "CustomAuth"));
				return await Task.FromResult(new AuthenticationState(claimsPrincipal));
			}
			catch (Exception)
			{
				return await Task.FromResult(new AuthenticationState(_anonymous));
			}

		}

		public async Task UpdateAuthenticationState(UserSession userSession)
		{
			ClaimsPrincipal claimsPrincipal;

			if(userSession != null)
			{
				await _sessionStorage.SetAsync("UserSession", userSession);
				claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Name, userSession.UserName), new Claim(ClaimTypes.Role, userSession.Role) }, "CustomAuth"));
			}
			else
			{
				await _sessionStorage.DeleteAsync("UserSession");
				claimsPrincipal = _anonymous;
			}
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
		}
	}
}

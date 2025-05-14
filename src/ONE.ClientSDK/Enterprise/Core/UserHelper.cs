using Newtonsoft.Json.Linq;
using ONE.ClientSDK.Enterprise.Authentication;
using ONE.ClientSDK.Enums;
using ONE.Models.CSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ONE.ClientSDK.Enterprise.Core
{
	public class UserHelper
	{
		public UserHelper(AuthenticationApi authenticationApi, CoreApi coreApi)
		{
			_coreApi = coreApi;
			_authenticationApi = authenticationApi;
		}

		private readonly CoreApi _coreApi;
		private readonly AuthenticationApi _authenticationApi;
		
		public static User GetByUserName(string username, List<User> users)
		{
			if (users == null || string.IsNullOrEmpty(username))
				return null;
			
			return users.FirstOrDefault(p => string.Equals(p.UserName, username, StringComparison.CurrentCulture));
		}

		public async Task<bool> SetUserRoleAsync(User user, string roleId, bool setRole)
		{
			var userHasRole = HasRole(user, roleId);
			if (setRole != userHasRole)
			{
				if (userHasRole)
				{
					if (await _coreApi.UserDeleteRoleRefAsync(user.Id, roleId))
						return false;
				}
				else
				{
					if (await _coreApi.UserCreateRoleRefAsync(user.Id, roleId))
						return true;
				}
			}
			return userHasRole;
		}

		public async Task<User> LoadCurrentUserAsync()
		{
			try
			{
				var result = await _authenticationApi.GetUserInfoAsync();
				var user = await GetUserFromUserInfoAsync(result);
				if (user != null)
					_authenticationApi.User = user;

				return user;
			}
			catch
			{
				return null;
			}
		}

		public async Task<User> GetUserFromUserInfoAsync(string userInfoJson, EnumUserExpand enumUserExpand = EnumUserExpand.role_feature)
		{
			var userInfo = JObject.Parse(userInfoJson);
			var id = userInfo["sub"]?.ToString();
			return await _coreApi.GetUserAsync(id, enumUserExpand);
		}

		public static DataTable GetDataTable(List<User> users)
		{
			var dataTable = new DataTable("Users");
			if (users != null)
			{
				var parameterProps = typeof(User).GetProperties(BindingFlags.Public | BindingFlags.Instance);
				dataTable.Columns.Add("Tenant");
				foreach (var prop in parameterProps)
				{
					//Setting column names as Property names  
					dataTable.Columns.Add(prop.Name);
				}

				//Load Data
				foreach (var user in users)
				{
					var row = dataTable.NewRow();
					for (var i = 0; i < parameterProps.Length; i++)
					{
						row[parameterProps[i].Name] = parameterProps[i].GetValue(user, null);
					}
					if (user.Tenants.Count == 1)
					{
						row["Tenant"] = user.Tenants[0].Name;
					}
					dataTable.Rows.Add(row);
				}
			}
			return dataTable;

		}

		public static bool HasRole(User user, string roleId)
		{
			foreach (var role in user.Roles)
			{
				if (role.Id.ToUpper() == roleId.ToUpper())
					return true;
			}
			return false;
		}
	}
}

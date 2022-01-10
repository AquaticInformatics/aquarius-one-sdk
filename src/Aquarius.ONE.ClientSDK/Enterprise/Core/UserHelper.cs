using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Enterprise.Core.Protobuf.Models;

namespace ONE.Enterprise.Core
{
    public class UserHelper
    {
        public UserHelper(CoreApi coreApi)
        {
            _coreApi = coreApi;
        }
        private CoreApi _coreApi;
        public static User GetByUserName(string username, List<User> users)
        {
            if (users == null || string.IsNullOrEmpty(username))
                return null;
            var matches = users.Where(p => String.Equals(p.UserName, username, StringComparison.CurrentCulture));
            if (matches.Any())
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }
        public async Task<bool> SetUserRoleAsync(User user, string roleId, bool setRole)
        {
            bool userHasRole = HasRole(user, roleId);
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
    
        public async Task<User> GetUserFromUserInfoAsync(string userInfoJson, CoreApi.EnumUserExpand enumUserExpand = CoreApi.EnumUserExpand.role_feature)
        {
            JObject userInfo = JObject.Parse(userInfoJson);
            string id = userInfo["sub"].ToString();
            return await _coreApi.GetUserAsync(id, enumUserExpand);
        }
        public static DataTable GetDataTable(List<User> users)
        {
            DataTable dataTable = new DataTable("Users");
            if (users != null)
            {
                PropertyInfo[] parameterProps = typeof(User).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                dataTable.Columns.Add("Tenant");
                foreach (PropertyInfo prop in parameterProps)
                {
                    //Setting column names as Property names  
                    dataTable.Columns.Add(prop.Name);
                }

                //Load Data
                foreach (User user in users)
                {
                    DataRow row = dataTable.NewRow();
                    for (int i = 0; i < parameterProps.Length; i++)
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
            foreach (Role role in user.Roles)
            {
                if (role.Id.ToUpper() == roleId.ToUpper())
                    return true;
            }
            return false;
        }
       
    }
}

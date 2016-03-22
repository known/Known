using Known.Data;
using Known.Extensions;
using System.Collections.Generic;
using System.Data;

namespace Known.Services
{
    public interface IRoleService
    {
        List<RoleInfo> GetRoles();
        List<RoleInfo> GetRoles(string ids);
        RoleInfo GetRole(string id);
        ValidateResult Remove(string id);
        ValidateResult Save(RoleInfo role);
        ValidateResult SaveRight(string roleId, string menus);
    }

    public class RoleService : IRoleService
    {
        public List<RoleInfo> GetRoles()
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from T_Roles";
            return command.ToList(r => { return GetRoleInfo(r, false); });
        }

        public List<RoleInfo> GetRoles(string ids)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = string.Format("select * from T_Roles where Id in ('{0}')", ids.Replace(",", "','"));
            return command.ToList(r => { return GetRoleInfo(r, true); });
        }

        public RoleInfo GetRole(string id)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from T_Roles where Id=?Id";
            command.Parameters.Add("Id", id);
            return command.ToEntity(r => { return GetRoleInfo(r, true); });
        }

        public ValidateResult Remove(string id)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "delete from T_Roles where Id=?Id";
            command.Parameters.Add("Id", id);
            var message = command.Execute();
            return new ValidateResult(message);
        }

        public ValidateResult Save(RoleInfo role)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select count(*) from T_Roles where Id=?Id";
            command.Parameters.Add("Id", role.Id);
            if (command.ToScalar<int>() > 0)
            {
                command.Text = "update T_Roles set Name=?Name,Description=?Description where Id=?Id";
                command.Parameters.Add("Id", role.Id);
            }
            else
            {
                command.Text = "insert into T_Roles(Id,Name,Description) values(?Id,?Name,?Description)";
                command.Parameters.Add("Id", DbHelper.NewId);
            }
            command.Parameters.Add("Name", role.Name);
            command.Parameters.Add("Description", role.Description);
            var message = command.Execute();
            return new ValidateResult(message);
        }

        public ValidateResult SaveRight(string roleId, string menus)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "delete from T_RoleMenus where RoleId=?RoleId";
            command.Parameters.Add("RoleId", roleId);
            command.ExecuteOnSubmit();
            command.Text = "insert into T_RoleMenus values(?RoleId,?MenuId)";
            foreach (var item in menus.Split(','))
            {
                command.Parameters.Add("RoleId", roleId);
                command.Parameters.Add("MenuId", item);
                command.ExecuteOnSubmit();
            }
            var message = DbHelper.Default.SubmitChanges();
            return new ValidateResult(message);
        }

        private static RoleInfo GetRoleInfo(DataRow row, bool initMenus)
        {
            var role = new RoleInfo
            {
                Id = row.Get<string>("Id"),
                Name = row.Get<string>("Name"),
                Description = row.Get<string>("Description"),
                Menus = row.Get<string>("Menus")
            };
            if (initMenus)
            {
                var command = DbHelper.Default.CreateCommand();
                command.Text = "select MenuId from T_RoleMenus where RoleId=?RoleId";
                command.Parameters.Add("RoleId", role.Id);
                var menus = command.ToList(r => { return r.Get<string>("MenuId"); });
                if (string.IsNullOrEmpty(role.Menus))
                {
                    role.Menus = string.Join(",", menus.ToArray());
                }
                else
                {
                    role.Menus = role.Menus + "," + string.Join(",", menus.ToArray());
                }
            }
            return role;
        }
    }
}

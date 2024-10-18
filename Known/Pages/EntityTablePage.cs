using Known.Blazor;
using Known.Extensions;
using Known.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Known.Pages
{
    public class EntityTablePage<TEntity> : BaseTablePage<TEntity> where TEntity : class, new()
    {
        private IEntityService<TEntity> Service;

        protected override async Task OnPageInitAsync()
        {
            Table = new TableModel<TEntity>(this, true);
            Table.DefaultQuery = DefaultQuery;

            Service = await CreateServiceAsync<IEntityService<TEntity>>();
            Table.OnQuery = Service.QueryAsync;
            Table.Toolbar.AddAction(nameof(New));
            Table.Toolbar.AddAction(nameof(DeleteM));

            Table.AddAction(nameof(Edit));
            Table.AddAction(nameof(Delete));
        }

        /// <summary>
        /// 弹出新增表单对话框。
        /// </summary>
        public void New() => Table.NewForm(Service.SaveAsync, new TEntity());

        /// <summary>
        /// 弹出编辑表单对话框。
        /// </summary>
        /// <param name="row">表格行绑定的对象。</param>
        public void Edit(TEntity row) => Table.EditForm(Service.SaveAsync, row);

        /// <summary>
        /// 删除一条数据。
        /// </summary>
        /// <param name="row">表格行绑定的对象。</param>
        public void Delete(TEntity row) => Table.Delete(Service.DeleteAsync, row);

        /// <summary>
        /// 批量删除多条数据。
        /// </summary>
        public void DeleteM() => Table.DeleteM(Service.DeleteAsync);
    }
}

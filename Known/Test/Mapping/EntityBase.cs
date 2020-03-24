using System;

namespace Known.Mapping
{
    /// <summary>
    /// 实体基类。
    /// </summary>
    public class EntityBase
    {
        /// <summary>
        /// 初始化一个实体类实例。
        /// </summary>
        public EntityBase()
        {
            Id = Utils.NewGuid;
            CreateBy = "temp";
            CreateTime = DateTime.Now;
            Version = 1;
            IsNew = true;
        }

        /// <summary>
        /// 取得或设置实体主键。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 取得或设置创建人。
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 取得或设置创建时间。
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 取得或设置修改人。
        /// </summary>
        public string ModifyBy { get; set; }

        /// <summary>
        /// 取得或设置修改时间。
        /// </summary>
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 取得或设置版本。
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 取得或设置扩展信息。
        /// </summary>
        public string Extension { get; set; }

        internal bool IsNew { get; set; }
        internal EntityBase Original { get; set; }
    }
}

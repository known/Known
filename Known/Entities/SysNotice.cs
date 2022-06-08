using System;

namespace Known.Entities
{
    /// <summary>
    /// 通知公告实体类。
    /// </summary>
    public class SysNotice : EntityBase
    {
        /// <summary>
        /// 取得或设置状态（暂存，已发布）。
        /// </summary>
        [Column("状态", "", true, "1", "50")]
        public string Status { get; set; }

        /// <summary>
        /// 取得或设置标题。
        /// </summary>
        [Column("标题", "", true, "1", "50")]
        public string Title { get; set; }

        /// <summary>
        /// 取得或设置内容。
        /// </summary>
        [Column("内容", "", false, "1", "4000")]
        public string Content { get; set; }

        /// <summary>
        /// 取得或设置发布人。
        /// </summary>
        [Column("发布人", "", false, "1", "50")]
        public string PublishBy { get; set; }

        /// <summary>
        /// 取得或设置发布时间。
        /// </summary>
        [Column("发布时间", "", false)]
        public DateTime? PublishTime { get; set; }
    }
}
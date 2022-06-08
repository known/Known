/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

namespace Known.Entities
{
    /// <summary>
    /// 系统文件实体类。
    /// </summary>
    public class SysFile : EntityBase
    {
        /// <summary>
        /// 取得或设置一级分类。
        /// </summary>
        [Column("一级分类", "", true, "1", "50")]
        public string Category1 { get; set; }

        /// <summary>
        /// 取得或设置二级分类。
        /// </summary>
        [Column("二级分类", "", false, "1", "50")]
        public string Category2 { get; set; }

        /// <summary>
        /// 取得或设置文件名称。
        /// </summary>
        [Column("文件名称", "", true, "1", "50")]
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置文件类型。
        /// </summary>
        [Column("文件类型", "", false, "1", "50")]
        public string Type { get; set; }

        /// <summary>
        /// 取得或设置文件路径。
        /// </summary>
        [Column("文件路径", "", true, "1", "250")]
        public string Path { get; set; }

        /// <summary>
        /// 取得或设置文件大小。
        /// </summary>
        [Column("文件大小", "", true)]
        public long Size { get; set; }

        /// <summary>
        /// 取得或设置原文件名。
        /// </summary>
        [Column("原文件名", "", true, "1", "50")]
        public string SourceName { get; set; }

        /// <summary>
        /// 取得或设置扩展名。
        /// </summary>
        [Column("扩展名", "", true, "1", "50")]
        public string ExtName { get; set; }

        /// <summary>
        /// 取得或设置备注。
        /// </summary>
        [Column("备注", "", false, "1", "500")]
        public string Note { get; set; }

        /// <summary>
        /// 取得或设置业务ID。
        /// </summary>
        [Column("业务ID", "", false, "1", "50")]
        public string BizId { get; set; }
    }
}
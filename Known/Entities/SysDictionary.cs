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
    public class SysDictionary : EntityBase
    {
        [Column("类别代码", "", true, "1", "50")]
        public string Category { get; set; }

        [Column("类别名称", "", true, "1", "50")]
        public string CategoryName { get; set; }

        [Column("代码", "", true, "1", "50")]
        public string Code { get; set; }

        [Column("名称", "", false, "1", "50")]
        public string Name { get; set; }

        [Column("顺序", "", true)]
        public int Sort { get; set; }

        [Column("状态", "", true)]
        public int Enabled { get; set; }

        [Column("备注", "", false, "1", "500")]
        public string Note { get; set; }
    }
}
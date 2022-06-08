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
    public class SysLog : EntityBase
    {
        [Column("Type", "", true, "1", "50")]
        public string Type { get; set; }
        [Column("Target", "", true, "1", "50")]
        public string Target { get; set; }
        [Column("Content")]
        public string Content { get; set; }
    }
}
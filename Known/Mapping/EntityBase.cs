using System;

namespace Known.Mapping
{
    public class EntityBase
    {
        public EntityBase()
        {
            Id = Utils.NewGuid;
            CreateBy = "temp";
            IsNew = true;
        }

        public string Id { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateTime { get; set; }
        public string ModifyBy { get; set; }
        public DateTime? ModifyTime { get; set; }
        public string Extension { get; set; }
        internal bool IsNew { get; set; }
    }
}

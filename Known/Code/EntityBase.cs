using System;
using System.Collections.Generic;

namespace Known
{
    public class EntityBase
    {
        [NonSerialized]
        private bool isNew;
        [NonSerialized]
        private Dictionary<string, object> original;

        public EntityBase()
        {
            Id = Utils.NewGuid;
            CreateBy = "temp";
            CreateTime = DateTime.Now;
            Version = 1;
            isNew = true;
        }

        internal bool IsNew
        {
            get { return isNew; }
            set { isNew = value; }
        }

        internal Dictionary<string, object> Original
        {
            get { return original; }
            set { original = value; }
        }

        public string Id { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateTime { get; set; }
        public string ModifyBy { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int Version { get; set; }
        public string Extension { get; set; }
    }
}

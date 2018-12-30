using System;
using Known.Mapping;

namespace Known.Tests.KnownTests
{
    [Table("a_test", "测试表", "item1,item2")]
    public class TestEntity : BaseEntity
    {
        [IntegerColumn("item1", "属性1", true)]
        public int Item1 { get; set; }

        [StringColumn("item2", "属性2", 1, 50, true)]
        public string Item2 { get; set; }

        [DateTimeColumn("item3", "属性3", false)]
        public DateTime Item3 { get; set; }

        public TestEnum Item4 { get; set; }

        public TestObject Test { get; set; }

        public string ItemOnlyRead { get; }
        public virtual string ItemVirtual { get; set; }
    }

    public class TestObject : BaseEntity
    {
        public string Item1 { get; set; }
        public string Item2 { get; set; }
    }
}

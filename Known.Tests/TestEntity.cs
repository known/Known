using System;
using Known.Mapping;

namespace Known.Tests
{
    [Table("A_TEST", "ITEM1", "测试表")]
    public class TestEntity : EntityBase
    {
        [IntegerColumn("ITEM1", "属性1", false)]
        public int Item1 { get; set; }

        [StringColumn("ITEM2", "属性2", false, 1, 50)]
        public string Item2 { get; set; }

        [DateTimeColumn("ITEM3", "属性3", false)]
        public DateTime Item3 { get; set; }

        public TestEnum Item4 { get; set; }

        public Test Test { get; set; }

        public string ItemOnlyRead { get; }
        public virtual string ItemVirtual { get; set; }
    }

    public class Test : EntityBase
    {
        public string Item1 { get; set; }
    }
}

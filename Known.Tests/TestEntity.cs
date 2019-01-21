using System;
using Known.Mapping;

namespace Known.Tests
{
    //[Table("a_test", "测试表", "item1,item2")]
    public class TestEntity : EntityBase
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

    class TestEntityMapper : EntityMapper<TestEntity>
    {
        public TestEntityMapper() :
            base("a_test", "测试表", "item1,item2")
        {
            this.Property(p => p.Item1)
                .IsIntegerColumn("item1", "属性1", true);

            this.Property(p => p.Item2)
                .IsStringColumn("item2", "属性2", 1, 50, true);

            this.Property(p => p.Item3)
                .IsDateTimeColumn("item3", "属性3", false);

            this.Property(p => p.Item4)
                .IsEnumColumn("item4", "属性4");

            this.Property(p => p.Test);
        }
    }

    public class TestObject : EntityBase
    {
        public string Item1 { get; set; }
        public string Item2 { get; set; }
    }
}

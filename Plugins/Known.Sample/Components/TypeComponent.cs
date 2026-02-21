namespace Known.Sample.Components;

public class TestForm : AntForm<TestInfo> { }
public class WeatherTable : PageTable<Weather_Forecast> { }

public class MaterialTypeForm : AntForm<TbMaterial> { }
public class WorkTypeForm : AntForm<TbWork> { }

public class PackFieldTypeTable : AntTable<PackFieldInfo> { }
public class AppFieldSelect : AntSelectEnum<AppFieldType> { }
public class WorkFieldSelect : AntSelectEnum<WorkFieldType> { }
namespace WebSite.Docus.Inputs.Numbers;

class Number3 : Form
{
    protected override void BuildFields(RenderTreeBuilder builder)
    {
        builder.Field<Number>("数量", "Qty")
               .Set(f => f.Unit, "个")
               .Set(f => f.OnValueChanged, OnQPAChanged)
               .Build();
        builder.Field<Number>("单价", "Price")
               .Set(f => f.Unit, "元")
               .Set(f => f.OnValueChanged, OnQPAChanged)
               .Build();
        builder.Field<Number>("金额", "Amount")
               .Set(f => f.Unit, "元")
               .Set(f => f.OnValueChanged, OnQPAChanged)
               .Build();
    }

    protected override void BuildButtons(RenderTreeBuilder builder) { }

    private static void OnQPAChanged(FieldContext context)
    {
        var qty = context.Fields["Qty"].ValueAs<decimal>();
        if (context.FieldId == "Qty" || context.FieldId == "Price")
        {
            var price = context.Fields["Price"].ValueAs<decimal>();
            if (qty * price > 0)
                context.Fields["Amount"].SetValue(Utils.Round(qty * price, 2));
        }
        else if (context.FieldId == "Amount")
        {
            var amount = context.Fields["Amount"].ValueAs<decimal>();
            context.Fields["Price"].SetValue(qty == 0 ? 0 : Utils.Round(amount / qty, 4));
        }
    }
}
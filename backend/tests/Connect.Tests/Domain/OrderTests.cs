using Connect.Domain.Entities;
using Connect.Domain.Enums;
using Xunit;

namespace Connect.Tests.Domain;

public class OrderTests
{
    private static Product Product(decimal price, int stock = 100) =>
        new() { Sku = "SKU", Name = "Test", UnitPrice = price, StockQuantity = stock };

    private static Order NewOrder() => new("ORD-1", Guid.NewGuid(), Guid.NewGuid());

    [Fact]
    public void AddLine_adds_a_new_line()
    {
        var order = NewOrder();

        order.AddLine(Product(10m), 2);

        Assert.Single(order.Lines);
        Assert.Equal(2, order.Lines.First().Quantity);
    }

    [Fact]
    public void AddLine_same_product_merges_quantity()
    {
        var order = NewOrder();
        var product = Product(10m);

        order.AddLine(product, 2);
        order.AddLine(product, 3);

        Assert.Single(order.Lines);
        Assert.Equal(5, order.Lines.First().Quantity);
    }

    [Fact]
    public void TotalAmount_sums_all_line_totals()
    {
        var order = NewOrder();

        order.AddLine(Product(10m), 2); // 20
        order.AddLine(Product(5.5m), 4); // 22

        Assert.Equal(42m, order.TotalAmount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddLine_rejects_non_positive_quantity(int quantity)
    {
        var order = NewOrder();

        Assert.Throws<ArgumentOutOfRangeException>(() => order.AddLine(Product(10m), quantity));
    }

    [Fact]
    public void New_order_starts_pending()
    {
        Assert.Equal(OrderStatus.Pending, NewOrder().Status);
    }
}

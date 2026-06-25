using Connect.Application.Features.Products;
using Xunit;

namespace Connect.Tests.Application;

public class PagedRequestTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(3, 3)]
    public void Page_is_clamped_to_at_least_one(int input, int expected)
    {
        var query = new ProductsQuery { Page = input };
        Assert.Equal(expected, query.Page);
    }

    [Theory]
    [InlineData(0, 20)]    // invalid -> default
    [InlineData(500, 20)]  // over max -> default
    [InlineData(50, 50)]   // valid -> kept
    public void PageSize_is_clamped_to_a_safe_range(int input, int expected)
    {
        var query = new ProductsQuery { PageSize = input };
        Assert.Equal(expected, query.PageSize);
    }

    [Fact]
    public void Skip_is_derived_from_page_and_size()
    {
        var query = new ProductsQuery { Page = 3, PageSize = 20 };
        Assert.Equal(40, query.Skip);
    }
}

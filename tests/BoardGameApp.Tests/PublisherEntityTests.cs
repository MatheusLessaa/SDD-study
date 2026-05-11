using BoardGameApp.Domain.Publishers;

namespace BoardGameApp.Tests;

public class PublisherEntityTests
{
    [Fact]
    public void Publisher_has_expected_default_values()
    {
        var publisher = new Publisher();

        Assert.Equal(0, publisher.Id);
        Assert.Equal(string.Empty, publisher.Name);
    }

    [Fact]
    public void Publisher_allows_setting_required_spec_fields()
    {
        var publisher = new Publisher
        {
            Id = 4,
            Name = "Galapagos"
        };

        Assert.Equal(4, publisher.Id);
        Assert.Equal("Galapagos", publisher.Name);
    }
}

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONE.Enterprise.Twin;

namespace Aquarius.ONE.ClientSDK.Tests.Enterprise.Twin;

public class HelperTests
{
    private const string Root = "MyRoot";
    private const string Path = "MyPath";
    private const string TestProperty = "MyProperty";
    private const string TestPath = Root + "\\" + Path;
    
    private readonly IFixture _fixture = new Fixture();
    
    private DigitalTwin GetDigitalTwin(object propertyValue)
    {
        var twinData = JObject.FromObject(
            new
            {
                MyRoot = new
                {
                    MyPath = new
                    {
                        MyProperty = propertyValue
                    }
                }
            });
        
        return _fixture.Build<DigitalTwin>()
            .With(t => t.TwinData, twinData.ToString)
            .Create();
    }
    
    [Fact]
    public void GetTwinDataProperty_TwinIsNull_ReturnsEmpty()
    {
        var result = Helper.GetTwinDataProperty(null, TestPath, TestProperty);

        result.Should().BeEmpty();
    }
    
    [Fact]
    public void GetTwinDataProperty_TwinDataIsNull_ReturnsEmpty()
    {
        var digitalTwin = _fixture.Build<DigitalTwin>()
            .Without(t => t.TwinData)
            .Create();

        var result = Helper.GetTwinDataProperty(digitalTwin, TestPath, TestProperty);

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetTwinDataProperty_TwinDataIsEmpty_ReturnsEmpty()
    {
        var digitalTwin = _fixture.Build<DigitalTwin>()
            .With(t => t.TwinData, string.Empty)
            .Create();

        var result = Helper.GetTwinDataProperty(digitalTwin, TestPath, TestProperty);

        result.Should().BeEmpty();
    }
    
    [Fact]
    public void GetTwinDataProperty_TwinDataIsInvalidJson_Throws()
    {
        var digitalTwin = _fixture.Build<DigitalTwin>()
            .With(t => t.TwinData, "invalidJson")
            .Create();
        
        var testAction = () => Helper.GetTwinDataProperty(digitalTwin, TestPath, TestProperty);

        testAction.Should().Throw<JsonReaderException>();
    }
    
    [Fact]
    public void GetTwinDataProperty_GivenInvalidPath_ReturnsEmpty()
    {
        var digitalTwin = GetDigitalTwin(string.Empty);
        
        var result = Helper.GetTwinDataProperty(digitalTwin, "invalidPath", TestProperty);

        result.Should().BeEmpty();
    }
    
    [Fact]
    public void GetTwinDataProperty_GivenInvalidPropertyName_ReturnsEmpty()
    {
        var digitalTwin = GetDigitalTwin(string.Empty);

        var result = Helper.GetTwinDataProperty(digitalTwin, TestPath, "invalidProperty");

        result.Should().BeEmpty();
    }
    
    public static IEnumerable<object[]> TestDateTimes =>
        new List<object[]>
        {
            new object[] { DateTime.MinValue },
            new object[] { DateTime.MaxValue },
            new object[] { DateTime.UtcNow },
            new object[] { DateTime.Now },
            new object[] { DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified) },
        };
    
    [Theory]
    [MemberData(nameof(TestDateTimes))]
    public void GetTwinDataProperty_PropertyIsValidDateTime_ReturnsExpected(object propertyValue)
    {
        Assert.True(propertyValue is DateTime, "propertyValue is DateTime");
        
        var digitalTwin = GetDigitalTwin(propertyValue);

        var result = Helper.GetTwinDataProperty(digitalTwin, TestPath, TestProperty);
        var resultDateTime = DateTime.Parse(result, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        resultDateTime.Should().Be((DateTime)propertyValue);
    }
}

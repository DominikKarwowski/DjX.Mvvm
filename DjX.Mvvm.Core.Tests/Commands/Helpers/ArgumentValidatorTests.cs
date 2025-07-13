using DjX.Mvvm.Core.Commands.Helpers;

namespace DjX.Mvvm.Core.Tests.Commands.Helpers;

[TestFixture]
public class ArgumentValidatorTests
{
    public static object[] ValidObjectCases =
    [
        new object[] { 1, typeof(int) },
        new object[] { true, typeof(bool) },
        new object[] { "hello", typeof(string) },
        new object[] { new DateTime(1985, 07, 23), typeof(DateTime) },
        new object[] { new List<string>{ "abc", "xyz" }, typeof(List<string>) },
    ];

    public static object[] InvalidObjectCases =
    [
        new object[] { 1, typeof(decimal) },
        new object[] { true, typeof(string) },
        new object[] { "hello", typeof(char) },
        new object[] { new DateTime(1985, 07, 23), typeof(TimeSpan) },
        new object[] { new List<string>{ "abc", "xyz" }, typeof(IEnumerable<string>) },
    ];
    
    [TestCaseSource(nameof(ValidObjectCases))]
    public void ThrowIfNullOrNotOfType_does_not_throw_if_parameter_is_valid(object param, Type expectedType)
    {
        var sut = typeof(ArgumentValidator)
            .GetMethod(nameof(ArgumentValidator.ThrowIfNullOrNotOfType))!
            .MakeGenericMethod(expectedType);
        
        Assert.DoesNotThrow(
            () => sut.Invoke(null, [param]));
    }

    [Test]
    public void ThrowIfNullOrNotOfType_throws_if_parameter_is_null()
    {
        object? testParam = null;
        
        Assert.Throws<ArgumentException>(
            () => ArgumentValidator.ThrowIfNullOrNotOfType<object?>(testParam),
            "The parameter cannot be null and must be of type System.Int32");
    }
    
    [Test]
    public void ThrowIfNullOrNotOfType_throws_if_parameter_is_of_incorrect_type()
    {
        object? testParam = null;
        
        Assert.Throws<ArgumentException>(
            () => ArgumentValidator.ThrowIfNullOrNotOfType<int>(testParam),
            "The parameter cannot be null and must be of type System.Int32");
    }
}

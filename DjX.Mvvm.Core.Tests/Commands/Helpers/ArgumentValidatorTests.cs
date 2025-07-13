using DjX.Mvvm.Core.Commands.Helpers;

namespace DjX.Mvvm.Core.Tests.Commands.Helpers;

[TestFixture]
public class ArgumentValidatorTests
{
    private static object[] validObjectCases =
    [
        new object[] { 1, typeof(int) },
        new object[] { true, typeof(bool) },
        new object[] { "hello", typeof(string) },
        new object[] { new DateTime(1985, 07, 23), typeof(DateTime) },
        new object[] { new List<string>{ "abc", "xyz" }, typeof(List<string>) },
    ];

    [TestCaseSource(nameof(validObjectCases))]
    public void ThrowIfNotOfType_does_not_throw_if_parameter_is_valid(object param, Type expectedType)
    {
        var sut = typeof(ArgumentValidator)
            .GetMethod(nameof(ArgumentValidator.ThrowIfNotOfType))!
            .MakeGenericMethod(expectedType);
        
        Assert.DoesNotThrow(() => sut.Invoke(null, [param]));
    }

    [Test]
    public void ThrowIfNotOfType_does_not_throw_if_parameter_is_null()
    {
        Assert.DoesNotThrow(() => ArgumentValidator.ThrowIfNotOfType<object?>(null));
    }
    
    [Test]
    public void ThrowIfNotOfType_throws_if_parameter_is_of_incorrect_type()
    {
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentException>(
                () => ArgumentValidator.ThrowIfNotOfType<decimal>(1),
                $"Non null parameter must be of type {typeof(decimal).FullName}");

            Assert.Throws<ArgumentException>(
                () => ArgumentValidator.ThrowIfNotOfType<string>(true),
                $"Non null parameter must be of type {typeof(decimal).FullName}");

            Assert.Throws<ArgumentException>(
                () => ArgumentValidator.ThrowIfNotOfType<char>("hello"),
                $"Non null parameter must be of type {typeof(char).FullName}");

            Assert.Throws<ArgumentException>(
                () => ArgumentValidator.ThrowIfNotOfType<TimeSpan>(new DateTime(1985, 07, 23)),
                $"Non null parameter must be of type {typeof(TimeSpan).FullName}");

            Assert.Throws<ArgumentException>(
                () => ArgumentValidator.ThrowIfNotOfType<IEnumerable<string>>(new List<string>{ "abc", "xyz" }),
                $"Non null parameter must be of type {typeof(IEnumerable<string>).FullName}");
        });        
    }
}

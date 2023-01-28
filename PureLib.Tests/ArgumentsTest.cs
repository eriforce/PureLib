using System.Collections.ObjectModel;
using PureLib.Common;
using Shouldly;

namespace PureLib.Tests {
    public class ArgumentsTest {
        private readonly Arguments _arguments = new Arguments(new[] {
            "-f", "format", "/d", "100.0", "dd", "--delay", "-1000",
        });

        [Theory]
        [InlineData("f", true)]
        [InlineData("d", true)]
        [InlineData("delay", true)]
        [InlineData("format", false)]
        public void Arguments_Contains_ShouldReturnTrueIfKeyExists(string key, bool expectedExists) {
            _arguments.Contains(key).ShouldBe(expectedExists);
        }

        [Theory]
        [InlineData("f", "format", true)]
        [InlineData("d", "100.0", true)]
        [InlineData("delay", "-1000", true)]
        [InlineData("format", null, false)]
        public void Arguments_TryGetValue_ShouldGetValue(string key, string expectedValue, bool expectedExists) {
            _arguments.TryGetValue(key, out var value).ShouldBe(expectedExists);
            value.ShouldBe(expectedValue);
        }

        [Theory]
        [InlineData("f", "format", true)]
        [InlineData("d", "100.0", true)]
        [InlineData("delay", "-1000", true)]
        [InlineData("format", "test", false)]
        public void Arguments_GetValueOrDefault_ShouldGetValue(string key, string expectedValue, bool expectedExists) {
            bool exists;
            string value = _arguments.Contains(key) ?
                _arguments.GetValueOrDefault(key, out exists) :
                _arguments.GetValueOrDefault(key, out exists, expectedValue);

            exists.ShouldBe(expectedExists);
            value.ShouldBe(expectedValue);
        }

        [Fact]
        public void Arguments_GetValueOrDefaultGeneric_ShouldGetValue() {
            bool exists;

            _arguments.GetValueOrDefault("d", out exists, double.Parse, 10).ShouldBe(100.0);
            exists.ShouldBeTrue();
            _arguments.GetValueOrDefault("dd", out exists, double.Parse, 10).ShouldBe(10.0);
            exists.ShouldBeFalse();

            _arguments.GetValueOrDefault("delay", out exists, int.Parse, 10).ShouldBe(-1000);
            exists.ShouldBeTrue();
            _arguments.GetValueOrDefault("delay2", out exists, int.Parse, -100).ShouldBe(-100);
            exists.ShouldBeFalse();
        }

        [Fact]
        public void Arguments_TryGetValues_ShouldGetValues() {
            ReadOnlyCollection<string> values;

            _arguments.TryGetValues("f", out values).ShouldBeTrue();
            values.ShouldBe(new[] { "format" });

            _arguments.TryGetValues("d", out values).ShouldBeTrue();
            values.ShouldBe(new[] { "100.0", "dd" });

            _arguments.TryGetValues("dd", out values).ShouldBeFalse();
            values.ShouldBeNull();
        }
    }
}

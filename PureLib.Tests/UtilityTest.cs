using PureLib.Common;
using Shouldly;

namespace PureLib.Tests {
    public class UtilityTest {
        [Fact]
        public void Utility_ChunkBy_ShouldReturnChunks() {
            var list = new List<int> { 1, 2, 3, 4, 5 };

            var chunks = list.ChunkBy(2);

            chunks.ShouldBeEquivalentTo(new List<int[]> {
                new[] { 1,2 },
                new[] { 3,4 },
                new[] { 5 },
            });
        }

        [Fact]
        public void Utility_ToEnum_ShouldParseEnum() {
            var enumString = "Sunday,Monday";

            var enums = enumString.ToEnum<DayOfWeek>();

            enums.ShouldBe(new[] {
                DayOfWeek.Sunday,
                DayOfWeek.Monday,
            });
        }
    }
}

using PureLib.Common;
using Shouldly;

namespace PureLib.Tests {
    public class PathWrapperTest {
        [Theory]
        [InlineData("test/File*Name.txt", '_', "test_File_Name.txt")]
        [InlineData("<test>File|Name.txt", '.', ".test.File.Name.txt")]
        public void PathWrapper_EscapeFileName_ShouldEscapeFileName(string fileName, char replacement, string expected) {
            fileName.EscapeFileName(replacement).ShouldBe(expected);
        }

        [Theory]
        [InlineData(@"C:\fold|er\test/File*Name.txt", '_', @"C:\fold_er\test_File_Name.txt")]
        [InlineData(@"D:\<test>\File|Name?!.txt", '.', @"D:\.test.\File.Name.!.txt")]
        public void PathWrapper_EscapePath_ShouldEscapePath(string path, char replacement, string expected) {
            path.EscapePath(replacement).ShouldBe(expected);
        }
    }
}

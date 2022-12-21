using System.Text;
using PureLib.Common;
using Shouldly;

namespace PureLib.Tests {
    public class Base64UrlTest {
        [Fact]
        public void Base64Url_Encode_ShouldReturnEncodedString() {
            var expected = "5aSH5rOo77ya5YiG6Iiq57q_6L-QIQ";

            var data = Encoding.UTF8.GetBytes("备注：分航线运!");
            var result = Base64Url.Encode(data);

            result.ShouldBe(expected);
        }

        [Fact]
        public void Base64Url_Decode_ShouldReturnData() {
            var expected = "备注：分航线运!";

            var base64Url = "5aSH5rOo77ya5YiG6Iiq57q_6L-QIQ";
            var data = Base64Url.Decode(base64Url);
            var result = Encoding.UTF8.GetString(data);

            result.ShouldBe(expected);
        }
    }
}
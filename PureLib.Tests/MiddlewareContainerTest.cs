using PureLib.Common;
using Shouldly;

namespace PureLib.Tests {
    public class MiddlewareContainerTest {
        [Fact]
        public void MiddlewareContainer_Use_ShouldComposeMiddlewares() {
            var container = new MiddlewareContainer<Context>();
            var middleware1 = (Context ctx, Func<Task> next) => {
                ctx.Result += "1";
                next();
                ctx.Result += "2";
                return Task.CompletedTask;
            };
            var middleware2 = (Context ctx, Func<Task> next) => {
                ctx.Result += "3";
                next();
                ctx.Result += "4";
                return Task.CompletedTask;
            };
            container.Use(middleware1);
            container.Use(middleware2);

            var app = container.Compose();
            var context = new Context();
            app(context);

            context.Result.ShouldBe("1342");
        }

        public class Context {
            public string Result { get; set; } = string.Empty;
        }
    }
}

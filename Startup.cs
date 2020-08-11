using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CommentsTest.Startup))]
namespace CommentsTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

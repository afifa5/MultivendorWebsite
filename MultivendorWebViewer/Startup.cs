using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartupAttribute(typeof(MultivendorWebViewer.Startup))]
namespace MultivendorWebViewer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

    }
}

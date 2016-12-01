using Topshelf;

namespace BtsPortal.Services.EsbAlert
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<Process>(s =>
                {
                    s.ConstructUsing(name => new Process());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.OnException(Process.HandleException);

                x.RunAsPrompt();

                x.SetDescription("BTS ESB Alert Service");
                x.SetDisplayName(AppSettings.SERVICE_NAME);
                x.SetServiceName(AppSettings.SERVICE_NAME);
            });
        }
    }
}

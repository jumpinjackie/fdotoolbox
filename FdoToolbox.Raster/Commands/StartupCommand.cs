using ICSharpCode.Core;

namespace FdoToolbox.Raster.Commands
{
    class StartupCommand : AbstractCommand
    {
        public override void Run()
        {
            ResourceService.RegisterNeutralStrings(Strings.ResourceManager);
        }
    }
}

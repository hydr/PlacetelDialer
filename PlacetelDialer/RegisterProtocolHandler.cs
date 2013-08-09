using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
// ReSharper disable PossibleNullReferenceException
namespace PlacetelDialer
{
    public class RegisterProtocolHandler
    {
        private readonly string _exeFilePath; 
        private readonly string _iconFilePath;
        private string Command { get { return "\"" + _exeFilePath + "\" \"%1\""; } }

        private readonly List<string> _protocols = new List<string>{"tel", "callto"};
        private const string ProtocolName = "placetel.callto";

        public RegisterProtocolHandler()
        {
            _exeFilePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            _iconFilePath = System.Reflection.Assembly.GetEntryAssembly().Location + ",0";
        }

        public RegisterProtocolHandler(string exeFilePath, string iconFilePath)
        {
            _iconFilePath = iconFilePath;
            _exeFilePath = exeFilePath;
        }

        public void Register()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (!isAdmin)
            {
                Console.WriteLine("You need admin rights to register the protocols.");
                return;
            }
            Console.WriteLine("Registering the protocols.");

            //Define our own class
            var protocolClass = Registry.ClassesRoot.CreateSubKey(ProtocolName);
            CreateProtocolTree(protocolClass);

            //Define our application in SOFTWARE\Clients\Internet Call\PlacetelDialer with its capabilities
            var programm = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Clients\Internet Call\PlacetelDialer");
            CreateTree(programm);

            //Set the protocol current values to the Placetel Dialer
            foreach (var protocol in _protocols)
            {
                var prot = Registry.ClassesRoot.CreateSubKey(protocol);
                CreateProtocolTree(prot);
            }
            Console.WriteLine("Protocols registered.");
        }

        private void CreateTree(RegistryKey baseKey)
        {
            var capabilities = baseKey.CreateSubKey(@"Capabilities");

            capabilities.SetValue("ApplicationName", "Placetel Dialer");
            capabilities.SetValue("ApplicationDescription", "Placetel Dialer");
            capabilities.SetValue("ApplicationIcon", _iconFilePath);

            var associations = capabilities.CreateSubKey(@"URLAssociations");
            foreach (var protocol in _protocols)
            {
                associations.SetValue(protocol, ProtocolName);
            }

            var protocols = baseKey.CreateSubKey(@"Protocols");
            foreach (var protocol in _protocols)
            {
                var p = protocols.CreateSubKey(protocol);
                CreateProtocolTree(p);
            }

            var command = protocols.CreateSubKey(@"shell\open\command");
            command.SetValue("",Command);
        }

        //Helper to define the protocol handler
        private void CreateProtocolTree(RegistryKey baseKey)
        {
            baseKey.SetValue("", "URL: CallTo Protocoll");

            var defaultIcon = baseKey.CreateSubKey("DefaultIcon");
            defaultIcon.SetValue("", _iconFilePath);

            var commandInner = baseKey.CreateSubKey(@"shell\open\command");
            commandInner.SetValue("",Command);
        }
    }

}
// ReSharper restore PossibleNullReferenceException
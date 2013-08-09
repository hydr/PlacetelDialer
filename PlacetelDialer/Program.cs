using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace PlacetelDialer
{
    class Program
    {
        private static Configuration _config;
        private static PlacetelDialerConfig _dialerConfig;
        private const string SectionName = "placetelDialer";

        static void Main(string[] args)
        {
            Init();
            CheckSettings();

            string phoneNumber = null;
            if (args.Length > 0)
                phoneNumber = args[0];

            if (phoneNumber == null)
            {
                Console.Write("Please enter the phone number: ");
                phoneNumber = Console.ReadLine();
                Console.WriteLine();
            }

            if (phoneNumber == null)
                throw new ArgumentException("No phone number entered");

            if (phoneNumber.Length > 50)
                throw new ArgumentException("Phone number is too long");

            Console.WriteLine("Dialing the phone number '{0}'", phoneNumber);

            phoneNumber = Regex.Replace(phoneNumber, @"^\+", "00"); //Placetel can't handel the + in the beginning
            phoneNumber = Regex.Replace(phoneNumber, @"[^0-9]", ""); //Strip all non number characters from the number.



            var client = new PlacetelApiClient.PlacetelClient(_dialerConfig.ApiKey);
            client.InitiateCall(_dialerConfig.SipUser, phoneNumber);
        }


        private static void Init()
        {
            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);

            // Map the roaming configuration file. This enables the application to access  the configuration file using the
            // System.Configuration.Configuration class
            //var configFileMap = new ExeConfigurationFileMap {ExeConfigFilename = roamingConfig.FilePath};

            // Get the mapped configuration file.
            //_config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            //Get the configuration section or create it
            _dialerConfig = (PlacetelDialerConfig) _config.GetSection(SectionName);
            if (_dialerConfig == null)
            {
                _dialerConfig = new PlacetelDialerConfig();

                _dialerConfig.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                _dialerConfig.SectionInformation.AllowOverride = true;

                _config.Sections.Add(SectionName, _dialerConfig);
                //Save the initial state
                _config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(SectionName);
            }
        }

        private static void SaveConfig()
        {
            _config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(SectionName);
        }

        public static void CheckSettings()
        {

            while (string.IsNullOrEmpty(_dialerConfig.ApiKey))
            {
                Console.Write("Please enter the Placetel API Key:");
                var enteredKey = Console.ReadLine();

                //Test the key
                var testClient = new PlacetelApiClient.PlacetelClient(enteredKey);
                var success = testClient.Test();
                if (success)
                    _dialerConfig.ApiKey = enteredKey;
                else
                    Console.WriteLine("The key was not tested successfully!");
            }

            SaveConfig();

            while (string.IsNullOrEmpty(_dialerConfig.SipUser))
            {
                var testClient = new PlacetelApiClient.PlacetelClient(_dialerConfig.ApiKey);
                var sipUsers = testClient.GetVoipUsers();

                Console.WriteLine("Select a telephone line:");
                for (var i = 0; i < sipUsers.Count; i++)
                {
                    var s = sipUsers[i];
                    Console.WriteLine("{0:000}: {1} ({2})", i+1, s.Name, s.Uid);
                }

                var enteredId = Console.ReadLine();
                int id;
                if (!int.TryParse(enteredId, out id) || sipUsers.ElementAtOrDefault(id-1) == null)
                {
                    Console.WriteLine("You must enter the ID of the line!");
                    continue;
                }
                var sipUser = sipUsers.ElementAt(id - 1);
                _dialerConfig.SipUser = sipUser.Uid;
            }

            SaveConfig();
        }


    }
}

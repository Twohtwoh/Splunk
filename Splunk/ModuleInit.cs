
/// <summary>
/// This is a module initializer.  This is an optional aspect to a module.
/// This code is run whenever ServiceHostManager is started so this code can be
/// used to check and enforce branding, ensure certain things are present, make registrations,
/// enforce licensing and more.  Since this code is run on EVERY start of SHM it should not 
/// always recreate the same things, but should check and create as needed.
/// </summary>




using DecisionsFramework;
using DecisionsFramework.Design;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.ServiceLayer.Services.Folder;
using DecisionsFramework.ServiceLayer.Services.Portal;
using DecisionsFramework.ServiceLayer.Utilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// This is a module initializer.  This is an optional aspect to a module.
/// This code is run whenever ServiceHostManager is started so this code can be
/// used to check and enforce branding, ensure certain things are present, make registrations,
/// enforce licensing and more.  Since this code is run on EVERY start of SHM it should not 
/// always recreate the same things, but should check and create as needed.
/// </summary>
namespace Splunk
{
    public class ModuleInit : ILogWriter, IInitializable
    {
        public static object instance;
        SplunkSettings SettingsInDecisionsForSplunk = ModuleSettingsAccessor<SplunkSettings>.GetSettings();
        public void Initialize()
        {

            instance = this;
            Log.AddLogWriter(this);
            //ChangedBranding();

            //SetupFolders();

            // EnsureCustomSettingsObject();

        }



        public void Write(LogData LogMessage)
        {

            if (SettingsInDecisionsForSplunk.EnableLogging == true)
            {


                if (LogMessage.Level == LogSeverity.Debug && SettingsInDecisionsForSplunk.LogDebug == true)
                {
                    PostToSplunk(LogMessage);
                }
                else if (LogMessage.Level == LogSeverity.Error && SettingsInDecisionsForSplunk.LogError == true)
                {
                    PostToSplunk(LogMessage);
                }
                else if (LogMessage.Level == LogSeverity.Fatal && SettingsInDecisionsForSplunk.LogFatal == true)
                {
                    PostToSplunk(LogMessage);
                }
                else if (LogMessage.Level == LogSeverity.Info && SettingsInDecisionsForSplunk.LogInfo == true)
                {
                    PostToSplunk(LogMessage);
                }

                else if (LogMessage.Level == LogSeverity.Warn && SettingsInDecisionsForSplunk.LogWarn == true)
                {
                    PostToSplunk(LogMessage);
                }


            }


        }


        public string GetIPAddress()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); // `Dns.Resolve()` method is deprecated.
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            return ipAddress.ToString();
        }





        public Boolean PostToSplunk(LogData inputOfError)
        {
            //SplunkSettings SettingsInDecisionsForSplunk = ModuleSettingsAccessor<SplunkSettings>.GetSettings();

            var client = new RestClient(SettingsInDecisionsForSplunk.SplunkCollectorURL);
            client.Timeout = -1;
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Splunk " + SettingsInDecisionsForSplunk.SplunkToken);
            request.AddHeader("Content-Type", "text/plain");
            var stringAsJson = JsonConvert.SerializeObject(inputOfError);
            var stringAsJson2 = "{\"event\":\"DecisionsLog\",\"fields\":" + stringAsJson + "}";
            request.AddParameter("text/plain", stringAsJson2, ParameterType.RequestBody);
            try
            {
                IRestResponse response = client.Execute(request);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }


        //private void SetupFolders()
        //{
        //    SystemUserContext system = new SystemUserContext();
        //    if (FolderService.Instance.Exists(system, "MY CO ROOT FOLDER") == false) {
        ///        // The null here is for a Folder Bheavior which allows a lot of customization
        //        // of a folder including custom actions and a lot of filtering capability.
        //        FolderService.Instance.CreateRootFolder(system, "MY CO ROOT FOLDER", "My Company Designs", null);
        //    }
        //}

        //private void ChangedBranding()
        //{
        //    ModuleSettingsAccessor<PortalSettings>.Instance.SloganText = "My Custom Rule Portal";

        //    ModuleSettingsAccessor<DesignerSettings>.Instance.StudioSlogan = "My Custom Rule Studio";

        //}
    }
}

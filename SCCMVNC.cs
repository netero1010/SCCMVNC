namespace SCCMVNC
{
    using System;
    using System.Management;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    
    class Program
    {
        static void PrintStatus(string message)
        {
            int totalWidth = 70;
            string separatorLine = new string('-', totalWidth);

            int padding = (totalWidth - message.Length) / 2;
            string output = "|" + new string(' ', padding) + message + new string(' ', padding) + "|";

            if (output.Length < totalWidth)
            {
                output = output.Insert(output.Length - 1, " ");
            }

            Console.WriteLine(separatorLine);
            Console.WriteLine(output);
            Console.WriteLine(separatorLine);
        }

        static void PrintError(string message)
        {
            Console.WriteLine("[-] {0}", message);
        }

        static void ReadSccmRemoteControlConfig(string computerName)
        {
            try
            {
                if (computerName != ".")
                {
                    PrintStatus(String.Format("Connecting to {0}...", computerName));
                }
                else
                {
                    PrintStatus("Executing in the localhost...");
                }

                // Construct the WMI scope path
                string wmiScopePath = String.Format(@"\\{0}\root\ccm\policy\Machine\ActualConfig", computerName);

                // Connect to the WMI namespace
                ManagementScope scope = new ManagementScope(wmiScopePath);
                scope.Connect();

                // WMI query to get the Ccm_RemoteToolsConfig class
                ObjectQuery query = new ObjectQuery("SELECT * FROM Ccm_RemoteToolsConfig");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection queryCollection = searcher.Get();

                // Iterate through the results (usually only one)
                foreach (ManagementObject mObject in queryCollection)
                {
                    // Extract the Permitted Viewers
                    string currentPermittedViewers = mObject["PermittedViewers"] == null ? "" : string.Join(",", mObject["PermittedViewers"] as string[]);

                    // Display the current value
                    Console.WriteLine("{0,-60}: {1}", "AccessLevel", mObject["AccessLevel"]);
                    Console.WriteLine("{0,-60}: {1}", "AllowClientChange", mObject["AllowClientChange"]);
                    Console.WriteLine("{0,-60}: {1}", "AllowLocalAdminToDoRemoteControl", mObject["AllowLocalAdminToDoRemoteControl"]);
                    Console.WriteLine("{0,-60}: {1}", "AllowRAUnsolicitedControl", mObject["AllowRAUnsolicitedControl"]);
                    Console.WriteLine("{0,-60}: {1}", "AllowRAUnsolicitedView", mObject["AllowRAUnsolicitedView"]);
                    Console.WriteLine("{0,-60}: {1}", "AllowRemCtrlToUnattended", mObject["AllowRemCtrlToUnattended"]);
                    Console.WriteLine("{0,-60}: {1}", "AudibleSignal", mObject["AudibleSignal"]);
                    Console.WriteLine("{0,-60}: {1}", "ClipboardAccessPermissionRequired", mObject["ClipboardAccessPermissionRequired"]);
                    Console.WriteLine("{0,-60}: {1}", "Enabled", mObject["Enabled"]);
                    Console.WriteLine("{0,-60}: {1}", "EnableRA", mObject["EnableRA"]);
                    Console.WriteLine("{0,-60}: {1}", "EnableTS", mObject["EnableTS"]);
                    Console.WriteLine("{0,-60}: {1}", "PermittedViewers", currentPermittedViewers);
                    Console.WriteLine("{0,-60}: {1}", "PermissionRequired", mObject["PermissionRequired"]);
                    Console.WriteLine("{0,-60}: {1}", "RemCtrlConnectionBar", mObject["RemCtrlConnectionBar"]);
                    Console.WriteLine("{0,-60}: {1}", "RemCtrlTaskbarIcon", mObject["RemCtrlTaskbarIcon"]);
                }
            }
            catch (ManagementException ex)
            {
                if (ex.ErrorCode == ManagementStatus.InvalidNamespace)
                {
                    PrintError("The specified WMI namespace is not valid.");
                }
                else
                {
                    PrintError("A management exception has occurred: " + ex.Message);
                }
            }
            catch (UnauthorizedAccessException)
            {
                PrintError("You do not have permission to access the remote computer.");
            }
            catch (Exception e)
            {
                PrintError("An unexpected error has occurred: " + e.Message);
            }
        }

        static void UpdateSccmRemoteControlConfig(string computerName, bool viewOnly, string viewer)
        {
            try
            {
                if (computerName != ".")
                {
                    PrintStatus(String.Format("Connecting to {0}...", computerName));
                }

                // Construct the WMI scope path
                string wmiScopePath = String.Format(@"\\{0}\root\ccm\policy\Machine\ActualConfig", computerName);

                // Connect to the WMI namespace
                ManagementScope scope = new ManagementScope(wmiScopePath);
                scope.Connect();

                // WMI query to get the Ccm_RemoteToolsConfig class
                ObjectQuery query = new ObjectQuery("SELECT * FROM Ccm_RemoteToolsConfig");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection queryCollection = searcher.Get();

                // Iterate through the results (It is supposed to be just one)
                foreach (ManagementObject mObject in queryCollection)
                {
                    // Extract the Permitted Viewers
                    string currentPermittedViewers = mObject["PermittedViewers"] == null ? "" : string.Join(",", mObject["PermittedViewers"] as string[]);
                    
                    // Display the current value
                    PrintStatus("Current setting");
                    Console.WriteLine("{0,-60}: {1}", "Current AccessLevel", mObject["AccessLevel"]);
                    Console.WriteLine("{0,-60}: {1}", "Current AllowClientChange", mObject["AllowClientChange"]);
                    Console.WriteLine("{0,-60}: {1}", "Current AllowLocalAdminToDoRemoteControl", mObject["AllowLocalAdminToDoRemoteControl"]);
                    Console.WriteLine("{0,-60}: {1}", "Current AllowRAUnsolicitedControl", mObject["AllowRAUnsolicitedControl"]);
                    Console.WriteLine("{0,-60}: {1}", "Current AllowRAUnsolicitedView", mObject["AllowRAUnsolicitedView"]);
                    Console.WriteLine("{0,-60}: {1}", "Current AllowRemCtrlToUnattended", mObject["AllowRemCtrlToUnattended"]);
                    Console.WriteLine("{0,-60}: {1}", "Current AudibleSignal", mObject["AudibleSignal"]);
                    Console.WriteLine("{0,-60}: {1}", "Current ClipboardAccessPermissionRequired", mObject["ClipboardAccessPermissionRequired"]);
                    Console.WriteLine("{0,-60}: {1}", "Current Enabled", mObject["Enabled"]);
                    Console.WriteLine("{0,-60}: {1}", "Current EnableRA", mObject["EnableRA"]);
                    Console.WriteLine("{0,-60}: {1}", "Current EnableTS", mObject["EnableTS"]);
                    Console.WriteLine("{0,-60}: {1}", "Current PermittedViewers", currentPermittedViewers);
                    Console.WriteLine("{0,-60}: {1}", "Current PermissionRequired", mObject["PermissionRequired"]);
                    Console.WriteLine("{0,-60}: {1}", "Current RemCtrlConnectionBar", mObject["RemCtrlConnectionBar"]);
                    Console.WriteLine("{0,-60}: {1}", "Current RemCtrlTaskbarIcon", mObject["RemCtrlTaskbarIcon"]);

                    // Set to view only with no remote control
                    mObject["AccessLevel"] = viewOnly ? 1 : 2;
                    mObject["AllowClientChange"] = 0;
                    mObject["AllowLocalAdminToDoRemoteControl"] = 1;
                    mObject["AllowRAUnsolicitedControl"] = 1;
                    mObject["AllowRAUnsolicitedView"] = 1;
                    mObject["AllowRemCtrlToUnattended"] = 1;
                    mObject["AudibleSignal"] = 0;
                    mObject["ClipboardAccessPermissionRequired"] = 0;
                    mObject["Enabled"] = 1;
                    mObject["EnableRA"] = 1;
                    mObject["EnableTS"] = 1;
                    // Add viewer
                    mObject["PermittedViewers"] = String.IsNullOrEmpty(viewer) ? null : viewer.Split(',');
                    mObject["PermissionRequired"] = 0;
                    mObject["RemCtrlConnectionBar"] = 0;
                    mObject["RemCtrlTaskbarIcon"] = 0;

                    PrintStatus("New setting");
                    mObject.Put();

                    // Display the new value
                    Console.WriteLine("{0,-60}: {1}", "New AccessLevel", mObject["AccessLevel"]);
                    Console.WriteLine("{0,-60}: {1}", "New AllowClientChange", mObject["AllowClientChange"]);
                    Console.WriteLine("{0,-60}: {1}", "New AllowLocalAdminToDoRemoteControl", mObject["AllowLocalAdminToDoRemoteControl"]);
                    Console.WriteLine("{0,-60}: {1}", "New AllowRAUnsolicitedControl", mObject["AllowRAUnsolicitedControl"]);
                    Console.WriteLine("{0,-60}: {1}", "New AllowRAUnsolicitedView", mObject["AllowRAUnsolicitedView"]);
                    Console.WriteLine("{0,-60}: {1}", "New AllowRemCtrlToUnattended", mObject["AllowRemCtrlToUnattended"]);
                    Console.WriteLine("{0,-60}: {1}", "New AudibleSignal", mObject["AudibleSignal"]);
                    Console.WriteLine("{0,-60}: {1}", "New ClipboardAccessPermissionRequired", mObject["ClipboardAccessPermissionRequired"]);
                    Console.WriteLine("{0,-60}: {1}", "New Enabled", mObject["Enabled"]);
                    Console.WriteLine("{0,-60}: {1}", "New EnableRA", mObject["EnableRA"]);
                    Console.WriteLine("{0,-60}: {1}", "New EnableTS", mObject["EnableTS"]);
                    Console.WriteLine("{0,-60}: {1}", "New PermittedViewers", String.IsNullOrEmpty(viewer) ? "" : viewer);
                    Console.WriteLine("{0,-60}: {1}", "New PermissionRequired", mObject["PermissionRequired"]);
                    Console.WriteLine("{0,-60}: {1}", "New RemCtrlConnectionBar", mObject["RemCtrlConnectionBar"]);
                    Console.WriteLine("{0,-60}: {1}", "New RemCtrlTaskbarIcon", mObject["RemCtrlTaskbarIcon"]);
                }
            }
            catch (ManagementException ex)
            {
                if (ex.ErrorCode == ManagementStatus.InvalidNamespace)
                {
                    PrintError("The specified WMI namespace is not valid.");
                }
                else
                {
                    PrintError("A management exception occurred: " + ex.Message);
                }
            }
            catch (UnauthorizedAccessException)
            {
                PrintError("You do not have permission to access the remote computer.");
            }
            catch (Exception e)
            {
                PrintError("An unexpected error occurred: " + e.Message);
            }
        }

        static void Help()
        {
            Console.WriteLine(@"Read existing SCCM Remote Control setting:
SCCMVNC.exe read [/target:CLIENT01]

Re-configure SCCM Remote Control setting to mute all the user conent requirement and notifications:
SCCMVNC.exe reconfig [/target:CLIENT01] [/viewonly] [viewer:user01,user02]");
        }

        static void Main(string[] args)
        {
            bool viewOnly = false;
            string computerName = "."; // Default to local computer
            string viewer = null;
            string input = string.Join(" ", args);

            string[] processedArgs = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (processedArgs.Length == 0)
            {
                Help();
                return;
            }

            string action = processedArgs[0].ToLower();

            for (int i = 1; i < processedArgs.Length; i++)
            {
                if (processedArgs[i].Equals("/viewonly", StringComparison.OrdinalIgnoreCase))
                {
                    viewOnly = true;
                }
                else if (processedArgs[i].StartsWith("/target:", StringComparison.OrdinalIgnoreCase))
                {
                    computerName = processedArgs[i].Substring("/target:".Length);
                }
                else if (processedArgs[i].StartsWith("/viewer:", StringComparison.OrdinalIgnoreCase))
                {
                    viewer = processedArgs[i].Substring("/viewer:".Length);
                }
            }

            switch (action)
            {
                case "reconfig":
                    UpdateSccmRemoteControlConfig(computerName, viewOnly, viewer);
                    break;
                case "read":
                    ReadSccmRemoteControlConfig(computerName);
                    break;
                case "help":
                    Help();
                    break;
                default:
                    PrintError("Unknown action.");
                    break;
            }
        }
    }
}
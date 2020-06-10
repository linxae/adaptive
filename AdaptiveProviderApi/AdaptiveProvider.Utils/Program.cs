using System;
using System.Runtime.CompilerServices;
using AdaptiveProvider.Core.Configuration;
using AdaptiveProvider.TowerAnsible;

namespace AdaptiveProvider.Utils
{
    class Program
    {
        private const string EncryptCmdName = "--encrypt";

        private const string DecryptCmdName = "--decrypt";
        private const string CypherArgName = "-cipher:";

        private const string LaunchTowerJobCmdName = "--launchtowerjob";
        private const string LaunchTowerJobConnectionStringArgName = "-connectionstring:";
        private const string LaunchTowerJobTemplatedIdArgName = "-jobtemplatedid:";
        private const string LaunchTowerJobExtraVarsArgName = "-extravars:";


        static void Main(string[] args)
        {
            Console.WriteLine("* * * * * * Adaptive Provider Utilities * * * * * * ");

            if (args.Length == 0)
            {
                DisplayHelpAndExit();
            }

            switch (args[0].ToLower())
            {
                case EncryptCmdName:
                    EncryptText(args);
                    break;

                case DecryptCmdName:
                    DecryptCypher(args);
                    break;

                case LaunchTowerJobCmdName:
                    LaunchTowerJob(args);
                    break;

                default:
                    DisplayHelpAndExit();
                    break;
            }
        }

        private static void LaunchTowerJob(string[] args)
        {
            Console.WriteLine(">>>>> Ansible Tower Job Template Tool <<<<<");
            int id = 0;

            if (args.Length < 3 ||
                !args[1].ToLower().StartsWith(LaunchTowerJobConnectionStringArgName) ||
                !args[2].ToLower().StartsWith(LaunchTowerJobTemplatedIdArgName))
            {
                Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} {LaunchTowerJobCmdName} {LaunchTowerJobConnectionStringArgName}<connection-string> {LaunchTowerJobTemplatedIdArgName}<job-template-id> [{LaunchTowerJobExtraVarsArgName}<extra-variables>]");
                Environment.Exit(0);
            }

            var cs = CleanArgument(LaunchTowerJobConnectionStringArgName, args[1]);
            var templateId = CleanArgument(LaunchTowerJobTemplatedIdArgName, args[2]);
            var extraVars = args.Length == 4 ? CleanArgument(LaunchTowerJobExtraVarsArgName, args[3]) : null;

            if (!int.TryParse(templateId, out id) || id < 0)
            {
                Console.WriteLine($"[!] Invalid template ID. The value must a postive integer greater than 0.");
            }

            try
            {
                Console.WriteLine($"> Starting Ansible Tower Job...");
                var service = new TowerService(cs);

                Console.WriteLine($"> Testing authentication...");
                var serviceMap = service.EnsureAuthenticated().Result;
                Console.WriteLine($">> authenticated to service. Ping to '{serviceMap.Ping}'.{Environment.NewLine}");

                if (extraVars == null)
                {
                    Console.WriteLine($"> Launching job template {id}...{Environment.NewLine}");
                }
                else
                {
                    Console.WriteLine($"> Launching job template {id} with extra varaibles:{Environment.NewLine}{extraVars}{Environment.NewLine}");
                }

                var job = service.LaunchJobTemplate(id, extraVars);
                Console.WriteLine($">> job '{job.JobUrl}' execution completed at '{job.Finished}'.{Environment.NewLine}");

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[!] Job execution failed:{Environment.NewLine}{ex.Message}");
                Environment.Exit(-1);
            }
        }

        private static void DecryptCypher(string[] args)
        {
            Console.WriteLine(">>>>> Secret Decryption Tool <<<<<");

            if (args.Length != 2 || !args[1].ToLower().StartsWith(CypherArgName))
            {
                Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} {DecryptCmdName} {CypherArgName}<cypher-text>");
                Environment.Exit(0);
            }

            Console.WriteLine("> If you continue, your secret will be dispayed in clear text.");
            Console.Write("> To continue anyway, type YES and hit ENTER: ");
            var agreement = Console.ReadLine();

            if (string.Compare(agreement, "yes", true) == 0)
            {
                try
                {
                    Console.WriteLine($"> Decrypted secret:{Environment.NewLine}{CleanArgument(CypherArgName, args[1]).Decrypt()}");
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("Invalid cypher format.");
                    Environment.Exit(-1);
                }

            }
        }

        private static string CleanArgument(string argumentName, string argument)
        {
            var input = (argument.Length > argumentName.Length ? argument : string.Empty).Substring(argumentName.Length);

            if ((input.StartsWith('"') && input.EndsWith('"')) ||
                (input.StartsWith('\'') && input.EndsWith('\'')))
                return input[1..^1];

            return input;
        }

        private static void EncryptText(string[] args)
        {
            Console.WriteLine(">>>>> Secret Encryption Tool <<<<<");
            Console.Write("> Enter your secret: ");
            string secret = "";
            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    secret += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && secret.Length > 0)
                    {
                        secret = secret[0..^1];
                        Console.Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine("");
                        break;
                    }
                }
            } while (true);

            Console.WriteLine("> Encrypted secret: ");
            Console.WriteLine(secret.Encrypt());
        }

        private static void DisplayHelpAndExit()
        {
            Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} --verb -arg1:input1 -arg2:input2");
            Environment.Exit(0);
        }
    }
}

using CommandLine;
using duc.Helpers;
using duc.Options;
using System;

namespace duc
{
    internal class Program
    {
        private static DynamicUpdateClient duc;
        private static ArgOptions Options { get; set; }
        private static string dnsIp;
        private static string currentIp;

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ArgOptions>(args)
                   .WithParsed<ArgOptions>(o =>
                   {
                       Options = o;
                       if (o.Verbose)
                       {
                           Console.WriteLine("Verbose output enabled.");
                       }

                       RunDuc();
                   });
        }

        private static void RunDuc()
        {
            try
            {
                //Ensure a default control panel url
                if (string.IsNullOrEmpty(Options.Url))
                {
                    Options.Url = "https://controlpanel.tech/";
                }

                //Get Current Ip
                currentIp = WebHelper.GetExternalIp(Options.Proxy);

                //Check Domain Ip by Pinging it
                dnsIp = WebHelper.GetDomainIp(Options.DomainName);

                //Compare Previous and Current Ip
                if ((currentIp != dnsIp) || Options.Force)
                {
                    //If no forced ip is supplied use the current ip
                    if (string.IsNullOrEmpty(Options.Ip))
                    {
                        Options.Ip = currentIp;
                    }

                    if (Options.Verbose && Options.Force == false)
                    {
                        Console.WriteLine($"Current IP: {currentIp} differs from Dns Ip: {dnsIp}");
                    }
                    else
                    {
                        Console.WriteLine($"Forced update of {Options.DomainName} requested with IP: {Options.Ip}");
                    }

                    duc = new DynamicUpdateClient(Options);

                    duc.UpdateIp();

                    Console.WriteLine($"{Options.DomainName} updated");
                }
                else
                {
                    if (Options.Verbose)
                    {
                        Console.WriteLine($"Current IP matches the dns resolved IP. No need for update");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
using CommandLine;

namespace duc.Options
{
    public class ArgOptions
    {
        #region Optional

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('d', "debug", Required = false, HelpText = "Set selenium driver to debug mode.")]
        public bool Debug { get; set; }

        [Option('r', "url", Required = false, HelpText = "The Dot Tech domain controlpanel URL")]
        public string Url { get; set; }

        [Option('x', "proxy", Required = false, HelpText = "Specify a proxy address")]
        public string Proxy { get; set; }

        [Option('a', "ipAddress", Required = false, HelpText = "The new IP for the DNS A Record")]
        public string Ip { get; set; }

        [Option('f', "force", Required = false, HelpText = "Forces an update by bypassing previous Ip check")]
        public bool Force { get; set; }

        [Option('i', "domainId", Required = false, HelpText = "The Dot Tech domain Id. Speeds up the process")]
        public string DomainId { get; set; }

        [Option('w', "windowMode", Required = false, HelpText = "Runs in Window Mode")]
        public bool WindowMode { get; set; }

        [Option('t', "timeout", Required = false, HelpText = "Set a default timeout for the web driver")]
        public int Timeout { get; set; }

        #endregion Optional

        #region Required

        [Option('u', "username", Required = true, SetName = "credentials", HelpText = "The Dot Tech domain controlpanel URL")]
        public string Username { get; set; }

        [Option('p', "password", Required = true, SetName = "credentials", HelpText = "The Dot Tech domain controlpanel URL")]
        public string Password { get; set; }

        [Option('n', "domainName", Required = true, SetName = "credentials", HelpText = "The Dot Tech domain Name")]
        public string DomainName { get; set; }

        #endregion Required
    }
}
# techduc #
## .tech domain dynamic update client ##

* Get yourself a .tech domain [here](https://get.tech/) 
* Administer the domain via its control panel [here](https://controlpanel.tech/)
* Setup your free DNS Service by adding one or many A Records *See A record management [here](https://controlpanel.tech/kb/servlet/KBServlet/faq471.html#ma)

## Windows Usage ##
* Download and install the Chrome Browser [here](https://www.google.com/chrome/)

* Download the Selenium Chrome driver [here](https://chromedriver.chromium.org/downloads)
 * Ensure the driver is for your version of Chrome.
* Extract driver to C:\Temp\chromedriver\chromedriver.exe

## Ubuntu Linux Usage ##
* Download the latest Google Chrome .deb package with wget:
``` 
wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.
```
* Install the *.deb package
```
sudo dpkg -i google-chrome-stable_current_amd64.deb
```
 * This will install Chrome to the /opt/google/chrome directory

* Download the Linux Selenium Chrome driver [here](https://chromedriver.chromium.org/downloads)
 * Ensure the driver is for your version of Chrome.
* Extract the chromedriver to /opt/selenium/
* Grant the driver rights to execute
```
sudo chmod +x /opt/selenium/chromedriver
```
* Copy the Published duc commandline app to either /opt/dottech/ or /usr/bin
* Grant it rights to execute
```
sudo chmod +x /usr/bin/duc
sudo chmod +x /opt/dottech/duc
```

* Install the Dotnet Core 3.1 Framework [Guide here](https://docs.microsoft.com/en-us/dotnet/core/install/linux-package-manager-ubuntu-1904)

```bash
wget -q https://packages.microsoft.com/config/ubuntu/19.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
```

```bash
sudo apt-get update
sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-sdk-3.1
```

## Usage ##

duc --help

-v, --verbose       Set output to verbose messages.

-d, --debug         Set selenium driver to debug mode.

-r, --url           The Dot Tech domain controlpanel URL

-x, --proxy         Specify a proxy address

-a, --ipAddress     The new IP for the DNS A Record

-f, --force         Forces an update by bypassing previous Ip check

-i, --domainId      The Dot Tech domain Id

-w, --windowMode    Runs in Window Mode

-t, --timeout       Set a default timeout for the web driver

-u, --username      Required. The Dot Tech domain controlpanel URL

-p, --password      Required. The Dot Tech domain controlpanel URL

-n, --domainName    Required. The Dot Tech domain Name

--help              Display this help screen.

--version           Display version information.

## Usage Example ##

### Ubuntu ###

```bash
duc -u user@domain.com -p 'P@$$w0rd!' -n domain.tech -f -v
```

### Windows ###

```cmd
duc.exe -u user@domain.com -p 'P@$$w0rd!' -n domain.tech -f -v
```

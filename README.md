# CPanel - Mqtt Devices Control Panel

>cd Cpanel/ClientApp<br>
>npm i

### Connect database<br>
CPanel/USQLCSharp/DataAccess/PeopleContext.cs <br>
and <br>
CPanel/CPanel/appsettings.json<br>

### package manager<br>
>update-database

cd Cpanel
>dotnet run


Install Mqtt Broker
Windows
mosquitto-2.0.9-install-windows-x64.exe (64-bit build, Windows Vista and up, built with Visual Studio Community 2019)
mosquitto-2.0.9-install-windows-x32.exe (32-bit build, Windows Vista and up, built with Visual Studio Community 2019)
Older installers can be found at https://mosquitto.org/files/binary/.

See also README-windows.md after installing.

Mac
Mosquitto can be installed from the homebrew project. See brew.sh and then use brew install mosquitto

Linux distributions with snap support
snap install mosquitto
Debian
Mosquitto is now in Debian proper. There will be a short delay between a new release and it appearing in Debian as part of the normal Debian procedures.
There are also Debian repositories provided by the mosquitto project, as described at https://mosquitto.org/2013/01/mosquitto-debian-repository
Raspberry Pi
Mosquitto is available through the main repository.

There are also Debian repositories provided by the mosquitto project, as described at https://mosquitto.org/2013/01/mosquitto-debian-repository/

Ubuntu
Mosquitto is available in the Ubuntu repositories so you can install as with any other package. If you are on an earlier version of Ubuntu or want a more recent version of mosquitto, add the mosquitto-dev PPA to your repositories list - see the link for details. mosquitto can then be installed from your package manager.

sudo apt-add-repository ppa:mosquitto-dev/mosquitto-ppa

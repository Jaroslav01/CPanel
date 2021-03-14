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

## Install Mqtt Broker
### Windows<br>
https://mosquitto.org/files/binary/win64/mosquitto-2.0.9-install-windows-x64.exe (64-bit)<br>
https://mosquitto.org/files/binary/win32/mosquitto-2.0.9-install-windows-x86.exe (32-bit)<br>

## Mac
Mosquitto can be installed from the homebrew project. See brew.sh and then use brew install mosquitto<br>

## Linux distributions with snap support
>snap install mosquitto
## Debian
https://mosquitto.org/2013/01/mosquitto-debian-repository<br>
## Raspberry Pi
Mosquitto is available through the main repository.<br>

There are also Debian repositories provided by the mosquitto project, as described at https://mosquitto.org/2013/01/mosquitto-debian-repository/<br>

## Ubuntu
Mosquitto is available in the Ubuntu repositories so you can install as with any other package.<br>
 If you are on an earlier version of Ubuntu or want a more recent version of mosquitto, <br>
add the mosquitto-dev PPA to your repositories list - see the link for details.<br>
 mosquitto can then be installed from your package manager.<br>

>sudo apt-add-repository ppa:mosquitto-dev/mosquitto-ppa

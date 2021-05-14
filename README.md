# sony-rcp-server
SONY virtual RCP server


[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-brightgreen.svg)](COPYING)
[![Build Status](https://dev.azure.com/oleksandr-nazaruk/sony-rcp-server/_apis/build/status/freehand-dev.sony-rcp-server?branchName=master)](https://dev.azure.com/oleksandr-nazaruk/sony-rcp-server/_build/latest?definitionId=9&branchName=master)


## Compile and install
Once you have installed all the dependencies, get the code:

	git clone https://github.com/freehand-dev/openprocurement-agent.git
	cd sony-rcp-server

Then just use:

	sudo mkdir /opt/sony-rcp-server/bin
	dotnet restore
	dotnet build
	sudo dotnet publish --runtime linux-x64 --output /opt/sony-rcp-server/bin -p:PublishSingleFile=true -p:PublishTrimmed=true ./sony-rcp-server

Install as daemon
   
	sudo nano /etc/systemd/system/sony-rcp-server.service

The content of the file will be the following one

	[Unit]
	Description=OpenProcurement Agent 

	[Service]
	Type=notify
	WorkingDirectory=/opt/sony-rcp-server/etc/sony-rcp-server
	Restart=always
	RestartSec=10
	KillSignal=SIGINT
	ExecStart=/opt/sony-rcp-server/bin/sony-rcp-server
	Environment=ASPNETCORE_ENVIRONMENT=Production 

	[Install]
	WantedBy=multi-user.target

Add daemon to startup

	sudo systemctl daemon-reload
	sudo systemctl start sony-rcp-server
	sudo systemctl status sony-rcp-server
	sudo systemctl enable sony-rcp-server


## Configure and start
To start the server, you can use the `sony-rcp-server` executable as the application or `sudo systemctl start sony-rcp-server` as a daemon. For configuration you can edit a configuration file:

	sudo nano /opt/sony-rcp-server/etc/sony-rcp-serversony-rcp-server.conf

The content of the file will be the following one

    [MCU]
    IPAddress=192.168.1.100

    #
    [RCP]
    RcpId=30

    #
    [Logging:LogLevel]
    Default=Debug
    Microsoft=Warning
    Microsoft.Hosting.Lifetime=Information

    #
    [Kestrel:Endpoints]
    Http:Url=http://*:6660
    Https:Url=https://*:6661
    Https:Certificate:Path=/opt/sony-rcp-server/etc/sony-rcp-server/cert.pfx
    Https:Certificate:Password=certpassword

## Docker

	$ docker pull oleksandrnazaruk/sony-rcp-server:latest
	
	$ docker volume create sony-rcp-server_data
	$ docker run --detach --name sony-rcp-server --restart=always -v sony-rcp-server_data:/opt/sony-rcp-server/etc/sony-rcp-server -p 6660:6660 -p 6661:6661  oleksandrnazaruk/sony-rcp-server:latest

	# Example run on Raspberry Pi 4 (Ubuntu 64bit)
	$ docker run --detach --name sony-rcp-server --restart=always -v sony-rcp-server_data:/opt/sony-rcp-server/etc/sony-rcp-server -p 6660:6660 -p 6661:6661 --env MCU__IPAddress=192.168.1.200 --env RCP__RcpId=31  oleksandrnazaruk/sony-rcp-server:arm64v8

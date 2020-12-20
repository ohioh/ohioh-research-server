<div align="center">
<pre>
  ___  _   _ ___ ___  _   _ 
 / _ \| | | |_ _/ _ \| | | |
| | | | |_| || | | | | |_| |
| |_| |  _  || | |_| |  _  |
 \___/|_| |_|___\___/|_| |_|
R   E   S   E   A   R   C   H
</pre>
<hr>
</div>

This repository holds the source code for the **OHIOH Research Server** software (ORS).
The ORS receives and decodes Bluetooth-Low-Energy (BLE) advertised-devices data which are gathered and sent by multiple ESP32 MCUs (the micro-controller code can be found [here](https://github.com/ohioh/ohioh-research-esp32)).\
For an in-depth explanation of the network- & packet protocol used by ORS, see the [network protocol documentation]().

## Installation
To get the ORS up & running, either clone this repository and [build from source](#building-from-source) or grab the latest [pre-built binary](https://github.com/ohioh/ohioh-research-esp32/releases/latest) for the operating system of choice.\
Example:
```bash
ors -p 15891 --password "Research" -d "root@localhost" -t "ors"
```

## Configuration
In order to configure the server, ORS can be launched with the following flags:
|Parameter    |Default value|Description|
|:--          |:--          |:--        | 
|`-p | --port`|`15891`|Launches ORS with the port specified after the flag.|
|`--password` ||Enforces the connecting clients to provide the password specified by this parameter (maximal length of a password is 128 bytes)|
|`-d | --database`|`root@localhost`|Specifies the MySQL database to connect to.|
|`-t | --table`|`ors`|The table to use.|

## Building from source
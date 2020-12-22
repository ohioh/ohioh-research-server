# Network protocol
The following documentation describes the network packet protocol used in the ORS. As a transport layer, solely TCP/IP is used.

## Packet identifiers
The first byte of every packet holds a packet indentifier which tells the server (or client) how to interpret the following payload.
|ID    |Sent by         |Message|
|:--   |:--             |:--    |
|`0x1` |Client          |[Client connect](#client-connect)|
|`0x2` |Client & Server |[Client disconnect](#client-disconnect)|
|`0xA` |Client          |[Status report](#status-report)|
|`0xB` |Client          |[Scan report](#scan-report)|
|`0x10`|Server          |[Set op-mode](#set-operation-mode)|

## Packets
The following section explains how to decode the packet's payload *after* the first packet identifier byte.
### Client connect
|Datatype|Content|
|:--     |:--    |
|Int32   |Network protocol version|
|Int16   |Client's MTU (default: 2048 bytes)|
|Byte    |Last bit is set if a password is required to connect and if so, the first seven bytes indicate the length `n` of the following password in bytes|
|`n` bytes|Password|

### Client disconnect

### Status report
|Datatype |Content|
|:--      |:--    |
|Byte     |Op-mode|

### Scan report
|Datatype |Content|
|:--      |:--    |
|Byte     |Amount of devices found|
|Int32    |Length `n` of following payload data|
|`n` bytes|Raw BLE advertised-device payload data|
*(if more than one device has been found, repeat reading at the Int32)*

### Set operation-mode
|Datatype |Content|
|:--      |:--    |
|Int32    |Target device IP|
|Byte     |Operation mode (`0`: SEND, `1`: RECEIVE)|
**Note:** Upon receiving a *set op-mode* packet, a device will automatically return a *scan report* packet to confirm the changes.

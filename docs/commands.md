# Commands
The ORS and its connected clients can be controlled via commands through the prompt. The following provides an overview of available commands and their respective actions.

- [Commands](#commands)
  - [get](#get)
    - [get status](#get-status)
  - [set](#set)
    - [set opmode](#set-opmode)

## get
### get status
**Description:**\
Get the status of one or more (or all) connected client(s).

**Syntax:**
```
get status <!client-ip/*>
```
**Example:**
```
# Get status of specific client
get status 192.168.0.110

# Get status of all connected clients
get status *
```

## set
### set opmode
**Description:**\
Set the operating mode of one or more connected clients. Valid operating mode values are `0` for `SEND` or `1` for `RECEIVE`.

**Syntax:**
```
set opmode <!mode> <!client-ip1> <?client-ip2>
```
**Example:**
```
# Set operation mode of specific clients to RECEIVE
set opmode 1 192.168.0.110 102.168.0.111 192.168.0.112 
```
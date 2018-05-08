# SteamGameNetworkingSocketsWrapper
## What it is
A C# Wrapper of the SteamGameNetworkingSockets(https://github.com/ValveSoftware/GameNetworkingSockets),now only support on MacOS. However I have done some changes in the SteamGameNetworkingSockets lib so that I can use the wrapper more easily and conveniently.
I have forked the lib from the source repo of ValveSoftware, so you can track my SteamGameNetworkingSockets lib in https://github.com/Salty-Sailor/GameNetworkingSockets

## How it works
The wrapper has two parts:
One is the dll which is written in C++, it's a transport level which has TCP-like API but use UDP protocol to send/recv data(See https://github.com/Salty-Sailor/GameNetworkingSockets/blob/master/README.md)
The other is a thin shell written in C# so that the developers could use it in Unity or .Net Core Project.

## A tiny sample
There is a very tiny send/recv sample under SteamWrapper/Test dir.

## PS:STILL IN PROGRESS

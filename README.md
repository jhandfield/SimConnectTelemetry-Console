# SimConnectTelemetry-Console
Simple console app proof-of-concept of reading aircraft telemetry data out of MSFS 2020 via SimConnect. Makes use of the [CTrue.FsConnect library](https://github.com/c-true/FsConnect) to interface with SimConnect, connects to your local MSFS instance and every second pulls data on the active flight's location, altitude, heading, indicated, and ground speeds.

## Third-Party Libraries
This project requires the Microsoft.FlightSimulator.SimConnect.dll and SimConnect.dll libraries to compile and function - obtain these files from the MSFS SDK and place in the "Third-Party" folder before compiling.

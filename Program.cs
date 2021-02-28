using System;
using Microsoft.FlightSimulator.SimConnect;
using CTrue.FsConnect;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;

namespace SimConnectTelemetry
{
    class Program
    {
        static FsConnect fs;
        static bool run;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct PlaneInfoResponse
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public String Title;
            public double Latitude;
            public double Longitude;
            public double Altitude;
            public double Heading;
            public double IASKnots;
            public double GSKnots;
        }

        public enum Requests
        {
            PlaneInfo = 0
        }

        static void Main(string[] args)
        {
            run = true;

            fs = new FsConnect();
            fs.Connect("FlightEconOnline");
            fs.ConnectionChanged += Fs_ConnectionChanged;
            fs.FsDataReceived += Fs_FsDataReceived;

            List<SimProperty> definition = new List<SimProperty>();

            // Consult the SDK for valid sim variable names, units and whether they can be written to.
            definition.Add(new SimProperty("Title", null, SIMCONNECT_DATATYPE.STRING256));
            definition.Add(new SimProperty("Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64));
            definition.Add(new SimProperty("Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64));

            // Can also use predefined enums for sim variables and units (incomplete)
            definition.Add(new SimProperty(FsSimVar.PlaneAltitude, FsUnit.Feet, SIMCONNECT_DATATYPE.FLOAT64));
            definition.Add(new SimProperty(FsSimVar.PlaneHeadingDegreesTrue, FsUnit.Degrees, SIMCONNECT_DATATYPE.FLOAT64));
            definition.Add(new SimProperty(FsSimVar.AirspeedIndicated, FsUnit.Knots, SIMCONNECT_DATATYPE.FLOAT64));
            definition.Add(new SimProperty(FsSimVar.GpsGroundSpeed, FsUnit.Knots, SIMCONNECT_DATATYPE.FLOAT64));

            fs.RegisterDataDefinition<PlaneInfoResponse>(Requests.PlaneInfo, definition);
            

            while (run) {
                // Request data
                fs.RequestData(Requests.PlaneInfo);

                // Sleep for a sec
                Thread.Sleep(1000);
            }
        }

        private static void Fs_FsDataReceived(object sender, FsDataReceivedEventArgs e)
        {
            if (e.RequestId == (uint)Requests.PlaneInfo)
            {
                PlaneInfoResponse r = (PlaneInfoResponse)e.Data;
                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}\t{r.Latitude:F4},{r.Longitude:F4}\t{r.Altitude:F1}ft\t{r.Heading:F1}deg\t\t{r.IASKnots:F0}kt\t\t{r.GSKnots:F0}kt");
            }
        }

        private static void Fs_ConnectionChanged(object sender, EventArgs e)
        {
            string state = fs.Connected ? "Connected" : "Disconnected";
            Console.WriteLine($"Connection state changed - {state}\n");
            Console.WriteLine("Timestamp\tLat/Lon\t\t\tAltitude\tHeading\t\tIndicatedSpd\tGroundSpd");
        }
    }
}

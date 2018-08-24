using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineCommunications {
    public static class TinyGSettings {
        public static double CNC_SmallMovementSpeed { get; set; }
        public static string TinyG_sys { get; internal set; }
        public static string TinyG_x { get; internal set; }
        public static string TinyG_y { get; internal set; }
        public static string TinyG_z { get; internal set; }
        public static string TinyG_m1 { get; internal set; }
        public static string TinyG_m2 { get; internal set; }
        public static string TinyG_m3 { get; internal set; }
    }
}

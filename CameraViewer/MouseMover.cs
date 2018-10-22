using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;

namespace CameraViewer {
    class MouseMover {
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        /// <summary>
        /// Clicks the mouse.
        /// </summary>
        public static void Click() {
            mouse_event(0x02, 0, 0, 0, 0);
            mouse_event(0x04, 0, 0, 0, 0);
        }

        /// <summary>
        /// Moves the mouse.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void ExecuteMoveAbsolute(Point step) {
            currentPosition = step;
            var inputXinPixels = step.X;
            var inputYinPixels = step.Y;
            var screenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var outputX = inputXinPixels * 65535 / 1200;
            var outputY = inputYinPixels * 65535 / 600;
            mouse_event(0x8000 | 0x0001, outputX, outputY, 0, 0);
        }

        static Point currentPosition = new Point(0, 0);
        internal static void ExecuteMoveTarget(Point targetLocation) {
            targetLocation.X = targetLocation.X - 25;           

            int xMove = 0;
            int yMove = 0;

            if (currentPosition.X > targetLocation.X) {
                xMove = -10;
            }
            else if (currentPosition.X < targetLocation.X) {
                xMove = 10;
            }

            if (currentPosition.Y > targetLocation.Y) {
                yMove = -10;
            }
            else if (currentPosition.Y < targetLocation.Y) {
                yMove = 10;
            }

            ExecuteMoveAbsolute(new Point(currentPosition.X + xMove, currentPosition.Y + yMove));
        }
    }
}

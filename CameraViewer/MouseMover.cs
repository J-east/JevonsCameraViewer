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

        /// <summary>
        /// Clicks the mouse.
        /// </summary>
        public static void Click() {
            mouse_event(0x02, 0, 0, 0, 0);
            mouse_event(0x04, 0, 0, 0, 0);
        }

        /// <summary>
        /// Executes the mouse moves.
        /// </summary>
        public static void ExecuteMove(Point step, bool async = false) {
            if (!async) {
                mouse_event(0x1, step.X, step.Y, 0, 0);
            }
            else {
                // Move X.
                new Thread(() => {
                    mouse_event(0x1, step.X, 0, 0, 0);
                }).Start();

                // Move Y.
                new Thread(() => {
                    mouse_event(0x1, 0, step.Y, 0, 0);
                }).Start();
            }
        }

        /// <summary>
        /// Moves the mouse.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void ExecuteMove(int x, int y) {
            var inputXinPixels = 200;
            var inputYinPixels = 200;
            var screenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var outputX = inputXinPixels * 65535 / screenBounds.Width;
            var outputY = inputYinPixels * 65535 / screenBounds.Height;
            mouse_event(0x8000 | 0x0001, outputX, outputY, 0, 0);
        }
    }
}

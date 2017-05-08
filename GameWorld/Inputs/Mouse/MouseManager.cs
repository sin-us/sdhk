using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Reflection;

namespace MonoGameWorld.Inputs.Mouse
{
    public static class MouseManager
    {
        private static Cursor customCursor;

        public static MouseStatus MouseStatus { get; }
        public static Cursor CustomCursor => customCursor;
        public static bool IsPointerVisible { get; set; }
        
        static MouseManager()
        {
            MouseStatus = new MouseStatus();
            customCursor = null;
            IsPointerVisible = false;
        }

        public static void Update()
        {
            MouseStatus.FromXnaMouseState(Microsoft.Xna.Framework.Input.Mouse.GetState());            
        }

        #region Custom cursor handling
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadCursorFromFile(string path);

        public static void LoadCustomCursor(string path)
        {
            IntPtr hCursor = LoadCursorFromFile(path);

            if (hCursor == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            var cursor = new Cursor(hCursor);
            // Note: force the cursor to own the handle so it gets released properly
            var fi = typeof(Cursor).GetField("ownHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            fi.SetValue(cursor, true);
            customCursor = cursor;
        }
        #endregion
    }
}
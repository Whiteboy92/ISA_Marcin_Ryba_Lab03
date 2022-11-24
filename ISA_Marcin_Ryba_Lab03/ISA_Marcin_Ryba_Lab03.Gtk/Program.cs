using System;
using Eto.Forms;

namespace ISA_Marcin_Ryba_Lab03.Gtk
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Application(Eto.Platforms.Gtk).Run(new MainForm());
        }
    }
}
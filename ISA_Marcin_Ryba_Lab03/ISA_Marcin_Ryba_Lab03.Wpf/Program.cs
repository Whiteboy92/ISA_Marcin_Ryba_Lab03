using System;
using Eto;
using Eto.Forms;

namespace ISA_Marcin_Ryba_Lab03.Wpf
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            StaticValues.Platform = Platforms.Wpf;
            new Application(StaticValues.Platform).Run(new MainForm());
        }
    }
}
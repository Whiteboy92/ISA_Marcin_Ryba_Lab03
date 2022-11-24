using System;
using Eto.Forms;

namespace ISA_Marcin_Ryba_Lab03.Mac
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Application(Eto.Platforms.Mac64).Run(new MainForm());
        }
    }
}
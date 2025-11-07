using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fluxguard
{
    internal static class Program
    {
        private static Mutex singleInstanceMutex;

        [STAThread]
        static void Main()
        {
            bool createdNew;
            // Use a named mutex to allow single instance per machine
            singleInstanceMutex = new Mutex(true, "Global\\FluxguardSingletonMutex_v1", out createdNew);
            if (!createdNew)
            {
                // Another instance is running - exit silently
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new Main());
            }
            finally
            {
                try { singleInstanceMutex.ReleaseMutex(); } catch { }
                try { singleInstanceMutex.Dispose(); } catch { }
            }
        }
    }
}

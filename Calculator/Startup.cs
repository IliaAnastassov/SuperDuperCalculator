namespace Calculator
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Forms;

    static class Startup
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CalculatorMainForm());
        }
    }
}

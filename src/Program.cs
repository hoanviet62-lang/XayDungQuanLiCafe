using System;
using System.Windows.Forms;
using QuanLyQuanAn.Forms;

namespace QuanLyQuanAn
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using var login = new FrmDangNhap();
            if (login.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new FrmMain());
            }
        }
    }
}
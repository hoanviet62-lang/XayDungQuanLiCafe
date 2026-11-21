using System;
using System.IO;
using Microsoft.Data.SqlClient;

namespace QuanLyQuanAn.DAL
{
    public static class DatabaseHelper
    {
        private static string? _connectionString;

        public static string ConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(_connectionString))
                    return _connectionString;

                // 1. Ưu tiên đọc từ file cấu hình cục bộ dbserver.txt (nếu máy thành viên có tên instance lạ)
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dbserver.txt");
                if (File.Exists(configPath))
                {
                    string customServer = File.ReadAllText(configPath).Trim();
                    if (!string.IsNullOrEmpty(customServer))
                    {
                        _connectionString = $"Server={customServer};Database=QuanLyQuanAnDB;Trusted_Connection=True;TrustServerCertificate=True;";
                        return _connectionString;
                    }
                }

                // 2. Tự động dò các SQL Server Instance phổ biến trên máy từng thành viên
                string[] servers =
                {
                    ".",
                    @".\SQLEXPRESS",
                    @".\MSSQLSERVER01",
                    @".\MSSQLSERVER02",
                    @"(localdb)\MSSQLLocalDB",
                    "localhost"
                };

                foreach (var srv in servers)
                {
                    string cs = $"Server={srv};Database=QuanLyQuanAnDB;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=1;";
                    try
                    {
                        using var conn = new SqlConnection(cs);
                        conn.Open();
                        _connectionString = $"Server={srv};Database=QuanLyQuanAnDB;Trusted_Connection=True;TrustServerCertificate=True;";
                        return _connectionString;
                    }
                    catch { }
                }

                throw new Exception("Không tìm thấy SQL Server chứa CSDL QuanLyQuanAnDB! Hãy chạy ScriptDB.sql trước.");
            }
        }

        public static SqlConnection GetConnection() => new SqlConnection(ConnectionString);
    }
}
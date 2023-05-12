using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ChatRWKV_PC.Utils
{
    public class PipUtils
    {
        public static string CreateType { get; set; } = "/c ";

        public static readonly string PyPath =  "set PATH=" + AppDomain.CurrentDomain.BaseDirectory + "Python3.10;" + AppDomain.CurrentDomain.BaseDirectory + "Python3.10\\Scripts;%PATH%; && ";

        static readonly string PyInstallStr = PyPath + "python.exe -m pip install ";

        static readonly string PyUpInstallStr = PyPath + "python.exe -m pip install --upgrade ";

        static readonly string PyShowStr = PyPath + "python.exe -m pip show ";

        static readonly string PyListStr = PyPath + "python.exe -m pip list ";

        static readonly string PyVersionsStr = PyPath + "python.exe -m pip index versions ";

        static readonly string PySwithSourceStr = PyPath + "python.exe -m pip config set global.index-url ";


        static PipUtils() 
        {
            Debug.WriteLine(PyPath);
        }

        public static void SwithSource(string url)
        {
            Process? process = new Process();
            process.StartInfo.Arguments = CreateType + PySwithSourceStr + url;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.Start();
            try
            {
                process.WaitForExit();
                process.Kill();
                process.CloseMainWindow();
                process.Close();
                process.Dispose();
                process = null;
            }
            catch
            {
                //强制释放
                process.Close();
                process.Dispose();
                process = null;
            }
        }

        /// <summary>
        /// 安装包
        /// </summary>
        /// <param name="packagname">可为多个包或单个包</param>
        public static void Installer(string packagname)
        {
            Process? process = new Process();
            process.StartInfo.Arguments = CreateType + PyInstallStr + packagname;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.Start();
            try
            {
                process.WaitForExit();
                process.Kill();
                process.CloseMainWindow();
                process.Close();
                process.Dispose();
                process = null;
            }
            catch
            {
                //强制释放
                process.Close();
                process.Dispose();
                process = null;
            }
            
        }

        public static void Upgrade(string packagname)
        {
            Process? process = new Process();
            process.StartInfo.Arguments = CreateType + PyUpInstallStr + packagname;

            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.Start();
            try
            {
                process.WaitForExit();
                process.Kill();
                process.CloseMainWindow();
                process.Close();
                process.Dispose();
                process = null;
            }
            catch
            {
                //强制释放
                process.Close();
                process.Dispose();
                process = null;
            }
        }

        public static string GetLocationVersion(string package,string url = "")
        {
            //当前版本
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            if (string.IsNullOrEmpty(url))
            {
                process.StartInfo.Arguments = CreateType + PyShowStr + package;
            }
            else
            {
                process.StartInfo.Arguments = CreateType + PyShowStr + package + url;
            }
            
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            while (!process.StandardOutput.EndOfStream)
            {
                string output = process.StandardOutput.ReadLine();
                if (output != null)
                {

                    if (output.IndexOf("Version") != -1)
                    {
                        process.CloseMainWindow();
                        process.Kill();
                        process.Close();
                        process.Dispose();
                        return output.Substring(output.IndexOf(":") + 1).Trim();

                    }
                }
            }
            process.CloseMainWindow();
            process.Kill();
            process.Close();
            process.Dispose();
            return "未安装";
        }

        public static string GetLocationVersionList(string package)
        {
            //当前版本
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = CreateType + PyListStr;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            while (!process.StandardOutput.EndOfStream)
            {
                string output = process.StandardOutput.ReadLine();
                if (output != null)
                {

                    if (output.IndexOf(package) != -1)
                    {
                        process.CloseMainWindow();
                        process.Kill();
                        process.Close();
                        process.Dispose();
                        return output.Replace(" ","").Replace(package,"").Trim();

                    }
                }
            }
            process.CloseMainWindow();
            process.Kill();
            process.Close();
            process.Dispose();
            return "未安装";
        }

        public static string GetAllVersion(string package, string url = "")
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            if (string.IsNullOrEmpty(url))
            {
                process.StartInfo.Arguments = CreateType + PyVersionsStr + package;
            }
            else
            {
                process.StartInfo.Arguments = CreateType + PyVersionsStr + package + url;
            }

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            while (!process.StandardOutput.EndOfStream)
            {
                string output = process.StandardOutput.ReadLine();
                if (output != null)
                {
                    if(output.IndexOf("Available versions:") != -1)
                    {
                        process.CloseMainWindow();
                        process.Kill();
                        process.Close();
                        process.Dispose();
                        return output.Replace("Available versions:","可更新版本：");
                    }
                }
            }
            process.CloseMainWindow();
            process.Kill();
            process.Close();
            process.Dispose();
            return "获取失败";
        }

        public static string GetLastVersion(string package, string url = "")
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            if (string.IsNullOrEmpty(url))
            {
                process.StartInfo.Arguments = CreateType + PyVersionsStr + package;
            }
            else
            {
                process.StartInfo.Arguments = CreateType + PyVersionsStr + package + url;
            }
            
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            while (!process.StandardOutput.EndOfStream)
            {
                string output = process.StandardOutput.ReadLine();
                if (output != null)
                {
                    int startIndex = output.IndexOf('(');
                    int endIndex = output.IndexOf(')');
                    if (startIndex >= 0 && endIndex > startIndex)
                    {
                        string result = output.Substring(startIndex + 1, endIndex - startIndex - 1);
                        process.CloseMainWindow();
                        process.Kill();
                        process.Close();
                        process.Dispose();
                        return result;
                    }
                }
            }
            process.CloseMainWindow();
            process.Kill();
            process.Close();
            process.Dispose();
            return "获取失败";
        }
    }
}

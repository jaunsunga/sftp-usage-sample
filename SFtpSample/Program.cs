using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFtpSample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("invalid parameter: 올바른 예시=> sftp get 로컬경로 파일명 또는 sftp get 로컬경로");
                    return;
                }
                string methods = string.Empty;
                string filename = string.Empty;
                string localdir = string.Empty;

                if (args.Length >= 3)
                {
                    methods = args[0];
                    localdir = args[1];
                    filename = args[2];

                    if (string.IsNullOrWhiteSpace(filename))
                    {
                        Console.WriteLine("invalid filename");
                        return;
                    }

                    switch (methods)
                    {
                        case "get":
                            GoGetFile(filename, localdir); break;
                        case "put":
                            GoPutFile(filename, localdir); break;
                        default:
                            Console.WriteLine("first argument should be get or put....");
                            break;
                    }
                }

                if (args.Length == 2)
                {
                    methods = args[0];
                    localdir = args[1];

                    switch (methods)
                    {
                        case "get":
                            GoGetFiles(localdir); break;
                        case "put":
                            Console.WriteLine("put does not support multi put...."); break;
                        default:
                            Console.WriteLine("first argument should be get or put....");
                            break;
                    }
                }
            }
            catch (Exception er)
            {
                Console.WriteLine("An unhandled exception has been caught " + er.ToString());
            }
            
                
        }

        static void GoGetFile(string filename, string localdir)
        {
            var craft = new Craft();
            craft.DownloadFile(filename, localdir);
        }

        static void GoGetFiles(string localdir)
        {
            var craft = new Craft();
            craft.DownloadFiles(localdir);
        }

        static void GoPutFile(string filename, string localdir)
        {
            var craft = new Craft();
            craft.UploadFile(filename, localdir);
        }

        static void GoPutFiles(string localdir)
        {
            var craft = new Craft();
            craft.UploadFiles(localdir);
        }






    }
}

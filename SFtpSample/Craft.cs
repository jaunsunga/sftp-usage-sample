using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFtpSample
{
    public class Craft
    {
        private string pathLocalGet = @"D:\INTERFACE\크래프트\GET";
        private string pathLocalPut = @"D:\INTERFACE\크래프트\PUT";

        public Craft()
        {
        }

        /// <summary>
        /// 파일명 없을 경우 약속된 파일목록 download
        /// </summary>
        public void DownloadFiles(string localdir)
        {
            string host = @"13.209.113.71";
            string username = "ubuntu";

            pathLocalGet = localdir;
            if (!Directory.Exists(pathLocalGet)) Directory.CreateDirectory(pathLocalGet);
            
            string[] remoteFiles = new string[] {
                "bnk_mp_fund.dat"
                , "bnk_mp_return.dat"
                , "bnk_bench_return.dat"
                , "bnk_index_return.dat"
                , "bnk_index_cov.dat"
            };

            
            string pathRemote = "/ftp/bnk_b/downloads/";
            string pathRemoteSendDone = "/ftp/bnk_b/downloads/send_done/";

            string[] pathRemoteFiles = remoteFiles.Select(f => Path.Combine(pathRemote, f)).ToArray();
            string[] pathRemoteSendDoneFiles = remoteFiles.Select(f => Path.Combine(pathRemoteSendDone, f)).ToArray();
            string[] pathLocalFiles = remoteFiles.Select(f => Path.Combine(pathLocalGet, f)).ToArray();


            

            PrivateKeyFile keyFile = new PrivateKeyFile(@"./bnk_sftp.pem");
            var keyFiles = new[] { keyFile };

            var methods = new List<AuthenticationMethod>();
            //methods.Add(new PasswordAuthenticationMethod(username, password));
            methods.Add(new PrivateKeyAuthenticationMethod(username, keyFiles));

            ConnectionInfo con = new ConnectionInfo(host, 10021, username, methods.ToArray());

            using (SftpClient sftp = new SftpClient(con))
            {
                try
                {
                    sftp.Connect();

                    var index = 0;
                    foreach (var pathLocalFile in pathLocalFiles)
                    {
                        if (sftp.Exists(pathRemoteFiles[index]))
                        {
                            File.Delete(pathLocalFile);

                            using (Stream fileStream = File.OpenWrite(pathLocalFile))
                            {

                                Console.WriteLine("Downloading {0}", pathRemoteFiles[index]);

                                sftp.DownloadFile(pathRemoteFiles[index], fileStream);
                                sftp.RenameFile(pathRemoteFiles[index], BuildRemoteSendDoneFileName(pathRemoteSendDoneFiles[index]));
                            }
                        }
                        else
                        {
                            Console.WriteLine("File not found skip {0}", pathRemoteFiles[index]);
                        }

                        index++;
                    }

                    sftp.Disconnect();
                }
                catch (Exception er)
                {
                    Console.WriteLine("An exception has been caught " + er.ToString());
                }
            }
        }

        /// <summary>
        /// 파일명 지정해서 download
        /// </summary>
        /// <param name="remoteFileName"></param>
        public void DownloadFile(string remoteFileName, string localdir)
        {
            string host = @"13.209.113.71";
            string username = "ubuntu";

            pathLocalGet = localdir;
            if (!Directory.Exists(pathLocalGet)) Directory.CreateDirectory(pathLocalGet);            
            

            string pathRemote = "/ftp/bnk_b/downloads/";
            string pathRemoteSendDone = "/ftp/bnk_b/downloads/send_done/";

            string pathRemoteFile = Path.Combine(pathRemote, remoteFileName);
            string pathRemoteSendDoneFile = Path.Combine(pathRemoteSendDone, remoteFileName);
            string pathLocalFile = Path.Combine(pathLocalGet, remoteFileName);


            PrivateKeyFile keyFile = new PrivateKeyFile(@"./bnk_sftp.pem");
            var keyFiles = new[] { keyFile };

            var methods = new List<AuthenticationMethod>();
            methods.Add(new PrivateKeyAuthenticationMethod(username, keyFiles));

            ConnectionInfo con = new ConnectionInfo(host, 10021, username, methods.ToArray());

            using (SftpClient sftp = new SftpClient(con))
            {
                try
                {
                    sftp.Connect();

                    if (sftp.Exists(pathRemoteFile))
                    {
                        File.Delete(pathLocalFile);

                        using (Stream fileStream = File.OpenWrite(pathLocalFile))
                        {
                            Console.WriteLine("Downloading {0}", pathRemoteFile);

                            sftp.DownloadFile(pathRemoteFile, fileStream);
                            sftp.RenameFile(pathRemoteFile, BuildRemoteSendDoneFileName(pathRemoteSendDoneFile));
                        }
                    }
                    else
                    {
                        Console.WriteLine("File not found skip {0}", pathRemoteFile);
                    }

                    sftp.Disconnect();
                }
                catch (Exception er)
                {
                    Console.WriteLine("An exception has been caught " + er.ToString());
                }
            }
        }

        /// <summary>
        /// 파일명 지정해서 upolad
        /// </summary>
        /// <param name="remoteFileName"></param>
        public void UploadFile(string remoteFileName, string localdir)
        {
            string host = @"13.209.113.71";
            string username = "ubuntu";
            
            string pathRemote = "/ftp/bnk_b/uploads/";

            pathLocalPut = localdir;
            if (!Directory.Exists(pathLocalPut)) Directory.CreateDirectory(pathLocalPut);
            

            string pathRemoteFile = Path.Combine(pathRemote, remoteFileName);
            string pathLocalFile = Path.Combine(pathLocalPut, remoteFileName);


            PrivateKeyFile keyFile = new PrivateKeyFile(@"./bnk_sftp.pem");
            var keyFiles = new[] { keyFile };

            var methods = new List<AuthenticationMethod>();
            methods.Add(new PrivateKeyAuthenticationMethod(username, keyFiles));

            ConnectionInfo con = new ConnectionInfo(host, 10021, username, methods.ToArray());

            if (!File.Exists(pathLocalFile))
            {
                Console.WriteLine("File Not Found {0}", pathLocalFile);
                return;
            }

            using (SftpClient sftp = new SftpClient(con))
            {
                try
                {
                    sftp.Connect();

                    
                    Console.WriteLine("Uploading {0}", pathLocalFile);

                    using (Stream fileStream = File.OpenRead(pathLocalFile))
                    {
                        sftp.UploadFile(fileStream, pathRemoteFile);
                    }

                    sftp.Disconnect();
                }
                catch (Exception er)
                {
                    Console.WriteLine("An exception has been caught " + er.ToString());
                }
            }
        }

        public void UploadFiles(string localdir)
        {
            string host = @"13.209.113.71";
            string username = "ubuntu";

            string pathRemote = "/ftp/bnk_b/uploads/";

            pathLocalPut = localdir;
            if (!Directory.Exists(pathLocalPut)) Directory.CreateDirectory(pathLocalPut);

            string[] remoteFiles = new string[] {
                "bnk_fund_unvs"                 //펀드유니버스
                , "bnk_speclt_mp_fund"          //전문가MP구성상품
            };

            string[] pathRemoteFiles = remoteFiles.Select(f => Path.Combine(pathRemote, string.Format("{0}.{1}", f, DateTime.Now.ToString("yyyyMMddHHmmss")))).ToArray();
            //string[] pathLocalFiles = remoteFiles.Select(f => Path.Combine(pathLocalPut, f)).ToArray();
            string[] pathLocalFiles = remoteFiles.Select(f => Path.Combine(pathLocalPut, string.Format("{0}.{1}", f, DateTime.Now.ToString("yyyyMMddHHmmss")))).ToArray();

            PrivateKeyFile keyFile = new PrivateKeyFile(@"./bnk_sftp.pem");
            var keyFiles = new[] { keyFile };

            var methods = new List<AuthenticationMethod>();
            
            methods.Add(new PrivateKeyAuthenticationMethod(username, keyFiles));

            ConnectionInfo con = new ConnectionInfo(host, 10021, username, methods.ToArray());

            using (SftpClient sftp = new SftpClient(con))
            {
                try
                {
                    sftp.Connect();
#if !multi
                    var index = 0;
                    foreach (var pathLocalFile in pathLocalFiles)
                    {
                        if( !File.Exists(pathLocalFile) )
                        {
                            Console.WriteLine("File Not Found {0}", pathLocalFile);
                            index++;
                            continue;
                        }
                        Console.WriteLine("Uploading {0}", pathLocalFile);

                        using (Stream fileStream = File.OpenRead(pathLocalFile))
                        {
                            sftp.UploadFile(fileStream, pathRemoteFiles[index]);
                            index++;
                        }
                    }

#else

                    


#endif

                    

                    sftp.Disconnect();
                }
                catch (Exception er)
                {
                    Console.WriteLine("An exception has been caught " + er.ToString());
                }
            }
        }

        private string BuildRemoteSendDoneFileName(string filename)
        {
            var filenameOnly = Path.GetFileNameWithoutExtension(filename);
            var pathnameOnly = string.Empty;
            var index = filename.LastIndexOf('/');
            if(index != -1)
            {
                pathnameOnly = filename.Substring(0, index + 1);
            }
            var removename = string.Format("{0}.{1}", filenameOnly, DateTime.Now.ToString("yyyyMMddHHmmss"));
            var senddone = string.Format("{0}{1}", pathnameOnly, removename);
            return senddone;
            
        }

        private void DownloadDirectory(SftpClient client, string source, string destination, bool recursive = false)
        {
            // List the files and folders of the directory
            var files = client.ListDirectory(source);

            // Iterate over them
            foreach (SftpFile file in files)
            {
                // If is a file, download it
                if (!file.IsDirectory && !file.IsSymbolicLink)
                {
                    DownloadFile(client, file, destination);
                }
                // If it's a symbolic link, ignore it
                else if (file.IsSymbolicLink)
                {
                    Console.WriteLine("Symbolic link ignored: {0}", file.FullName);
                }
                // If its a directory, create it locally (and ignore the .. and .=) 
                //. is the current folder
                //.. is the folder above the current folder -the folder that contains the current folder.
                else if (file.Name != "." && file.Name != "..")
                {
                    var dir = Directory.CreateDirectory(Path.Combine(destination, file.Name));
                    // and start downloading it's content recursively :) in case it's required
                    if (recursive)
                    {
                        DownloadDirectory(client, file.FullName, dir.FullName);
                    }
                }
            }
        }

        /// <summary>
        /// Downloads a remote file through the client into a local directory
        /// </summary>
        /// <param name="client"></param>
        /// <param name="file"></param>
        /// <param name="directory"></param>
        private void DownloadFile(SftpClient client, SftpFile file, string directory)
        {
            Console.WriteLine("Downloading {0}", file.FullName);
            //client.RenameFile("", "");

            using (Stream fileStream = File.OpenWrite(Path.Combine(directory, file.Name)))
            {
                client.DownloadFile(file.FullName, fileStream);
            }
        }

        static void DownloadWithKey()
        {
            string host = @"13.209.113.71";
            string username = "root";
            string password = @"p4ssw0rd";

            PrivateKeyFile keyFile = new PrivateKeyFile(@"path/to/bnk_sftp.ppk");
            var keyFiles = new[] { keyFile };

            var methods = new List<AuthenticationMethod>();
            methods.Add(new PasswordAuthenticationMethod(username, password));
            methods.Add(new PrivateKeyAuthenticationMethod(username, keyFiles));

            ConnectionInfo con = new ConnectionInfo(host, 10021, username, methods.ToArray());
            using (var client = new SftpClient(con))
            {
                client.Connect();

                // Do what you need with the client !

                client.Disconnect();
            }
        }
    }
}

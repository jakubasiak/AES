using System;
using System.IO;
using System.Text;
using static Aes.AES;

namespace Aes
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Choose the type of action ('d' - Decryption, 'any other key' - Encryption):");
            var action = (Console.ReadKey().Key == ConsoleKey.D ? Process.Decryptopn : Process.Encryption);
            Console.WriteLine("Enter the password:");
            var password = Console.ReadLine();
            CheckInput(password);
            TransformFiles(action, password, args[0]);
            Console.WriteLine($"File was {(action == Process.Encryption ? "encrypted" : "decrypted")} successfully! Your password: {password}");
            Console.ReadKey();
        }

        private static void TransformFiles(Process process, string password, string path)
        {
            string currentPath;
            if (string.IsNullOrEmpty(path))
            {
                currentPath = System.IO.Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)).FullName;
            }
            else
            {
                if (Directory.Exists(path))
                {
                    currentPath = path;
                }
                else
                {
                    throw new ArgumentException("Path must be directory");
                }
            }

            var allFiles = (new DirectoryInfo(currentPath)).GetFiles();
            var aes = new AES();
            if(process == Process.Encryption) 
            {
                foreach (var fileInfo in allFiles)
                {
                    var filePath = fileInfo.FullName;
                    var file = File.ReadAllBytes(filePath);
                    var encryptedFile = aes.Encrypt(file, password);
                    var encryptedFileName = Convert.ToBase64String(Encoding.UTF8.GetBytes(Path.GetFileName(filePath)));
                    File.WriteAllBytes($"{currentPath}{Path.DirectorySeparatorChar}{encryptedFileName}", encryptedFile);
                    File.Delete(filePath);
                }

            }
            if(process == Process.Decryptopn)
            {
                foreach (var fileInfo in allFiles)
                {
                    var filePath = fileInfo.FullName;
                    var file = File.ReadAllBytes(filePath);
                    var decryptedFile = aes.Decrypt(file, password);
                    var decryptedFileName = Encoding.UTF8.GetString(Convert.FromBase64String(Path.GetFileName(filePath)));
                    File.WriteAllBytes($"{currentPath}{Path.DirectorySeparatorChar}{decryptedFileName}", decryptedFile);
                    File.Delete(filePath);
                }
            }
        }

        private static void CheckInput(string password) 
        {
            if(string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
        }
    }
}

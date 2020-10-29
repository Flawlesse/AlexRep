using System;
using System.IO.Compression;
using System.IO;
using System.Threading;


namespace STW_Service
{
    public static class Archivator
    {
        public readonly static string errorLog = @"D:\УЧЁБА\C#\lab2cs\Server\ErrorLog.txt";
        private static object obj = new object();
        static string TargetEncryptedFilePath(string fileName, string targetDir)
        {
            fileName = fileName.Replace(Path.GetDirectoryName(fileName), targetDir);
            return fileName.Replace(Path.GetFileName(fileName), Path.GetFileNameWithoutExtension(fileName) + 
                                    "_encrypted" + Path.GetExtension(fileName));
        }
        static string TargetDecryptedFilePath(string entryFileName, string targetDir)
        {
            entryFileName = Path.Combine(targetDir, entryFileName);
            string sname = Path.GetFileNameWithoutExtension(entryFileName);
            sname = sname.Replace("_encrypted", "_decrypted");
            entryFileName = entryFileName.Replace(Path.GetFileNameWithoutExtension(entryFileName), sname);

            string tmpname = Path.Combine(Path.GetDirectoryName(entryFileName), Path.GetFileNameWithoutExtension(entryFileName));
            string ext = Path.GetExtension(entryFileName);
            string newname = tmpname + ext;
            int i = 1;
            while (File.Exists(newname))
            {
                tmpname += $" ({i++})" + ext;
                newname = tmpname;
                tmpname = Path.Combine(Path.GetDirectoryName(entryFileName), Path.GetFileNameWithoutExtension(entryFileName));
            }
            return newname;
        }
        public static void Archivate(string fileName, string targetDir)
        {
            FileStream fileToEncrypt = null;
            try
            {
                string encryptedFileName = TargetEncryptedFilePath(fileName, targetDir);
                using (MemoryStream memory = new MemoryStream())//here we can store our new zip archive for some time
                {
                    while (File.Exists(fileName)) //catching file from other thread
                    {
                        try
                        {
                            fileToEncrypt = new FileStream(fileName, FileMode.Open);
                        }
                        catch (IOException)
                        {
                            continue;
                        }
                        break;
                    }
                    if (fileToEncrypt == null)
                        throw new Exception("\nНевозможно получить доступ к удалённому файлу и закончить шифрование!");

                    using (ZipArchive zip = new ZipArchive(memory, ZipArchiveMode.Create, true))
                    {
                        //move the whole encrypted thing to Memory Stream first
                        ZipArchiveEntry newEntry = zip.CreateEntry(Path.GetFileName(encryptedFileName));
                        using (Stream entryStream = newEntry.Open())
                        {
                            Encryptor.Encrypt(fileToEncrypt, entryStream);
                        }
                    }

                    //checking for name availability
                    string tmpname = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(fileName));
                    string ext = ".zip";
                    string newname = tmpname + ext;
                    int i = 1;
                    while (File.Exists(newname))
                    {
                        tmpname += $" ({i++})" + ext;
                        newname = tmpname;
                        tmpname = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(fileName));
                    }
                    //recreating the new zip archive bringing it back from RAM
                    //by pushing all the bytes to an example in File System
                    using (FileStream encryptedFS = new FileStream(Path.Combine(newname), FileMode.Create))
                    {
                        memory.Seek(0, SeekOrigin.Begin);
                        memory.CopyTo(encryptedFS);
                    }
                }
            }
            catch (Exception ex)
            {
                lock (obj)
                {
                    using (StreamWriter errorStream = new StreamWriter(new FileStream(errorLog, FileMode.OpenOrCreate)))
                    {
                        errorStream.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
            finally
            {
                fileToEncrypt?.Dispose();
            }
        }
        public static void Dearchivate(string fileName, string targetDir)
        {
            ZipArchive zip = null;
            try
            {
                while (File.Exists(fileName)) //catching file from other thread
                {
                    try
                    {
                        zip = ZipFile.OpenRead(fileName);
                    }
                    catch (IOException)
                    {
                        continue;
                    }
                    break;
                }
                if (zip == null)
                    throw new Exception("\nНевозможно получить доступ к удалённому файлу и закончить шифрование!");

                ZipArchiveEntry fileInZip = zip.Entries[0];
                string decryptedFileName = TargetDecryptedFilePath(fileInZip.Name, targetDir);

                using (FileStream targetStream = new FileStream(decryptedFileName, FileMode.OpenOrCreate, FileAccess.Write))
                using (Stream zipEntryStream = fileInZip.Open())
                    Encryptor.Decrypt(zipEntryStream, targetStream);
            }
            catch (Exception ex)
            {
                lock (obj)
                {
                    using (StreamWriter errorStream = new StreamWriter(new FileStream(errorLog, FileMode.OpenOrCreate)))
                    {
                        errorStream.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
            finally
            { 
                zip?.Dispose(); 
            }
        }
    }
}

using ICSharpCode.SharpZipLib.Tar;

internal static class LaunchScanHelpers
{

    public static Stream CreateTarballForDockerfileDirectory(string directory)
    {
        var tarball = new MemoryStream();
        var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

        using var archive = new TarOutputStream(tarball)
        {
            //Prevent the TarOutputStream from closing the underlying memory stream when done
            IsStreamOwner = false
        };

        foreach (var file in files)
        {
            //Replacing slashes as KyleGobel suggested and removing leading /
            string tarName = file.Substring(directory.Length).Replace('\\', '/').TrimStart('/');

            //Let's create the entry header
            var entry = ICSharpCode.SharpZipLib.Tar.TarEntry.CreateTarEntry(tarName);
            using var fileStream = System.IO.File.OpenRead(file);
            entry.Size = fileStream.Length;
            entry.TarHeader.Mode = Convert.ToInt32("100755", 8); //chmod 755
            archive.PutNextEntry(entry);

            //Now write the bytes of data
            byte[] localBuffer = new byte[32 * 1024];
            while (true)
            {
                int numRead = fileStream.Read(localBuffer, 0, localBuffer.Length);
                if (numRead <= 0)
                    break;

                archive.Write(localBuffer, 0, numRead);
            }

            //Nothing more to do with this entry
            archive.CloseEntry();
        }
        archive.Close();

        //Reset the stream and return it, so it can be used by the caller
        tarball.Position = 0;
        return tarball;
    }

    public static string RandomString(
        this Random rnd,
        string allowedChars,
        (int Min, int Max) length)
    {
        (int min, int max) = length;
        char[] chars = new char[max];
        int setLength = allowedChars.Length;

        int stringLength = rnd.Next(min, max + 1);

        for (int i = 0; i < stringLength; ++i)
        {
            chars[i] = allowedChars[rnd.Next(setLength)];
        }

        string randomString = new string(chars, 0, stringLength);

        return randomString;
    }
}
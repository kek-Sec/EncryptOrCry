using System;
using System.IO;
using PgpCore;


namespace EncryptOrCry
{
    class PGP_Service //Basic service to just strings using the public key provided by the user.
    {

        public static string PGPEncryptMessage(string[] input)
        {
            string FileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".in";
            string OutPutName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".out";
            File.WriteAllLines(FileName, input);
            using (PGP pgp = new PGP())
            {

                using (FileStream inputFileStream = new FileStream(FileName, FileMode.Open))
                using (Stream outputFileStream = File.Create(OutPutName))
                using (Stream publicKeyStream = new FileStream(Properties.Settings.Default.public_key, FileMode.Open))
                    pgp.EncryptStream(inputFileStream, outputFileStream, publicKeyStream, true, true);
            }
            String output = File.ReadAllText(OutPutName);
            File.WriteAllText(FileName, "010101010");
            File.WriteAllText(OutPutName, "0110101010");
            File.Delete(FileName);
            File.Delete(OutPutName);
            return output;
        }

    }
}

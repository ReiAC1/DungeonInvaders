using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ionic.Zip;

namespace DungeonInvaders.Plugins
{
    public static class PluginPackager
    {
        #region Fields

        const string EncryptionSalt = "*McPP1a%@8*dSi>@12AdvvUtE8)_=|\\1vGFF|32!aaDFnyAEgnsrio$aR%y^%ue%sFBzCXCgfJ>m.";
        const int EncryptionHash = 1262877428;

        #endregion

        #region Methods

        public static int GetHashForPlugin(string filename)
        {
            unchecked
            {
                return (int)new FileInfo(filename).Length ^ EncryptionHash ^ EncryptionSalt.Length ^ (EncryptionHash / 2);
            }
        }

        public static void CompilePlugin(string folder)
        {
            bool deleteExtras = false;

            Console.WriteLine("Compiling Plugin: " + folder);
            
            if (File.Exists("Plugins/" + folder + ".xnb")) { File.Delete("Plugins/" + folder + ".xnb"); }

            if (!File.Exists("Plugins/" + folder + "/version.txt"))
            {
                StreamWriter writer = File.CreateText("Plugins/" + folder + "/version.txt");
                writer.Write(Util.User.Version);
                writer.Close();
                deleteExtras = true;
            }

            System.IO.Compression.ZipFile.CreateFromDirectory("Plugins/" + folder, "Plugins/" + folder + ".xnb");
            
            if (deleteExtras)
            {
                File.Delete("Plugins/" + folder + "/version.txt");
            }

            EncryptFile("Plugins/" + folder + ".xnb");
            
            Console.WriteLine("Finished Compiling Plugin: " + folder);
        }

        public static ZipFile LoadPlugin(string file)
        {
            ZipFile zip;

            MemoryStream stream = new MemoryStream(DecryptData(file));

            zip = ZipFile.Read(stream);

            return zip;
        }

        static void EncryptFile(string file)
        {
            Random random = new Random(EncryptionHash ^ GetHashForPlugin(file));

            byte[] data = File.ReadAllBytes(file);

            EncryptString(ref data, random);

            File.WriteAllBytes(file, data);
        }

        static byte[] DecryptData(string file)
        {
            Random random = new Random(EncryptionHash ^ GetHashForPlugin(file));

            byte[] data = File.ReadAllBytes(file);

            DecryptString(ref data, random);

            return data;
        }

        public static void EncryptString(ref byte[] array, Random random)
        {
            unchecked
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (i % (array.Length / 20) == 0 || i == array.Length - 1)
                    {
                        Console.WriteLine("{0}% complete", (int)Math.Ceiling((i * 100.0f) / array.Length));
                    }

                    int c = array[i];
                    int saltChar = EncryptionSalt[(i ^ array.Length) % EncryptionSalt.Length];

                    c = c + ((array.Length + i) / 2) + saltChar - (i + saltChar / 2) + random.Next() - (i ^ random.Next());
                    c = c + ((array.Length - i) / 2) + saltChar + (i + saltChar / 2) + random.Next() - (i ^ random.Next());

                    c = c ^ EncryptionHash;

                    array[i] = (byte)c;
                }
            }
        }

        public static void DecryptString(ref byte[] array, Random random)
        {
            unchecked
            {
                for (int i = 0; i < array.Length; i++)
                {
                    int c = array[i] ^ EncryptionHash;
                    int saltChar = EncryptionSalt[(i ^ array.Length) % EncryptionSalt.Length];

                    c = c - ((array.Length + i) / 2) - saltChar + (i + saltChar / 2) - random.Next() + (i ^ random.Next());
                    c = c - ((array.Length - i) / 2) - saltChar - (i + saltChar / 2) - random.Next() + (i ^ random.Next());

                    array[i] = (byte)c;
                }
            }
        }

        #endregion
    }
}

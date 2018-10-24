using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonInvaders.Util
{
    public static class User
    {
        public const int Version = 0;

#if !PACKAGER
        #region Fields

        public static int ItemStart = 2;

        public static int[] OldItemStarts = null;

        public const int MaxStatLevel = 99, MaxItem = 99;

        public static int Gold = 100;

        public static readonly string[] StatNames = new string[]
        {
            "Attack", "Defense", "Strength", "Magic", "Archery", "Health", // Combat
            "Mining", "Smithing", "Fishing", "Cooking", "Crafting", // Support
        };

        public static int[] StatLevels = new int[]
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };

        public static short[,] Items = new short[128, 2];

        public static bool[] Keys = new bool[256];

        #endregion

        #region Properties

        public static String Name { get; private set; }

        public static String BaseFolder
        {
            get
            {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                    "\\.DungeonInvaders\\";

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                return folder;
            }
        }

        public static bool IsMale { get; set; }

        public static int EXP { get; set; }

        public static int NextLevelEXP
        {
            get
            {
                int value = Level;

                if (value > MaxLevel)
                {
                    return 0;
                }

                return (int)Math.Pow(value, 2.5);
            }
        }

        public static int LevelEXP
        {
            get
            {
                int value = Level - 1;

                if (value > MaxLevel || value < 0)
                {
                    return 0;
                }

                return (int)Math.Pow(value, 2.5);
            }
        }

        public static int Level
        {
            get
            {
                int value = 0;

                while (EXP >= (int)Math.Pow(value, 2.5) &&
                    value < MaxLevel)
                {
                    value++;
                }

                return value;
            }

            set
            {
                EXP = (int)Math.Pow(value, 2.5);
            }
        }

        public static int MaxLevel { get { return StatLevels.Length * 98; } }

        public static int AvailableStatPoints
        {
            get
            {
                int level = Level;
                int val = 0;

                foreach(int amt in StatLevels)
                {
                    val += amt - 1;
                }

                return level - val;
            }
        }

        #endregion

        #region Methods

        static void Initialize()
        {
            IsMale = true;
            Level = 0;

            Name = (Steamworks.SteamAPI.IsSteamRunning()) ? Steamworks.SteamFriends.GetPersonaName() : "???";

            AddItem(0, 1, false);
            AddItem(0, 10, true);
            AddItem(1, 1, true);
        }

        public static void SaveSettings()
        {
            if (!Directory.Exists(BaseFolder))
            {
                Directory.CreateDirectory(BaseFolder);
            }

            StreamWriter writer = new StreamWriter(BaseFolder + "user.dat");

            writer.Write((char)(Version & 0xFFFF));
            writer.Write((char)(Version >> 16));
            writer.Write((char)(Gold & 0xFFFF));
            writer.Write((char)(Gold >> 16));
            writer.Write(IsMale ? 'm' : 'f');
            writer.Write((char)(EXP & 0xFFFF));
            writer.Write((char)(EXP >> 16));

            writer.Write((char)StatLevels.Length);

            foreach(byte b in StatLevels)
            {
                writer.Write((char)b);
            }

            for (int i = 0; i < Items.GetLength(0); i++)
            {
                writer.Write((char)Items[i, 0]);
                writer.Write((char)Items[i, 1]);
            }

            writer.Write((char)Entities.PlayerEntity.HeadGear);
            writer.Write((char)Entities.PlayerEntity.BodyGear);
            writer.Write((char)Entities.PlayerEntity.LegGear);
            writer.Write((char)Entities.PlayerEntity.Ring);
            writer.Write((char)Entities.PlayerEntity.Necklace);
            writer.Write((char)Entities.PlayerEntity.Arrow);
            writer.Write((char)Entities.PlayerEntity.Spell);
            writer.Write((char)Entities.PlayerEntity.Weapon);
            writer.Write((char)Entities.PlayerEntity.Item);

            writer.Close();
        }

        public static void LoadSettings()
        {
            if (!Directory.Exists(BaseFolder) || !File.Exists(BaseFolder + "user.dat"))
            {
                Initialize();
                return;
            }

            StreamReader reader = new StreamReader(BaseFolder + "user.dat");

            int version = (reader.Read() & 0xFFFF) | (reader.Read() << 16);

            Gold = (reader.Read() & 0xFFFF) | (reader.Read() << 16);

            IsMale = reader.Read() == 'm';
            EXP = (reader.Read() & 0xFFFF) | (reader.Read() << 16);

            int size = reader.Read();

            for (int i = 0; i < size; i++)
            {
                int val = reader.Read();

                if (val > 99) { Environment.Exit(0); }

                StatLevels[i] = val;
            }

            int itemStart = Version != version ? OldItemStarts[version] : ItemStart;

            for (int i = 0; i < Items.GetLength(0); i++)
            {
                Items[i, 0] = (short)reader.Read();
                Items[i, 1] = (short)reader.Read();

                if (Items[i, 0] >= itemStart)
                {
                    Items[i, 0] = (short)(Items[i, 0] - itemStart + ItemStart);
                }
            }

            if (reader.EndOfStream)
            {
                reader.Close();
                return;
            }

            Entities.PlayerEntity.HeadGear = (short)reader.Read();
            Entities.PlayerEntity.BodyGear = (short)reader.Read();
            Entities.PlayerEntity.LegGear = (short)reader.Read();
            Entities.PlayerEntity.Ring = (short)reader.Read();
            Entities.PlayerEntity.Necklace = (short)reader.Read();
            Entities.PlayerEntity.Arrow = (short)reader.Read();
            Entities.PlayerEntity.Spell = (short)reader.Read();
            Entities.PlayerEntity.Weapon = (short)reader.Read();
            Entities.PlayerEntity.Item = (short)reader.Read();

            reader.Close();
        }

        public static void StartDungeon()
        {
            for (int i = 0; i < Keys.Length; i++)
            {
                Keys[i] = false;
            }
        }

        public static bool AddItem(int id, int amount)
        {
            if (id >= 1000000)
            {
                Keys[id - 1000000] = true;
                return true;
            }

            if (id == -5)
            {
                Gold += amount;
                return true;
            }

            if (amount > MaxItem) { amount = MaxItem; }

            for (int i = 0; i < Items.GetLength(0); i++)
            {
                if (Items[i, 0] == id && Items[i, 1] > 0)
                {
                    Items[i, 1] += (short)amount;

                    if (Items[i, 1] > MaxItem)
                    {
                        Items[i, 1] = MaxItem;
                    }

                    return true;
                }
            }

            for (int i = 0; i < Items.GetLength(0); i++)
            {
                if (Items[i, 1] < 1)
                {
                    Items[i, 0] = (short)id;
                    Items[i, 1] += (short)amount;

                    return true;
                }
            }

            return false;
        }

        public static bool AddItem(int id, int amount, bool startFromItemStart)
        {
            if (startFromItemStart) { id += ItemStart; }
            return AddItem(id, amount);
        }

        public static void AddEXP(int amount)
        {
            int level = Level + 1;
            int expAmount = NextLevelEXP;

            EXP += amount;

            if (Level > MaxLevel)
            {
                EXP = NextLevelEXP;
            }
        }

        #endregion
#endif
    }
}

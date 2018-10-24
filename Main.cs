using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DungeonInvaders.Util.Entities;
using DungeonInvaders.Util.Entities.Monsters;
using DungeonInvaders.Util.Entities.Weapons;
using DungeonInvaders.Util.Items;

namespace DungeonInvaders
{
    public class Main : Game
    {
        public static GraphicsDeviceManager Graphics { get; private set; }
        public static SpriteBatch SpriteBatch { get; private set; }
        public static ContentManager ContentManager { get; private set; }

        public static Main Singleton;

        public Main()
            : base()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(delegate(object obj, PreparingDeviceSettingsEventArgs args)
            {
                args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            });

            Graphics.SynchronizeWithVerticalRetrace = true;

            Content.RootDirectory = "Content";

            ContentManager = Content;

            Singleton = this;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Window.Position = new Point((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2) - (Graphics.PreferredBackBufferWidth / 2),
                (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2) - (Graphics.PreferredBackBufferHeight / 2));
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Util.Camera.Position = Vector2.Zero;

            States.StateManager.CurrentState = new States.MainMenuState();

            AddWeapons();
            AddItems();

            AddMonsters();

            Util.User.LoadSettings();
        }

        protected override void UnloadContent()
        {
            if (!(States.StateManager.CurrentState is States.GameState))
            {
                Util.User.SaveSettings();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            Steamworks.SteamAPI.RunCallbacks();

            Util.TextHelper.UpdateStates();
            Util.InputManager.Update();

            States.StateManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            States.StateManager.Draw(gameTime);

            SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.DepthRead, RasterizerState.CullCounterClockwise, null, Util.Camera.View);

            Util.InputManager.DrawCursor();

            SpriteBatch.End();

            base.Draw(gameTime);
        }

        public void MiniUpdate()
        {
            Steamworks.SteamAPI.RunCallbacks();

            Util.TextHelper.UpdateStates();
            Util.InputManager.Update();
        }

        public void MiniDraw()
        {
            BeginDraw();
            Draw(new GameTime());
            EndDraw();
        }

        void AddMonsters()
        {
            Microsoft.Xna.Framework.Audio.SoundEffect hitEffect = Content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>("hit-1");

            short itemStart = (short)Util.User.ItemStart;

            Walker bandit = new Walker("Monsters/Bandit");

            bandit.Name = "Bandit";
            bandit.MaxHealth = 15;
            bandit.PathParams.Speed = 1;
            bandit.Damage = 3;
            bandit.Defense = 2;
            bandit.ArrowDefense = 2;
            bandit.MeleeDefense = 2;
            bandit.MagicDefense = 2;
            bandit.AttackCooldown = 10;
            bandit.DropItems = new short[] { 1, itemStart, itemStart, itemStart, itemStart };
            bandit.DropItemAmounts = new short[] { 1, 2, 2, 2, 2 };
            bandit.ItemDropChance = 30;
            bandit.OnHitSoundEffect = hitEffect;

            Walker cockroach = new Walker("Monsters/Cockroach");

            cockroach.Name = "Cockroach";
            cockroach.MaxHealth = 5;
            cockroach.PathParams.Speed = 2;
            cockroach.Damage = 2;
            cockroach.Defense = 2;
            cockroach.ArrowDefense = 2;
            cockroach.MeleeDefense = 2;
            cockroach.MagicDefense = 2;
            cockroach.AttackCooldown = 15;
            cockroach.DropItems = new short[] { itemStart };
            cockroach.DropItemAmounts = new short[] { 2 };
            cockroach.ItemDropChance = 20;
            cockroach.OnHitSoundEffect = hitEffect;

            Walker hornet = new Walker("Monsters/Hornet");

            hornet.Name = "Hornet";
            hornet.MaxHealth = 7;
            hornet.PathParams.Speed = 2;
            hornet.Damage = 1;
            hornet.Defense = 3;
            hornet.ArrowDefense = 3;
            hornet.MagicDefense = 3;
            hornet.MeleeDefense = 3;
            hornet.AttackCooldown = 15;

            Walker scorpion = new Walker("Monsters/Scorpion");

            scorpion.Name = "Scorpion";
            scorpion.MaxHealth = 10;
            scorpion.PathParams.Speed = 2;
            scorpion.Damage = 3;
            scorpion.Defense = 1;
            scorpion.ArrowDefense = 1;
            scorpion.MagicDefense = 1;
            scorpion.MeleeDefense = 1;
            scorpion.AttackCooldown = 20;

            Walker chitiniac = new Walker("Monsters/Chitiniac");

            chitiniac.Name = "Chitiniac";
            chitiniac.MaxHealth = 100;
            chitiniac.PathParams.Speed = 1;
            chitiniac.Damage = 8;
            chitiniac.Defense = 4;
            chitiniac.ArrowDefense = 4;
            chitiniac.MagicDefense = 4;
            chitiniac.MeleeDefense = 4;
            chitiniac.AttackCooldown = 30;


            MonsterEntity.Register(bandit);
            MonsterEntity.Register(cockroach);
            MonsterEntity.Register(hornet);
            MonsterEntity.Register(scorpion);
            MonsterEntity.Register(chitiniac);
        }

        void AddItems()
        {
            HealingItem healthPotion = new HealingItem(20);
            healthPotion.Name = "Potion of Health";
            healthPotion.Texture = Content.Load<Texture2D>("item_0");

            TeleportHomeItem tph = new TeleportHomeItem();
            tph.Name = "Homeward stone";
            tph.Cooldown = 0;
            tph.Texture = Content.Load<Texture2D>("item_1");

            /** ORES **/

            Item coal = new Item();
            coal.Name = "Coal";

            Item tinOre = new Item();
            tinOre.Name = "Tin Ore";

            Item copperOre = new Item();
            copperOre.Name = "Copper Ore";

            Item ironOre = new Item();
            ironOre.Name = "Iron Ore";

            Item titaniumOre = new Item();
            titaniumOre.Name = "Titanium Ore";

            Item obsidianOre = new Item();
            obsidianOre.Name = "Obsidian Ore";

            Item demonBone = new Item();
            demonBone.Name = "Demon Ash";

            Item dragonBone = new Item();
            dragonBone.Name = "Dragon Heart";

            /** INGOTS **/

            Item bronzeIngot = new Item();
            bronzeIngot.Name = "Bronze Ingot";
            bronzeIngot.Texture = Content.Load<Texture2D>("bronzeingot");

            Item steelIngot = new Item();
            steelIngot.Name = "Steel Ingot";
            steelIngot.Texture = Content.Load<Texture2D>("steelingot");

            Item titaniumIngot = new Item();
            titaniumIngot.Name = "Titanium Ingot";
            titaniumIngot.Texture = Content.Load<Texture2D>("titaniumingot");

            Item obsidianIngot = new Item();
            obsidianIngot.Name = "Obsidian Ingot";
            obsidianIngot.Texture = Content.Load<Texture2D>("obsidianingot");

            Item demonIngot = new Item();
            demonIngot.Name = "Demon Ingot";
            demonIngot.Texture = Content.Load<Texture2D>("demoningot");

            Item dragonIngot = new Item();
            dragonIngot.Name = "Dragon Ingot";
            dragonIngot.Texture = Content.Load<Texture2D>("dragoningot");

            Util.User.ItemStart = Item.Register(healthPotion);
            Item.Register(tph);

            OreRockEntity.Ores.Add(Item.Register(coal));
            OreRockEntity.Ores.Add(Item.Register(tinOre));
            OreRockEntity.Ores.Add(Item.Register(copperOre));
            OreRockEntity.Ores.Add(Item.Register(ironOre));
            OreRockEntity.Ores.Add(Item.Register(titaniumOre));
            OreRockEntity.Ores.Add(Item.Register(obsidianOre));
            OreRockEntity.Ores.Add(Item.Register(demonBone));
            OreRockEntity.Ores.Add(Item.Register(dragonBone));

            FurnaceEntity.SmeltingOutputs.Add(Item.Register(bronzeIngot));
            FurnaceEntity.SmeltingOutputs.Add(Item.Register(steelIngot));
            FurnaceEntity.SmeltingOutputs.Add(Item.Register(titaniumIngot));
            FurnaceEntity.SmeltingOutputs.Add(Item.Register(obsidianIngot));
            FurnaceEntity.SmeltingOutputs.Add(Item.Register(demonIngot));
            FurnaceEntity.SmeltingOutputs.Add(Item.Register(dragonIngot));
        }

        void AddWeapons()
        {

            /* Swords */
            WeaponItem bronzeSword = new WeaponItem(0, 16, false);
            bronzeSword.Name = "Bronze Sword";
            bronzeSword.Cooldown = 40;
            bronzeSword.WeaponType = WeaponType.Sword;
            bronzeSword.Texture = Content.Load<Texture2D>("sword_0");

            WeaponItem steelSword = new WeaponItem(0, 16, false);
            steelSword.Name = "Steel Sword";
            steelSword.Cooldown = 40;
            steelSword.WeaponID = 1;
            steelSword.RequiredLevel = 10;
            steelSword.WeaponType = WeaponType.Sword;
            steelSword.Texture = Content.Load<Texture2D>("sword_1");

            WeaponItem titaniumSword = new WeaponItem(0, 16, false);
            titaniumSword.Name = "Titanium Sword";
            titaniumSword.Cooldown = 40;
            titaniumSword.WeaponID = 2;
            titaniumSword.RequiredLevel = 20;
            titaniumSword.WeaponType = WeaponType.Sword;
            titaniumSword.Texture = Content.Load<Texture2D>("sword_2");

            WeaponItem blackSword = new WeaponItem(0, 16, false);
            blackSword.Name = "Obsidian Sword";
            blackSword.Cooldown = 40;
            blackSword.WeaponID = 3;
            blackSword.RequiredLevel = 40;
            blackSword.WeaponType = WeaponType.Sword;
            blackSword.Texture = Content.Load<Texture2D>("sword_3");

            WeaponItem demonSword = new WeaponItem(0, 16, false);
            demonSword.Name = "Demon Sword";
            demonSword.Cooldown = 40;
            demonSword.WeaponID = 4;
            demonSword.RequiredLevel = 40;
            demonSword.WeaponType = WeaponType.Sword;
            demonSword.Texture = Content.Load<Texture2D>("sword_4");

            WeaponItem dragonSword = new WeaponItem(0, 16, false);
            dragonSword.Name = "Dragon Sword";
            dragonSword.Cooldown = 40;
            dragonSword.WeaponID = 5;
            dragonSword.RequiredLevel = 80;
            dragonSword.WeaponType = WeaponType.Sword;
            dragonSword.Texture = Content.Load<Texture2D>("sword_5");

            /* Axes */
            WeaponItem bronzeAxe = new WeaponItem(0, 16, false);
            bronzeAxe.Name = "Bronze Axe";
            bronzeAxe.Cooldown = 55;
            bronzeAxe.Texture = Content.Load<Texture2D>("axe_0");
            bronzeAxe.WeaponType = WeaponType.Axe;
            bronzeAxe.WeaponID = 6;
            /* Bows */

            /* Staffs */

            /** Entities **/

            /* Swords */
            SwordEntity bsE = new SwordEntity();
            bsE.Power = 3;
            bsE.Texture = Content.Load<Texture2D>("sword_0");
            bsE.Origin = new Vector2(12, 16);
            bsE.Size = new Vector2(24, 24);

            SwordEntity ssE = new SwordEntity();
            ssE.Power = 6;
            ssE.Texture = Content.Load<Texture2D>("sword_1");
            ssE.Origin = new Vector2(12, 16);
            ssE.Size = new Vector2(24, 24);

            SwordEntity tsE = new SwordEntity();
            tsE.Power = 10;
            tsE.Texture = Content.Load<Texture2D>("sword_2");
            tsE.Origin = new Vector2(12, 16);
            tsE.Size = new Vector2(24, 24);

            SwordEntity blsE = new SwordEntity();
            blsE.Power = 15;
            blsE.Texture = Content.Load<Texture2D>("sword_3");
            blsE.Origin = new Vector2(12, 16);
            blsE.Size = new Vector2(24, 24);

            SwordEntity dsE = new SwordEntity();
            dsE.Power = 24;
            dsE.Texture = Content.Load<Texture2D>("sword_4");
            dsE.Origin = new Vector2(12, 16);
            dsE.Size = new Vector2(24, 24);

            SwordEntity drsE = new SwordEntity();
            drsE.Power = 32;
            drsE.Texture = Content.Load<Texture2D>("sword_5");
            drsE.Origin = new Vector2(12, 16);
            drsE.Size = new Vector2(24, 24);

            /* Axes */
            SwordEntity baE = new SwordEntity();
            baE.Power = 5;
            baE.Texture = Content.Load<Texture2D>("axe_0");
            baE.Origin = new Vector2(12, 16);
            baE.Size = new Vector2(24, 24);
            baE.SwingSpeed /= 1.5f;
            baE.SwingSpeed *= 1.5f;

            /** Registration **/

            WeaponEntity.Register(bsE);
            WeaponEntity.Register(ssE);
            WeaponEntity.Register(tsE);
            WeaponEntity.Register(blsE);
            WeaponEntity.Register(dsE);
            WeaponEntity.Register(drsE);

            WeaponEntity.Register(baE);

            Item.Register(bronzeSword);
            Item.Register(steelSword);
            Item.Register(titaniumSword);
            Item.Register(blackSword);
            Item.Register(demonSword);
            Item.Register(dragonSword);

            Item.Register(bronzeAxe);
        }

        static T LoadResource<T>(string resource)
        {
            if (System.IO.File.Exists("Content/ " + resource) || Plugins.PluginManager.CurrentPlugin == null)
            { return ContentManager.Load<T>(resource); }

            string name = ContentManager.RootDirectory;

            string loadResource = resource;

            if (resource.EndsWith(".xnb")) { loadResource = resource.Remove(resource.Length - 4); }

            if (!System.IO.File.Exists(name + "/" + resource))
            {
                Plugins.PluginManager.CurrentPlugin[resource].Extract(ContentManager.RootDirectory);
                var v = ContentManager.Load<T>(loadResource);
                System.IO.Directory.Delete(ContentManager.RootDirectory, true);
                return v;
            }

            return ContentManager.Load<T>(loadResource);
        }

        public static void LoadScript(string script)
        {
            string data = "";

            var v = Plugins.PluginManager.CurrentPlugin[script].OpenReader();

            for (int i = 0; i < v.Length; i++)
            {
                data += (char)v.ReadByte();
            }

            Plugins.PluginManager.Execute(data);
        }

        public static Texture2D LoadImage(string image)
        {
            return LoadResource<Texture2D>(image);
        }

        public static Microsoft.Xna.Framework.Audio.SoundEffect LoadSound(string sound)
        {
            return LoadResource<Microsoft.Xna.Framework.Audio.SoundEffect>(sound);
        }

        public static T[] CreateArray<T>(int size)
        {
            return new T[size];
        }
    }
}

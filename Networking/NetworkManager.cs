/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DungeonInvaders.Networking
{
    public static class NetworkManager
    {
        #region Fields

        public const int MaxPlayers = 4;

        static bool JoiningLobby = false;
        static int LobbyID = 0;
        static SteamAPICall_t LobbyJoinCall;

        #endregion

        #region Properties

        public static bool IsJoiningLobby
        {
            get
            {
                if (!JoiningLobby) { return false; }
                return true;
            }
        }

        public static bool LobbyJoinSuccess
        {
            get
            {
                if (JoiningLobby) { return false; }
                return true;
            }
        }

        #endregion

        #region Methods

        public static int SearchForLobby(ELobbyDistanceFilter distanceFilter, int maxPlayerCount, int area, int minDifficulty,
            int maxDifficulty, int size, bool allowPlugins)
        {
            SteamMatchmaking.AddRequestLobbyListDistanceFilter(distanceFilter);
            
            SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(MaxPlayers - maxPlayerCount);

            SteamMatchmaking.AddRequestLobbyListNumericalFilter("plugins", allowPlugins ? 1 : 0,
                ELobbyComparison.k_ELobbyComparisonEqual);

            SteamMatchmaking.AddRequestLobbyListNumericalFilter("difficulty", minDifficulty,
                ELobbyComparison.k_ELobbyComparisonEqualToOrGreaterThan);

            SteamMatchmaking.AddRequestLobbyListNumericalFilter("difficulty", maxDifficulty,
                ELobbyComparison.k_ELobbyComparisonEqualToOrLessThan);

            if (area != -1)
            {
                SteamMatchmaking.AddRequestLobbyListNumericalFilter("area", area, ELobbyComparison.k_ELobbyComparisonEqual);
            }

            if (size != -1)
            {
                SteamMatchmaking.AddRequestLobbyListNumericalFilter("size", size, ELobbyComparison.k_ELobbyComparisonEqual);
            }

            SteamMatchmaking.AddRequestLobbyListResultCountFilter(100);
            SteamMatchmaking.RequestLobbyList();

            int i;

            for (i = 0; i < 100; i++)
            {
                if (!SteamMatchmaking.GetLobbyByIndex(i).IsLobby()) { return i; }
            }

            return i;
        }

        public static string[] GetLobbyString(int id)
        {
            CSteamID steamID = SteamMatchmaking.GetLobbyByIndex(id);

            if (!steamID.IsLobby())
            {
                return null;
            }

            string[] text = new string[6];

            CSteamID owner = SteamMatchmaking.GetLobbyOwner(steamID);

            int area, difficulty, size;
            bool plugins;

            area = int.Parse(SteamMatchmaking.GetLobbyData(steamID, "area"));
            difficulty = int.Parse(SteamMatchmaking.GetLobbyData(steamID, "difficulty"));
            size = int.Parse(SteamMatchmaking.GetLobbyData(steamID, "size"));
            plugins = SteamMatchmaking.GetLobbyData(steamID, "plugins")[0] == '1';

            text[0] = SteamFriends.GetFriendPersonaName(owner);
            text[1] = SteamMatchmaking.GetNumLobbyMembers(steamID)+ "/" + SteamMatchmaking.GetLobbyMemberLimit(steamID);
            text[2] = ((Util.Map.Size)size).ToString();
            text[3] = difficulty == 0 ? "Easy" : difficulty == 1 ? "Normal" : difficulty == 2 ? "Hard" : "Master";
            text[4] = area.ToString();
            text[5] = plugins.ToString();

            return text;
        }

        public static SteamAPICall_t JoinLobby(int id)
        {
            CSteamID steamID = SteamMatchmaking.GetLobbyByIndex(id);

            if (!steamID.IsLobby())
            {
                return SteamAPICall_t.Invalid;
            }

            LobbyID = id;

            JoiningLobby = true;
            return SteamMatchmaking.JoinLobby(steamID);
        }

        #endregion
    }
}
*/
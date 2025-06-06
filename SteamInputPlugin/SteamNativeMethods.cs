using System;
using System.Runtime.InteropServices;
using Steamworks;

namespace com.github.lhervier.ksp
{
    public static class SteamNativeMethods
    {
        private const string STEAM_API_DLL = "GameData/SteamInput/lib/steam_api64.dll";

        [DllImport(STEAM_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern ESteamAPIInitResult SteamInternal_SteamAPI_Init(InteropHelp.UTF8StringHandle pszInternalCheckInterfaceVersions, IntPtr pOutErrMsg);

        [DllImport(STEAM_API_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SteamAPI_Shutdown();

        // Méthode pour obtenir le pipe handle
        [DllImport(STEAM_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SteamAPI_GetHSteamPipe();

        // Méthode pour obtenir le user handle
        [DllImport(STEAM_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SteamAPI_GetHSteamUser();

        [DllImport(STEAM_API_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr SteamInternal_CreateInterface(InteropHelp.UTF8StringHandle ver);

        [DllImport(STEAM_API_DLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr SteamAPI_ISteamClient_GetISteamInput(IntPtr instancePtr, int hSteamUser, int hSteamPipe, InteropHelp.UTF8StringHandle pchVersion);

		









        // Méthode pour obtenir l'interface ISteamInput
        [DllImport(STEAM_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SteamAPI_ISteamInput_v006();

        // Méthode pour obtenir l'interface ISteamClient
        [DllImport(STEAM_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SteamAPI_SteamClient_v017();

        // Méthode pour obtenir l'interface ISteamInput via ISteamClient
        [DllImport(STEAM_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SteamClient_GetISteamInput(
            IntPtr instancePtr, 
            int hSteamUser, 
            int hSteamPipe, 
            string pchVersion
        );
    }
} 
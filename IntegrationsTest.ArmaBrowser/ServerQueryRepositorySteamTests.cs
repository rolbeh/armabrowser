using System;
using System.IO;
using System.Linq;
using ArmaBrowser.Data.DefaultImpl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationsTest.ArmaBrowser
{
    [TestClass]
    public class ServerQueryRepositorySteamTests
    {
        [TestMethod]
        public void ReadRules_Version_134787_176_77_11_19()
        {
            using (FileStream unframedFile = File.OpenRead(@"TestData\ServerRules\V_1.56.134787_176.77.11.19.rdefrag"))
            using (SteamDecodedBytes data = (new SteamUnframedBytes(ToArray(unframedFile))).DecodeSteamRuleFile_1_56())
            {
                ServerRepositorySteam.ReadRuleFile(data).ToArray();
            }
        }

        [TestMethod]
        public void ReadRules_Version_134627_188_165_245_178()
        {
            using(FileStream unframedFile =  File.OpenRead(@"TestData\ServerRules\V_1.56.134627_188.165.245.178.rdefrag"))
            using (SteamDecodedBytes data = (new SteamUnframedBytes(ToArray(unframedFile))).DecodeSteamRuleFile_1_56())
            {
                ServerRepositorySteam.ReadRuleFile(data).ToArray();
            }
        }

        [TestMethod]
        public void ReadRules_Version_134787_188_165_32_82()
        {
            using (FileStream unframedFile = File.OpenRead(@"TestData\ServerRules\V_1.56.134787_188.165.32.82.rdefrag"))
            using (SteamDecodedBytes data = (new SteamUnframedBytes(ToArray(unframedFile))).DecodeSteamRuleFile_1_56())
            {
                ServerRepositorySteam.ReadRuleFile(data).ToArray();
            }
        }

        [TestMethod]
        public void ReadRules_Version_134787_90_116_171_48()
        {
            using (FileStream unframedFile = File.OpenRead(@"TestData\ServerRules\V_1.56.134787_90.116.171.48.rdefrag"))
            using (SteamDecodedBytes data = (new SteamUnframedBytes(ToArray(unframedFile))).DecodeSteamRuleFile_1_56())
            {
                ServerRepositorySteam.ReadRuleFile(data).ToArray();
            }
        }

        [TestMethod]
        public void ReadRules_Version_134787_87_229_120_234()
        {
            SteamServerRule[] array;
            using (SteamDecodedBytes file = new SteamDecodedBytes(File.OpenRead(@"TestData\ServerRules\V_1.56.134787_87.229.120.234.rdecoded")))
            {
                array = ServerRepositorySteam.ReadRuleFile(file).ToArray();
            }

            Assert.AreEqual("modNames:1-48", array[0].Key);
            Assert.AreEqual("TRYK's Multi-Play Unifrom's pack", array[0].Name);

            Assert.AreEqual("modNames:2-48", array[1].Key);
            Assert.AreEqual("Task Force Arrowhead Radio 0.9.8", array[1].Name);

            Assert.AreEqual("modNames:3-48", array[2].Key);
            Assert.AreEqual("@sthud_a3", array[2].Name);

            Assert.AreEqual("modNames:4-48", array[3].Key);
            Assert.AreEqual("SMA", array[3].Name);

            Assert.AreEqual("modNames:5-48", array[4].Key);
            Assert.AreEqual("Ryan's Zombies & Demons", array[4].Name);

            Assert.AreEqual("modNames:6-48", array[5].Key);
            Assert.AreEqual("@rhs_usf3", array[5].Name);

            Assert.AreEqual("modNames:7-48", array[6].Key);
            Assert.AreEqual("RHS: Armed Forces of the Russian Federation", array[6].Name);

            Assert.AreEqual("modNames:8-48", array[7].Key);
            Assert.AreEqual("@rh_pistol_a3", array[7].Name);

            Assert.AreEqual("modNames:9-48", array[8].Key);
            Assert.AreEqual("@rds_civpack", array[8].Name);

            Assert.AreEqual("modNames:10-48", array[9].Key);
            Assert.AreEqual("@rds_ag_comp", array[9].Name);

            Assert.AreEqual("modNames:11-48", array[10].Key);
            Assert.AreEqual("R3F Armes 3.4", array[10].Name);

            Assert.AreEqual("modNames:12-48", array[11].Key);
            Assert.AreEqual("Kunduz, Afghanistan", array[11].Name);

            Assert.AreEqual("modNames:13-48", array[12].Key);
            Assert.AreEqual("MRT Accessory Functions", array[12].Name);

            Assert.AreEqual("modNames:14-48", array[13].Key);
            Assert.AreEqual("MELB v0.00003", array[13].Name);

            Assert.AreEqual("modNames:15-48", array[14].Key);
            Assert.AreEqual("Leights OPFOR Pack", array[14].Name);

            Assert.AreEqual("modNames:16-48", array[15].Key);
            Assert.AreEqual("konyo's MH-47E Chinook. v1.5", array[15].Name);

            Assert.AreEqual("modNames:17-48", array[16].Key);
            Assert.AreEqual("@hueypack", array[16].Name);

            Assert.AreEqual("modNames:18-48", array[17].Key);
            Assert.AreEqual("@hlcmods_m60e4", array[17].Name);

            Assert.AreEqual("modNames:19-48", array[18].Key);
            Assert.AreEqual("@hlcmods_g3", array[18].Name);

            Assert.AreEqual("modNames:20-48", array[19].Key);
            Assert.AreEqual("@hlcmods_fal", array[19].Name);

            Assert.AreEqual("modNames:21-48", array[20].Key);
            Assert.AreEqual("Half-Life Creations Mod Set", array[20].Name);

            Assert.AreEqual("modNames:22-48", array[21].Key);
            Assert.AreEqual("@hlcmods_M14", array[21].Name);

            Assert.AreEqual("modNames:23-48", array[22].Key);
            Assert.AreEqual("Half-Life Creations Mod Set", array[22].Name);

            Assert.AreEqual("modNames:24-48", array[23].Key);
            Assert.AreEqual("Half-Life Creations Mod Set", array[23].Name);

            Assert.AreEqual("modNames:25-48", array[24].Key);
            Assert.AreEqual("[GGzerosum] M1 Garand", array[24].Name);

            Assert.AreEqual("modNames:26-48", array[25].Key);
            Assert.AreEqual("FHQ Accessories Pack", array[25].Name);

            Assert.AreEqual("modNames:27-48", array[26].Key);
            Assert.AreEqual("@fata_a3", array[26].Name);

            Assert.AreEqual("modNames:28-48", array[27].Key);
            Assert.AreEqual("F/A-18 Super Hornet pack", array[27].Name);

            Assert.AreEqual("modNames:29-48", array[28].Key);
            Assert.AreEqual("@eods", array[28].Name);

            Assert.AreEqual("modNames:30-48", array[29].Key);
            Assert.AreEqual("@em", array[29].Name);

            Assert.AreEqual("modNames:31-48", array[30].Key);
            Assert.AreEqual("CUP Terrains - Maps 1.0.1", array[30].Name);

            Assert.AreEqual("modNames:32-48", array[31].Key);
            Assert.AreEqual("CUP Terrains - Core 1.0.1", array[31].Name);

            Assert.AreEqual("modNames:33-48", array[32].Key);
            Assert.AreEqual("cTab - Blue Force Tracking", array[32].Name);

            Assert.AreEqual("modNames:34-48", array[33].Key);
            Assert.AreEqual("CAF AGGRESSORS v 1.5", array[33].Name);

            Assert.AreEqual("modNames:35-48", array[34].Key);
            Assert.AreEqual("@burneslcac", array[34].Name);

            Assert.AreEqual("modNames:36-48", array[35].Key);
            Assert.AreEqual("Zade's BackpackonChest-Mod", array[35].Name);

            Assert.AreEqual("modNames:37-48", array[36].Key);
            Assert.AreEqual("@asdg_jm", array[36].Name);

            Assert.AreEqual("modNames:38-48", array[37].Key);
            Assert.AreEqual("@asdg_attachments", array[37].Name);

            Assert.AreEqual("modNames:39-48", array[38].Key);
            Assert.AreEqual("Advanced Combat Environment 3.5.1", array[38].Name);

            Assert.AreEqual("modNames:40-48", array[39].Key);
            Assert.AreEqual("75th Ranger Regiment", array[39].Name);

            Assert.AreEqual("modNames:41-48", array[40].Key);
            Assert.AreEqual("TAC Vests (TRYK)", array[40].Name);

            Assert.AreEqual("modNames:42-48", array[41].Key);
            Assert.AreEqual("@HiddenIdentityPackv3", array[41].Name);

            Assert.AreEqual("modNames:43-48", array[42].Key);
            Assert.AreEqual("HAF Units Mod", array[42].Name);

            Assert.AreEqual("modNames:44-48", array[43].Key);
            Assert.AreEqual("@DBMask", array[43].Name);

            Assert.AreEqual("modNames:45-48", array[44].Key);
            Assert.AreEqual("Community Base Addons v2.3.1", array[44].Name);

            Assert.AreEqual("modNames:46-48", array[45].Key);
            Assert.AreEqual("@ASDG_JR", array[45].Name);

            Assert.AreEqual("modNames:47-48", array[46].Key);
            Assert.AreEqual("Arma 3: ALiVE", array[46].Name);

            Assert.AreEqual("modNames:48-48", array[47].Key);
            Assert.AreEqual("31St Support Unit", array[47].Name);


        }

        [TestMethod]
        public void ReadRules_Version_134627_82_211_2_97()
        {
            SteamServerRule[] array;
            using (SteamDecodedBytes file = new SteamDecodedBytes(File.OpenRead(@"TestData\ServerRules\V_1.56.134627_82.211.2.97.rdecoded")))
            {
                array = ServerRepositorySteam.ReadRuleFile(file).ToArray();
            }
            
            Assert.AreEqual("modNames:1-38", array[0].Key);
            Assert.AreEqual("@server", array[0].Name);

            Assert.AreEqual("modNames:2-38", array[1].Key);
            Assert.AreEqual("TAC Vests", array[1].Name);

            Assert.AreEqual("modNames:3-38", array[2].Key);
            Assert.AreEqual("@Realistic Tank Crew", array[2].Name);

            Assert.AreEqual("modNames:4-38", array[3].Key);
            Assert.AreEqual("R_Unfold", array[3].Name);

            Assert.AreEqual("modNames:5-38", array[4].Key);
            Assert.AreEqual("FoxFort Camo Pack", array[4].Name);

            Assert.AreEqual("modNames:6-38", array[5].Key);
            Assert.AreEqual("@BRIDGE_Knocking", array[5].Name);

            Assert.AreEqual("modNames:7-38", array[6].Key);
            Assert.AreEqual("Advanced Towing", array[6].Name);

            Assert.AreEqual("modNames:8-38", array[7].Key);
            Assert.AreEqual("LAxemann's DynaSound", array[7].Name);

            Assert.AreEqual("modNames:9-38", array[8].Key);
            Assert.AreEqual("@mine_detector", array[8].Name);

            Assert.AreEqual("modNames:10-38", array[9].Key);
            Assert.AreEqual("@l_suppress", array[9].Name);

            Assert.AreEqual("modNames:11-38", array[10].Key);
            Assert.AreEqual("LAxemann's Mount", array[10].Name);

            Assert.AreEqual("modNames:12-38", array[11].Key);
            Assert.AreEqual("@hlcmods_g3", array[11].Name);

            Assert.AreEqual("modNames:13-38", array[12].Key);
            Assert.AreEqual("Half-Life Creations Mod Set", array[12].Name);

            Assert.AreEqual("modNames:14-38", array[13].Key);
            Assert.AreEqual("@hlc_wp_mp5", array[13].Name);

            Assert.AreEqual("modNames:15-38", array[14].Key);
            Assert.AreEqual("Half-Life Creations Mod Set", array[14].Name);

            Assert.AreEqual("modNames:16-38", array[15].Key);
            Assert.AreEqual("Half-Life Creations Mod Set", array[15].Name);

            Assert.AreEqual("modNames:17-38", array[16].Key);
            Assert.AreEqual("TF47 LAUNCHERS", array[16].Name);

            Assert.AreEqual("modNames:18-38", array[17].Key);
            Assert.AreEqual("@RH_m4", array[17].Name);

            Assert.AreEqual("modNames:19-38", array[18].Key);
            Assert.AreEqual("@RH_acc", array[18].Name);

            Assert.AreEqual("modNames:20-38", array[19].Key);
            Assert.AreEqual("@RDS_Civ", array[19].Name);

            Assert.AreEqual("modNames:21-38", array[20].Key);
            Assert.AreEqual("MELB v0.00003", array[20].Name);

            Assert.AreEqual("modNames:22-38", array[21].Key);
            Assert.AreEqual("@Ares", array[21].Name);

            Assert.AreEqual("modNames:23-38", array[22].Key);
            Assert.AreEqual("@reshmaan", array[22].Name);

            Assert.AreEqual("modNames:24-38", array[23].Key);
            Assert.AreEqual("@POOK", array[23].Name);

            Assert.AreEqual("modNames:25-38", array[24].Key);
            Assert.AreEqual("@HiddenIdentityPackV2", array[24].Name);

            Assert.AreEqual("modNames:26-38", array[25].Key);
            Assert.AreEqual("CUP Weapons ACE Compatibility 1.2", array[25].Name);

            Assert.AreEqual("modNames:27-38", array[26].Key);
            Assert.AreEqual("CUP Weapons 1.5", array[26].Name);

            Assert.AreEqual("modNames:28-38", array[27].Key);
            Assert.AreEqual("CUP Vehicles - 1.2", array[27].Name);

            Assert.AreEqual("modNames:29-38", array[28].Key);
            Assert.AreEqual("CUP Units 1.2.1", array[28].Name);

            Assert.AreEqual("modNames:30-38", array[29].Key);
            Assert.AreEqual("CUP Terrains - Full 1.0.1", array[29].Name);

            Assert.AreEqual("modNames:31-38", array[30].Key);
            Assert.AreEqual("@BWA3_Weapons_ACE", array[30].Name);

            Assert.AreEqual("modNames:32-38", array[31].Key);
            Assert.AreEqual("@bw_addition", array[31].Name);

            Assert.AreEqual("modNames:33-38", array[32].Key);
            Assert.AreEqual("Arma Role Play Objects 2 ", array[32].Name);

            Assert.AreEqual("modNames:34-38", array[33].Key);
            Assert.AreEqual("Advanced Combat Radio Environment 2", array[33].Name);

            Assert.AreEqual("modNames:35-38", array[34].Key);
            Assert.AreEqual("Advanced Combat Environment 3.5.1", array[34].Name);

            Assert.AreEqual("modNames:36-38", array[35].Key);
            Assert.AreEqual("@fata_a3", array[35].Name);

            Assert.AreEqual("modNames:37-38", array[36].Key);
            Assert.AreEqual("Community Base Addons v2.3.1", array[36].Name);

            Assert.AreEqual("modNames:38-38", array[37].Key);
            Assert.AreEqual("Bundeswehr Mod", array[37].Name);


        }

        private static byte[] ToArray(FileStream stream)
        {
            long oldPos = stream.Position;
            stream.Position = 0;
            byte[] result = new byte[stream.Length];
            stream.Read(result, 0, result.Length);
            stream.Seek(oldPos, SeekOrigin.Begin);
            return result;
        }

        private static void PrintAsserts(SteamServerRule[] array)
        {
            var i = 0;
            foreach (var steamServerRule in array)
            {
                Console.WriteLine($"Assert.AreEqual(\"{steamServerRule.Key}\", array[{i}].Key);");
                Console.WriteLine($"Assert.AreEqual(\"{steamServerRule.Name}\", array[{i}].Name);");
                Console.WriteLine();
                i++;
            }
        }
    }
}
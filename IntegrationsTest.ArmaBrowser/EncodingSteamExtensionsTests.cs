using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationsTest.ArmaBrowser.TestData
{
    [TestClass]
    public sealed class EncodingSteamExtensionsTests
    {
        private Stream BuildStream(uint value)
        {
            var mem = new MemoryStream();
            var bw = new BinaryWriter(mem);

            bw.Write(value);
            mem.Position = 0;
            return mem;
        }

        private Stream BuildStream(byte[] value)
        {
            var mem = new MemoryStream();
            var bw = new BinaryWriter(mem);

            bw.Write(value);
            mem.Position = 0;
            return mem;
        }

        [TestMethod]
        public void SteamUInt32_Normal()
        {
            const uint expected = 2837701075U;
            using (var mem = BuildStream(expected))
            using (var br = new BinaryReader(mem))
            {
                Assert.AreEqual(expected, br.ReadSteamUInt32());
                Assert.AreEqual(4, mem.Position);
            }
        }

        [TestMethod]
        public void SteamUInt32_Decode_0xFF()
        {
            using (var mem = BuildStream(new byte[] {0x2e, 0x53, 0x01, 0x03, 0xda}))
            using (var br = new BinaryReader(mem))
            {
                Assert.AreEqual(0xDAFF532E, br.ReadSteamUInt32());
                Assert.AreEqual(5, mem.Position);
            }
        }

        //101b5dc
        [TestMethod]
        public void SteamUInt32_Decode_DCB501010101()
        {
            byte[] testSeq = {0xDC, 0xB5, 0x01, 0x01, 0x01, 0x01};
            const uint expected = 0x101b5dc;
            using (var mem = BuildStream(testSeq))
            using (var br = new BinaryReader(mem))
            {
                Assert.AreEqual(expected, br.ReadSteamUInt32());
                Assert.AreEqual(6, mem.Position);
            }
        }

        [TestMethod]
        public void SteamByte_Decode_0x01()
        {
            using (var mem = BuildStream(new byte[] { 0x01, 0x01 }))
            using (var br = new BinaryReader(mem))
            {
                Assert.AreEqual(0x1, br.ReadSteamByte());
                Assert.AreEqual(2, mem.Position);
            }
        }

        [TestMethod]
        public void SteamByte_Decode_0102_to_0x00()
        {
            using (var mem = BuildStream(new byte[] { 0x01, 0x02 }))
            using (var br = new BinaryReader(mem))
            {
                Assert.AreEqual(0x0, br.ReadSteamByte());
                Assert.AreEqual(2, mem.Position);
            }
        }

        [TestMethod]
        public void UnFrameRuleFile_V_1_56_134627_82_211_2_97()
        {
            byte[] bytes;
            using (var file = File.OpenRead(@"TestData\ServerRules\V_1.56.134627_82.211.2.97.rdat"))
            {
                bytes = new byte[file.Length];
                file.Read(bytes, 0, bytes.Length);
            }
            bytes = bytes.UnframeSteamBytes_1_56().Bytes;
        }

        [TestMethod]
        public void DecodeSteamRuleFile_V_1_56_134627_82_211_2_97()
        {
            string name = "V_1.56.134627_82.211.2.97";
            byte[] bytes;
            using (var file = File.OpenRead($@"TestData\ServerRules\{name}.rdefrag"))
            using (var reader = new BinaryReader(file))
            using (var targetStream = new MemoryStream())
            {
                while (file.Position < file.Length)
                {
                    targetStream.WriteByte(reader.ReadSteamByte());
                }

                bytes = targetStream.ToArray();
            }
            File.WriteAllBytes($@"TestData\ServerRules\{name}.rdecoded", bytes);
        }
    }
}
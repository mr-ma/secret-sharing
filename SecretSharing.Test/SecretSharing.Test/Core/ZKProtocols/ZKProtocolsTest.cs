using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSharingCore.ZKProtocols;
using SecretSharingCore;

namespace SecretSharing.Test.Core.ZKProtocols
{
    /// <summary>
    /// Summary description for ZKProtocols
    /// </summary>
    [TestClass]
    public class ZKProtocolsTest
    {
        // check base1 base2 secret result1 result2 must be in ZZ
        // check c and r power to ZZ and ZZ_p maybe is wrong

        [TestMethod]
        public void TestComputeProof()
        {
            int fieldSize = 1024 / 8;
            int base1 = 3;
            int base2 = 5;
            int secret = 7;
            int result1 = base1 ^ secret;
            int result2 = base2 ^ secret;
            byte[] r = null;
            byte[] c = null;
            PrimeGenerator pg = new PrimeGenerator();
            var prime = pg.GenerateRandomPrime(fieldSize);
            NonInteractiveChaumPedersen ncp = new NonInteractiveChaumPedersen(prime);
            ncp.ComputeProofs(BitConverter.GetBytes(base1), BitConverter.GetBytes(base2), BitConverter.GetBytes(result1)
                , BitConverter.GetBytes(result2), BitConverter.GetBytes(secret), ref r, ref c);

            Assert.IsNotNull(r);
            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void TestVerifyProof()
        {
            byte[] Prime = NTLHelper.NumberToZZByte(17);
            NTLHelper.InitZZ_p(Prime);

            byte[] Base1 = NTLHelper.NumberToZZpByte(3);
            byte[] Base2 = NTLHelper.NumberToZZpByte(5);
            byte[] Result1 = NTLHelper.NumberToZZpByte(11);
            byte[] Result2 = NTLHelper.NumberToZZpByte(10);
            byte[] Secret = NTLHelper.NumberToZZpByte(7);
            byte[] R = null;
            byte[] C = null;

            NonInteractiveChaumPedersen ncp = new NonInteractiveChaumPedersen(Prime);
            ncp.ComputeProofs(Base1, Base2, Result1
                , Result2, Secret, ref R, ref C);

            // we are not providing the secret to prove only c and r are provided
            var proved = ncp.VerifyProofs(Base1, Base2, Result1
                , Result2, R, C);
            Assert.IsTrue(proved);
        }
    }
}

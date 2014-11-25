using GaloisField;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.Test
{
    [TestClass]
    public class GaloisFieldTest
    {
        [TestMethod]
        public void TestGaloisFieldMultiply()
        {
            var p = 3299;
            var field =new  FiniteFieldArithmetic.FiniteField(64);
            var val1 = 23;
            var val2 = 12;
            var w = 6;
            GaloisField.GF gf = new GF();
            var val3 = gf.galois_single_multiply(val1, val2, w);
            var element1 = new FiniteFieldArithmetic.FiniteFieldElement() { Value = val1, Field = field };
            var element2 = new FiniteFieldArithmetic.FiniteFieldElement() { Value = val2, Field = field };

           var element3 = element1 * element2;

           Assert.AreEqual(element3.Value, val3);
        }
    }
}

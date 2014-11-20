using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SecretSharing.Test
{
    [TestClass]
    public class FiniteFieldTest
    {
        [TestMethod]
        public void FiniteFieldAdd()
        {
            var p = 3299;
            var field = new FiniteFieldArithmetic.FiniteField(p);
            var val1 = 140;
            var val2 = 190;

            var element1 = new FiniteFieldArithmetic.FiniteFieldElement() { Value = val1, Field = field };
            var element2 = new FiniteFieldArithmetic.FiniteFieldElement() { Value = val2, Field = field };

            var element3 = element1 + element2;

            Assert.IsTrue(element3.Value <= p);
        }

        [TestMethod]
        public void FiniteFieldSubtract()
        {
            var p = 3299;
            var field = new FiniteFieldArithmetic.FiniteField(p);
            var val1 = 140;
            var val2 = 190;

            var element1 = new FiniteFieldArithmetic.FiniteFieldElement() { Value = val1, Field = field };
            var element2 = new FiniteFieldArithmetic.FiniteFieldElement() { Value = val2, Field = field };

            var element3 = element1 - element2;

            Assert.IsTrue(element3.Value <= p);
        }
        [TestMethod]
        public void FiniteFieldDiv()
        {
            var p = 3299;
            var field = new FiniteFieldArithmetic.FiniteField(p);
            var val1 = 140;
            var val2 = 190;

            var element1 = new FiniteFieldArithmetic.FiniteFieldElement() { Value = val1, Field = field };
            var element2 = new FiniteFieldArithmetic.FiniteFieldElement() { Value = val2, Field = field };

            var element3 = element1 / element2;

            Assert.IsTrue(element3.Value <= p);
        }

        [TestMethod]
        public void FiniteFieldMul()
        {
            var p = 3299;
            var field = new FiniteFieldArithmetic.FiniteField(p);
            var val1 = 140;
            var val2 = 190;

            var element1 = new FiniteFieldArithmetic.FiniteFieldElement() { Value = val1, Field = field };
            var element2 = new FiniteFieldArithmetic.FiniteFieldElement() { Value = val2, Field = field };

            var element3 = element1 * element2;

            Assert.IsTrue(element3.Value <= p);
        }
    }
}

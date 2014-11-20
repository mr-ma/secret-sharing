    ﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
namespace SecretSharing.FiniteFieldArithmetic
{


    /*
    Author: Halil Kemal TAŞKIN
    Web: http://hkt.me
    */

    public class FiniteField
    {
        #region Constructor

        public FiniteField(int p)
        {
            if (!MathTools.IsPrime(p))
                throw new Exception("p should be a prime number.");

            _Elements = new FiniteFieldElement[0];

            for (int i = 0; i < p; i++)
            {
                FiniteFieldElement elt = new FiniteFieldElement();
                elt.Value = i;
                elt.Field = this;

                Array.Resize(ref _Elements, _Elements.Length + 1);
                _Elements[_Elements.Length - 1] = elt;
            }

            this._Characteristic = p;
            this._Order = p - 1;
        }

        #endregion

        #region Fields

        private int _Order;
        private int _Characteristic;
        private FiniteFieldElement[] _Elements;

        #endregion

        #region Properties

        public int Order { get { return this._Order; } }
        public int Characteristic { get { return this._Characteristic; } }
        public FiniteFieldElement[] Elements { get { return _Elements; } }

        #endregion

        #region Methods

        public override string ToString()
        {
            return "GF(" + this.Characteristic + ")";
        }

        #endregion
    }

    public class FiniteFieldElement
    {
        #region Fields

        private FiniteFieldElement inv = null;

        #endregion

        #region Properties

        public int Value { get; set; }

        public FiniteField Field { get; set; }

        public FiniteFieldElement Inverse
        {
            get
            {
                if (inv == null)
                {
                    inv = new FiniteFieldElement();
                    inv = this.Field.Elements[1] / this;
                    return inv;
                }
                else
                    return inv;
            }
        }

        #endregion

        #region Operator Overloads

        public static FiniteFieldElement operator +(FiniteFieldElement element1, FiniteFieldElement element2)
        {
            if (element1.Field.Characteristic != element2.Field.Characteristic)
            {
                throw new Exception("Both elements should be in the same field.");
            }

            FiniteFieldElement result = new FiniteFieldElement();
            result.Field = element1.Field;
            result.Value = (element1.Value + element2.Value) % element1.Field.Characteristic;

            return result;
        }

        public static FiniteFieldElement operator -(FiniteFieldElement element1, FiniteFieldElement element2)
        {
            if (element1.Field.Characteristic != element2.Field.Characteristic)
            {
                throw new Exception("Both elements should be in the same field.");
            }

            FiniteFieldElement result = new FiniteFieldElement();
            result.Field = element1.Field;
            result.Value = (element1.Value - element2.Value) % element1.Field.Characteristic;
            if (result.Value < 0)
                result.Value += result.Field.Characteristic;
            return result;
        }

        public static FiniteFieldElement operator *(FiniteFieldElement element1, FiniteFieldElement element2)
        {
            if (element1.Field.Characteristic != element2.Field.Characteristic)
            {
                throw new Exception("Both elements should be in the same field.");
            }

            FiniteFieldElement result = new FiniteFieldElement();
            result.Field = element1.Field;
            result.Value = (element1.Value * element2.Value) % element1.Field.Characteristic;

            return result;
        }

        public static FiniteFieldElement operator /(FiniteFieldElement element1, FiniteFieldElement element2)
        {
            if (element1.Field.Characteristic != element2.Field.Characteristic)
            {
                throw new Exception("Both elements should be in the same field.");
            }

            foreach (FiniteFieldElement elt in element1.Field.Elements)
            {
                if ((element2 * elt).Value == element1.Value)
                    return elt;
            }

            throw new NotImplementedException("There was a problem about element operation handling.");
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return this.Value + " over " + this.Field;
        }

        #endregion
    }

    public class Polynomial
    {
        #region Constructor

        public Polynomial(params FiniteFieldElement[] coefficients)
        {
            this._coefficients = (FiniteFieldElement[])coefficients.Clone();
        }

        public Polynomial(FiniteField F, params int[] coefficients)
        {
            _coefficients = new FiniteFieldElement[0];

            for (int i = 0; i < coefficients.Length; i++)
            {
                FiniteFieldElement elt = new FiniteFieldElement();
                elt.Field = F;
                elt.Value = coefficients[i] % elt.Field.Characteristic;

                Array.Resize(ref _coefficients, _coefficients.Length + 1);
                _coefficients[_coefficients.Length - 1] = elt;
            }

        }

        public Polynomial()
        {

        }

        #endregion

        #region Fields

        private FiniteFieldElement[] _coefficients;
        //private Polynomial _monic;

        #endregion

        #region Properties

        public int Degree
        {
            get
            {
                return _coefficients.Length - 1;
            }
        }

        public bool IsIdentity
        {
            get
            {
                return (this._coefficients.Length == 1 && this._coefficients[0].Value == 1);
            }
        }

        public bool IsZero
        {
            get
            {
                return (this._coefficients.Length == 1 && this._coefficients[0].Value == 0);
            }
        }

        public int Order
        {
            get
            {
                for (int i = 1; i < int.MaxValue; i++)
                {
                    Polynomial p1 = Polynomial.GenerateFromTerm(this.Field, this.Field.Elements[1], i);
                    Polynomial p2 = new Polynomial(this.Field.Elements[this.Field.Characteristic - 1]);
                    Polynomial ord = p1 + p2;
                    Polynomial kontrol = ord % this;
                    if (kontrol.IsZero)
                        return i;

                    //if (i % 100 == 0)
                    //    Debug.WriteLine(i);
                }

                throw new Exception("Unknown error!");
            }
        }

        public FiniteField Field
        {
            get
            {
                return _coefficients[0].Field;
            }
        }

        public FiniteFieldElement[] Coefficients
        {
            get
            {
                return _coefficients;
            }
        }

        /* Monic
                public Polynomial Monic
                {
                    get
                    {
                        if (this._monic == null)
                        {
                            FiniteFieldElement[] moniccoefs = new FiniteFieldElement[this._coefficients.Length];
                            for (int i = moniccoefs.Length - 1; i > -1; i--)
                            {
                                moniccoefs[i] = this._coefficients[i];// / this._coefficients[0];
                            }
                            moniccoefs[moniccoefs.Length-1] = moniccoefs[moniccoefs.Length-1].Field.Elements[1];
                            Polynomial p = new Polynomial(moniccoefs);
                            return p;
                        }
                        else
                            return this._monic;
                    }
                }
        */

        #endregion

        #region Methods

        /// <summary>
        /// Decides if the element generates the extension field with over base field with given primitive polynomial. 
        /// </summary>
        /// <param name="primitivePolynomial">Should be in the same field with the element selected.</param>
        public bool IsPrimitiveElement(Polynomial primitivePolynomial)
        {
            if (primitivePolynomial.Field.Characteristic != this.Field.Characteristic)
            {
                throw new Exception("Given primitive polynomial should be in the same field with the polynomial.");
            }

            int[] dv = MathTools.Divisors(Math.Pow(this.Field.Characteristic, primitivePolynomial.Degree) - 1);
            for (int i = 0; i < dv.Length - 1; i++)
            {
                Polynomial test = Polynomial.ModPow(this, dv[i], primitivePolynomial);
                if (test.IsIdentity)
                    return false;
            }
            return true;

        }

        public override string ToString()
        {
            string s = "";

            if (_coefficients.Length == 1 && _coefficients[0].Value == 0)
                return "0 over " + _coefficients[0].Field.ToString();

            for (int i = 0; i < _coefficients.Length; i++)
            {
                int index = _coefficients.Length - i - 1;

                if (_coefficients[index].Value != 0)
                {
                    if (index == 0)
                        s += _coefficients[index].Value + " + ";
                    else if (index == 1 && _coefficients[index].Value == 1)
                        s += "x + ";
                    else if (index == 1)
                        s += _coefficients[index].Value + "*x + ";
                    else if (_coefficients[index].Value == 1)
                        s += "x^" + index + " + ";
                    else
                        s += _coefficients[index].Value + "*x^" + index + " + ";
                }
            }
            if (s.Length > 2)
                s = s.Remove(s.Length - 3, 3);
            s += " over " + _coefficients[0].Field.ToString();

            return s;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Generates a polynomial which consists only 1 term over given field F. e.g. (F,alfa,4) returns the polynomial alfa*x^4 over F and alfa in F.
        /// </summary>
        public static Polynomial GenerateFromTerm(FiniteField F, FiniteFieldElement Coefficient, int Degree)
        {
            if (F.Characteristic != Coefficient.Field.Characteristic)
            {
                throw new Exception("Coefficient must be an element of given Finite Field F.");
            }
            FiniteFieldElement[] coefs = new FiniteFieldElement[0];
            for (int i = 0; i < Degree; i++)
            {
                Array.Resize(ref coefs, coefs.Length + 1);
                coefs[coefs.Length - 1] = F.Elements[0];
            }
            Array.Resize(ref coefs, coefs.Length + 1);
            FiniteFieldElement f = Coefficient;
            coefs[coefs.Length - 1] = f;
            return new Polynomial(coefs);
        }

        public static Polynomial Pow(Polynomial p, int exp)
        {
            Polynomial result = new Polynomial(p.Field, 1);

            for (int i = 0; i < exp; i++)
            {
                result *= p;
                //if (i % 1000 == 0)
                //    Debug.WriteLine(i + "/" + exp);
            }
            return result;
        }

        public static Polynomial ModPow(Polynomial poly, int exponent, Polynomial modulus)
        {
            Polynomial result = new Polynomial(poly.Field, 1);

            for (int i = 0; i < exponent; i++)
            {
                result = (result * poly) % modulus;
                //if (i % 1000 == 0)
                //    Debug.WriteLine(i + "/" + exponent);
            }
            return result;
        }

        /// <summary>
        /// Generates a monic polynomial of given degree over given field F.
        /// </summary>
        public static Polynomial RandomMonic(FiniteField F, int degree)
        {
            Random rnd = new Random();
            int[] coefs = new int[degree + 1];
            for (int i = 0; i < coefs.Length - 1; i++)
                coefs[i] = rnd.Next(F.Elements[0].Value, F.Elements[F.Elements.Length - 1].Value + 1);
            coefs[coefs.Length - 1] = 1;
            Polynomial p = new Polynomial(F, coefs);
            return p;
        }

        public static Polynomial Random(FiniteField F, int degree)
        {
            Random rnd = new Random(Environment.TickCount);
            int[] coefs = new int[degree + 1];
            for (int i = 0; i < coefs.Length - 1; i++)
                coefs[i] = rnd.Next(F.Elements[0].Value, F.Elements[F.Elements.Length - 1].Value + 1);
            coefs[coefs.Length - 1] = rnd.Next(F.Elements[1].Value, F.Elements[F.Elements.Length - 1].Value + 1);
            Polynomial p = new Polynomial(F, coefs);
            return p;
        }

        public static Polynomial Random(FiniteField F, int degree, Random rnd)
        {
            int[] coefs = new int[degree + 1];
            for (int i = 0; i < coefs.Length - 1; i++)
                coefs[i] = rnd.Next(F.Elements[0].Value, F.Elements[F.Elements.Length - 1].Value + 1);
            coefs[coefs.Length - 1] = rnd.Next(F.Elements[1].Value, F.Elements[F.Elements.Length - 1].Value + 1);
            Polynomial p = new Polynomial(F, coefs);
            return p;
        }

        /// <summary>
        /// If the characteristic of the finite field is less or equal then 7 you can't use seperators.
        /// Example; For char=5 you should write string as "11232340134324320"
        /// For char=11 you should write string as "1-10-0-8-2-1-3-10-9-2"
        /// You can use . , ; _ - and space char as seperator. 
        /// </summary>
        /// <param name="F"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Polynomial Parse(FiniteField F, string s, bool trimZeros)
        {
            FiniteFieldElement[] list = new FiniteFieldElement[0];

            if (F.Characteristic > 7)
            {
                string[] slist = s.Split('.', ',', ';', '_', '-', ' ');

                for (int i = 0; i < slist.Length; i++)
                {
                    FiniteFieldElement f = new FiniteFieldElement();
                    f.Field = F;
                    f.Value = int.Parse(slist[i].ToString()) % F.Characteristic;

                    Array.Resize(ref list, list.Length + 1);
                    list[list.Length - 1] = f;
                }

            }
            else
            {
                for (int i = 0; i < s.Length; i++)
                {
                    FiniteFieldElement f = new FiniteFieldElement();
                    f.Field = F;
                    f.Value = int.Parse(s[i].ToString()) % F.Characteristic;

                    Array.Resize(ref list, list.Length + 1);
                    list[list.Length - 1] = f;
                }
            }

            // Trim 0s
            if (trimZeros)
            {
                int listlen = list.Length;
                for (int i = 0; i < listlen - 1; i++)
                {
                    if (list[listlen - i - 1].Value == 0)
                        Array.Resize(ref list, list.Length - 1);
                    else
                        break;
                }
            }

            return new Polynomial(list);

        }

        #endregion

        #region Operator Overloads

        public static Polynomial operator +(Polynomial poly1, Polynomial poly2)
        {
            if (poly1.Field.Characteristic != poly2.Field.Characteristic)
            {
                throw new Exception("Both polynomials should be defined in the same field.");
            }

            FiniteFieldElement[] elts = poly1.Degree > poly2.Degree ? (FiniteFieldElement[])poly1.Coefficients.Clone() : (FiniteFieldElement[])poly2.Coefficients.Clone();

            Polynomial p = new Polynomial(elts);

            int len = Math.Min(poly1.Degree, poly2.Degree);

            for (int i = 0; i <= len; i++)
                p._coefficients[i] = poly1._coefficients[i] + poly2._coefficients[i];

            int plen = p._coefficients.Length;
            for (int i = 0; i < plen - 1; i++)
            {
                if (p._coefficients[plen - i - 1].Value == 0)
                    Array.Resize(ref p._coefficients, p._coefficients.Length - 1);
                else
                    break;
            }

            return p;
        }

        public static Polynomial operator -(Polynomial poly1, Polynomial poly2)
        {
            if (poly1.Field.Characteristic != poly2.Field.Characteristic)
            {
                throw new Exception("Both polynomials should be defined in the same field.");
            }

            FiniteFieldElement[] elts = poly1.Degree > poly2.Degree ? (FiniteFieldElement[])poly1.Coefficients.Clone() : (FiniteFieldElement[])poly2.Coefficients.Clone();

            Polynomial p = new Polynomial(elts);

            int len = Math.Min(poly1.Degree, poly2.Degree);

            for (int i = 0; i <= len; i++)
                p._coefficients[i] = poly1._coefficients[i] - poly2._coefficients[i];

            FiniteFieldElement sifir = new FiniteFieldElement();
            sifir.Field = p.Field;
            sifir.Value = 0;

            for (int i = len + 1; i < p._coefficients.Length; i++)
                p._coefficients[i] = sifir - p._coefficients[i];

            int plen = p._coefficients.Length;
            for (int i = 0; i < plen - 1; i++)
            {
                if (p._coefficients[plen - i - 1].Value == 0)
                    Array.Resize(ref p._coefficients, p._coefficients.Length - 1);
                else
                    break;
            }

            return p;
        }

        public static Polynomial operator *(Polynomial poly1, Polynomial poly2)
        {
            if (poly1.Field.Characteristic != poly2.Field.Characteristic)
            {
                throw new Exception("Both polynomials should be defined in the same field.");
            }

            Polynomial p = new Polynomial(poly1.Field, new int[poly1.Degree + poly2.Degree + 1]);

            for (int i = 0; i < poly1._coefficients.Length; i++)
            {
                for (int j = 0; j < poly2._coefficients.Length; j++)
                    p._coefficients[i + j] += poly1._coefficients[i] * poly2._coefficients[j];
            }

            int plen = p._coefficients.Length;
            for (int i = 0; i < plen - 1; i++)
            {
                if (p._coefficients[plen - i - 1].Value == 0)
                    Array.Resize(ref p._coefficients, p._coefficients.Length - 1);
                else
                    break;
            }

            return p;

        }

        /// <returns>Exact division q of polynomials a and b s.t. a = b*q + r</returns>
        public static Polynomial operator /(Polynomial a, Polynomial b)
        {
            if (a.Field.Characteristic != b.Field.Characteristic)
            {
                throw new Exception("Both polynomials should be defined in the same field.");
            }
            //if (a.Degree < b.Degree)
            //{
            //    throw new Exception("First polynomial should have higher degree.");
            //}

            Polynomial result = new Polynomial(a.Field, 0);
            Polynomial r = a, q = new Polynomial(a.Field, 0);
            while (r.Degree >= b.Degree && b.Degree != 0)
            {
                q = Polynomial.GenerateFromTerm(r.Field, r._coefficients[r._coefficients.Length - 1] / b._coefficients[b._coefficients.Length - 1], r.Degree - b.Degree);
                result += q;
                r = r - (b * q);
            }

            return result;
        }

        /// <returns>Remainder r of polynomials a and b s.t. a = b*q + r</returns>
        public static Polynomial operator %(Polynomial a, Polynomial b)
        {
            if (a.Field.Characteristic != b.Field.Characteristic)
            {
                throw new Exception("Both polynomials should be defined in the same field.");
            }
            //if (a.Degree < b.Degree)
            //{
            //    throw new Exception("First polynomial should have higher degree.");
            //}

            Polynomial result = new Polynomial(a.Field, 0);
            Polynomial r = (Polynomial)a.MemberwiseClone(), q = new Polynomial(a.Field, 0);
            while (r.Degree >= b.Degree && b.Degree != 0)
            {
                q = Polynomial.GenerateFromTerm(r.Field, r._coefficients[r._coefficients.Length - 1] / b._coefficients[b._coefficients.Length - 1], r.Degree - b.Degree);
                result += q;
                r = r - (b * q);
            }

            return r;
        }

        #endregion
    }

    public class ExtensionField
    {
        #region Constructor

        public ExtensionField(Polynomial primitivePolynomial)
        {
            this._baseField = primitivePolynomial.Field;
            this._primitivePolynomial = primitivePolynomial;
            this.rnd = new Random();
        }

        #endregion

        #region Fields

        private Random rnd;

        private FiniteField _baseField;

        private Polynomial _primitivePolynomial;

        #endregion

        #region Properties

        public int Order
        {
            get
            {
                return (int)(Math.Pow(_baseField.Characteristic, _primitivePolynomial.Degree) - 1);
            }
        }

        public int Characteristic
        {
            get
            {
                return this._baseField.Characteristic;
            }
        }

        public Polynomial DefiningPolynomial
        {
            get
            {
                return _primitivePolynomial;
            }
        }

        public ExtensionFieldElement RandomElement
        {
            get
            {
                Polynomial p = Polynomial.Random(this._baseField, rnd.Next(0, this.DefiningPolynomial.Degree), this.rnd);
                ExtensionFieldElement elt = new ExtensionFieldElement(this, p);
                return elt;
            }
        }

        #endregion

        #region Methods

        public ExtensionFieldElement RandomPrimitiveElement(int degree)
        {
            if (degree > this._primitivePolynomial.Degree - 1)
            {
                throw new Exception("Element's degree can't be equal to or higher than Primitive polynomial's degree.");
            }

            Polynomial p = Polynomial.Random(this._baseField, degree, this.rnd);
            while (!p.IsPrimitiveElement(this._primitivePolynomial))
            {
                p = Polynomial.Random(this._baseField, degree, this.rnd);
            }
            ExtensionFieldElement elt = new ExtensionFieldElement(this, p);
            return elt;
        }

        public override string ToString()
        {
            return "Extension Field with polynomial " + this._primitivePolynomial.ToString();
        }

        #endregion
    }

    public class ExtensionFieldElement
    {
        #region Constructor

        public ExtensionFieldElement()
        {

        }

        public ExtensionFieldElement(ExtensionField ExtField, Polynomial ElementValue)
        {
            this.Value = ElementValue;
            this.Field = ExtField;
        }

        #endregion

        #region Properties

        public Polynomial Value { get; set; }

        public ExtensionField Field { get; set; }

        public int Order
        {
            get
            {
                int[] dv = MathTools.Divisors(this.Field.Order);
                for (int i = 0; i < dv.Length - 1; i++)
                {
                    Polynomial test = Polynomial.ModPow(this.Value, dv[i], this.Field.DefiningPolynomial);
                    if (test.IsIdentity)
                        return dv[i];
                }
                return this.Field.Order;
            }
        }

        public FiniteFieldElement CoerceIntoBaseField
        {
            get
            {
                if (this.Value.Degree > 0)
                {
                    throw new Exception("Only degree 0 elements can be coerced into the base field.");
                }
                FiniteFieldElement elt = new FiniteFieldElement();
                elt.Field = this.Field.DefiningPolynomial.Field;
                elt.Value = this.Value.Coefficients[0].Value;
                return elt;
            }
        }

        public ExtensionFieldElement[] Conjugates
        {
            get
            {
                ExtensionFieldElement[] cs = new ExtensionFieldElement[this.Field.DefiningPolynomial.Degree];

                for (int i = 0; i < this.Field.DefiningPolynomial.Degree; i++)
                {
                    Polynomial p = Polynomial.ModPow(this.Value, (int)Math.Pow(this.Field.Characteristic, i), this.Field.DefiningPolynomial);
                    ExtensionFieldElement exelt = new ExtensionFieldElement(this.Field, p);
                    cs[i] = exelt;
                }

                return cs;
            }
        }

        public FiniteFieldElement Trace
        {
            get
            {
                ExtensionFieldElement[] eltConjs = this.Conjugates;

                ExtensionFieldElement trace = new ExtensionFieldElement(this.Field, new Polynomial(this.Field.DefiningPolynomial.Field, 0));
                for (int i = 0; i < eltConjs.Length; i++)
                    trace += eltConjs[i];
                return trace.CoerceIntoBaseField;
            }
        }

        public FiniteFieldElement Norm
        {
            get
            {
                ExtensionFieldElement[] eltConjs = this.Conjugates;

                ExtensionFieldElement trace = new ExtensionFieldElement(this.Field, new Polynomial(this.Field.DefiningPolynomial.Field, 1));
                for (int i = 0; i < eltConjs.Length; i++)
                    trace *= eltConjs[i];
                return trace.CoerceIntoBaseField;
            }
        }

        #endregion

        #region Operator Overloads

        public static ExtensionFieldElement operator +(ExtensionFieldElement element1, ExtensionFieldElement element2)
        {
            if (element1.Field != element2.Field)
            {
                throw new Exception("Both elements should be in the same field.");
            }

            ExtensionFieldElement result = new ExtensionFieldElement();
            result.Field = element1.Field;
            result.Value = (element1.Value + element2.Value) % element1.Field.DefiningPolynomial;

            return result;
        }

        public static ExtensionFieldElement operator -(ExtensionFieldElement element1, ExtensionFieldElement element2)
        {
            if (element1.Field != element2.Field)
            {
                throw new Exception("Both elements should be in the same field.");
            }

            ExtensionFieldElement result = new ExtensionFieldElement();
            result.Field = element1.Field;
            result.Value = (element1.Value - element2.Value) % element1.Field.DefiningPolynomial;

            return result;
        }

        public static ExtensionFieldElement operator *(ExtensionFieldElement element1, ExtensionFieldElement element2)
        {
            if (element1.Field != element2.Field)
            {
                throw new Exception("Both elements should be in the same field.");
            }

            ExtensionFieldElement result = new ExtensionFieldElement();
            result.Field = element1.Field;
            result.Value = (element1.Value * element2.Value) % element1.Field.DefiningPolynomial;

            return result;
        }

        public static ExtensionFieldElement operator /(ExtensionFieldElement element1, ExtensionFieldElement element2)
        {
            if (element1.Field != element2.Field)
            {
                throw new Exception("Both elements should be in the same field.");
            }

            ExtensionFieldElement result = new ExtensionFieldElement();
            result.Field = element1.Field;
            result.Value = (element1.Value / element2.Value) % element1.Field.DefiningPolynomial;

            return result;

            //throw new NotImplementedException("There was a problem about element operation handling.");
        }

        public static ExtensionFieldElement operator %(ExtensionFieldElement element1, ExtensionFieldElement element2)
        {
            if (element1.Field != element2.Field)
            {
                throw new Exception("Both elements should be in the same field.");
            }

            ExtensionFieldElement result = new ExtensionFieldElement();
            result.Field = element1.Field;
            result.Value = (element1.Value % element2.Value) % element1.Field.DefiningPolynomial;

            return result;

            //throw new NotImplementedException("There was a problem about element operation handling.");
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            string s = Value.ToString();
            s = s.Remove(s.Length - 1, 1) + "^" + this.Field.DefiningPolynomial.Degree + ")";
            return s;
        }

        public static ExtensionFieldElement Zero(ExtensionField F)
        {
            ExtensionFieldElement zero = new ExtensionFieldElement(F, new Polynomial(F.DefiningPolynomial.Field, 0));
            return zero;
        }

        public ExtensionFieldElement Zero()
        {

            ExtensionFieldElement zero = new ExtensionFieldElement(this.Field, new Polynomial(this.Field.DefiningPolynomial.Field, 0));
            return zero;

        }

        public static ExtensionFieldElement One(ExtensionField F)
        {
            ExtensionFieldElement one = new ExtensionFieldElement(F, new Polynomial(F.DefiningPolynomial.Field, 1));
            return one;
        }

        public ExtensionFieldElement One()
        {
            ExtensionFieldElement one = new ExtensionFieldElement(this.Field, new Polynomial(this.Field.DefiningPolynomial.Field, 1));
            return one;
        }

        // TODO: Inverse

        #endregion
    }

    public class ExtensionPolynomial
    {
        #region Constructor

        public ExtensionPolynomial(params ExtensionFieldElement[] coefficients)
        {
            this._coefficients = (ExtensionFieldElement[])coefficients.Clone();
        }

        #endregion

        #region Fields

        private ExtensionFieldElement[] _coefficients;

        #endregion

        #region Properties

        public int Degree
        {
            get
            {
                return _coefficients.Length - 1;
            }
        }

        public bool IsIdentity
        {
            get
            {
                return (this._coefficients.Length == 1 && this._coefficients[0].Value.IsIdentity);
            }
        }

        public bool IsZero
        {
            get
            {
                return (this._coefficients.Length == 1 && this._coefficients[0].Value.IsZero);
            }
        }

        public ExtensionField Field
        {
            get
            {
                return _coefficients[0].Field;
            }
        }

        public ExtensionFieldElement[] Coefficients
        {
            get
            {
                return _coefficients;
            }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < this._coefficients.Length; i++)
            {
                s += "c_" + i + " = " + this._coefficients[i].ToString();
                s += i == this._coefficients.Length - 1 ? "" : Environment.NewLine;
            }

            return s;
        }

        #endregion

        #region Operator Overloads

        public static ExtensionPolynomial operator +(ExtensionPolynomial poly1, ExtensionPolynomial poly2)
        {
            if (poly1.Field.Characteristic != poly2.Field.Characteristic)
            {
                throw new Exception("Both polynomials should be defined in the same field.");
            }

            ExtensionFieldElement[] elts = poly1.Degree > poly2.Degree ? (ExtensionFieldElement[])poly1.Coefficients.Clone() : (ExtensionFieldElement[])poly2.Coefficients.Clone();

            ExtensionPolynomial p = new ExtensionPolynomial(elts);

            int len = Math.Min(poly1.Degree, poly2.Degree);

            for (int i = 0; i <= len; i++)
                p._coefficients[i] = poly1._coefficients[i] + poly2._coefficients[i];

            int plen = p._coefficients.Length;
            for (int i = 0; i < plen - 1; i++)
            {
                if (p._coefficients[plen - i - 1].Value.IsZero)
                    Array.Resize(ref p._coefficients, p._coefficients.Length - 1);
                else
                    break;
            }

            return p;
        }

        public static ExtensionPolynomial operator *(ExtensionPolynomial poly1, ExtensionPolynomial poly2)
        {
            if (poly1.Field.Characteristic != poly2.Field.Characteristic)
            {
                throw new Exception("Both polynomials should be defined in the same field.");
            }

            ExtensionFieldElement[] elts = new ExtensionFieldElement[poly1.Degree + poly2.Degree + 1];

            for (int i = 0; i < elts.Length; i++)
            {
                ExtensionFieldElement e = ExtensionFieldElement.One(poly1.Field);
                elts[i] = e;
            }

            ExtensionPolynomial p = new ExtensionPolynomial(elts);

            for (int i = 0; i < poly1._coefficients.Length; i++)
            {
                for (int j = 0; j < poly2._coefficients.Length; j++)
                    p._coefficients[i + j] += poly1._coefficients[i] * poly2._coefficients[j];
            }

            int plen = p._coefficients.Length;
            for (int i = 0; i < plen - 1; i++)
            {
                if (p._coefficients[plen - i - 1].Value.IsZero)
                    Array.Resize(ref p._coefficients, p._coefficients.Length - 1);
                else
                    break;
            }

            return p;
        }

        /* TODO: - / %
        public static Polynomial operator -(Polynomial poly1, Polynomial poly2)
        {
            if (poly1.Field.Characteristic != poly2.Field.Characteristic)
            {
                throw new Exception("Both polynomials should be defined in the same field.");
            }
            FiniteFieldElement[] elts = poly1.Degree > poly2.Degree ? (FiniteFieldElement[])poly1.Coefficients.Clone() : (FiniteFieldElement[])poly2.Coefficients.Clone();
            Polynomial p = new Polynomial(elts);
            int len = Math.Min(poly1.Degree, poly2.Degree);
            for (int i = 0; i <= len; i++)
                p._coefficients[i] = poly1._coefficients[i] - poly2._coefficients[i];
            FiniteFieldElement sifir = new FiniteFieldElement();
            sifir.Field = p.Field;
            sifir.Value = 0;
            for (int i = len + 1; i < p._coefficients.Length; i++)
                p._coefficients[i] = sifir - p._coefficients[i];
            int plen = p._coefficients.Length;
            for (int i = 0; i < plen - 1; i++)
            {
                if (p._coefficients[plen - i - 1].Value == 0)
                    Array.Resize(ref p._coefficients, p._coefficients.Length - 1);
                else
                    break;
            }
            return p;
        }
       
        /// <returns>Exact division q of polynomials a and b s.t. a = b*q + r</returns>
        public static Polynomial operator /(Polynomial a, Polynomial b)
        {
            if (a.Field.Characteristic != b.Field.Characteristic)
            {
                throw new Exception("Both polynomials should be defined in the same field.");
            }
            //if (a.Degree < b.Degree)
            //{
            //    throw new Exception("First polynomial should have higher degree.");
            //}
            Polynomial result = new Polynomial(a.Field, 0);
            Polynomial r = a, q = new Polynomial(a.Field, 0);
            while (r.Degree >= b.Degree && b.Degree != 0)
            {
                q = Polynomial.GenerateFromTerm(r.Field, r._coefficients[r._coefficients.Length - 1] / b._coefficients[b._coefficients.Length - 1], r.Degree - b.Degree);
                result += q;
                r = r - (b * q);
            }
            return result;
        }
        /// <returns>Remainder r of polynomials a and b s.t. a = b*q + r</returns>
        public static Polynomial operator %(Polynomial a, Polynomial b)
        {
            if (a.Field.Characteristic != b.Field.Characteristic)
            {
                throw new Exception("Both polynomials should be defined in the same field.");
            }
            //if (a.Degree < b.Degree)
            //{
            //    throw new Exception("First polynomial should have higher degree.");
            //}
            Polynomial result = new Polynomial(a.Field, 0);
            Polynomial r = (Polynomial)a.MemberwiseClone(), q = new Polynomial(a.Field, 0);
            while (r.Degree >= b.Degree && b.Degree != 0)
            {
                q = Polynomial.GenerateFromTerm(r.Field, r._coefficients[r._coefficients.Length - 1] / b._coefficients[b._coefficients.Length - 1], r.Degree - b.Degree);
                result += q;
                r = r - (b * q);
            }
            return r;
        }
        */

        #endregion
    }
}

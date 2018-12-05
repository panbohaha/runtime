// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

using TestLibrary;
using TypeEquivalenceTypes;

public class Simple
{
    private class EmptyType2 : IEmptyType
    {
        /// <summary>
        /// Create an instance of <see cref="EmptyType" />
        /// </summary>
        public static object Create()
        {
            return new EmptyType2();
        }
    }

    private static void InterfaceTypesFromDifferentAssembliesAreEquivalent()
    {
        Console.WriteLine($"{nameof(InterfaceTypesFromDifferentAssembliesAreEquivalent)}");
        var inAsm = EmptyType.Create();
        var otherAsm = EmptyType2.Create();

        AreNotSameObject((IEmptyType)inAsm, (IEmptyType)otherAsm);

        void AreNotSameObject(IEmptyType a, IEmptyType b)
        {
            Assert.AreNotEqual(a, b);
        }
    }

    private class MethodTestDerived : MethodTestBase
    {
        private readonly int scaleValue;

        private IMethodTestType inner;

        /// <summary>
        /// Create an instance of <see cref="MethodTestDerived" />
        /// </summary>
        public static object Create(int scaleValue, int baseScaleValue)
        {
            return new MethodTestDerived(scaleValue, baseScaleValue);
        }

        private MethodTestDerived(int scaleValue, int baseScaleValue)
            : base(baseScaleValue)
        {
            this.scaleValue = scaleValue;
        }

        public override int ScaleInt(int i)
        {
            return base.ScaleInt(i) * this.scaleValue;
        }

        public override string ScaleString(string s)
        {
            string baseValue = base.ScaleString(s);
            var sb = new StringBuilder(this.scaleValue * baseValue.Length);
            for (int i = 0; i < this.scaleValue; ++i)
            {
                sb.Append(baseValue);
            }

            return sb.ToString();
        }
    }

    private static void InterfaceTypesMethodOperations()
    {
        Console.WriteLine($"{nameof(InterfaceTypesMethodOperations)}");

        int baseScale = 2;
        int derivedScale = 3;
        object baseInst = MethodTestBase.Create(baseScale);
        object derivedInst = MethodTestDerived.Create(derivedScale, baseScaleValue: baseScale);

        var baseInterface = (IMethodTestType)baseInst;
        var derivedBase = (MethodTestBase)derivedInst;

        {
            int input = 67;
            int expectedBaseValue = input * baseScale;
            int expectedDerivedValue = expectedBaseValue * derivedScale;

            Assert.AreEqual(expectedBaseValue, baseInterface.ScaleInt(input));
            Assert.AreEqual(expectedDerivedValue, derivedBase.ScaleInt(input));
        }

        {
            string input = "stringToScale";
            string expectedBaseValue = string.Concat(Enumerable.Repeat(input, baseScale));
            string expectedDerivedValue = string.Concat(Enumerable.Repeat(expectedBaseValue, derivedScale));

            Assert.AreEqual(expectedBaseValue, baseInterface.ScaleString(input));
            Assert.AreEqual(expectedDerivedValue, derivedBase.ScaleString(input));
        }
    }

    private static void CallSparseInterface()
    {
        Console.WriteLine($"{nameof(CallSparseInterface)}");

        int sparseTypeMethodCount = typeof(ISparseType).GetMethods(BindingFlags.Public | BindingFlags.Instance).Length;
        Assert.AreEqual(2, sparseTypeMethodCount, "Should have limited method metadata");

        var sparseType = (ISparseType)SparseTest.Create();
        Assert.AreEqual(20, SparseTest.GetSparseInterfaceMethodCount(), "Should have all method metadata");

        int input = 63;
        Assert.AreEqual(input * 7, sparseType.MultiplyBy7(input));
        Assert.AreEqual(input * 18, sparseType.MultiplyBy18(input));
    }

    public static int Main(string[] noArgs)
    {
        try
        {
            InterfaceTypesFromDifferentAssembliesAreEquivalent();
            InterfaceTypesMethodOperations();
            CallSparseInterface();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Test Failure: {e}");
            return 101;
        }

        return 100;
    }
}
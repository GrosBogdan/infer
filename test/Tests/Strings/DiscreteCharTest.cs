// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.ML.Probabilistic.Tests
{
    using System;
    using System.Collections.Generic;
    using Xunit;
    using Microsoft.ML.Probabilistic.Distributions;
    using Assert = Xunit.Assert;

    /// <summary>
    /// Tests for <see cref="DiscreteChar"/>.
    /// </summary>
    public class DiscreteCharTest
    {
        /// <summary>
        /// Runs a set of common distribution tests for <see cref="DiscreteChar"/>.
        /// </summary>
        [Fact]
        [Trait("Category", "StringInference")]
        public void CharDistribution()
        {
            var rng = DiscreteChar.UniformInRanges("bdgi");
            var unif = DiscreteChar.Uniform();
            var mix = new DiscreteChar();
            mix.SetToSum(0.8, rng, 0.2, unif);

            DistributionTests.DistributionTest(unif, mix, false);
            DistributionTests.PointMassTest(mix, 'b');
            DistributionTests.UniformTest(rng, 'b');
        }

        /// <summary>
        /// Tests the support of the standard character distribution.
        /// </summary>
        [Fact]
        [Trait("Category", "StringInference")]
        public void CommonChars()
        {
            TestSupport("digit", DiscreteChar.Digit(), "0123456789", "Ab !Ј");
            TestSupport("lower", DiscreteChar.Lower(), "abcdefghixyz", "ABC0123, ");
            TestSupport("upper", DiscreteChar.Upper(), "ABCDEFGHUXYZ", "abc0123, ");
            TestSupport("letter", DiscreteChar.Letter(), "aBcDeFgGhxyzXYZ", "0123! ,");
            TestSupport("letterOrDigit", DiscreteChar.LetterOrDigit(), "abcABC0123xyzXYZ789", " !Ј$,");
            TestSupport("wordChar", DiscreteChar.WordChar(), "abc_ABC_0123s", " !:.,");
            TestSupport("whitespace", DiscreteChar.Whitespace(), " \t", "abcABC0123,:!");
        }

        [Fact]
        [Trait("Category", "StringInference")]
        public void BadRanges()
        {
            try
            {
                var a = DiscreteChar.UniformInRanges("aавz");
            }
            catch
            {
                return;
            }

            Assert.True(false);
        }

        [Fact]
        [Trait("Category", "StringInference")]
        public void SampleFromUniformCharDistribution()
        {
            // 10 chars in distributions
            const int numChars = 10;
            const int charExp = 1000;
            const int numTrials = numChars * charExp; 
            var dist = DiscreteChar.UniformInRanges("aj");

            var count = new int[numChars];
            for (var i = 0; i < numTrials; ++i)
            {
                count[dist.Sample() - 'a'] += 1;
            }

            
            // We could calculate some statistical test (like chi-squared) to verify that distribution is
            // really uniform. Instead just check that observed counts are somewhat close to expected values
            foreach (var c in count)
            {
                Assert.True(c >= charExp * 0.8 && c <= charExp * 1.2);
            }
        }

        /// <summary>
        /// Tests the support of a character distribution.
        /// </summary>
        /// <param name="distributionName">The name of the distribution.</param>
        /// <param name="distribution">The distribution.</param>
        /// <param name="included">A list of characters that must be included in the support of the distribution.</param>
        /// <param name="excluded">A list of characters that must not be included in the support of the distribution.</param>
        private static void TestSupport(
            string distributionName,
            DiscreteChar distribution,
            IEnumerable<char> included,
            IEnumerable<char> excluded)
        {
            Console.WriteLine(distributionName.PadLeft(12) + ":" + distribution);
            
            foreach (var ch in included)
            {
                Assert.True(!double.IsNegativeInfinity(distribution.GetLogProb(ch)), distribution + " should contain " + ch);
            }
            
            foreach (var ch in excluded)
            {
                Assert.True(double.IsNegativeInfinity(distribution.GetLogProb(ch)), distribution + " should not contain " + ch);
            }
        }
    }
}
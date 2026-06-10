using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KY.Core.Common.Standard.Tests;

[TestClass]
public class Random2Tests
{
    private const string Alphabet = "ABCDE";

    [TestMethod]
    public void NextString_Length_Is_Within_Requested_Range()
    {
        for (int i = 0; i < 10_000; i++)
        {
            int length = Random2.NextString(5, 25).Length;
            Assert.IsTrue(length >= 5 && length < 25, $"length {length} is outside [5, 25)");
        }
    }

    [TestMethod]
    public void NextString_Uses_Only_Characters_From_The_Alphabet()
    {
        for (int i = 0; i < 10_000; i++)
        {
            foreach (char c in Random2.NextString(5, 25, Alphabet))
            {
                Assert.IsTrue(Alphabet.Contains(c), $"unexpected character '{c}'");
            }
        }
    }

    [TestMethod]
    public void NextString_Can_Produce_Every_Character_Including_The_Last()
    {
        // Regression test: the previous implementation used random.Next(alphabet.Length - 1),
        // so the last character of the alphabet was never produced.
        HashSet<char> seen = new HashSet<char>();
        for (int i = 0; i < 5_000; i++)
        {
            foreach (char c in Random2.NextString(5, 25, Alphabet))
            {
                seen.Add(c);
            }
        }
        foreach (char expected in Alphabet)
        {
            Assert.IsTrue(seen.Contains(expected), $"character '{expected}' was never produced");
        }
        Assert.IsTrue(seen.Contains('E'), "the last character of the alphabet was never produced");
    }

    [TestMethod]
    public void NextString_With_Empty_Alphabet_Returns_Empty_String()
    {
        Assert.AreEqual(string.Empty, Random2.NextString(5, 25, string.Empty));
    }

    [TestMethod]
    public void NextString_With_Equal_Min_And_Max_Returns_Fixed_Length()
    {
        Assert.AreEqual(8, Random2.NextString(8, 8).Length);
        // The shape used by Anonymizer to pick a single character.
        Assert.AreEqual(1, Random2.NextString(1, 1, "abc").Length);
    }

    [TestMethod]
    public void NextString_Produces_Highly_Unique_Values()
    {
        // Guards against a broken generator that returns a constant value.
        HashSet<string> values = new HashSet<string>();
        for (int i = 0; i < 1_000; i++)
        {
            values.Add(Random2.NextString());
        }
        Assert.IsTrue(values.Count >= 995, $"expected near-unique values, got {values.Count} distinct of 1000");
    }

    [TestMethod]
    public void Next_Int_Is_Within_Range()
    {
        for (int i = 0; i < 10_000; i++)
        {
            int value = Random2.Next(0, 255);
            Assert.IsTrue(value >= 0 && value < 255, $"value {value} is outside [0, 255)");
        }
    }

    [TestMethod]
    public void Next_Int_Default_Is_Non_Negative()
    {
        for (int i = 0; i < 10_000; i++)
        {
            Assert.IsTrue(Random2.Next() >= 0);
        }
    }

    [TestMethod]
    public void Next_Double_Is_Within_Range()
    {
        for (int i = 0; i < 10_000; i++)
        {
            double value = Random2.Next(1.0, 2.0);
            Assert.IsTrue(value >= 1.0 && value < 2.0, $"value {value} is outside [1.0, 2.0)");
        }
    }

    [TestMethod]
    public void Next_From_List_Returns_A_Contained_Element()
    {
        List<int> list = new List<int> { 10, 20, 30, 40, 50 };
        for (int i = 0; i < 1_000; i++)
        {
            Assert.IsTrue(list.Contains(Random2.Next(list)));
        }
    }

    [TestMethod]
    public void Next_From_Empty_List_Returns_Default()
    {
        Assert.IsNull(Random2.Next(new List<string>()));
        Assert.AreEqual(0, Random2.Next(new List<int>()));
    }

    [TestMethod]
    public void Unorder_Keeps_All_Elements()
    {
        List<int> source = Enumerable.Range(0, 100).ToList();
        IList<int> result = Random2.Unorder(source);
        Assert.AreEqual(source.Count, result.Count);
        CollectionAssert.AreEquivalent(source, result.ToList());
    }

    [TestMethod]
    public void Unorder_Does_Not_Modify_The_Input()
    {
        List<int> source = Enumerable.Range(0, 100).ToList();
        List<int> snapshot = source.ToList();
        Random2.Unorder(source);
        CollectionAssert.AreEqual(snapshot, source);
    }

    [TestMethod]
    public void Next_Is_Thread_Safe()
    {
        // Regression test: the previous implementation shared a single System.Random,
        // which is not thread-safe and can corrupt its state (e.g. returning all zeros)
        // or throw under concurrent access.
        int outOfRange = 0;
        ConcurrentDictionary<int, byte> distinct = new ConcurrentDictionary<int, byte>();
        Parallel.For(0, 200_000, _ =>
        {
            int value = Random2.Next(0, 1000);
            if (value < 0 || value >= 1000)
            {
                Interlocked.Increment(ref outOfRange);
            }
            distinct[value] = 0;
        });
        Assert.AreEqual(0, outOfRange, "concurrent Next produced out-of-range values");
        Assert.IsTrue(distinct.Count > 100, $"expected a variety of values, got {distinct.Count} (generator may be corrupted)");
    }
}

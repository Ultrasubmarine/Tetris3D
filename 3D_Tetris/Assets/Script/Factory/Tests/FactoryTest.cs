using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Helper.Patterns.Factory;


internal enum TestEnumName
{
    concrete1,
    concrete2,
    concrete3,
}

internal class TestBase
{
}

internal class Concret1 : TestBase
{
}

internal class Concret2 : TestBase
{
}

internal class Concret3 : TestBase
{
}

public class FactoryTest
{
    [Test]
    public void FactoryTestSimplePasses()
    {
        // Use the Assert class to test conditions.
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator FactoryTestCreateFactory()
    {
        // Use the Assert class to test conditions.
        var FactoryBase = new Factory<TestBase>();
        Assert.NotNull(FactoryBase);
        // yield to skip a frame
        yield return null;
    }

    [UnityTest]
    public IEnumerator FactoryTestCreateObjects()
    {
        // Use the Assert class to test conditions.
        var FactoryBase = new Factory<TestBase>();

        FactoryBase.AddCreator<Concret1>(TestEnumName.concrete1.ToString());
        FactoryBase.AddCreator<Concret2>(TestEnumName.concrete2.ToString());
        FactoryBase.AddCreator<Concret3>(TestEnumName.concrete3.ToString());

        Assert.NotNull(FactoryBase.Create(TestEnumName.concrete1.ToString()));
        Assert.NotNull(FactoryBase.Create(TestEnumName.concrete2.ToString()));
        Assert.NotNull(FactoryBase.Create(TestEnumName.concrete3.ToString()));


        // yield to skip a frame
        yield return null;
    }

    [UnityTest]
    public IEnumerator FactoryTestCreateConcreteObjects()
    {
        // Use the Assert class to test conditions.
        var FactoryBase = new Factory<TestBase>();

        FactoryBase.AddCreator<Concret1>(TestEnumName.concrete1.ToString());
        FactoryBase.AddCreator<Concret2>(TestEnumName.concrete2.ToString());
        FactoryBase.AddCreator<Concret3>(TestEnumName.concrete3.ToString());

        var t1 = FactoryBase.Create(TestEnumName.concrete1.ToString());
        var t2 = FactoryBase.Create(TestEnumName.concrete2.ToString());
        var t3 = FactoryBase.Create(TestEnumName.concrete3.ToString());

        Assert.True(t1 is Concret1);
        Assert.True(t2 is Concret2);
        Assert.True(t3 is Concret3);

        Assert.False(t1 is Concret2);
        Assert.False(t2 is Concret3);
        Assert.False(t3 is Concret1);
        // yield to skip a frame
        yield return null;
    }
}
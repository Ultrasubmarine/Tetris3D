using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class teter {

    [Test]
    public void teterSimplePasses() {
        // Use the Assert class to test conditions.
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator teterWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        Debul.Log("Hellk");
        yield return null;
    }
}

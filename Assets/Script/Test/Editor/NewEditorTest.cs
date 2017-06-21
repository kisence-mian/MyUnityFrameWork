using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class NewEditorTest
{

    [Test]
    public void EditorTest()
    {
        //取出两个
        HeapTest a = HeapObjectPool<HeapTest>.GetObject();
        HeapTest b = HeapObjectPool<HeapTest>.GetObject();

        Assert.AreEqual(a.isFetch, true);
        Assert.AreEqual(b.isFetch, true);

        //放回一个
        HeapObjectPool<HeapTest>.PutObject(a);
        Assert.AreEqual(a.isFetch, false);

        //取出三个
        HeapTest c = HeapObjectPool<HeapTest>.GetObject();
        HeapTest d = HeapObjectPool<HeapTest>.GetObject();
        HeapTest e = HeapObjectPool<HeapTest>.GetObject();

        Assert.AreEqual(a, c);

        Assert.AreEqual(c.isFetch, true);
        Assert.AreEqual(d.isFetch, true);
        Assert.AreEqual(e.isFetch, true);
    }



    class HeapTest : IHeapObjectInterface
    {
        public bool isFetch = false;

        public void OnInit()
        {

        }

        public void OnPop()
        {
            isFetch = true;
        }

        public void OnPush()
        {
            isFetch = false;
        }
    }
}

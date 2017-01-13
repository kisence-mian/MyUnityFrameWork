using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// 实现编辑器界面协同
/// </summary>
public static class EditorCoroutineRunner
{
    private class EditorCoroutine : IEnumerator
    {
        private Stack<IEnumerator> executionStack;

        public EditorCoroutine(IEnumerator iterator)
        {
            this.executionStack = new Stack<IEnumerator>();
            this.executionStack.Push(iterator);
        }

        public bool MoveNext()
        {
            IEnumerator i = this.executionStack.Peek();

            if (i.MoveNext())
            {
                object result = i.Current;
                if (result != null && result is IEnumerator)
                {
                    this.executionStack.Push((IEnumerator)result);
                }

                return true;
            }
            else
            {
                if (this.executionStack.Count > 1)
                {
                    this.executionStack.Pop();
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            throw new System.NotSupportedException("This Operation Is Not Supported.");
        }

        public object Current
        {
            get { return this.executionStack.Peek().Current; }
        }

        public bool Find(IEnumerator iterator)
        {
            return this.executionStack.Contains(iterator);
        }
    }

    private static List<EditorCoroutine> editorCoroutineList;
    private static List<IEnumerator> buffer;

    public static IEnumerator StartEditorCoroutine(IEnumerator iterator)
    {
        if (editorCoroutineList == null)
        {
            // test
            editorCoroutineList = new List<EditorCoroutine>();
        }
        if (buffer == null)
        {
            buffer = new List<IEnumerator>();
        }
        if (editorCoroutineList.Count == 0)
        {
            EditorApplication.update += Update;
        }

        // add iterator to buffer first
        buffer.Add(iterator);

        return iterator;
    }

    public static void StopAllEditorCoroutine()
    {
        if (editorCoroutineList != null)
        {
            editorCoroutineList.Clear();
        }
    }

    private static bool Find(IEnumerator iterator)
    {
        // If this iterator is already added
        // Then ignore it this time
        foreach (EditorCoroutine editorCoroutine in editorCoroutineList)
        {
            if (editorCoroutine.Find(iterator))
            {
                return true;
            }
        }

        return false;
    }

    private static void Update()
    {
        // EditorCoroutine execution may append new iterators to buffer
        // Therefore we should run EditorCoroutine first
        editorCoroutineList.RemoveAll
        (
            coroutine => { return coroutine.MoveNext() == false; }
        );

        // If we have iterators in buffer
        if (buffer.Count > 0)
        {
            foreach (IEnumerator iterator in buffer)
            {
                // If this iterators not exists
                if (!Find(iterator))
                {
                    // Added this as new EditorCoroutine
                    editorCoroutineList.Add(new EditorCoroutine(iterator));
                }
            }

            // Clear buffer
            buffer.Clear();
        }

        // If we have no running EditorCoroutine
        // Stop calling update anymore
        if (editorCoroutineList.Count == 0)
        {
            EditorApplication.update -= Update;
        }
    }
}
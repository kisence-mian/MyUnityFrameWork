using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class MonoBehaviourExtend : MonoBehaviour
    {
        /// <summary>
        /// 越大调用优先级越高
        /// </summary>
        public int callLevel = 1;
        // Update is called once per frame
        public void UpdateEx()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate() { }
        public void FixedUpdateEx()
        {
            OnFixedUpdate();
        }
        protected virtual void OnFixedUpdate() { }
        public void LateUpdateEx()
        {
            OnLateUpdate();
        }
        protected virtual void OnLateUpdate() { }

        public void OnGUIEx()
        {
            OnGUIUpdate();
        }
        protected virtual void OnGUIUpdate() { }
        void OnEnable()
        {
            MonoBehaviourManager.Add(this);
        }

        void OnDestroy()
        {
            MonoBehaviourManager.Remove(this);
        }
    }


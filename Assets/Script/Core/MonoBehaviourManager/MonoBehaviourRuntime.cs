using UnityEngine;
using System.Collections;


    public class MonoBehaviourRuntime : MonoSingleton<MonoBehaviourRuntime>
    {
        public CallBack OnUpdate;
        public CallBack<float> OnUpdateExpend;
        public CallBack OnFixedUpdate;
        public CallBack OnLateUpdate;
        public CallBack OnGUIUpdate;
        public CallBack OnDrawGizmosUpdate;
        public CallBack OnApplicationExit;
        protected override void Init()
        {
            DontDestroyOnLoad(this);
        }


        // Update is called once per frame
        void Update()
        {
            if (OnUpdate != null)
                OnUpdate();

            if (OnUpdateExpend != null)
            {
                OnUpdateExpend(Time.deltaTime);
            }
        }
         void FixedUpdate()
        {
            if (OnFixedUpdate != null)
                OnFixedUpdate();
        }

         void LateUpdate()
        {
            if (OnLateUpdate != null)
                OnLateUpdate();
        }

         private void OnGUI()
        {
            if (OnGUIUpdate != null)
                OnGUIUpdate();
        }
        private void OnDrawGizmos()
        {
            if (OnDrawGizmosUpdate != null)
                OnDrawGizmosUpdate();
        }
        private void OnApplicationQuit()
        {
            if (OnApplicationExit != null)
            {
                OnApplicationExit();
            }
        }
    }


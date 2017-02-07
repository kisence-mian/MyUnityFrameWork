using UnityEngine;
using System.Collections;

	/// <summary>
	/// 帧率计算器
	/// </summary>
	public class FPSCounter
	{
		// 帧率计算频率
		private const float calcRate = 0.5f;
		// 本次计算频率下帧数
		private int frameCount = 0;
		// 频率时长
		private float rateDuration = 0f;
		// 显示帧率
		private int fps = 0;

        public static bool s_enable = true;

        //public FPSCounter()
        //{

        //}

        public void Init()
        {
            //GUIConsole.onUpdateCallback += Update;
            //GUIConsole.onGUICallback += OnGUI;

            if (ApplicationManager.AppMode != AppMode.Release)
            {
                ApplicationManager.s_OnApplicationUpdate += Update;
                ApplicationManager.s_OnApplicationOnGUI += OnGUI2;
            }
        }

		void Start()
		{
			this.frameCount = 0;
			this.rateDuration = 0f;
			this.fps = 0;
		}

		void Update()
		{
			++this.frameCount;
			this.rateDuration += Time.deltaTime;
			if (this.rateDuration > calcRate)
			{
				// 计算帧率
				this.fps = (int)(this.frameCount / this.rateDuration);
				this.frameCount = 0;
				this.rateDuration = 0f;
			}

            if (Input.GetKeyDown(KeyCode.F2))
            {
                s_enable = !s_enable;
            }
		}

		void OnGUI()
		{
            //GUI.color = Color.black;

            GUI.Label(new Rect(3, 3, 1200, GUIUtil.FontSize), "FPS:" + this.fps.ToString());
        }

        void OnGUI2()
        {
            //GUI.color = Color.black;
            if (s_enable)
            {
                GUILayout.TextField("FPS:" + fps.ToString());
            }
        }
	}

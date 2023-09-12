using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Module(1)]
    public static AssetModule Asset { get => TGameFrameWork.Instance.GetModule<AssetModule>(); }
    /// <summary>
    /// 流程组件
    /// </summary>
    [Module(2)]
    public static ProcedureModule Procedure { get => TGameFrameWork.Instance.GetModule<ProcedureModule>(); }
    [Module(3)]
    public static UIModule UI { get => TGameFrameWork.Instance.GetModule<UIModule>(); }
    [Module(4)]
    public static TimeModule Time { get => TGameFrameWork.Instance.GetModule<TimeModule>(); }
    [Module(5)]
    public static AudioModule Audio { get => TGameFrameWork.Instance.GetModule<AudioModule>(); }
    [Module(6)]
    public static MessageModule Message { get => TGameFrameWork.Instance.GetModule<MessageModule>(); }
    [Module(7)]
    public static ECSModule ECS { get => TGameFrameWork.Instance.GetModule<ECSModule>(); }
    [Module(98)]
    public static SaveModule Save { get => TGameFrameWork.Instance.GetModule<SaveModule>(); }
    /// 定时器模块
    /// </summary>
    [Module(99)]
    public static ScheduleModule Schedule { get => TGameFrameWork.Instance.GetModule<ScheduleModule>(); }

    private bool activing;

    private void Awake()
    {
        if (TGameFrameWork.Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        activing = true;
        DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR
        UnityLog.StartupEditor();
#else
            UnityLog.Startup();
#endif

        Application.logMessageReceived += OnReceiveLog;
        TGameFrameWork.Initialize();
        StartupModules();
        TGameFrameWork.Instance.InitModules();
    }

    private void Start()
    {
        TGameFrameWork.Instance.StartModules();
        Procedure.StartProcedure().Coroutine();
    }

    private void Update()
    {
        TGameFrameWork.Instance.Update();
    }

    private void LateUpdate()
    {
        TGameFrameWork.Instance.LateUpdate();
    }

    private void FixedUpdate()
    {
        TGameFrameWork.Instance.FixedUpdate();
    }

    private void OnDestroy()
    {
        if (activing)
        {
            Application.logMessageReceived -= OnReceiveLog;
            TGameFrameWork.Instance.Destroy();
            Debug.Log("销毁");
        }
    }
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ModuleAttribute : Attribute, IComparable<ModuleAttribute>
    {
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; private set; }
        /// <summary>
        /// 模块
        /// </summary>
        public BaseGameModule Module { get; set; }

        /// <summary>
        /// 添加该特性才会被当作模块
        /// </summary>
        /// <param name="priority">控制器优先级,数值越小越先执行</param>
        public ModuleAttribute(int priority)
        {
            Priority = priority;
        }

        int IComparable<ModuleAttribute>.CompareTo(ModuleAttribute other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }
    /// <summary>
    /// 初始化模块
    /// </summary>
    public void StartupModules()
    {
        List<ModuleAttribute> moduleAttrs = new List<ModuleAttribute>();
        PropertyInfo[] propertyInfos = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        Type baseCompType = typeof(BaseGameModule);
        for (int i = 0; i < propertyInfos.Length; i++)
        {
            PropertyInfo property = propertyInfos[i];
            if (!baseCompType.IsAssignableFrom(property.PropertyType))
                continue;

            object[] attrs = property.GetCustomAttributes(typeof(ModuleAttribute), false);
            if (attrs.Length == 0)
                continue;

            Component comp = GetComponentInChildren(property.PropertyType);
            if (comp == null)
            {
                Debug.LogError($"Can't Find GameModule:{property.PropertyType}");
                continue;
            }

            ModuleAttribute moduleAttr = attrs[0] as ModuleAttribute;
            moduleAttr.Module = comp as BaseGameModule;
            moduleAttrs.Add(moduleAttr);
        }

        moduleAttrs.Sort();
        for (int i = 0; i < moduleAttrs.Count; i++)
        {
            TGameFrameWork.Instance.AddModule(moduleAttrs[i].Module);
        }
    }
    private void OnApplicationQuit()
    {
        //UnityLog.Teardown();
    }
    private void OnReceiveLog(string condition, string stackTrace, LogType type)
    {
#if !UNITY_EDITOR
            if (type == LogType.Exception)
            {
                UnityLog.Fatal($"{condition}\n{stackTrace}");
            }
#endif
    }
}

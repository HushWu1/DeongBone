using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EventModule : BaseGameModule
{
    public delegate void EventHandler(BaseEventType e);

    /// <summary>
    /// 处理器
    /// </summary>
    public Dictionary<Type, List<EventHandler>> Handlers { get; private set; }

    private Queue<ICommand> commands;

    protected internal override void OnModuleInit()
    {
        base.OnModuleInit();
        Handlers = new Dictionary<Type, List<EventHandler>>();
        commands = new Queue<ICommand>();
    }

    protected internal override void OnModuleStop()
    {
        base.OnModuleStop();
        foreach (var handlerList in Handlers.Values)
        {
            handlerList.Clear();
        }
        Handlers.Clear();
        commands.Clear();
    }

    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <param name="handler"></param>
    public void Subscribe<T>(EventHandler handler) where T : BaseEventType
    {
        Subscribe(typeof(T), handler);
    }
    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <param name="handler"></param>
    public void Subscribe(Type messageType, EventHandler handler)
    {
        commands.Enqueue(new SubscribeCommand(messageType, handler));
        LoopCommands();
    }

    /// <summary>
    /// 订阅唯一消息,保证一个handler对于一个消息不会订阅多次
    /// </summary>
    /// <param name="key"></param>
    /// <param name="handler"></param>
    public void SubscribeUnique<T>(EventHandler handler) where T : BaseEventType
    {
        if (HasSubscribed<T>(handler))
            return;

        Subscribe<T>(handler);
    }

    /// <summary>
    /// 检测指定Handler是否已经订阅过一个消息
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    public bool HasSubscribed<T>(EventHandler handler) where T : BaseEventType
    {
        return FindHandlerList<T>().Contains(handler);
    }

    /// <summary>
    /// 取消订阅消息
    /// </summary>
    /// <param name="handler"></param>
    public void Unsubscribe<T>(EventHandler handler) where T : BaseEventType
    {
        Unsubscribe(typeof(T), handler);
    }
    /// <summary>
    /// 取消订阅消息
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="handler"></param>
    public void Unsubscribe(Type messageType, EventHandler handler)
    {
        commands.Enqueue(new UnsubscribeCommand(messageType, handler));
        LoopCommands();
    }

    /// <summary>
    /// 取消某个对象订阅的所有消息 
    /// </summary>
    /// <param name="target"></param>
    public void UnsubscribeAllByTarget(object target)
    {
        foreach (var handlerPair in Handlers)
        {
            foreach (var handler in handlerPair.Value)
            {
                if (handler.Target == target)
                {
                    Unsubscribe(handlerPair.Key, handler);
                }
            }
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="arg"></param>
    private void Fire<T>(T arg) where T : BaseEventType
    {
        commands.Enqueue(new FireCommand(typeof(T), arg));
        LoopCommands();
    }

    public void Fire<T>(Action<T> onInit = null) where T : BaseEventType, new()
    {
        T t = new T();
        onInit?.Invoke(t);

        Fire(t);
    }

    private bool isLooping;
    public void LoopCommands()
    {
        if (isLooping)
            return;

        isLooping = true;
        while (commands.Count > 0)
        {
            commands.Dequeue().Do();
        }
        isLooping = false;
    }

    /// <summary>
    /// 查找消息处理列表
    /// </summary>
    /// <returns></returns>
    public List<EventHandler> FindHandlerList<T>() where T : BaseEventType
    {
        return FindHandlerList(typeof(T));
    }
    public List<EventHandler> FindHandlerList(Type messageType)
    {
        List<EventHandler> list;
        if (!Handlers.TryGetValue(messageType, out list))
        {
            list = new List<EventHandler>();
            Handlers.Add(messageType, list);
        }
        return list;
    }
}


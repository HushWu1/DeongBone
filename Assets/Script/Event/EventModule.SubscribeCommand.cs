using System;


	public partial class EventModule : BaseGameModule
    {
        internal class SubscribeCommand : ICommand
        {
            private Type messageType;
            private EventHandler handler;

            public SubscribeCommand(Type messageType, EventHandler handler)
            {
                this.messageType = messageType;
                this.handler = handler;
            }

            void ICommand.Do()
            {
                if (handler == null)
                    return;

                TGameFrameWork.Instance.GetModule<EventModule>().FindHandlerList(messageType).Add(handler);
            }
        }
    }

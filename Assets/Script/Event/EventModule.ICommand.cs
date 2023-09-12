using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EventModule : BaseGameModule
{
    internal interface ICommand
    {
        void Do();
    }
}

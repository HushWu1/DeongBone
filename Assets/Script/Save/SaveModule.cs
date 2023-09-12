using System.Collections.Generic;
using System.Threading.Tasks;


public class SaveModule : BaseGameModule
{
    public string directory;

    private Queue<BaseTask> taskQueue;
    private bool isSaving;

    protected internal override void OnModuleStart()
    {
        base.OnModuleStart();
        taskQueue = new Queue<BaseTask>();
    }

    public void AddTask(BaseTask task)
    {
        if (task == null)
            return;

        taskQueue.Enqueue(task);
    }

    protected internal override void OnModuleUpdate(float deltaTime)
    {
        base.OnModuleUpdate(deltaTime);

        if (!isSaving)
        {
            //StartSaveingTask().Coroutine();
        }
    }

    //private async Task StartSaveingTask()
    //{
    //    if (isSaving)
    //        return;

    //    isSaving = true;
    //    while (taskQueue.Count > 0)
    //    {
    //        try
    //        {
    //            BaseTask task = taskQueue.Dequeue();
    //            await task.Do();
    //        }
    //        catch (System.Exception e)
    //        {
    //            //UnityLog.Error(e.Message);
                
    //        }
    //    }

    //    isSaving = false;
    //}
}

using System.Threading.Tasks;


    public abstract class BaseTask
    {
        public string Path { get; set; }

        public abstract Task Do();
    }


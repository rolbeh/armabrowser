using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Logic
{
    static class UiTask
    {
        #region Fields

        private static TaskScheduler _uiTaskScheduler;
        private static TaskFactory _uiTaskFactory;

        #endregion Fields
        

        public static void Initialize()
        {
            if (_uiTaskScheduler != null) return;
            _uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _uiTaskFactory = new TaskFactory(_uiTaskScheduler);
        }

        public static TaskScheduler TaskScheduler
        {
            get { return _uiTaskScheduler; }
        }

        public static void Run(Action action)
        {
            _uiTaskFactory.StartNew(action);
        }

        public static void Run<T1>(Action<T1> action, T1 obj1)
        {
            Tuple<Action<T1>, T1> tuple = new Tuple<Action<T1>, T1>(action, obj1);

            _uiTaskFactory.StartNew(t => ((Tuple<Action<T1>, T1>)t).Item1(((Tuple<Action<T1>, T1>)t).Item2), tuple);
        }

        //public static void Run<T1, T2>(Action<T1, T2> action, T1 obj1, T2 obj2)
        //{
        //    Tuple<Action<T1, T2>, T1, T2> tuple = new Tuple<Action<T1, T2>, T1, T2>(action, obj1, obj2);

        //    _uiTaskFactory.StartNew(t => ((Tuple<Action<T1, T2>, T1, T2>)t).Item1(((Tuple<Action<T1, T2>, T1, T2>)t).Item2, ((Tuple<Action<T1, T2>, T1, T2>)t).Item3), tuple);
        //}

        public static Task Run<T1, T2>(Action<T1, T2> action, T1 obj1, T2 obj2)
        {
            Tuple<Action<T1, T2>, T1, T2> tuple = new Tuple<Action<T1, T2>, T1, T2>(action, obj1, obj2);

            return _uiTaskFactory.StartNew(t => ((Tuple<Action<T1, T2>, T1, T2>)t).Item1(((Tuple<Action<T1, T2>, T1, T2>)t).Item2, ((Tuple<Action<T1, T2>, T1, T2>)t).Item3), tuple);
        }

        public static async Task RunAsync<T1, T2>(Action<T1, T2> action, T1 obj1, T2 obj2, System.Threading.CancellationToken cancellationToken)
        {
            Tuple<Action<T1, T2>, T1, T2> tuple = new Tuple<Action<T1, T2>, T1, T2>(action, obj1, obj2);

            await _uiTaskFactory.StartNew(t => ((Tuple<Action<T1, T2>, T1, T2>)t).Item1(((Tuple<Action<T1, T2>, T1, T2>)t).Item2, ((Tuple<Action<T1, T2>, T1, T2>)t).Item3), tuple, cancellationToken);
        }

        public static async Task RunAsync(Action action)
        {
            await _uiTaskFactory.StartNew(action);
        }

        public static Task Run(Action action, System.Threading.CancellationToken cancellationToken)
        {
            return _uiTaskFactory.StartNew(action, cancellationToken);
        }

        public static T RunInUi<T>(Func<T> function)
        {
            var task = _uiTaskFactory.StartNew<T>(function);
            task.Wait();
            return task.Result;
        }

        public static async Task<T> RunInUiAsync<T>(Func<T> function)
        {
            return await _uiTaskFactory.StartNew<T>(function); 
        }
    }
}

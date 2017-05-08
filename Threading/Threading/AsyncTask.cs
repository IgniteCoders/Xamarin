using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
namespace Threading {
	public abstract class AsyncTask<Params, Progress, Result> {

		public interface OnTaskCompleted {
			void TaskCompletedWithResult(int taskIdentifier, Result result);
		}

		public int taskIdentifier = -1;
		public OnTaskCompleted taskDelegate;

		public AsyncTask() {

		}

		public AsyncTask(OnTaskCompleted taskDelegate) {
			this.taskDelegate = taskDelegate;
		}

		public AsyncTask(int identifier, OnTaskCompleted taskDelegate) {
			this.taskIdentifier = identifier;
			this.taskDelegate = taskDelegate;
		}

		public void Execute() {
			Execute(null);
		}

		public void Execute(Dictionary<String, Params> parameters) {
			Result result = default(Result);

			Task.Factory.StartNew(() => {
				try {
					result = DoInBackground(parameters);
				} catch (Exception e) {
					Debug.WriteLine(e.ToString());
				}
			}).ContinueWith(
			   t => {
				   PostExecute(result);
				   if (taskDelegate != null) {
					   taskDelegate.TaskCompletedWithResult(taskIdentifier, result);
				   }
			   }, TaskScheduler.FromCurrentSynchronizationContext()
			);
		}

		public virtual void PreExecute() { }

		public abstract Result DoInBackground(Dictionary<String, Params> parameters);

		public virtual void PostExecute(Result result) { }
	}
}
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
namespace Threading {
	public abstract class AsyncTask<Params, Progress, Result> {
		public AsyncTask() {

		}

		public void Execute(List<Params> parameters) {
			Result result = default(Result);

			Task.Factory.StartNew(() => {
				try {
					result = DoInBackground(parameters);
				} catch (Exception e) {
					Debug.WriteLine(e.StackTrace);
				}
			}).ContinueWith(
			   t => {
				   PostExecute(result);
			   }, TaskScheduler.FromCurrentSynchronizationContext()
			);
		}

		public virtual void PreExecute() {

		}

		public abstract Result DoInBackground(List<Params> parameters);

		public virtual void PostExecute(Result result) {

		}
	}
}
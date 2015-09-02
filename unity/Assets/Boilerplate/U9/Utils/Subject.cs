using UnityEngine;
using System.Collections.Generic;

public class Subject<T> {
	
	T currentValue;

	public Subject () : this( default(T) )
	{
		
	}
	
	public Subject (T value)
	{
		this.currentValue = value;
	}


	public class SubjectChangedEventArgs : System.EventArgs {

		public SubjectChangedEventArgs() {
			Transitions = new List<U9Transition>();
		}

		public List<U9Transition> Transitions { get; internal set; }
		public T OldValue { get; internal set; }
		public T NewValue { get; internal set; }
		public bool IsInitialEvent { get; internal set; }
	}

	private event System.EventHandler<SubjectChangedEventArgs> ChangedInternal;

	public event System.EventHandler<SubjectChangedEventArgs> Changed {
		add {
			ChangedInternal += value;
			SubjectChangedEventArgs e = new SubjectChangedEventArgs () { OldValue = currentValue, NewValue = currentValue, IsInitialEvent = true };		
			value ( this, e);		
			U9Transition[] transitions = e.Transitions.ToArray ();
			U9T.PrioritySequence (transitions).Begin();
		}		
		remove {
			ChangedInternal -= value;
		}
	}

	void OnChanged( SubjectChangedEventArgs e ) {
		ChangedInternal (this, e);
	}

	public T GetValue() {
		return currentValue;
	}

	public void SetValue( T newValue ) {
		U9Transition[] transitions;
		SetValue (newValue, out transitions );
		U9T.PrioritySequence (transitions).Begin ();
	}

	public void SetValue( T newValue, out U9Transition[] transitions ) {
		T oldValue = currentValue;
		currentValue = newValue;

		SubjectChangedEventArgs e = new SubjectChangedEventArgs () { OldValue = oldValue, NewValue = newValue, IsInitialEvent = false };

		OnChanged(e);

		transitions = e.Transitions.ToArray ();
	}



	
}

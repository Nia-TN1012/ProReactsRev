#region Version Info.
/**
*	@file Chronoir_net.ProReactsRev.cs
*	@brief Provides the function to update property and automatically notify it changed, when the progress status or result information reported from the outside.
*
*		PROgress information REACTive notification System - REporting Value  : Pro-Reacts Rev	
*
*	@par Target platform
*	- .NET Framework 4.5
*	@par Version
*	1.0.1.0
*	@par Author
*	Nia Tomonaka
*	@par Copyright
*	Copyright (C) 2014-2017 Chronoir.net
*	@par Released Day
*	2017/03/12
*	@par Last modified Day
*	2017/03/12
*	@par licence
*	MIT licence
*	@par Contact
*	@@nia_tn1012（ https://twitter.com/nia_tn1012/ ）
*	@par Homepage
*	- http://chronoir.net/ （Homepage）
*	- https://github.com/Nia-TN1012/ProReactsRev/ （GitHub）
*	- https://www.nuget.org/packages/Chronoir_net.ProReactsRev/ （NuGet Gallery）
*/
#endregion
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Chronoir_net.MVVMExtensions {

	/// <summary>
	///		Provides the helper class to notify property changed.
	/// </summary>
	public abstract class NotifyPropertyChangedHelper : INotifyPropertyChanged {

		/// <summary>
		///		Represents the event handler executed when a property changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///		Gets the <see cref="INotifyPropertyChanged.PropertyChanged"/> event handler.
		/// </summary>
		public PropertyChangedEventHandler PropertyChangedFromThis =>
			PropertyChanged;

		/// <summary>
		///		Notifies the change of the specified property.
		/// </summary>
		/// <param name="propertyName">Property name (On the default, set the caller's property name.)</param>
		protected virtual void NotifyPropertyChanged( [CallerMemberName]string propertyName = null ) {
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}

		/// <summary>
		///		Sets the value of the specified property and notifies it changed.
		/// </summary>
		/// <typeparam name="T">Type of property</typeparam>
		/// <param name="property">Target property</param>
		/// <param name="value">Value to set for the property</param>
		/// <param name="propertyName">Property name (On the default, set the caller's property name.)</param>
		protected virtual void SetProperty<T>( ref T property, T value, [CallerMemberName]string propertyName = null ) {
			property = value;
			NotifyPropertyChanged( propertyName );
		}
	}

	/// <summary>
	///		Providess <see cref="ProReactsCore{T}"/> class to store the progress information of the 'Model'.
	/// </summary>
	/// <typeparam name="T">Type of object that stores progress information</typeparam>
	public class ProReactsCore<T> : NotifyPropertyChangedHelper {

		/// <summary>
		///		Gets the identifier given when creating the current instance.
		/// </summary>
		public string ID { get; private set; }

		/// <summary>
		///		Represents an object that stores progress information.
		/// </summary>
		private T progressInfo;
		/// <summary>
		///		Gets / sets the object that stores the progress information.
		/// </summary>
		/// <remarks>
		///		By data-binding to the UI through this property,
		///		when has called Reporter.Report of <see cref="ProReactsRev{T}"/> to report the progress information,
		///		calls the set side of this property to store the value and notifies the property changed.
		///	</remarks>
		public T ProgressInfo {
			get { return progressInfo; }
			set {
				// Sets a value in the property and notifies it changed.
				// This will be reflected in the value of the UI,
				// data-binding through ProReactsCore<T> property of ProReactsRev<T>.
				SetProperty( ref progressInfo, value );
				// Raises the ProgressInfoChanged event.
				ProgressInfoChanged?.Invoke( this, progressInfo );
			}
		}

		/// <summary>
		///		Represents the event handler executed when the progress information is changed.
		/// </summary>
		public event EventHandler<T> ProgressInfoChanged;

		/// <summary>
		///		Creates an instance of <see cref="ProReactsCore{T}"/> from specified identifier.
		/// </summary>
		/// <param name="id">Indentifier</param>
		public ProReactsCore( string id = null ) : base() {
			ID = id;
			progressInfo = default( T );
		}
	}

	/// <summary>
	///		Provides <see cref="ProReactsRev{T}"/> class reporting 'Model''s progress information to <see cref="ProReactsCore{T}"/>.
	/// </summary>
	/// <typeparam name="T">Type of object that stores progress information</typeparam>
	public class ProReactsRev<T> : NotifyPropertyChangedHelper {

		/// <summary>
		///		Gets an instance of <see cref="ProReactsCore{T}"/>.
		/// </summary>
		/// <remarks>If binding data to the UI through this property, do not need to notify the property changed on 'ViewModel'.</remarks>
		public ProReactsCore<T> ProReactsCore { get; private set; }

		/// <summary>
		///		Represents a <see cref="Progress{T}"/> object to call back when receiving a report from <see cref="IProgress{T}"/>.
		/// </summary>
		private Progress<T> progress;

		/// <summary>
		///		Gets the <see cref = "IProgress {T}" /> object for reporting to <see cref="ProReactsCore{T}"/>.
		/// </summary>
		public IProgress<T> Reporter =>
			progress;

		/// <summary>
		///		Creates an instance of <see cref="ProReactsRev{T}"/> from specified identifier.
		/// </summary>
		/// <param name="id">Indentifier</param>
		public ProReactsRev( string id = null ) : base() {
			// Specifies the identifier and initializes ProReactsCore<T>.
			ProReactsCore = new ProReactsCore<T>( id );

			// Initializes the Progress<T>
			progress = new Progress<T>(
				// Configures the process to be executed when receive a report from Reporter.Report of ProReactsRev<T>.
				// Parameter "e" contains information on the progress status reported
				e =>
					// Set the ProgressInfo property of ProReactsCore<T> to an object that stores progress information.
					ProReactsCore.ProgressInfo = e
			);
		}
	}
}
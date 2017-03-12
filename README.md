# ProReactsRev
Provides the function to update property and automatically notify it changed, when the progress status or result information reported from the outside.
PROgress information REACTive notification System - REporting Value  

## Library summary

|Name|Chronoir_net.ProReactsRev|
|---|---|
|Type|.NET Framework class library|
|Author|Nia Tomonaka|
|Version|1.0.1.0|
|Released date|2017/03/12|
|Last modified date|2017/03/12|
|Programing Lang.|C# 6.0|
|Target framework|.NET Framework 4.5|
|Licence|MIT Licence|
|Homepage|https://chronoir.net/|
|GitHub|https://github.com/Nia-TN1012/ProReactsRev/|
|NuGet Gallery|https://www.nuget.org/packages/Chronoir_net.ProReactsRev/|
|Twitter|[@nia_tn1012](https://twitter.com/nia_tn1012)|

## Introduction

Pro-Reacts-Rev rovides the function to update property and automatically notify it changed, when the progress status or result information reported from the outside.

## Architecture

This library consists of the following three classes.
* **`NotifyPropertyChangedHelper`** class : The helper class that has a function to notify property changed to be used in MVVM pattern.
* **`ProReactsCore\<T\>`** class : Stores progress status and result information as ProgressInfo property and notifies change of ProgressInfo property when value is changed.
* **`ProReactsRev\<T\>`** class : This class has `ProReactsCore\<T\>` and `Progress\<T\>` instances, 
that implements the function to automatically set the value reported in the ProgressInfo of `ProReactsCore\<T\>` class
when the value is reported via `IProgress\<T\>`.

## How to use

Use the `ProReactsRev\<T\>` class in the Model class of the MVVM pattern.

```csharp
using Chronoir_net.MVVMExtentions;

// Model
class Model {

	private ProReactsRev<string> status;
	public ProReactsCore<string> Status =>
		status.ProReactsCore;
		
	private ProReactsRev<string> status2;
	public ProReactsCore<string> Status2 =>
		status2.ProReactsCore;
	
	public Model() {
		status = new ProReactsRev<string>( "Status1" );
		// The identifier to set for the ID property of the ProReactsCore<T> class, can be omitted.
		status2 = new ProReactsRev<string>();
	}
	
	public DoSomething() {
		// To report progress and results, call the 'Reporter.Report' method of 'ProReactsRev<T>' class.
		status.Reporter.Report( "Process start" );
		// ...
		status.Reporter.Report( "Process completed" );
	}
	
	public DoSomething2() {
		status2.Reporter.Report( "Process start" );
		// ...
		status2.Reporter.Report( "Process completed" );
	}
}

// ViewModel
class ViewModel : NotifyPropertyChangedHelper {
	// Model
	private model;
	
	// Gets a value from the Model's 'Status' property.
	public ProReactsCore<string> Status =>
		model.Status;
		
	// Gets a value from the Model's 'Status2.ProgressInfo' property.
	public string StatusMessage =>
		model.Status2.ProgressInfo;
		
	public ViewModel() {
		model = new Model();
		
		// If directly bind an instance of 'ProReactsCore<T>' like the 'Status' property,
		// do not need to notify the property via the 'ProgressInfoChanged' event handler.
		// Automatically reflected by property changed notification in 'ProReactsCore<T>' class.
		
		// When data binding the 'ProgressInfo' property of 'ProReactsCore<T>' like the 'StatusMessage' property,
		// notifies the property changed via the 'ProgressInfoChanged' event handler.
		// In that case, make sure to create an instance of the 'ProReactsRev<T>' class on the same thread as the UI.
		model.Status2.ProgressInfoChanged =>
			( sender, e ) =>
				NotifyPropertyChanged( nameof( StatusMessage ) );
	}
	
	// ...
}
```

You can also use the 'ProReactsRev\<T\>' class inherited from the Model class.
In that case, the 'ProReactsCore' property and the 'Reporter' property are exposed outside the Model class.

```csharp
using Chronoir_net.ModelExtentions;

// Model
class Model : ProReactsRev<string> {

	public Model : base() {	}
	
	public DoSomething() {
		Reporter.Report( "Process start );
		// ...
		Reporter.Report( "Process completed" );
	}
}

// ViewModel
class ViewModel : NotifyPropertyChangedHelper {
	private model;
	public ProReactsCore<string> Status =>
		model.ProReactsCore;
		
	// ...
}
```


## Legal Disclaimer

The author and Chronoir.net accept no any responsibility for any obstacles or damages caused by using this library.
Please be understanding of this beforehand.


## Release note

* Ver. 1.0.1.0 : 2017/03/12 
　 - First release（Re-released from Nia_Tech.ProReactsRev. XML comments are supported in Japanese and English）

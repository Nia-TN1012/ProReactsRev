# ProReactsRev
---
MVVMパターン用、進行状況の情報の自動通知システム「プロリアクトス・レヴ」  
PROgress information REACTive notification System - REporting Value  
  
バージョン : Ver. 1.0.0.1 ( Mar. 09, 2016 )  
  
種別 : .NET クラスライブラリ  
  
ターゲットプラットフォーム :  
.NET Framework 4.5 
  
作成者 : 智中 ニア（ [@nia_tn1012](https://twitter.com/nia_tn1012/) ）  
ライセンス : MIT Licence  
  
リンク  
　http://chronoir.net （ホームページ）  
　https://github.com/Nia-TN1012/ProReactsRev （GitHubのリポジトリ）  
  
NuGet Gallery（.NuGetパッケージは近日公開予定です）  
　[https://www.nuget.org/packages/Nia_Tech.ProReactsRev/]()


---

##◆ はじめに

プロリアクトス・レヴは、外部から進行状況や結果の情報がレポートされた時、その情報を格納するプロパティの更新とその通知を自動的に行う機能を提供します。

##◆ アーキテクチャー

このライブラリには、以下の3つのクラスで構成されています。
* **NotifyPropertyChangedHelper**クラス : MVVMパターンで利用する、プロパティ変更の通知するための機能を持つヘルパークラスです。
* **ProReactsCore\<T\>**クラス : 進行状況や結果の情報をProgressInfoプロパティとして格納し、値が変更された時にProgressInfoプロパティの変更通知を行うクラスです。
* **ProReactsRev\<T\>**クラス : ProReactsCore\<T\>クラスとProgress\<T\>のインスタンスを持ち、IProgress\<T\>経由で値がレポートされた時に、
そのProReactsCore\<T\>クラスのProgressInfoにレポートされた値を自動的に設定する機能を実装したクラスです。

##◆ 使い方

MVVMパターンのModelクラスでProReactsRev\<T\>クラスを使用します。

```csharp
using Nia_Tech.ModelExtentions;

// MVVMのModelクラス
class Model {

	private ProReactsRev<string> status;
	public ProReactsCore<string> Status =>
		status.ProReactsCore;
		
	private ProReactsRev<string> status2;
	public ProReactsCore<string> Status2 =>
		status2.ProReactsCore;
	
	public Model() {
		status = new ProReactsRev<string>( "ステータス1" );
		// ProReactsCoreクラスのIDプロパティに設定する識別名は省略可能です。
		status2 = new ProReactsRev<string>();
	}
	
	public DoSomething() {
		// 進行状況や結果のレポートは、ProReactsRevクラスのReporterプロパティからReportメソッドを呼び出して行います。
		status.Reporter.Report( "処理開始" );
		// ...
		status.Reporter.Report( "処理完了" );
	}
	
	public DoSomething2() {
		status2.Reporter.Report( "処理開始" );
		// ...
		status2.Reporter.Report( "処理完了" );
	}
}

// MVVMのViewModelクラス
class ViewModel : NotifyPropertyChangedHelper {
	// Model
	private model;
	
	// ModelのStatusプロパティを直接取得します。
	public ProReactsCore<string> Status =>
		model.Status;
		
	// ModelのStatus2プロパティからProgressInfoプロパティを取得します。
	public string StatusMessage =>
		model.Status2.ProgressInfo;
		
	public ViewModel() {
		model = new Model();
		
		// StatusのようにProReactsCoreプロパティを取得するViewModelのプロパティを直接データバインディングする場合、
		// ProgressInfoChangedイベントハンドラーを経由してのプロパティを通知は不要です。
		// ProReactsCoreクラス内のプロパティ変更通知により、自動的に反映されます。
		
		// StatusMessageのように、ProReactsCoreのProgressInfoを取得するViewModelのプロパティをデータバインディングする場合、
		// ProgressInfoChangedイベントハンドラーを経由してのプロパティを通知します。
		// その場合、ProReactsRevクラスのインスタンスは必ずUIと同じスレッド上で作成してください。
		model.Status2.ProgressInfoChanged =>
			( sender, e ) =>
				NotifyPropertyChanged( nameof( StatusMessage ) );
	}
	
	// ...
}
```

また、ModelクラスにProReactsRev\<T\>クラスを継承して利用することもできます。
その場合、ProReactsCoreプロパティとReporterプロパティはModelクラスの外部に公開されます。

```csharp
using Nia_Tech.ModelExtentions;

// MVVMのModelクラス
class Model : ProReactsRev<string> {

	public Model : base() {	}
	
	public DoSomething() {
		Reporter.Report( "処理開始" );
		// ...
		Reporter.Report( "処理完了" );
	}
}

// MVVMのViewModelクラス
class ViewModel : NotifyPropertyChangedHelper {
	private model;
	public ProReactsCore<string> Status =>
		model.ProReactsCore;
		
	// ...
}
```


##◆ 免責条項

このライブラリを使用しことにより生じたいかなるトラブル・損害において、
作成者及びNia-Tech、Chronoir.netは一切の責任を負いかねます。あらかじめご了承ください。


##◆ リリースノート

* Ver. 1.0.0.1 : 2016/03/09  
　 - 初版リリース

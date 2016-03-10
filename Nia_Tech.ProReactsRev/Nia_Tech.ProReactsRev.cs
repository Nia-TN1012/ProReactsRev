#region バージョン情報
/**
*	@file
*	@brief MVVMパターンでの、モデルの進行状況情報自動通知システム「プロリアクトス・レヴ」
*
*		PROgress information REACTive notification System - REporting Value  : Pro-Reacts Rev	
*
*		外部からモデルの進行状況や結果の情報がレポートされた時、
*		その情報を格納するプロパティの更新とその通知を自動的に行う機能です。
*
*	@par 対応プラットフォーム
*	- .NET Framework 4.5 以上
*	@par バージョン Version
*	1.0.0
*	@par 作成者 Author
*	智中ニア（Nia Tomonaka）
*	@par コピーライト Copyright
*	Copyright (C) 2016 Nia Tomonaka
*	@par 作成日
*	2016/03/09
*	@par 最終更新日
*	2016/03/09
*	@par ライセンス license
*	MIT license
*	@par 連絡先 Contact
*	@@nia_tn1012（ https://twitter.com/nia_tn1012/ ）
*	@par ホームページ Homepage
*	- GitHub : https://github.com/Nia-TN1012/ProReactsRev/
*	@par リリースノート Release note
*	- 2016/03/09 Ver. 1.0.0
*		- NTC-00000 : 初版リリース
*/
#endregion
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nia_Tech.ModelExtentions {

	/// <summary>
	///		プロパティの変更通知するヘルパークラスを定義します。
	/// </summary>
	public abstract class NotifyPropertyChangedHelper : INotifyPropertyChanged {

		/// <summary>
		///		プロパティを変更した時に発生します。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///		PropertyChangedのイベントハンドラーを取得します。
		/// </summary>
		public PropertyChangedEventHandler PropertyChangedFromThis =>
			PropertyChanged;

		/// <summary>
		///		指定したプロパティの変更を通知します。
		/// </summary>
		/// <param name="propertyName">変更を通知するプロパティ名（ 省略時、呼び出し元のプロパティ名となります。）</param>
		protected virtual void NotifyPropertyChanged( [CallerMemberName]string propertyName = null ) {
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}

		/// <summary>
		///		指定したプロパティに値を設定し、そのプロパティの変更を通知します。
		/// </summary>
		/// <typeparam name="T">プロパティの型</typeparam>
		/// <param name="property">設定先のプロパティ</param>
		/// <param name="value">プロパティに設定する値</param>
		/// <param name="propertyName">プロパティの名前（ 省略時、呼び出し元のプロパティ名となります。）</param>
		protected virtual void SetProperty<T>( ref T property, T value, [CallerMemberName]string propertyName = null ) {
			property = value;
			NotifyPropertyChanged( propertyName );
		}
	}

	/// <summary>
	///		Modelの進行状況の情報を格納する、プロリアクトス・コアを定義します。
	/// </summary>
	/// <typeparam name="T">進行状況の情報を格納するオブジェクトの型</typeparam>
	public class ProReactsCore<T> : NotifyPropertyChangedHelper {

		/// <summary>
		///		現在のインスタンスを生成する時に付けた識別子を取得します。
		/// </summary>
		public string ID { get; private set; }

		/// <summary>
		///		進行状況の情報を格納したオブジェクトを表します。
		/// </summary>
		private T progressInfo;
		/// <summary>
		///		進行状況の情報を格納したオブジェクトを取得・設定します。
		/// </summary>
		/// <remarks>
		///		このプロパティを通してUIにデータバインディングすることで、
		///		プロリアクトス・レヴのReporter.Report から進行状況の情報をレポートした時に、
		///		このプロパティのセット側が呼び出されて値を格納するとともに、変更の通知を行います。
		///	</remarks>
		public T ProgressInfo {
			get { return progressInfo; }
			set {
				// プロパティに値をセットし、その変更を通知します。これにより、
				// プロリアクトス・レヴのProReactsCoreプロパティを通して
				// データバインディングしているUIの値に反映されます。
				SetProperty( ref progressInfo, value );
				// ProgressInfoChangedイベントを発生させます。
				ProgressInfoChanged?.Invoke( this, progressInfo );
			}
		}

		/// <summary>
		///		進行状況の情報が変更された時に発生します。
		/// </summary>
		/// <example>
		///		プロリアクトス・コアのイベント発生時、ViewModelで定義したプロリアクトス・コアの
		///		ProgressInfoプロパティの値を返すプロパティの変更を通知します。
		///		◆ ViewModel
		///			pulic T ProgressInfo =>
		///				ModelのProReactosCore.ProgressInfo;
		///				
		///			public ViewModel(){
		///				// Modelを初期化
		///				
		///				// ViewModelのProgressInfoプロパティの変更を通知し、バインド先のUIの値に反映させます。
		///				ModelのProReactosCore.ProgressInfoChanged +=
		///					PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( nameof( ProgressInfo ) ) );
		///			}
		/// </example>
		public event EventHandler<T> ProgressInfoChanged;

		/// <summary>
		///		識別子から、ProReactosCoreクラスの新しいインスタンスを生成します。
		/// </summary>
		/// <param name="id">識別子</param>
		public ProReactsCore( string id = null ) {
			ID = id;
			progressInfo = default( T );
		}
	}

	/// <summary>
	///		プロリアクトス・コアにモデルの進行状況の情報をレポートする、プロリアクトス・レヴ を定義します。
	/// </summary>
	/// <typeparam name="T">進行状況の情報を格納するオブジェクトの型</typeparam>
	public class ProReactsRev<T> : NotifyPropertyChangedHelper {

		/// <summary>
		///		現在のインスタンスのプロリアクトス・コアを取得します。
		/// </summary>
		/// <remarks>このプロパティ通してUIにバインディングしている場合、プロパティの変更通知は不要です。</remarks>
		public ProReactsCore<T> ProReactsCore { get; private set; }

		/// <summary>
		///		IProgressからレポートを受け取った時にコールバックする、Progressオブジェクト表します。
		/// </summary>
		private Progress<T> progress;

		/// <summary>
		///		プロリアクトス・レヴにレポートするためのIProgressオブジェクトを取得します。
		/// </summary>
		public IProgress<T> Reporter =>
			progress;

		/// <summary>
		///		識別子から、ProReactsRevクラスの新しいインスタンスを生成します。
		/// </summary>
		/// <param name="id">識別子</param>
		public ProReactsRev( string id = null ) {
			// 識別子を指定し、プロリアクトス・コアを初期化します。
			ProReactsCore = new ProReactsCore<T>( id );

			// Progress<T>を初期化します。
			progress = new Progress<T>(
				// プロリアクトス・レヴのReporter.Reportからレポートを受け取った時に、実行する処理を構成します。
				// パラメーター「e」にレポートされた進行状況の情報が格納されています。
				e =>
					// プロリアクトス・コアのProgressInfoプロパティに、進行状況の情報を格納したオブジェクトを代入します。
					ProReactsCore.ProgressInfo = e
			);
		}
	}
}
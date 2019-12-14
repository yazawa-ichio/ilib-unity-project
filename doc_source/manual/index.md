# マニュアル

## 概要

個人開発用に作っていた自分用ライブラリをパッケージ化したものです。<br>
複数のプロジェクトで使うためUPM化したかったため公開しました。<br>
<br>
ライセンスは各パッケージのLICENSEファイルを確認してください。<br>
他のライセンスに依存しない限り、基本的に[NYSL](https://github.com/yazawa-ichio/ilib-unity-project/blob/master/LICENSE)を採用する方針のため煮るなり焼くなり好きにご自由にしてください。<br>

## 導入方法

リポジトリから直接落としてきて入れることも出来ますがUPM経由での導入をサポートしています。<br>
UPM経由で導入する場合、UnityプロジェクトのPackages以下にあるmanifest.jsonに以下のように追記してください。<br>
Unity2019.3以降はPackageManagerUIの方から直接リポジトリを指定できます。<br>

manifest.json
```json
{
  "dependencies": {
~~~
    "com.unity.assetbundlebrowser": "1.7.0",
    "jp.yazawa-ichio.ilib-abloader": "https://github.com/yazawa-ichio/ilib-abloader.git",
    "com.unity.modules.assetbundle": "1.0.0",
~~~
  }
}
```

### バージョンを指定する
インストールするバージョンを指定する場合はリポジトリのURLの後ろに `#0.1.0` のようにタグを指定します。<br>

manifest.json
```json
    "jp.yazawa-ichio.ilib-abloader": "https://github.com/yazawa-ichio/ilib-abloader.git#0.1.0",
```

## 一覧

### [ilib-unity-etc](ilib-unity-etc/index.md) ([リポジトリ](https://github.com/yazawa-ichio/ilib-unity-etc))

ゲームの全体管理やUI管理、簡易なステートマシーンなど、プロジェクトに必要な小物が入ったパッケージです。<br>
色々は入っているのはUPMのパッケージが依存関係を解決してくれないので、依存関係があるパッケージを全部突っ込んでいます。<br>
依存関係が解決してくれるようになれば、おそらく分割します。

### [ilib-mvvm](ilib-mvvm/index.md) ([リポジトリ](https://github.com/yazawa-ichio/ilib-mvvm))

UnityでMVVMでUIのバインディングを行うパッケージです。<br>
リファレクションの実行がボキシングがないため比較的軽量に動作します。

### [ilib-ui-control](ilib-ui-control/index.md) ([リポジトリ](https://github.com/yazawa-ichio/ilib-ui-control))

UnityでUIをスタックやキューで管理できるパッケージです。  
1から実装するよりは少し便利なぐらいの機能です。  

### [ilib-abloader](ilib-abloader/index.md) ([リポジトリ](https://github.com/yazawa-ichio/ilib-abloader))

AssetBundleのロード管理とファイルサイズ等を収集する簡単なビルドパイプラインのパッケージです。<br>
Updateを利用しない縛りで作ってみたが、アンロード周りの自動解放でUpdateを使ってしまった中途半端なヤツです。<br>
パフォーマンスを切り詰めてませんが、実務レベルで利用するのに必要なAPIは最低限揃っているかと思います。

### [ilib-serv-injector](ilib-serv-injector/index.md) ([リポジトリ](https://github.com/yazawa-ichio/ilib-serv-injector))

シンプルなサービスロケーターと最低限のDIを行えるパッケージです。<br>
参照関係の整理やテストを行いやすくするためにDIを使用したいが、巨大なDIコンテナは導入したくないケースに有効です。<br>
Interfaceによる注入では型解決するため比較的高速に動作します。

### [ilib-unity-save](ilib-unity-save/index.md) ([リポジトリ](https://github.com/yazawa-ichio/ilib-unity-save))

Unity上でPlayerPrefs等によるセーブ時にキーの難読化や文字列の暗号化等を行うパッケージです。<br>
プロジェクト毎に実装する方が早いぐらいの奴です。自分用。

### [ilib-sound-control](ilib-sound-control/index.md) ([リポジトリ](https://github.com/yazawa-ichio/ilib-sound-control))

オレオレサウンドマネージャーを作りやすくするためSEやBGMの発音制御などが入ったパッケージです。<br>
リファレンス実装としてのサウンドマネージャーも入っています。<br>
3Dサウンドは未対応です。

### [ilib-scwidgets](ilib-scwidgets/index.md) ([リポジトリ](https://github.com/yazawa-ichio/ilib-scwidgets))

IMGUIで使える簡単なウィジェットツールです。<br>
多少見た目がいいスキンを利用もできます。<br>
内部的にウィジェットを利用したランタイム用のデバックメニューも含みます。

### [ilib-unity-cl](ilib-unity-cl/index.md) ([リポジトリ](https://github.com/yazawa-ichio/ilib-unity-cl))

Unityで利用するコマンドラインのヘルパーとツールです。<br>
今のところ引数のバインダーの機能しかありません。
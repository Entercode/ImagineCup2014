Himagine
========

Azure
-----

ファイル内訳

* packages
 > NuGetパッケージによるdllがあるフォルダ

* SynapseServer
 > Windows Azule クラウドサービスの環境設定フォルダ

* PageRole
 > ASP.NETのウェブページプロジェクト 新規登録とログインで使用

* WorkerRole
 > バックグラウンドで動く処理のプロジェクト 近い人の情報の受け渡しで使用

* SynapseServerHelper
 > 雑多なヘルパークラス 定数定義とSQL処理のシンタックスシュガー

ページ一覧

* 新規登録
 > http://synapse-server.cloudapp.net/SignUp.aspx

* ログイン
 > http://synapse-server.cloudapp.net/Login.aspx

* ログアウト
 > http://synapse-server.cloudapp.net/Logout.aspx

* すれ違い情報送信(ログイン中)
 > http://synapse-server.cloudapp.net:4724/

* データ閲覧(POSTでない)
 > http://synapse-server.cloudapp.net/test/data.aspx
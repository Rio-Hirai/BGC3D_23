# BGC3D_23
## 概要
IPLAB視線班の3次元Bubble Gaze Cursorの実験アプリケーション（の公開バージョン）
  
主な中身はAssets/Gaze_Team

- Materials：カーソル類のマテリアルが入っている
- Scenes：実験環境のシーンが入っている
- Scripts：各種Scriptが入っている（実験システムの肝）
- Sources：各種効果音が入っている

## 環境
Windows11(22H2) + Unity 2023.1.8f1 + Vive Pro Eye + SRanipal Runtime 1.3.2.0

※Windows11(23H2)でも動作可能

## 使用法
実験環境にあたるシーンは，Assets/Gaze_Team/BGC3D/Scenesの「BGC3D_2023_v2.unity」

serverオブジェクトのinspectorにあるReceiverスクリプトから実験条件を設定する（詳細は該当するScriptのコメントを参照）

主な設定項目は以下の通り，
- 被験者名
- 被験者ID
- 使用手法（実験手法）
- 配置条件

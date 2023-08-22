# BGC3D_23
## 概要
IPLAB視線班の3次元Bubble Gaze Cursorの実験アプリケーション（の公開バージョン）
  
主な中身はAssets/Gaze_Team

- Materials：カーソル類のマテリアルが入っている
- Scenes：実験環境のシーンが入っている
- Scripts：各種Scriptが入っている（実験システムの肝）
- Sources：各種効果音が入っている

## 環境
Windows11(22H2) + Unity 2023.1.9f1 + Vive Pro Eye + SRanipal Runtime 1.3.2.0

※Windows11(23H2)でも動作可能

## 使用法
実験環境にあたるシーンは，Assets/Gaze_Team/BGC3D/Scenesの「BGC3D_2023_v2.unity」

serverオブジェクトのinspectorにあるreceiverスクリプトから実験条件を設定する（詳細は該当するScriptのコメントを参照）

主な設定項目は以下の通り，
- 被験者名
- 被験者ID
- 使用手法（実験手法）
- 配置条件

## 使用手法の種類
- Bubble Gaze Cursor with FocusPoint（3次元焦点座標を用いたBubble Gaze Cursor）
- Bubble Gaze Cursor with FocusPoint + Raycast with Gaze
- Bubble Gaze Cursor 3D（視線を用いたBubble Gaze Cursor）
- Bubble Gaze Cursor 3D + Raycast with Gaze
- Raycast with Gaze（視線を用いたRaycast）
- Raycast with Controller（コントローラを用いたRaycast）

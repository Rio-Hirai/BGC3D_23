# BGC3D_23
## 概要
IPLAB視線班の3次元Bubble Gaze Cursorの実験アプリケーション（の公開バージョン）
  
主な中身はAssets/Gaze_Team

- Materials：カーソル類のマテリアルが入っている
- Scenes：実験環境のシーンが入っている
- Scripts：各種Scriptが入っている（実験システムの肝）
- Sources：各種効果音が入っている

## 環境
Windows11(22H2) + Unity 2023.1.9f1 + Vive Pro Eye + SRanipal Runtime 1.3.2.0<br>
※Windows11(23H2)でも動作可能

## 使用法
実験環境にあたるシーンは，Assets/Gaze_Team/BGC3D/Scenesの「BGC3D_2023_v2.unity」<br>
serverオブジェクトのinspectorにあるreceiverスクリプトから実験条件を設定する（詳細は該当するScriptのコメントを参照）<br>
<br>
主な設定項目は以下の通り，
- 被験者名
- 被験者ID
- 使用手法（実験手法）
- 配置条件

## 使用手法
### 一覧
- Bubble Gaze Cursor with FocusPoint（Bubble_Gaze_Cursor1）
- Bubble Gaze Cursor with FocusPoint + Raycast with Gaze（Bubble_Gaze_Cursor2）
- Bubble Gaze Cursor 3D（Bubble_Gaze_Cursor3）
- Bubble Gaze Cursor 3D + Raycast with Gaze（Bubble_Gaze_Cursor_with_Gaze_Ray）
- Bubble Gaze Cursor 3D + nod（未実装）
- Raycast with Gaze（Gaze_Raycast）
- Bubble Gaze Cursor 3D + nod（未実装）
- Raycast with Controller（Controller_Raycast）

### Bubble Gaze Cursor with FocusPoint
3次元焦点座標を用いたBubble Gaze Cursor<br>
3次元焦点に最も近いターゲットが注視・選択される<br>
デフォルトの連続注視時間は1.0秒<br>
※正常に動かない可能性あり<br>

### Bubble Gaze Cursor with FocusPoint + Raycast with Gaze
3次元焦点座標を用いたBubble Gaze Cursor + 視線を用いたRaycast<br>
視線のRayが当たっているターゲットに対してサッケード運動に対するフィルタのしきい値を上げることで注視を逸れづらくする<br>
デフォルトの連続注視時間は1.0秒<br>
※正常に動かない可能性あり<br>

### Bubble Gaze Cursor 3D
視線を用いたBubble Gaze Cursor<br>
視線のレイに最も近いターゲットが注視・選択される<br>
デフォルトの連続注視時間は1.0秒<br>
※本命手法<br>

### Bubble Gaze Cursor 3D + Raycast with Gaze
視線を用いたBubble Gaze Cursor + 視線を用いたRaycast<br>
Bubble Gaze Cursorで注視状態になったターゲットに対して視線のRayを当てることで連続注視時間を重複的に加算する<br>
デフォルトの連続注視時間は1.0秒<br>
※本命手法<br>

### Bubble Gaze Cursor 3D + nod（未実装）
視線を用いたBubble Gaze Cursor + 頷きによる選択<br>

### Raycast with Gaze
視線を用いたRaycast<br>
デフォルトの連続注視時間は1.0秒<br>

### Bubble Gaze Cursor 3D + nod（未実装）
視線を用いたRaycast + 頷きによる選択<br>

### Raycast with Controller
コントローラを用いたRaycast<br>

## 配置条件
### 一覧
- 高密度条件（High_Density）
- 高オクルージョン条件（High_Occlusion）
- 密度＆オクルージョン（Density_and_Occlusion）
- 通常条件（Density_and_Occlusion2）
- テスト条件（TEST）
- ランダム条件（Random）
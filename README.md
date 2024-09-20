# Copy Scale Adjuster
アバター本体にあるMA Scale Adjusterを衣装側のボーンにコピーするツール  
MA側で正式実装されるまでの繋ぎに使ってください

## ダウンロード
- [VCC](https://rerigferl.github.io/vpm/)
- [UnityPackage](https://github.com/Rerigferl/modular-avatar-copy-scale-adjuster/releases/)

## 使い方

導入後、右クリックメニューの`Modular Avatar`の項目内に`Setup Outfit with Copy Scale Adjuster`が追加されます  
![image](https://github.com/user-attachments/assets/d322dbf2-5f9a-4600-808f-5339da26bae3)
- Setup Outfitを実行した後に、アバター側のScale Adjusterを検索して値をコピーします
  - 検索はパス一致（接頭辞、接尾辞は無視）で行われるため、ボーン名が一致していないとコピーされないことが多いです
  - コピー先に既にScale Adjusterが存在する場合は値が上書きされます
----

MA Merge Armatureが付いているオブジェクト、またはMA Merge Armatureのコンテキストメニューで`Copy Scale Adjuster`を選択することでもコピーが行われます  
![image](https://github.com/user-attachments/assets/fbcee695-5deb-410e-b24f-b314bc2aeb13)
- アバター側のScale Adjusterの値を衣装側に同期したい場合はこちらの方が便利です（多分）

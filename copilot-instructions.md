## copilot向けの指示

- 説明の際は、まずサンプルコードを書き、その後に説明を書いてください。

## マイコーディングルール

- Assembly.GetCallingAssembly()、Assembly.GetExecutingAssembly()、Assembly.GetEntryAssembly()は.NETの単一exe発行時に機能しなくなるため使用禁止。
	- これらが使用されていることを検知した場合は警告を出し、代替案を提案してください。

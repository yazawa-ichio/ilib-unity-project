using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using ILib.AssetBundles;
using UnityEngine.Assertions;
using ABLog = ILib.AssetBundles.Logger.Log;
using System.Threading.Tasks;

#if UNITY_EDITOR
public class EditorTest : AssetBundleTest
{
	protected override bool IsEditorTest() { return true; }
	protected override ILoadOperator LoadOperator(bool useExManifet)
	{
		UseExManifest = false;
		return new InternalLoadOperator("", "");
	}
}
#endif

public class InternalTest : AssetBundleTest
{
	protected override ILoadOperator LoadOperator(bool useExManifet)
	{
		string manifest = UseExManifest ? "manifest" : "StandaloneWindows64";
		string assetName = UseExManifest ? "ExtensionManifestAsset" : "AssetBundleManifest";
		return new InternalLoadOperator(Application.streamingAssetsPath, manifest, assetName);
	}
}

public class SimulateDownloadTest : AssetBundleTest
{
	protected override bool UseCache() { return true; }
	protected override ILoadOperator LoadOperator(bool useExManifet)
	{
		string path = "file://" + Application.streamingAssetsPath;
		string cache = Path.Combine(Application.temporaryCachePath, "AssetBundles");
		string manifest = UseExManifest ? "manifest" : "StandaloneWindows64";
		string assetName = UseExManifest ? "ExtensionManifestAsset" : "AssetBundleManifest";
		return new NetworkLoadOperator(path, cache, manifest, "", assetName);
	}
}


public abstract class AssetBundleTest
{
	protected abstract ILoadOperator LoadOperator(bool useExManifet);
	protected virtual bool UseCache() { return false; }
	protected virtual bool IsEditorTest() { return false; }
	protected bool UseExManifest { get; set; }

	IEnumerator Init(bool clearCache = true, bool useExManifet = false)
	{
		yield return ABLoader.Stop();
#if UNITY_EDITOR
		ABLoader.UseEditorAsset = IsEditorTest();
#endif
		UseExManifest = useExManifet;
		//エラーのログを有効化
		ABLog.Level = ABLog.LogLevel.Debug;
		ABLog.Enabled = true;
		ABLog.EnabledAssert = true;
		ABLoader.UnloadMode = UnloadMode.Immediately;
		AutoUnloader.UnloadCycle = 2f;
		AutoUnloader.Pause = false;

		ABLoader.MaxDownloadCount = 5;
		ABLoader.MaxLoadCount = 10;

		var loadOperator = LoadOperator(useExManifet);
		if (UseCache() && clearCache && Directory.Exists(loadOperator.GetCacheRoot()))
		{
			Directory.Delete(loadOperator.GetCacheRoot(), true);
		}
		yield return ABLoader.Initialize(loadOperator, null, (ex) => throw ex);
	}

	bool LoadedTest(string name, bool expected)
	{
		if (IsEditorTest()) return expected;
		bool actual = false;
		foreach (var bundle in AssetBundle.GetAllLoadedAssetBundles())
		{
			if (bundle.name == name)
			{
				actual = true;
				break;
			}
		}
		Assert.AreEqual(expected, actual);
		return actual;
	}

	string GetCachePath(string name)
	{
		if (!UseCache()) return "";
		var op = LoadOperator(false);
		var rootPath = op.GetCacheRoot();
		var checkPath = op.LoadPath(name, "");
		var dirPath = Path.GetDirectoryName(checkPath);
		var checkName = Path.GetFileName(checkPath);
		if (Directory.Exists(dirPath))
		{
			foreach (var path in Directory.GetFiles(dirPath, checkName + "*", SearchOption.TopDirectoryOnly))
			{
				if (Path.GetFileName(path).StartsWith(checkName))
				{
					return path;
				}
			}
		}
		return "";
	}

	bool CacheTest(string name, bool expected)
	{
		if (!UseCache()) return expected;
		bool actual = !string.IsNullOrEmpty(GetCachePath(name));
		Assert.AreEqual(expected, actual);
		return actual;
	}

	[UnityTest]
	public IEnumerator LoadTest1()
	{
		//ロードテスト
		yield return Init();
		BundleContainerRef containerRef = null;
		System.Exception exception = null;
		ABLoader.LoadContainer("prefabs", (c) => containerRef = c, e => exception = e);
		yield return new WaitWhile(() => containerRef == null && exception == null);
		Debug.Assert(containerRef != null);
		Debug.Assert(exception == null, exception);
		Debug.Assert(containerRef.LoadAsset<GameObject>("Cube") != null);
		Debug.Assert(containerRef.LoadAsset<GameObject>("Cylinder") != null);
		Debug.Assert(containerRef.LoadAsset<GameObject>("Dummy") == null);
		containerRef?.Dispose();

		//キャッシュテスト
		yield return Init(false);
		//ダウンロードさせない
		ABLoader.MaxDownloadCount = 0;
		//キャッシュがある場合はダウンロードしない
		containerRef = null;
		exception = null;
		ABLoader.LoadContainer("prefabs", (c) => containerRef = c, e => exception = e);
		yield return new WaitWhile(() => containerRef == null && exception == null);
		Debug.Assert(containerRef != null);
		Debug.Assert(exception == null, exception);
		Debug.Assert(containerRef.LoadAsset<GameObject>("Cube") != null);

	}

	[UnityTest]
	public IEnumerator LoadTest2()
	{
		//成功コールバック内でDisposeしても正常にリファレスカウントが正常に動く
		yield return Init();
		ABLoader.LoadContainer("prefabs", (c) => c.Dispose(), ex => throw ex);
		ABLoader.LoadContainer("prefabs", (c) => c.Dispose(), ex => throw ex);
		ABLoader.LoadContainer("prefabs", (c) => c.Dispose(), ex => throw ex);
		BundleContainerRef containerRef = null;
		ABLoader.LoadContainer("prefabs", (c) => containerRef = c, ex => throw ex);
		yield return new WaitWhile(() => containerRef == null);
		Debug.Assert(containerRef.LoadAsset<GameObject>("Cube") != null);

		//ロード済みの際は即コールバックが実行される
		bool loaded = false;
		ABLoader.LoadContainer("prefabs", (c) => { c.Dispose(); loaded = true; }, ex => throw ex);
		Assert.IsTrue(loaded);

		containerRef.Dispose();
	}

	[UnityTest]
	public IEnumerator LoadTest3()
	{
		//参照カウントが切れた際にアンロードされる
		yield return Init();
		BundleContainerRef containerRef = null;
		ABLoader.LoadContainer("materials/testmaterial", (c) => c.Dispose(), ex => throw ex);
		ABLoader.LoadContainer("materials/testmaterial", (c) => containerRef = c, ex => throw ex);
		yield return new WaitWhile(() => containerRef == null);
		Debug.Assert(containerRef.LoadAsset<Material>("TestMaterial") != null);
		LoadedTest("materials/testmaterial", true);
		containerRef.Dispose();
		LoadedTest("materials/testmaterial", false);

		//何度Disposeを実行しても有効なのは1度だけ
		containerRef = null;
		ABLoader.LoadContainer("materials/testmaterial", (c) => containerRef = c, ex => throw ex);
		ABLoader.LoadContainer("materials/testmaterial", (c) => { c.Dispose(); c.Dispose(); c.Dispose(); }, ex => throw ex);
		yield return new WaitWhile(() => containerRef == null);
		LoadedTest("materials/testmaterial", true);
		containerRef.Dispose();
		LoadedTest("materials/testmaterial", false);

	}

	[UnityTest]
	public IEnumerator LoadTest4()
	{
		//ロードのコールバックで例外が出た際は内部でDisposeが実行される
		yield return Init();

		//ログの無効化
		ABLog.Enabled = false;

		//単発だと即時解放
		ABLoader.LoadContainer("materials/testmaterial", (c) => throw new System.Exception("test error"), ex => throw ex);
		LoadedTest("materials/testmaterial", false);

		//複数のコールバックでも正常に動く
		BundleContainerRef containerRef = null;
		ABLoader.LoadContainer("materials/testmaterial", (c) => containerRef = c, ex => throw ex);
		ABLoader.LoadContainer("materials/testmaterial", (c) => throw new System.Exception("test error"), ex => throw ex);
		yield return new WaitWhile(() => containerRef == null);
		Debug.Assert(containerRef.LoadAsset<Material>("TestMaterial") != null);
		LoadedTest("materials/testmaterial", true);
		containerRef.Dispose();
		LoadedTest("materials/testmaterial", false);
	}

	[UnityTest]
	public IEnumerator LoadTest5()
	{
		//アセットを直接ロードする
		yield return Init();
		GameObject asset = null;
		ABLoader.LoadAsset<GameObject>("prefabs", "Cube", (x) => asset = x, ex => throw ex);
		yield return new WaitWhile(() => asset == null);
		Assert.IsNotNull(asset);
		//勝手にアンロードされる
		LoadedTest("materials/testmaterial", false);

		//非同期でも問題ない
		asset = null;
		ABLoader.LoadAssetAsync<GameObject>("prefabs", "Cylinder", (x) => asset = x, ex => throw ex);
		yield return new WaitWhile(() => asset == null);
		Assert.IsNotNull(asset);
		LoadedTest("materials/testmaterial", false);

		//同じバンドルを二つ同時に呼んでも大丈夫
		asset = null;
		GameObject asset2 = null;
		ABLoader.LoadAsset<GameObject>("prefabs", "Cube", (x) => asset = x, ex => throw ex);
		ABLoader.LoadAssetAsync<GameObject>("prefabs", "Cylinder", (x) => asset2 = x, ex => throw ex);
		yield return new WaitWhile(() => asset == null || asset2 == null);
		Assert.IsNotNull(asset);
		Assert.IsNotNull(asset2);
		LoadedTest("materials/testmaterial", false);

	}

	[UnityTest]
	public IEnumerator LoadTest6()
	{
		if (!UseCache()) yield break;
		//キャッシュ削除
		yield return Init();
		GameObject asset = null;
		ABLoader.LoadAsset<GameObject>("prefabs", "Cube", (x) => asset = x, ex => throw ex);
		yield return new WaitWhile(() => asset == null);
		Assert.IsNotNull(asset);
		CacheTest("prefabs", true);
		CacheTest("materials/testmaterial", true);
		CacheTest("_dummy", false);
		yield return ABLoader.CacheClear();
		//キャッシュが削除されている
		CacheTest("prefabs", false);
		CacheTest("materials/testmaterial", false);
	}


	void LoadSprites(List<Sprite> assets)
	{
		ABLoader.LoadAsset<Sprite>("sprites/circle", "Circle", (x) => assets.Add(x), ex => throw ex);
		ABLoader.LoadAsset<Sprite>("sprites/diamond", "Diamond", (x) => assets.Add(x), ex => throw ex);
		ABLoader.LoadAsset<Sprite>("sprites/hexagon", "Hexagon", (x) => assets.Add(x), ex => throw ex);
		ABLoader.LoadAsset<Sprite>("sprites/polygon", "Polygon", (x) => assets.Add(x), ex => throw ex);
		ABLoader.LoadAsset<Sprite>("sprites/triangle", "Triangle", (x) => assets.Add(x), ex => throw ex);
	}

	[UnityTest]
	public IEnumerator LoadTest7()
	{
		if (!UseCache()) yield break;
		//ロード並列数の制御
		yield return Init();

		ABLoader.MaxDownloadCount = 0;
		ABLoader.MaxLoadCount = 0;

		List<Sprite> assets = new List<Sprite>();
		LoadSprites(assets);

		yield return new WaitForSeconds(2f);

		//キャッシュされていない
		CacheTest("sprites/circle", false);
		CacheTest("sprites/diamond", false);
		CacheTest("sprites/hexagon", false);
		CacheTest("sprites/polygon", false);
		CacheTest("sprites/triangle", false);
		Assert.AreEqual(assets.Count, 0);

		//先頭から2つ分キャッシュが詰まれる
		ABLoader.MaxDownloadCount = 2;
		//残りはダウンロードさせない
		ABLoader.MaxDownloadCount = 0;

		yield return new WaitForSeconds(2f);

		//2つはキャッシュされている
		CacheTest("sprites/circle", true);
		CacheTest("sprites/diamond", true);
		//キャッシュされていない
		CacheTest("sprites/hexagon", false);
		CacheTest("sprites/polygon", false);
		CacheTest("sprites/triangle", false);
		//ロードはされない
		Assert.AreEqual(assets.Count, 0);

		//ロード数を増やす
		ABLoader.MaxLoadCount = 10;

		//ロード待ち
		yield return new WaitWhile(() => assets.Count < 2);

		//ダウンロードさせる
		ABLoader.MaxDownloadCount = 5;

		yield return new WaitWhile(() => assets.Count < 5);

		//キャッシュされた
		CacheTest("sprites/hexagon", true);
		CacheTest("sprites/polygon", true);
		CacheTest("sprites/triangle", true);

	}

	[UnityTest]
	public IEnumerator LoadTest8()
	{
		//停止処理
		yield return Init();

		//実行待ちが出るぐらいに調整
		ABLoader.MaxDownloadCount = 3;
		ABLoader.MaxLoadCount = 3;

		List<Sprite> assets = new List<Sprite>();
		LoadSprites(assets);

		//ダウンロードリクエスト後即中断
		yield return ABLoader.Stop();

		//ゴミがないか？
		CacheTest("sprites/circle", false);
		CacheTest("sprites/diamond", false);
		CacheTest("sprites/hexagon", false);
		assets.Clear();

		yield return Init();

		assets.Clear();
		LoadSprites(assets);

		//少し進めてから止める
		yield return new WaitForSeconds(0.01f);
		yield return ABLoader.Stop();

		yield return Init();

		//ダウンロードだけさせておく
		ABLoader.MaxDownloadCount = 5;
		ABLoader.MaxLoadCount = 0;

		assets.Clear();
		LoadSprites(assets);

		yield return new WaitForSeconds(2f);

		CacheTest("sprites/circle", true);
		CacheTest("sprites/diamond", true);
		CacheTest("sprites/hexagon", true);
		CacheTest("sprites/polygon", true);
		CacheTest("sprites/triangle", true);

		//ロード途中に停止する
		ABLoader.MaxLoadCount = 3;
		yield return new WaitForSeconds(0.1f);
		yield return ABLoader.Stop();

		yield return new WaitForSeconds(2f);

		//正常に破棄されている
		LoadedTest("sprites/circle", false);
		LoadedTest("sprites/diamond", false);
		LoadedTest("sprites/hexagon", false);
	}

	[UnityTest]
	public IEnumerator LoadTest9()
	{
		if (IsEditorTest()) yield break;
		//unloadAllフラグを立てるとアセットもアンロードされる
		yield return Init();
		BundleContainerRef containerRef = null;
		Material mat = null;
		ABLoader.LoadAsset<Material>("materials/testmaterial", "TestMaterial", (x) => mat = x, ex => throw ex);
		ABLoader.LoadContainer("materials/testmaterial", (c) => containerRef = c, ex => throw ex);
		yield return new WaitWhile(() => containerRef == null);
		Assert.IsNotNull(mat);
		containerRef.SetUnloadAll(true);
		containerRef.Dispose();
		Assert.IsNull(mat);

		//依存関係も設定した場合
		containerRef = null;
		mat = null;
		ABLoader.LoadAsset<Material>("materials/testmaterial", "TestMaterial", (x) => mat = x, ex => throw ex);
		ABLoader.LoadContainer("prefabs", (c) => containerRef = c, ex => throw ex);
		yield return new WaitWhile(() => containerRef == null);
		Assert.IsNotNull(mat);
		containerRef.SetUnloadAll(true, true);
		containerRef.Dispose();
		Assert.IsNull(mat);


		//停止処理でアンロードされた場合
		containerRef = null;
		mat = null;
		ABLoader.LoadAsset<Material>("materials/testmaterial", "TestMaterial", (x) => mat = x, ex => throw ex);
		ABLoader.LoadContainer("prefabs", (c) => containerRef = c, ex => throw ex);
		yield return new WaitWhile(() => containerRef == null);
		Assert.IsNotNull(mat);
		containerRef.SetUnloadAll(true, true);

		//Disposeしていないコンテナを確保
		var nonDisposeRef = containerRef;
		yield return ABLoader.Stop();
		Assert.IsNull(mat);

		yield return Init();

		//後で設定したほうが優先される
		containerRef = null;
		mat = null;
		ABLoader.LoadAsset<Material>("materials/testmaterial", "TestMaterial", (x) => mat = x, ex => throw ex);
		ABLoader.LoadContainer("prefabs", (c) => containerRef = c, ex => throw ex);
		yield return new WaitWhile(() => containerRef == null);
		Assert.IsNotNull(mat);
		containerRef.SetUnloadAll(true, true);
		containerRef.SetUnloadAll(false, true);
		containerRef.Dispose();
		Assert.IsNotNull(mat);

		//コンテナの有効期間テスト
		containerRef = null;
		mat = null;
		ABLoader.LoadAsset<Material>("materials/testmaterial", "TestMaterial", (x) => mat = x, ex => throw ex);
		ABLoader.LoadContainer("prefabs", (c) => containerRef = c, ex => throw ex);
		yield return new WaitWhile(() => containerRef == null);
		Assert.IsNotNull(mat);
		Assert.IsFalse(nonDisposeRef.Disposed);
		nonDisposeRef.SetUnloadAll(true);
		nonDisposeRef.Dispose();

		//停止前のコンテナが無効化されている
		LoadedTest("prefabs", true);
		containerRef.Dispose();
		LoadedTest("prefabs", false);
		Assert.IsNotNull(mat);

	}


	[UnityTest]
	public IEnumerator LoadTest10()
	{
		if (!UseCache()) yield break;

		//破損ファイルのロード
		yield return Init();

		//ロード無効
		ABLoader.MaxLoadCount = 0;

		System.Exception error = null;
		ABLoader.LoadContainer("prefabs", (c) => c.Dispose(), ex => error = ex);
		yield return new WaitForSeconds(1);
		CacheTest("prefabs", true);
		File.Delete(GetCachePath("prefabs"));

		//キャッシュがない状態でロード
		ABLoader.MaxLoadCount = 1;
		yield return new WaitWhile(() => error == null);
		Assert.IsNotNull(error);

		ABLoader.MaxLoadCount = 0;
		error = null;
		ABLoader.LoadAsset<GameObject>("prefabs", "Cylinder", null, ex => error = ex);

		yield return new WaitForSeconds(1);
		CacheTest("prefabs", true);

		//データ改ざん
		File.WriteAllText(GetCachePath("prefabs"), "error data");

		ABLoader.MaxLoadCount = 1;
		yield return new WaitWhile(() => error == null);
		Assert.IsNotNull(error);
		//不正なキャッシュは消されている
		CacheTest("prefabs", false);

	}


	[UnityTest]
	public IEnumerator LoadTest11()
	{
		if (!UseCache()) yield break;

		//一括ダウンロード
		yield return Init();
		string[] names = new string[]
		{
			"prefabs",
			"testscene",
			"sprites/circle",
			"sprites/diamond",
			"sprites/hexagon",
			"sprites/polygon",
			"sprites/triangle",
		};


		//prefabsはtestmaterialに依存している
		Assert.AreEqual(ABLoader.GetSize(names, ignoreDpend: true), names.Length);
		Assert.AreEqual(ABLoader.GetSize(names), names.Length + 1);

		Assert.IsTrue(ABLoader.GetSize(names) == ABLoader.GetDownloadSize(names));

		bool loaded = false;
		ABLoader.LoadAsset<Sprite>("sprites/circle", "Circle", (x) => loaded = true, ex => throw ex);
		yield return new WaitWhile(() => loaded);

		//一つダウンロードされたので1減る
		Assert.AreEqual(ABLoader.GetSize(names) - ABLoader.GetDownloadSize(names), 1);

		ABLoader.MaxDownloadCount = 0;

		loaded = false;
		var progress = ABLoader.Download(names, ret => loaded = ret, ex => throw ex);

		Assert.AreEqual(progress(), 0);

		ABLoader.MaxDownloadCount = 1;

		float lastProgress = 0;
		//進捗が取れる
		while (!loaded)
		{
			var p = progress();
			Assert.IsTrue(lastProgress <= p);
			lastProgress = p;
			Debug.LogFormat("download progress {0}", p);
			yield return null;
		}
		Assert.AreEqual(progress(), 1);

		//キャッシュはされているがロードはされていない
		foreach (var name in names)
		{
			CacheTest(name, true);
			LoadedTest(name, false);
		}


	}


	[UnityTest]
	public IEnumerator LoadTest12()
	{
		if (!UseCache()) yield break;

		//一括ダウンロードエラーハンドリング
		yield return Init();
		string[] names = new string[]
		{
			"prefabs",
			"testscene",
			"sprites/circle",
			"sprites/diamond",
			"sprites/hexagon",
			"sprites/polygon",
			"sprites/triangle",
			"error_data1",
			"error_data2",
		};

		ABLog.EnabledAssert = false;
		bool complete = false;
		bool ret = false;
		List<System.Exception> errors = new List<System.Exception>();
		var progress = ABLoader.Download(names, r => { ret = r; complete = true; }, ex => errors.Add(ex));

		yield return new WaitWhile(() => !complete);

		//全体は失敗
		Assert.IsFalse(ret);
		//エラーは2個
		Assert.AreEqual(errors.Count, 2);

		foreach (var name in names)
		{
			bool error = name.Contains("error");
			CacheTest(name, !error);
			LoadedTest(name, false);
		}

	}

	[UnityTest]
	public IEnumerator LoadTest14()
	{
		//オートアンロードテスト
		yield return Init();
		ABLoader.UnloadMode = UnloadMode.Auto;
		AutoUnloader.UnloadCycle = 0;
		BundleContainerRef containerRef = null;
		ABLoader.LoadContainer("materials/testmaterial", (c) => c.Dispose(), ex => throw ex);
		ABLoader.LoadContainer("materials/testmaterial", (c) => containerRef = c, ex => throw ex);
		yield return new WaitWhile(() => containerRef == null);
		Debug.Assert(containerRef.LoadAsset<Material>("TestMaterial") != null);
		LoadedTest("materials/testmaterial", true);
		containerRef.Dispose();
		//オートモードでは即時解放されない
		LoadedTest("materials/testmaterial", true);
		yield return null;
		yield return null;
		//次のフレームで解放される
		LoadedTest("materials/testmaterial", false);

		//Unloaderを一旦無効にする
		AutoUnloader.Pause = true;
		System.WeakReference reference = null;
		//GCで回収される
		ABLoader.LoadContainer("materials/testmaterial", (c) => reference = new System.WeakReference(c), ex => throw ex);
		yield return new WaitWhile(() => reference == null);
		LoadedTest("materials/testmaterial", true);
		while (reference.IsAlive)
		{
			yield return null;
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
			yield return null;
		}
		LoadedTest("materials/testmaterial", true);
		//手動で解放する
		ABLoader.Unload();
		LoadedTest("materials/testmaterial", false);


		reference = null;

		//GCで解放した後、再ダウンロードしても正常に動く
		ABLoader.LoadContainer("materials/testmaterial", (c) => reference = new System.WeakReference(c), ex => throw ex);
		yield return new WaitWhile(() => reference == null);
		LoadedTest("materials/testmaterial", true);
		while (reference.IsAlive)
		{
			yield return null;
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
			yield return null;
		}
		LoadedTest("materials/testmaterial", true);
		yield return null;
		//実際にはアンロードされていないので即時ロードされる
		ABLoader.LoadContainer("materials/testmaterial", (c) => containerRef = c, ex => throw ex);
		Assert.IsNotNull(containerRef);

		//解放されない
		ABLoader.Unload();
		LoadedTest("materials/testmaterial", true);
		//アンロード
		containerRef.Dispose();
		ABLoader.Unload();
		LoadedTest("materials/testmaterial", false);

	}

	[UnityTest]
	public IEnumerator LoadTest15()
	{
		//拡張マニフェストを利用する
		yield return Init(useExManifet: true);
		if (!UseExManifest)
		{
			yield break;
		}
		string[] bundles = new string[] {
			"prefabs",
			"testscene",
			"sprites/circle",
			"sprites/diamond",
			"sprites/hexagon",
			"sprites/polygon",
			"sprites/triangle",
		};

		//サイズが取れている
		Assert.AreNotEqual(ABLoader.GetSize(bundles), bundles.Length);

		//CRCでのロードが出来る
		List<BundleContainerRef> refs = new List<BundleContainerRef>();
		for (int i = 0; i < bundles.Length; i++)
		{
			ABLoader.LoadContainer(bundles[i], (r) => refs.Add(r), ex => throw ex);
		}
		yield return new WaitUntil(() => refs.Count == bundles.Length);

		//GUIDの参照からロードする
		var (bundle, name) = ABLoader.GetReference("083e98c42f9344e4a92b1871cc1b5e2b");
		Assert.AreEqual(bundle, "sprites/diamond");
		Assert.AreEqual(name, "Assets/IchioLib.ABLoader.Test/Tests/AssetBundles/Sprites/Diamond.png");
		Sprite sprite = null;
		ABLoader.LoadAsset<Sprite>(bundle, name, x => sprite = x, ex => throw ex);
		yield return new WaitUntil(() => sprite != null);

		bool ret = false;
		var (length, progress) = ABLoader.DownloadByTags(new string[] { "Sprites" }, (r) => ret = r, ex => throw ex);
		yield return new WaitUntil(() => ret);
		Assert.AreEqual(length, 6);
	}

	IEnumerator TaskTest(System.Func<Task> action)
	{
		yield return Init();
		try
		{
			var task = action();
			while (!task.IsCompleted)
			{
				yield return null;
			}
			if (task.IsFaulted)
			{
				throw task.Exception;
			}
		}
		finally
		{
			ABLoader.Unload();
		}
	}

	[UnityTest]
	public IEnumerator LoadTest16() => TaskTest(async () =>
	{
		//ロード出来た
		var c = await ABLoader.Load("materials/testmaterial");
		Assert.IsNotNull(c);
		//コンテナからの非同期ロードも待てる
		var mat = await c.LoadAssetAsync<Material>("TestMaterial");
		Assert.IsNotNull(mat);

		//個別のアセットのロード
		var sprite = await ABLoader.LoadAsset<Sprite>("sprites/circle", "Circle");
		Assert.IsNotNull(sprite);
	});


	[UnityTest]
	public IEnumerator LoadTest17() => TaskTest(async () =>
	{
		//エディタモードではテストしない
		if (IsEditorTest()) return;

		//エラーのリトライのハンドリング
		System.Exception error = null;
		var loading = ABLoader.Load("dummy");
		loading.MaxRetryCount = 10;
		int retry = 0;
		loading.RetryHandle = (x, err, handle) =>
		{
			//リトライのフック
			retry++;
			handle(true);
		};
		loading.ErrorHandle += x => error = x;

		//Errorを無視しても完了する
		loading.IgnoreError = true;
		loading.ForceAwaiterCompleteIfError = true;

		try
		{
			//一旦、エラーを無視する
			LogAssert.ignoreFailingMessages = true;
			var c = await loading;
			LogAssert.ignoreFailingMessages = false;
			Assert.IsNull(c);
			Assert.IsNull(error);
			//エディタではリトライのエラーはないので
			Assert.AreEqual(retry, loading.MaxRetryCount);

		}
		finally
		{
			LogAssert.ignoreFailingMessages = false;
		}

	});

	[UnityTest]
	public IEnumerator LoadTest18() => TaskTest(async () =>
	{
		//エディタモードではテストしない
		if (IsEditorTest()) return;

		//エラーのハンドリング
		System.Exception catchError = null;
		try
		{
			LogAssert.ignoreFailingMessages = true;
			await ABLoader.Load("dummy");
		}
		catch (System.Exception ex)
		{
			catchError = ex;
		}
		finally
		{
			LogAssert.ignoreFailingMessages = false;
		}
		Assert.IsNotNull(catchError);
		catchError = null;

		try
		{
			LogAssert.ignoreFailingMessages = true;
			System.Exception handleError = null;

			var loading = ABLoader.Load("dummy");
			loading.ErrorHandle = x => handleError = x;
			var now = System.DateTime.Now;
			try
			{
				await (await Task.WhenAny(loading, Task.Delay(2000)));
			}
			catch (System.Exception ex)
			{
				catchError = ex;
			}

			LogAssert.ignoreFailingMessages = false;

			//ErrorHandleが指定された場合は、ErrorHandleが優先されawaitでエラーは発生しない
			Assert.IsNull(catchError);
			Assert.IsTrue(System.TimeSpan.FromMilliseconds(1500) < (System.DateTime.Now - now));
			Assert.IsNotNull(handleError);
			Assert.IsNotNull(loading.Error);

		}
		finally
		{
			LogAssert.ignoreFailingMessages = false;
		}

		try
		{
			LogAssert.ignoreFailingMessages = true;
			System.Exception handleError = null;

			var loading = ABLoader.Load("dummy");
			loading.ErrorHandle = x => handleError = x;
			loading.ForceAwaiterCompleteIfError = true;
			var now = System.DateTime.Now;
			try
			{
				await (await Task.WhenAny(loading, Task.Delay(2000)));
			}
			catch (System.Exception ex)
			{
				catchError = ex;
			}

			LogAssert.ignoreFailingMessages = false;

			//ForceAwaiterCompleteIfErrorが有効な場合はどちらも発火される
			Assert.IsTrue(System.TimeSpan.FromMilliseconds(500) > (System.DateTime.Now - now));
			Assert.IsNotNull(catchError);
			Assert.IsNotNull(handleError);
			Assert.IsNotNull(loading.Error);

		}
		finally
		{
			LogAssert.ignoreFailingMessages = false;
		}

	});

}
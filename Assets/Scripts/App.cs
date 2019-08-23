using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class App : MonoBehaviour {
	private enum CustomLevelsRoot { Documents, StreamingAssets };

	[Header("Splash")]
	[SerializeField] private GameObject splash;
	[SerializeField] private ProgressBar loadingBar;
	[Header("Data")]
	[SerializeField] private UnitFactory unitFactory;
	[SerializeField] private TowerFactory towerFactory;
	[Space(5), SerializeField] private CustomLevelsRoot customLevelsRoot = CustomLevelsRoot.StreamingAssets;
	
	private string customLevelsDir;
	private string officialLevelsDir;
	private string officialLevelsDir_FullPath;

	private List<LevelFile> officialLevels, customLevels;
	private LevelFile currentLevel;

	void Awake() {
		customLevelsDir = Application.streamingAssetsPath;
		if (customLevelsRoot == CustomLevelsRoot.Documents)
			customLevelsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", Application.productName);
		customLevelsDir = Path.Combine(customLevelsDir, "Levels");
		if (!Directory.Exists(customLevelsDir)) Directory.CreateDirectory(customLevelsDir);

		officialLevelsDir = "Levels";
#if UNITY_EDITOR
		officialLevelsDir_FullPath = Path.Combine(Application.dataPath, "Resources", officialLevelsDir);
		if (!Directory.Exists(officialLevelsDir_FullPath)) Directory.CreateDirectory(officialLevelsDir_FullPath);
#endif

		DontDestroyOnLoad(gameObject);
		DontDestroyOnLoad(splash);
		
		EventManager.AddListener<ExitToMenuEvent>(delegate { StartCoroutine(LaunchMenu()); });
		EventManager.AddListener<RestartGameEvent>(delegate { StartCoroutine(LaunchGame(currentLevel)); });
		EventManager.AddListener<LoadLevelEvent>(delegate(LoadLevelEvent evt) { StartCoroutine(LaunchEditor(evt.level.Clone())); });
	}

	IEnumerator Start() {
		unitFactory = Instantiate(unitFactory);
		towerFactory = Instantiate(towerFactory);

		// TODO: Improve loading bar progress so it feels more natural.
		yield return StartCoroutine(LoadOfficialLevels());
		yield return StartCoroutine(LoadCustomLevels());
		yield return StartCoroutine(LaunchMenu());
	}

	private IEnumerator LoadOfficialLevels() {
		officialLevels = new List<LevelFile>();

		// TODO: Heavy, think of something else.
		var levelAssets = Resources.LoadAll<TextAsset>(officialLevelsDir);
		foreach (var levelAsset in levelAssets) {
			LevelFile level = ByteSerializer.Deserialize<LevelFile>(levelAsset.bytes);
			level.fileName = levelAsset.name;
			level.isOfficial = true;
			officialLevels.Add(level);
		}

		yield return null;
	}

	private IEnumerator LoadCustomLevels() {
		customLevels = new List<LevelFile>();

		DirectoryInfo dir = new DirectoryInfo(customLevelsDir);
		FileInfo[] files = dir.GetFiles("*.lvl");
		foreach (var file in files) {
			LevelFile level = ByteSerializer.Deserialize<LevelFile>(File.ReadAllBytes(file.FullName));
			level.fileName = Path.GetFileNameWithoutExtension(file.FullName);
			level.isOfficial = false;
			customLevels.Add(level);
		}

		yield return null;
	}

	private IEnumerator LaunchMenu() {
		splash.SetActive(true);

		var async = SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
		while (!async.isDone) {
			loadingBar.Progress = async.progress * 0.9f;
			yield return null;
		}

		MainMenu mainMenu = FindObjectOfType<MainMenu>();
		mainMenu.Set_OnEditorButton(delegate { StartCoroutine(LaunchEditor(new LevelFile())); });
		mainMenu.Enciclopedia.InjectTowers(towerFactory.Sprites, towerFactory.Data);
		mainMenu.Enciclopedia.InjectEnemies(unitFactory.Sprites, unitFactory.Data);
		mainMenu.LevelSelector.Inject(officialLevels, customLevels);
		mainMenu.LevelSelector.Set_OnLaunch(delegate(LevelFile level) { StartCoroutine(LaunchGame(level)); });

		yield return null;
		loadingBar.Progress = 1.0f;

		splash.SetActive(false);
	}

	private IEnumerator LaunchEditor(LevelFile level) {
		currentLevel = level;

		splash.SetActive(true);
		
		var async = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
		while (!async.isDone) {
			loadingBar.Progress = async.progress * 0.7f;
			yield return null;
		}
		
		Destroy(FindObjectOfType<GameManager>());

		LevelEditorManager levelEditorManager = FindObjectOfType<LevelEditorManager>();
		levelEditorManager.SetEditorFunctions(OnSaveLevel, OnDeleteLevel);
#if UNITY_EDITOR
		levelEditorManager.LoadEditor(level, officialLevels, customLevels, delegate(float progress) { loadingBar.Progress = 0.7f + progress * 0.3f; });
#else
		levelEditorManager.LoadEditor(level, new List<LevelFile>(), customLevels, delegate (float progress) { loadingBar.Progress = 0.7f + progress * 0.3f; });
#endif

		while (loadingBar.Progress < 0.95f) yield return null;
		loadingBar.Progress = 1.0f;

		splash.SetActive(false);
	}

	private IEnumerator LaunchGame(LevelFile level) {
		currentLevel = level;

		splash.SetActive(true);

		var async = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
		while (!async.isDone) {
			loadingBar.Progress = async.progress * 0.7f;
			yield return null;
		}

		Destroy(FindObjectOfType<LevelEditorManager>());

		GameManager gameManager = FindObjectOfType<GameManager>();
		gameManager.LoadGame(level, towerFactory, unitFactory, delegate(float progress) { loadingBar.Progress = 0.7f + progress * 0.3f; });

		while (loadingBar.Progress < 0.95f) yield return null;
		loadingBar.Progress = 1.0f;

		splash.SetActive(false);
	}

	private void OnSaveLevel(LevelFile level) {
		level = level.Clone();

		byte[] bytes = ByteSerializer.Serialize(level);
		string path = level.isOfficial ? officialLevelsDir_FullPath : customLevelsDir;
		path = Path.Combine(path, level.fileName) + (level.isOfficial ? ".txt" : ".lvl");
		File.WriteAllBytes(path, bytes);

		if (level.isOfficial) RegisterLevel(officialLevels, level);
		else RegisterLevel(customLevels, level);

		EventManager.Raise(new LevelSavedEvent(level));

#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh();
#endif
	}
	private void RegisterLevel(List<LevelFile> levels, LevelFile level) {
		int idx = levels.FindIndex(obj => obj.Equals(level));
		if (idx == -1) levels.Add(level);
		else levels[idx] = level;
	}
	private void OnDeleteLevel(LevelFile level) {
		string path = level.isOfficial ? officialLevelsDir_FullPath : customLevelsDir;
		path = Path.Combine(path, level.fileName) + (level.isOfficial ? ".txt" : ".lvl");
		File.Delete(path);
#if UNITY_EDITOR
		File.Delete(path + ".meta");
#endif

		officialLevels.Remove(level);
		customLevels.Remove(level);

		EventManager.Raise(new LevelDeletedEvent(level));

#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh();
#endif
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;

[CustomEditor(typeof(Tower))]
public class TowerEditor : Editor {
	private Tower tower;
	private MonoScript script;
	private List<bool> foldouts;

	void OnEnable() {
		tower = target as Tower;
		script = MonoScript.FromMonoBehaviour(tower);

		if (tower.data.levels == null) tower.data.levels = new List<Tower.Level>();
		if (tower.data.levels.Count == 0) tower.data.levels.Add(new Tower.Level());

		foldouts = new List<bool>(tower.data.levels.Count);
		for (int i = 0; i < tower.data.levels.Count; ++i) foldouts.Add(false);
	}
	
	public override void OnInspectorGUI() {
		while (foldouts.Count < tower.data.levels.Count) foldouts.Add(false);
		if (foldouts.Count > tower.data.levels.Count) foldouts.RemoveRange(tower.data.levels.Count, foldouts.Count - tower.data.levels.Count);

		GUI.enabled = false;
		script = EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false) as MonoScript;
		GUI.enabled = true;
		
		tower.data.towerName = EditorGUILayout.TextField("Tower Name", tower.data.towerName);
		tower.turret = EditorGUILayout.ObjectField("Turret", tower.turret, typeof(Transform), true) as Transform;

		EditorGUILayout.Space();
		tower.data.attackTarget = (Tower.AttackTarget)EditorGUILayout.EnumPopup("Attack Target", tower.data.attackTarget);
		if (tower.data.attackTarget != Tower.AttackTarget.None) {
			tower.data.attackType = (Tower.AttackType)EditorGUILayout.EnumPopup("Attack Type", tower.data.attackType);
			tower.data.onHitEffect = (Tower.OnHitEffect)EditorGUILayout.EnumPopup("On Hit Effect", tower.data.onHitEffect);
		}

		EditorGUILayout.Space();
		tower.data.buffTarget = (Tower.BuffTarget)EditorGUILayout.EnumPopup("Buff Target", tower.data.buffTarget);
		if (tower.data.buffTarget == Tower.BuffTarget.Aura)
			tower.data.buffType = (Tower.BuffType)EditorGUILayout.EnumPopup("Buff Type", tower.data.buffType);

		EditorGUILayout.Space();
		for (int i = 0; i < tower.data.levels.Count; ++i) {
			Tower.Level towerLevel = tower.data.levels[i];
			
			if (foldouts[i]) EditorGUILayout.Space();
			foldouts[i] = EditorGUILayout.Foldout(foldouts[i], "Level " + i, true);
			if (foldouts[i]) {
				towerLevel.cost = (ushort)EditorGUILayout.IntField("Cost", towerLevel.cost);
				if (tower.data.attackTarget != Tower.AttackTarget.None) {
					towerLevel.attackDamage = EditorGUILayout.FloatField("Attack Damage", towerLevel.attackDamage);
					towerLevel.attackInterval = EditorGUILayout.FloatField("Attack Interval", towerLevel.attackInterval);
					towerLevel.attackRange = EditorGUILayout.FloatField("Attack Range", towerLevel.attackRange);
					if (tower.data.attackType == Tower.AttackType.AOE)
						towerLevel.aoeRadius = EditorGUILayout.FloatField("Attack AOE Radius", towerLevel.aoeRadius);
				}
				if (tower.data.buffTarget != Tower.BuffTarget.None) {
					towerLevel.auraRange = EditorGUILayout.FloatField("Aura Range", towerLevel.auraRange);
					if (tower.data.buffType == Tower.BuffType.AttackDamage)
						towerLevel.attackDamageAura = EditorGUILayout.FloatField("Attack Damage Bonus", towerLevel.attackDamageAura);
					else if (tower.data.buffType == Tower.BuffType.AttackSpeed)
						towerLevel.attackSpeedAura = EditorGUILayout.FloatField("Attack Speed Bonus", towerLevel.attackSpeedAura);
				}

				if (i > 0 && GUILayout.Button("Delete", GUILayout.Width(100), GUILayout.Height(20))) {
					tower.data.levels.RemoveAt(i);
					foldouts.RemoveAt(i);
				} else
					tower.data.levels[i] = towerLevel;
			}
		}

		EditorGUILayout.Space(); EditorGUILayout.Space();
		if (GUILayout.Button("New")) {
			tower.data.levels.Add(new Tower.Level());
			foldouts.Add(true);
		}

		if (GUI.changed) {
			EditorUtility.SetDirty(tower);

			var scene = SceneManager.GetActiveScene();
			if (scene != null) EditorSceneManager.MarkSceneDirty(scene);

			var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
			if (prefabStage != null) EditorSceneManager.MarkSceneDirty(prefabStage.scene);
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnitSelectionPanel : MonoBehaviour {
	[SerializeField] private UnitSelectionEntry unitEntry;
	[Space(10)][SerializeField] private UnityEvent showEvent;

	private List<UnitSelectionEntry> unitEntries;
	private int currentIndex;
	private Action<int> onNewIndex;

	void Awake() {
		int unitCount = UnitFactory.NumberOfUnits;
		unitEntries = new List<UnitSelectionEntry>(unitCount);
		unitEntries.Add(unitEntry);
		for (int i = 1; i < unitCount; ++i)
			unitEntries.Add(Instantiate(unitEntry, unitEntry.transform.parent));

		for (int i = 0; i < unitCount; ++i)
			unitEntries[i].Inject(i, UnitFactory.GetSprite(i), UnitFactory.GetData(i).name, OnEntrySelected);
	}

	public void Show(int currentIndex, Action<int> onNewIndex) {
		showEvent.Invoke();

		this.onNewIndex = onNewIndex;
		this.currentIndex = currentIndex;
		unitEntries[currentIndex].ForceSelect();
	}

	private void OnEntrySelected(int index) {
		if (onNewIndex != null) onNewIndex(index);
	}
}

using UnityEngine;
using UnityEditor;
using System.Collections;

public static class TimescaleTools
{
	[MenuItem("Tools/Timescale/1x")]
	public static void TS_1x() {
		Time.timeScale = 1f;
	}

	[MenuItem("Tools/Timescale/2x")]
	public static void TS_2x() {
		Time.timeScale = 2f;
	}

	[MenuItem("Tools/Timescale/10x")]
	public static void TS_10x() {
		Time.timeScale = 10f;
	}
}
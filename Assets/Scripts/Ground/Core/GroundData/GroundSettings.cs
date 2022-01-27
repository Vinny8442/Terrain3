
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ground
{
	[CreateAssetMenu(fileName = "GroundSettings", menuName = "ScriptableObjects/GroundSettings")]
	public class GroundSettings :ScriptableObject
	{
		public List<PerlinNoiseSettings> Noises;
		public float SectorSize = 100f;
	}
	
	
}
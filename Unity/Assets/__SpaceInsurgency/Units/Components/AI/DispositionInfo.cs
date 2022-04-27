using UnityEngine;
using System.Collections;

namespace SpaceInsurgency
{
	public class DispositionInfo
	{
		public DynamicAgent ship;
		public Vector3 lastSeen;
		public Faction.Relation initialOpinion;
		public int Disposition
		{
			get
			{
				int ret = 0;
				switch (initialOpinion)
				{
					case Faction.Relation.Amicable:
						ret += 20;
						break;
					case Faction.Relation.Friendly:
						ret += 10;
						break;
					case Faction.Relation.Neutral:
						// 0
						break;
					case Faction.Relation.Cautious:
						ret -= 10;
						break;
					case Faction.Relation.Hostile:
						ret -= 20;
						break;
				}

#warning lower this more here if has been attacked

				return ret;
			}
		}
	}
}
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using SpaceInsurgency;
using SpaceInsurgency.Items;
using AdvancedInspector;

namespace SpaceInsurgency
{
	public class Cargo : EquippableComponent
	{
		protected override void Awake()
		{
			base.Awake();

			ItemsMax = int.MaxValue;
		}


		public override IEnumerable<Base> ItemsShownInActionArea
		{
			get
			{
				yield break;
			}
		}

		public override CanAddItemAnswer CanAddItem(Base newItem, int count)
		{
			CanAddItemAnswer answer = base.CanAddItem(newItem, count);
			if (CanAddItemAnswer.Yes != answer)
				return answer;
			
			return newItem.CanBeAddedToCargo ? CanAddItemAnswer.Yes : CanAddItemAnswer.ItemDefinitionDisallows;
		}

		#region Serialization

		[Serializable]
		public class Serialized
		{
			public List<string> items = new List<string>();

			public Serialized() { }
		}

		public Serialized GetSerialized()
		{
			Serialized serialized = new Serialized();

			for (int i = 0; i < Items.Count; i++)
				serialized.items.Add(Items[i].ItemDefinition.UniqueName);

			return serialized;
		}

		public void RestoreSerialized(Serialized serialized)
		{
			for (int i = 0; i < serialized.items.Count; i++)
				AddItem(Base.GetFromUniqueName(serialized.items[i]));
		}

		#endregion Serialization
	}
}
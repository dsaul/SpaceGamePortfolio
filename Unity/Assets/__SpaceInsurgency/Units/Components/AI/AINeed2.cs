using UnityEngine;
using System;
using System.Collections;
using SpaceInsurgency;
using AdvancedInspector;
using SharedCode;

namespace SpaceInsurgency
{
	public class AINeed2 : SharedCode.Behaviours.Base, IComparable
	{
		#region Variables

		AI m_AI;
		/// <summary>
		/// A reference to which ai system we are apart of.
		/// </summary>
		[Inspect, ReadOnly]
		public virtual AI ai
		{
			get { return m_AI; }
			set { m_AI = value; }
		}

		#endregion Variables
		#region Events

		/// <summary>
		/// This method is called in whatever order that this was deemed its order of importance (phrasing?).
		/// </summary>
		public virtual void PriorityLottery()
		{

		}

		#endregion Events
		#region Anccessors

		/// <summary>
		/// The higher the more important, causes ties to get beat. This should be static.
		/// </summary>
		[Inspect]
		public virtual int Importance
		{
			get
			{
				return int.MinValue;
			}
		}

		/// <summary>
		/// What determines which need is acted upon.
		/// </summary>
		[Inspect]
		public virtual int Urgency
		{
			get
			{
				return 0;
			}
		}

		public override string ToString()
		{
			return GetType().FullName;
		}

		#endregion Anccessors
		#region IComparable

		public int CompareTo(object other)
		{
			AINeed2 otherANP = other as AINeed2;

			// If it is some other object type?
			if (null == otherANP)
				return this.GetHashCode().CompareTo(other.GetHashCode());

			// Another AINeedsProvider
			int urgencyCmp = Urgency.CompareTo(otherANP.Urgency);

			// If urgency is not the same, then this is all we go by.
			if (0 != urgencyCmp)
				return urgencyCmp;

			// If it is the same, we then determine based upon the predifined importance tie breakers.
			return Importance.CompareTo(otherANP.Importance);
		}

		#endregion IComparable
	}

}
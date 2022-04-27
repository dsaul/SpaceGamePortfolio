using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using SpaceInsurgency;
using AdvancedInspector;

namespace SpaceInsurgency.Mouseover
{
	[AdvancedInspector]
	public class DetailText : SharedCode.Behaviours.InstanceTracked<DetailText>
	{
		#region Variables

		[Inspect, Group("Membership"), Descriptor(Name = "Faction Display Name")]
		public Text m_MembershipFactionDisplayName;

		[Inspect, Group("Common"), Descriptor(Name = "Display Name Label")]
		public Text m_DisplayNameLabel;

		[Inspect, Group("Dynamic Agent"), Descriptor(Name = "Buttons")]
		public RectTransform m_DynamicAgentButtons;

		[Inspect, Group("Membership"), Descriptor(Name = "Faction Disposition")]
		public Text m_MembershipFactionDisposition;

		[Inspect, Group("Static Agent"), Descriptor(Name = "Buttons")]
		public RectTransform m_StaticAgentButtons;

		[Inspect, Group("Pickup Agent"), Descriptor(Name = "Buttons")]
		public RectTransform m_PickupAgentButtons;

		[Inspect, Group("Common"), Descriptor(Name = "Dynamic or Static Agent Buttons")]
		public RectTransform m_DynamicOrStaticAgentButtons;

		[Inspect, Group("Editor")]
		public CanvasGroup m_ListCanvasGroup;

		[Inspect, Group("Editor")]
		public RectTransform m_ListTransform;

		[Inspect, Group("Editor")]
		public RectTransform m_UnitRaycastBlockerTransform;

		[Inspect, Group("Editor")]
		public RectTransform m_SelectionResponderTransform;

		[Inspect, Group("Dynamic Agent")]
		public UnityEngine.UI.Button m_StartCommunicationButton;

		[Inspect, Group("Dynamic Agent")]
		public UnityEngine.UI.Button m_ViewInventoryButton;

		[Inspect, Group("Static Agent")]
		public UnityEngine.UI.Button m_TradeButton;

		[Inspect, Group("Common")]
		public UnityEngine.UI.Button m_DemandSurrenderButton;

		[Inspect, Group("Common")]
		public UnityEngine.UI.Button m_BoardButton;

		[Inspect, Group("Editor")]
		public UnityEngine.UI.Button m_PickUpButton;

		SpaceObject m_Target;
		[Inspect, ReadOnly, Group("Runtime")]
		public SpaceObject Target
		{
			get { return m_Target; }
			set
			{
				m_Target = value;
				m_ListCanvasGroup.alpha = 0f;
				UpdateDisplayedValues();
			}
		}


		bool m_IsPointerOver = false;
		[Inspect, ReadOnly, Group("Runtime")]
		public bool IsPointerOver
		{
			get { return m_IsPointerOver; }
			private set { m_IsPointerOver = value; }
		}

		#endregion Variables
		#region Setup

		protected override void Start()
		{
			base.Start();

			UpdateDisplayedValues();

			SubSignal();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			SubSignal();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			UnSubSignal();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			UnSubSignal();
		}

		bool signalSub = false;

		void SubSignal()
		{
			if (true == signalSub)
				return;
			if (null == InputManager.Main)
				return;

			InputManager.Main.OnMouseoverSpaceObjectFudgedChanged += InputManager_OnMouseoverSpaceObjectFudgedChanged;

			signalSub = true;
		}

		void UnSubSignal()
		{
			if (false == signalSub)
				return;
			if (null == SelectionManager.Main)
				return;

			InputManager.Main.OnMouseoverSpaceObjectFudgedChanged -= InputManager_OnMouseoverSpaceObjectFudgedChanged;

			signalSub = false;
		}

		#endregion Setup
		#region Events

		void InputManager_OnMouseoverSpaceObjectFudgedChanged(InputManager source, List<SpaceObject> objects)
		{
			if (false == InputManager.Main.IsOverUIElement)
				Target = InputManager.Main.MouseOverSpaceObjectsFudged.TryGet<SpaceObject>(0);
		}

		public void UnitRaycastBlocker_PointerExit(BaseEventData data)
		{
			Target = InputManager.Main.MouseOverSpaceObjectsFudged.TryGet<SpaceObject>(0);
		}

		public void SelectionResponder_PointerClick(BaseEventData data)
		{

			if (InputManager.Main.CurrentInputMode != InputManager.InputMode.Targeting)
				SelectionManager.Main.MouseSelectImpl(Target);
		}

		public void SelectionResponder_PointerEnter(BaseEventData data)
		{
			m_IsPointerOver = true;
		}

		public void SelectionResponder_PointerExit(BaseEventData data)
		{
			m_IsPointerOver = false;
		}

		public void StartCommunication_OnClick()
		{
			Debug.Log("StartCommunication_OnClick()");
		}

		public void ViewInventory_OnClick()
		{
			Debug.Log("ViewInventory_OnClick()");
		}

		public void Trade_OnClick()
		{
			Debug.Log("Trade_OnClick()");
		}

		public void DemandSurrender_OnClick()
		{
			Debug.Log("DemandSurrender_OnClick()");
		}

		public void Board_OnClick()
		{
			Debug.Log("Board_OnClick()");
		}

		public void PickUp_OnClick()
		{
			Debug.Log("PickUp_OnClick()");


		}

		#endregion Events
		#region Main

		void UpdateDisplayedValues()
		{
			DynamicAgent dynamicAgent = null;
			if (null != m_Target)
				dynamicAgent = m_Target.GetComponent<DynamicAgent>();

			StaticAgent staticAgent = null;
			if (null != m_Target)
				staticAgent = m_Target.GetComponent<StaticAgent>();

			PickupAgent pickupAgent = null;
			if (null != m_Target)
				pickupAgent = m_Target.GetComponent<PickupAgent>();

			Membership membership = null;
			if (null != m_Target)
				membership = m_Target.GetComponent<Membership>();

			m_DynamicAgentButtons.gameObject.SetActive(null != dynamicAgent);
			m_StaticAgentButtons.gameObject.SetActive(null != staticAgent);
			m_DynamicOrStaticAgentButtons.gameObject.SetActive(null != dynamicAgent || null != staticAgent);
			m_PickupAgentButtons.gameObject.SetActive(null != pickupAgent);
			
			// Whether the blocker stuff is enabled.
			if (null == m_Target)
			{
				m_UnitRaycastBlockerTransform.gameObject.SetActive(false);
				m_SelectionResponderTransform.gameObject.SetActive(false);
			}
			else
			{
				Update();
				m_UnitRaycastBlockerTransform.gameObject.SetActive(true);
				m_SelectionResponderTransform.gameObject.SetActive(true);
			}
			
			// Faction Text
			if (null != membership && null != membership.Faction)
			{
				m_MembershipFactionDisplayName.text = membership.Faction.DisplayName;
				Update();
				m_MembershipFactionDisplayName.gameObject.SetActive(true);
			}
			else
			{
				m_MembershipFactionDisplayName.text = "";
				m_MembershipFactionDisplayName.gameObject.SetActive(false);

			}


			// Display Name Label
			if (null != dynamicAgent && null != dynamicAgent.AgentDefinition)
			{
				m_DisplayNameLabel.text = dynamicAgent.AgentDefinition.DisplayName;
				m_DisplayNameLabel.gameObject.SetActive(true);
			}
			else if (null != staticAgent)
			{
				m_DisplayNameLabel.text = staticAgent.DisplayName;
				Update();
				m_DisplayNameLabel.gameObject.SetActive(true);
			}
			else if (null != pickupAgent)
			{
				m_DisplayNameLabel.text = pickupAgent.Item.FriendlyName;
				Update();
				m_DisplayNameLabel.gameObject.SetActive(true);
			}
			else
			{
				m_DisplayNameLabel.text = "";
				m_DisplayNameLabel.gameObject.SetActive(false);
			}

			// Disposition Text
			if (null != membership && null != membership.Faction && false == membership.Faction.IsPlayerFaction)
			{
				m_MembershipFactionDisposition.text = membership.Faction.DispositionOfPlayer.ToString();
				Update();
				m_MembershipFactionDisposition.gameObject.SetActive(true);
			}
			else
			{
				m_MembershipFactionDisposition.text = "";
				m_MembershipFactionDisposition.gameObject.SetActive(false);
			}

			// Start Communication Button
			bool enableCommunication = true;
			do
			{
				if (null == m_Target)
				{
					enableCommunication = false;
					break;
				}

				if (null != membership && membership.Faction == Faction.PlayerFaction)
				{
					enableCommunication = false;
					break;
				}

				if (null != staticAgent)
				{
					Communication communication = staticAgent.GetComponent<Communication>();
					if (null == communication || null == communication.Dialogue)
					{
						enableCommunication = false;
						break;
					}
				}

				if (null != dynamicAgent)
				{
					Communication communication = dynamicAgent.GetComponent<Communication>();
					if (null == communication || null == communication.Dialogue)
					{
						enableCommunication = false;
						break;
					}
				}
				
			} while (false);
			
			m_StartCommunicationButton.interactable = enableCommunication;


			// Cargo
			bool enableInventory = true;
			do
			{
				if (null == m_Target)
				{
					enableInventory = false;
					break;
				}


				EquippableComponent[] e = m_Target.GetComponents<EquippableComponent>();
				if (0 == e.Length)
				{
					enableInventory = false;
					break;
				}

			} while (false);

			m_ViewInventoryButton.interactable = enableInventory;

			// Trade
			bool enableTrade = true;
			do
			{
				if (null == m_Target)
				{
					enableTrade = false;
					break;
				}

				Trader trader = m_Target.GetComponent<Trader>();
				if (null == trader || false == trader.WillTrade)
				{
					enableTrade = false;
					break;
				}

			} while (false);

			m_TradeButton.interactable = enableTrade;

#warning TODO: Implement demanding surrender.
			m_DemandSurrenderButton.interactable = false;

#warning TODO: Implement boarding of ships.
			m_BoardButton.interactable = false;

#warning TODO: Implement picking up items.
			m_PickUpButton.interactable = true;
		}


		protected override void Update()
		{
			base.Update();

			if (null == m_Target)
				return;
			if (
				null == m_Target.selectionLastMinX || 
				null == m_Target.selectionLastMinY || 
				null == m_Target.selectionLastMaxX || 
				null == m_Target.selectionLastMaxY
				)
			{
				//Target = null;
				return;
			}

			float minX = m_Target.selectionLastMinX.Value;
			float minY = m_Target.selectionLastMinY.Value;
			float maxX = m_Target.selectionLastMaxX.Value;
			float maxY = m_Target.selectionLastMaxY.Value;

			float unitSWX = minX;
			float unitSWY = minY;

			float unitSEX = maxX;
			float unitSEY = minY;

			float unitNEX = maxX;
			float unitNEY = maxY;

			float unitNWX = minX;
			float unitNWY = maxY;

			float unitHeight = maxY - minY;
			float unitWidth = maxX - minX;

			m_SelectionResponderTransform.position = new Vector3(unitNWX, unitNWY);
			m_SelectionResponderTransform.sizeDelta = new Vector2(unitWidth, unitHeight);

			/*
			 * List
			 */
			Vector2 listSize = m_ListTransform.sizeDelta;
			
			// See if we can position the text on the right.
			Vector3 listOnRightTopRightMostPoint = new Vector3(unitNEX + listSize.x, unitNEY, 0f);
			Vector3 listOnRightTopRightMostPointViewport = CameraManager.Main.CameraGame.ScreenToViewportPoint(listOnRightTopRightMostPoint);

			bool listCanBeOnRight = listOnRightTopRightMostPointViewport.x.IsBetween<float>(0,0.9f);
			bool listCanBeOnTop = listOnRightTopRightMostPointViewport.y.IsBetween<float>(0, 0.9f);

			float listNWX = true == listCanBeOnRight ? unitNEX : unitNWX - listSize.x;
			float listNWY = true == listCanBeOnTop ? unitNWY : unitSWY + listSize.y;

			float listNEX = listNWX + listSize.x;
			float listNEY = listNWY;

			float listSWX = listNEX;
			float listSWY = listNWY - listSize.y;

			float listSEX = listNEX;
			float listSEY = listSWY;

			m_ListTransform.transform.position = new Vector3(listNWX, listNWY);
			m_MembershipFactionDisplayName.alignment = true == listCanBeOnRight ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
			m_DisplayNameLabel.alignment = true == listCanBeOnRight ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
			m_MembershipFactionDisposition.alignment = true == listCanBeOnRight ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
			
			//Debug.Log("unitHeight " + unitHeight + " unitWidth " + unitWidth);

			/* 
			 * Unit raycast blocker.
			 */
			float blockerNWX = Mathf.Min(unitNWX, listNWX)-5f;
			float blockerNWY = Mathf.Max(unitNWY, listNWY)+5f;

			float blockerNEX = Mathf.Max(unitNEX, listNEX)+5f;
			float blockerNEY = Mathf.Max(unitNEY, listNEY)+5f;

			float blockerSWX = Mathf.Min(unitSWX, listSWX)-5f;
			float blockerSWY = Mathf.Min(unitSWY, listSWY)-5f;

			float blockerSEX = Mathf.Max(unitSEX, listSEX)+5f;
			float blockerSEY = Mathf.Min(unitSEY, listSEY)-5f;

			m_UnitRaycastBlockerTransform.position = new Vector3(blockerNWX, blockerNWY);

			m_UnitRaycastBlockerTransform.sizeDelta = new Vector2(blockerNEX - blockerNWX, blockerNWY - blockerSWY);
			
			m_ListCanvasGroup.alpha = 1f;
		}

		#endregion Main
	}
}
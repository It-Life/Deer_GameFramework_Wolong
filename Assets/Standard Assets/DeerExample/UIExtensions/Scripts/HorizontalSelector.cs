using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace tackor.UIExtension
{
	/// <summary>
	/// 水平选择器
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class HorizontalSelector : MonoBehaviour
	{
		//Resources
		public TMP_Text label;
		public TMP_Text labelHelper;
		public Image labelIcon;
		public Image labelIconHelper;
		public HorizontalLayoutGroup contentLayout;
		public HorizontalLayoutGroup contentLayoutHelper;

		public Transform indicatorParent;  //指示器父物体
		public GameObject indicatorObject;
		public TMP_Text indicatorText;     //指示文本

		private string newItemTitle;

		//Saving
		public bool enableIcon = true;
		public bool saveSelected = false;  //?

		const string prefsSaveKey = "HSelector_";
		public string saveKey = "My Selector";

		//Settings
		public IndicatorType indicatorType = IndicatorType.Point;
		public bool invokeAtStart;
		public bool invertAnimation;
		public bool loopSelection;
		[Range(0.25f, 2.5f)] public float iconScale = 1;
		[Range(1, 50)] public int contentSpacing = 15;
		public int defaultIndex = 0;
		[HideInInspector] public int index = 0;

		//Items
		public List<Item> items = new List<Item>();

		//Animation
		public Animator m_Animator;

		//Audio

		//Events
		public SelectorEvent onValueChanged;
		public ItemTextChangedEvent onItemTextChanged;

		private void Awake()
		{
			if (m_Animator == null)
				m_Animator = gameObject.GetComponent<Animator>();

			if (label == null || labelHelper == null)
			{
				Debug.LogError("<b><Horizontal Selector></b> Error: missing resources!", this);
				return;
			}

			SetupSelector();
			UpdateContentLayout();

			if (invokeAtStart)
			{
				items[index].onItemSelect.Invoke();
				onValueChanged.Invoke(index);
			}
		}

		private void OnEnable()
		{
			if (gameObject.activeInHierarchy)
				StartCoroutine("DisableAnimator");
		}

		public void NextItem()
		{
			if (items.Count == 0) return;

			StopCoroutine("DisableAnimator");
			m_Animator.enabled = true;

			if (loopSelection)
			{
				labelHelper.text = label.text;
				if (enableIcon && labelIcon != null)
					labelIconHelper.sprite = labelIcon.sprite;

				index++;
				if (index > items.Count - 1)
					index = 0;

				label.text = items[index].itemTitle;
				onItemTextChanged?.Invoke(label);
				if (enableIcon && labelIcon != null)
					labelIcon.sprite = items[index].itemIcon;

				items[index].onItemSelect.Invoke();
				onValueChanged.Invoke(index);

				m_Animator.Play(null);
				m_Animator.StopPlayback();

				if (invertAnimation)
					m_Animator.Play("Previous");
				else
					m_Animator.Play("Forward");
			}
			else
			{
				if (index != items.Count - 1)
				{
					labelHelper.text = label.text;
					if (enableIcon && labelIcon != null)
						labelIconHelper.sprite = labelIcon.sprite;

					if (index >= items.Count - 1)
						index = 0;
					else
						index++;

					label.text = items[index].itemTitle;
					onItemTextChanged?.Invoke(label);
					if (enableIcon && labelIcon != null)
						labelIcon.sprite = items[index].itemIcon;

					items[index].onItemSelect.Invoke();
					onValueChanged.Invoke(index);

					m_Animator.Play(null);
					m_Animator.StopPlayback();

					if (invertAnimation)
						m_Animator.Play("Previous");
					else
						m_Animator.Play("Forward");
				}
			}

			if (saveSelected)
				PlayerPrefs.SetInt($"{prefsSaveKey}{saveKey}", index);

			switch (indicatorType)
			{
				case IndicatorType.Point:
					for (int i = 0; i < items.Count; i++)
					{
						GameObject go = indicatorParent.GetChild(i).gameObject;
						Transform onTrans = go.transform.Find("On");
						Transform offTrans = go.transform.Find("Off");

						onTrans.gameObject.SetActive(index == i);
						offTrans.gameObject.SetActive(index != i);
					}
					break;
				case IndicatorType.Text:
					indicatorText.text = String.Format("{0}/{1}", index, items.Count);
					break;
				case IndicatorType.None:
				default:
					break;
			}

			if (gameObject.activeInHierarchy)
				StartCoroutine("DisableAnimator");
		}

		public void PreItem()
		{
			if (items.Count == 0) return;

			StopCoroutine("DisableAnimator");
			m_Animator.enabled = true;

			if (loopSelection)
			{
				labelHelper.text = label.text;
				if (enableIcon && labelIcon != null)
					labelIconHelper.sprite = labelIcon.sprite;

				index--;
				if (index < 0)
					index = items.Count - 1;

				label.text = items[index].itemTitle;
				onItemTextChanged?.Invoke(label);
				if (enableIcon && labelIcon != null)
					labelIcon.sprite = items[index].itemIcon;

				items[index].onItemSelect.Invoke();
				onValueChanged.Invoke(index);

				m_Animator.Play(null);
				m_Animator.StopPlayback();

				if (invertAnimation)
					m_Animator.Play("Forward");
				else
					m_Animator.Play("Previous");
			}
			else
			{
				if (index != 0)
				{
					labelHelper.text = label.text;
					if (enableIcon && labelIcon != null)
						labelIconHelper.sprite = labelIcon.sprite;

					if (index == 0)
						index = items.Count - 1;
					else
						index--;

					label.text = items[index].itemTitle;
					onItemTextChanged?.Invoke(label);
					if (enableIcon && labelIcon != null)
						labelIcon.sprite = items[index].itemIcon;

					items[index].onItemSelect.Invoke();
					onValueChanged.Invoke(index);

					m_Animator.Play(null);
					m_Animator.StopPlayback();

					if (invertAnimation)
						m_Animator.Play("Forward");
					else
						m_Animator.Play("Previous");
				}
			}

			if (saveSelected)
				PlayerPrefs.SetInt($"{prefsSaveKey}{saveKey}", index);

			switch (indicatorType)
			{
				case IndicatorType.Point:
					for (int i = 0; i < items.Count; i++)
					{
						GameObject go = indicatorParent.GetChild(i).gameObject;
						Transform onTrans = go.transform.Find("On");
						Transform offTrans = go.transform.Find("Off");

						onTrans.gameObject.SetActive(index == i);
						offTrans.gameObject.SetActive(index != i);
					}
					break;
				case IndicatorType.Text:
					indicatorText.text = String.Format("{0}/{1}", index + 1, items.Count);
					break;
				case IndicatorType.None:
				default:
					break;
			}

			if (gameObject.activeInHierarchy)
				StartCoroutine("DisableAnimator");
		}


		public void CreateNewItem(string title)
		{
			Item item = new Item(title);
			newItemTitle = title;
			items.Add(item);
		}

		public void CreateNewItem(string title, Sprite sprite)
		{
			Item item = new Item(title, sprite);
			newItemTitle = title;
			items.Add(item);
		}

		public void RemoveItem(string itemTitle)
		{
			items.Remove(items.Find(x => x.itemTitle == itemTitle));
			SetupSelector();
		}


		void SetupSelector()
		{
			if (items.Count == 0) return;

			if (saveSelected)
			{
				if (PlayerPrefs.HasKey($"{prefsSaveKey}{saveKey}"))
					defaultIndex = PlayerPrefs.GetInt($"{prefsSaveKey}{saveKey}");
				else
					PlayerPrefs.SetInt($"{prefsSaveKey}{saveKey}", defaultIndex);
			}

			label.text = labelHelper.text = items[defaultIndex].itemTitle;
			onItemTextChanged?.Invoke(label);

			if (enableIcon)
			{
				if (labelIcon != null)
					labelIcon.sprite = labelIconHelper.sprite = items[defaultIndex].itemIcon;
			}
			else
			{
				if (labelIcon != null)
					labelIcon.gameObject.SetActive(false);
				if (labelIconHelper != null)
					labelIconHelper.gameObject.SetActive(false);
			}

			index = defaultIndex;

			switch (indicatorType)
			{
				case IndicatorType.Point:
					UpdatePointIndicators();
					break;
				case IndicatorType.Text:
					UpdateTextIndicator();
					break;
				case IndicatorType.None:
				default:
					indicatorText.gameObject.SetActive(false);
					indicatorParent.gameObject.SetActive(false);
					Destroy(indicatorParent.gameObject);
					break;
			}
		}

		void UpdateTextIndicator()
		{
			if (indicatorType == IndicatorType.None) return;

			indicatorText.gameObject.SetActive(true);
			indicatorText.text = String.Format("{0}/{1}", index + 1, items.Count);

			indicatorParent.gameObject.SetActive(false);
		}

		void UpdatePointIndicators()
		{
			if (indicatorType == IndicatorType.None) return;

			indicatorParent.gameObject.SetActive(true);
			indicatorText.gameObject.SetActive(false);

			foreach (Transform item in indicatorParent)
			{
				Destroy(item.gameObject);
			}
			for (int i = 0; i < items.Count; i++)
			{
				GameObject go = Instantiate(indicatorObject, Vector3.zero, Quaternion.identity);
				go.transform.SetParent(indicatorParent, false);
				go.name = items[i].itemTitle;

				Transform onObj = go.transform.Find("On");
				Transform offObj = go.transform.Find("Off");

				onObj.gameObject.SetActive(index == i);
				offObj.gameObject.SetActive(index != i);
			}
		}

		public void UpdateContentLayout()
		{
			if (contentLayout != null)
				contentLayout.spacing = contentSpacing;
			if (contentLayoutHelper != null)
				contentLayoutHelper.spacing = contentSpacing;

			if (labelIcon != null)
				labelIcon.transform.localScale = labelIconHelper.transform.localScale = Vector3.one * iconScale;

			LayoutRebuilder.ForceRebuildLayoutImmediate(label.transform.GetComponent<RectTransform>());
			LayoutRebuilder.ForceRebuildLayoutImmediate(label.transform.parent.GetComponent<RectTransform>());
		}

		IEnumerator DisableAnimator()
		{
			yield return new WaitForSeconds(0.5f);
			m_Animator.enabled = false;
		}

		// ---------------------------------------------------------------
		[Serializable] public class SelectorEvent : UnityEvent<int> { }
		[Serializable] public class ItemTextChangedEvent : UnityEvent<TMP_Text> { }

		[Serializable]
		public class Item
		{
			public string itemTitle = "Item Title";
			public Sprite itemIcon;
			public UnityEvent onItemSelect = new UnityEvent();

			public Item(string itemTitle, Sprite itemIcon)
			{
				this.itemTitle = itemTitle;
				this.itemIcon = itemIcon;
			}

			public Item(string itemTitle)
			{
				this.itemTitle = itemTitle;
			}
		}

		public enum IndicatorType
		{
			None,
			Point,
			Text
		}
	}

}
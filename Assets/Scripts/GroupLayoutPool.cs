using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Используется совместно с одним из лейаутов. Инстанцирует и создаёт элементы лейаута через пул.
/// </summary>
public class GroupLayoutPool : MonoBehaviour 
{
	[SerializeField]
	private LayoutElement elementPrefab;
	private Stack<LayoutElement> m_stack;
	private List<LayoutElement> activeElements;
	private Stack<LayoutElement> ElementsStack 
	{
		get 
		{
			if (m_stack == null) 
			{
				m_stack = new Stack<LayoutElement>();
			}
			return m_stack;
		}
	}

	void Awake()
	{
		if (activeElements == null) 
			activeElements = new List<LayoutElement>();

		if (elementPrefab!= null && elementPrefab.transform.parent == transform)	// Если префаб лежит внутри
			DestroyElement(elementPrefab);
	}

	/// <summary>
	/// Инстанцирует элемент с префаба или достаёт из пула.
	/// </summary>
	/// <returns></returns>
	public LayoutElement InstantiateElement() 
	{
		LayoutElement element;
		if (ElementsStack.Count > 0) 
		{
			element = ElementsStack.Pop();
		}
		else 
		{
			element = ((GameObject)Instantiate(elementPrefab.gameObject)).GetComponent<LayoutElement>();
			element.transform.SetParent(this.transform);
		}
		element.gameObject.SetActive(true);
		if (activeElements == null) 
		{
			activeElements = new List<LayoutElement>();
		}
		activeElements.Add(element);
		element.transform.SetAsLastSibling();
		element.transform.localScale = Vector3.one;
		return element;
	}

	/// <summary>
	/// Деактивирует элемент и помещает его в пул доступных объектов.
	/// </summary>
	public void DestroyElement(LayoutElement element) 
	{
		ElementsStack.Push(element);
		activeElements.Remove(element);
		element.gameObject.SetActive(false);
	}

	/// <summary>
	/// Деактивирует все элементы
	/// </summary>
	public void Clear() 
	{
		if (activeElements != null)
		{
			while(activeElements.Count > 0)
				DestroyElement(activeElements[0]);
		}
	}
}

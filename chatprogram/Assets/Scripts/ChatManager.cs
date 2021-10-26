using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
	public static ChatManager instance;
	void Awake() => instance = this;

	public InputField SendInput;
	public RectTransform ChatContent;
	public Text ChatText;
	public ScrollRect ChatScrollRect;
	public GameObject ChatWindow;

	private bool isShortChat = false;

	public void ChangeChatState(bool isValue)
    {
		isShortChat = isValue;

		if(isShortChat)
        {
			ChatWindow.GetComponent<RectTransform>().sizeDelta = new Vector2(550,60);
        }
		else
        {
			ChatWindow.GetComponent<RectTransform>().sizeDelta = new Vector2(550, 550);
		}
    }

	public void ShowMessage(string data)
	{
		ChatText.text += ChatText.text == "" ? data : "\n" + data;

		Fit(ChatText.GetComponent<RectTransform>());
		Fit(ChatContent);
		Invoke("ScrollDelay", 0.03f);
	}

	void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);

	void ScrollDelay() => ChatScrollRect.verticalScrollbar.value = 0;
}
